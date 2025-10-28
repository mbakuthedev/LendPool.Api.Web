using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LendPool.Application.Services.Interfaces;
using LendPool.Application.DTOs;
using LendPool.Domain.DTOs;
using LendPool.Domain.Enums;
using LendPool.Domain.Models;
using LendPool.Domain.Responses;
using LendPool.Infrastructure.Repositories.Interfaces;

namespace LendPool.Application.Services.Implementation
{
    public class LenderpoolService : ILenderpoolService
    {
        private readonly ILendpoolRepository _poolRepository;
        private readonly IWalletService _walletService;
        private readonly IUserService _userService;
        private readonly ILenderInvestmentService _lenderInvestmentService;

        public LenderpoolService(ILendpoolRepository poolRepository, IWalletService walletService, IUserService userService, ILenderInvestmentService lenderInvestmentService)
        {
            _poolRepository = poolRepository;
            _walletService = walletService;
            _userService = userService;
            _lenderInvestmentService = lenderInvestmentService;
        }

        public async Task<GenericResponse<string>> AddUserToPoolAsync(AddUserToPoolDto dto, string actingUserId)
        {
            var pool = await _poolRepository.GetByIdAsync(dto.PoolId);
            if (pool == null || pool.Data == null)
            {
                return GenericResponse<string>.FailResponse("Pool not found.", 404);
            }

            if (pool.Data.CreatedByUserId != actingUserId)
            {
                return GenericResponse<string>.FailResponse("Only the pool creator can add members.", 403);
            }

            var user = await _userService.GetUserByIdAsync(dto.UserId);
            if (user == null)
            {
                return GenericResponse<string>.FailResponse("User not found.", 404);
            }

            var isAlreadyMember = await _poolRepository.IsUserInPoolAsync(dto.UserId, dto.PoolId);
            if (isAlreadyMember)
            {
                return GenericResponse<string>.FailResponse("User is already a member of this pool.", 400);
            }

            var membership = new LenderPoolMembership
            {
                Id = Guid.NewGuid().ToString(),
                LenderPoolId = dto.PoolId,
                UserId = dto.UserId,
                JoinedAt = DateTime.UtcNow
            };

            var result = await _poolRepository.AddUserToPoolAsync(membership);

            if (!result)
            {
                return GenericResponse<string>.FailResponse("Failed to add user to pool.", 500);
            }

            return GenericResponse<string>.SuccessResponse("User successfully added to pool.", 200);
        }



        public async Task<GenericResponse<LenderPool>> CreateLenderPoolAsync(CreateLenderPoolDto dto, string creatorUserId)
        {
            var pool = new LenderPool
            {
                Name = dto.Name,
                Description = dto.Description,
                InterestRate = dto.InterestRate,
                MinimumAmount = dto.MinimumAmount,
                MaximumAmount = dto.MaximumAmount,
                TotalCapital = 0,
                CreatedByUserId = creatorUserId,
                LenderPoolMemberships = new List<LenderPoolMembership>
                {
                    new LenderPoolMembership
                    {
                        UserId = creatorUserId,
                        JoinedAt = DateTime.UtcNow,
                        Role = UserRole.SuperLender.ToString(),
                    }
                }
            };

            try
            {
                await _poolRepository.CreateAsync(pool);

                return GenericResponse<LenderPool>.SuccessResponse(pool, 201, "Pool created successfully.");
            }
            catch (Exception ex)
            {
                // Log the exception as needed
                return GenericResponse<LenderPool>.FailResponse($"Failed to create pool: {ex.Message}", 500);
            }
        }

        public async Task<GenericResponse<LenderPool>> EditPoolInformation(string userId, string poolId, EditPoolInformationDto dto)
        {
            try
            {
                var pool = await _poolRepository.GetPoolById(poolId);

                if (pool.Data == null)
                {
                    return GenericResponse<LenderPool>.FailResponse("Pool does not exist", 404);
                }

              
                if (pool.Data.CreatedByUserId != userId)
                {
                    return GenericResponse<LenderPool>.FailResponse("You are not authorized to edit this pool", 403);
                }

                pool.Data.Description = dto.Description ?? pool.Data.Description;
                pool.Data.Rules = dto.Rules ?? pool.Data.Rules;
                pool.Data.MaximumAmount = dto.MaximumAmount != 0 ? dto.MaximumAmount : pool.Data.MaximumAmount;
                pool.Data.MinimumAmount = dto.MinimumAmount != 0 ? dto.MinimumAmount : pool.Data.MinimumAmount;
                pool.Data.DateModified = DateTime.UtcNow;

                await _poolRepository.UpdateLenderPool(pool.Data);

                return GenericResponse<LenderPool>.SuccessResponse(pool.Data, 200, "Pool information updated successfully");
            }
            catch (Exception ex)
            {
              return GenericResponse<LenderPool>.FailResponse($"An error occurred while updating the pool: {ex.Message}", 500);
             
            }
        }


        public async Task<GenericResponse<bool>> ContributeToPoolAsync(ContributeToPoolDto dto)
        {
            try
            {
                var wallet = await _walletService.GetWalletByUserIdAsync(dto.UserId);

                if (wallet == null || wallet.Data == null)
                    return GenericResponse<bool>.FailResponse("Wallet not found for the specified user.", 404);

                if (wallet.Data.Balance < dto.Amount)
                    return GenericResponse<bool>.FailResponse("Balance is too low to perform transaction", 400);

                // Step 1: Debit wallet first
                var walletDebitSuccess = await _walletService.DebitAsync(wallet.Data.Id, dto.Amount);
                if (!walletDebitSuccess)
                {
                    return GenericResponse<bool>.FailResponse("Failed to debit wallet", 400);
                }

                // Step 2: Create pool contribution
                var contribution = new PoolContribution
                {
                    LenderId = dto.UserId,
                    PoolId = dto.PoolId,
                    Amount = dto.Amount
                };

                var contributionResult = await _poolRepository.ContributeAsync(contribution);
                if (!contributionResult.Success)
                {
                    // Rollback wallet debit if contribution fails
                    await _walletService.CreditAsync(wallet.Data.Id, dto.Amount);
                    return contributionResult;
                }

                // Step 3: Create lender investment
                var investmentDto = new CreateLenderInvestmentDto
                {
                    LenderId = dto.UserId,
                    PoolId = dto.PoolId,
                    InvestmentAmount = dto.Amount
                };

                var investmentResult = await _lenderInvestmentService.CreateInvestmentAsync(investmentDto);
                if (!investmentResult.Success)
                {
                    // Rollback both wallet debit and pool contribution if investment fails
                    await _walletService.CreditAsync(wallet.Data.Id, dto.Amount);
                    
                    // Rollback pool contribution by withdrawing the amount
                    await _poolRepository.WithdrawAsync(dto.PoolId, dto.Amount);
                    
                    return GenericResponse<bool>.FailResponse($"Transaction failed: {investmentResult.Message}. All changes have been rolled back.", 500);
                }

                return GenericResponse<bool>.SuccessResponse(true, 200, "Contribution and investment tracking successful");
            }
            catch (Exception ex)
            {
                // In case of any unexpected error, attempt to rollback wallet debit
                try
                {
                    var wallet = await _walletService.GetWalletByUserIdAsync(dto.UserId);
                    if (wallet?.Data != null)
                    {
                        await _walletService.CreditAsync(wallet.Data.Id, dto.Amount);
                    }
                }
                catch (Exception rollbackEx)
                {
                    // Log rollback failure but don't throw - original exception is more important
                    // In a production system, you'd want to log this to a monitoring system
                }

                return GenericResponse<bool>.FailResponse($"Transaction failed with error: {ex.Message}. Attempted rollback.", 500);
            }
        }
        public async Task<GenericResponse<IEnumerable<LoanDto>>> GetActiveLoansByPoolAsync(string poolId)
        {
            var loans = await _poolRepository.GetActiveLoansByPoolAsync(poolId);

            var loanData = loans?.Data;

            if (loanData == null || !loanData.Any())
            {
                return GenericResponse<IEnumerable<LoanDto>>.FailResponse("No active loans found for this pool.", 404);
            }

            // Manually map Loan to LoanDto
            var loanDtos = loanData.Select(loan => new LoanDto
            {
               
                Amount = loan.Amount,
                InterestRate = loan.InterestRate,
                StartDate = loan.StartDate,
                DueDate = loan.DueDate,
               IsActive = loan.IsActive,
                PoolId = loan.PoolId,
                LoanStatus = loan.LoanStatus
                // Map other necessary fields
            });

            return GenericResponse<IEnumerable<LoanDto>>.SuccessResponse(loanDtos, 200, "Successfully fetched loans.");
        }


        public async Task<GenericResponse<IEnumerable<LenderPool>>> GetAllPoolsAsync()
        {
            var pools = await _poolRepository.GetAllAsync();
            if (pools == null || !pools.Data.Any())
            {
                return GenericResponse<IEnumerable<LenderPool>>.FailResponse("No pools found.", 404);
            }
            return GenericResponse<IEnumerable<LenderPool>>.SuccessResponse(pools.Data, 200, "Successfully fetched all pools.");
        }

        public async Task<GenericResponse<IEnumerable<RepaymentDto>>> GetRepaymentsAndEarningsAsync(string poolId)
        {
            var repayments = await _poolRepository.GetRepaymentsAsync(poolId);

            var repaymentsData = repayments?.Data;

            if (repayments == null || repaymentsData == null || !repaymentsData.Any())
            {
                return GenericResponse<IEnumerable<RepaymentDto>>.FailResponse("No active repayments found for this pool.", 404);
            }

            var repaymentDtos = repaymentsData.Select(repayment =>
            {
                var interestRate = repayment.Loan?.InterestRate ?? 0;
                var interestEarned = repayment.AmountPaid * (interestRate / 100);

                return new RepaymentDto
                {
                    Amount = repayment.AmountPaid,
                    StartDate = repayment.PaymentDate,
                    PoolId = repayment.LenderpoolId,
                    InterestRate = interestRate,
                    DueDate = repayment.Loan?.DueDate ?? DateTime.MinValue,
                    UserId = repayment.Loan?.UserId ?? "Unknown",
                    InterestEarned = interestEarned
                };
            });

            return GenericResponse<IEnumerable<RepaymentDto>>.SuccessResponse(repaymentDtos, 200, "Successfully fetched repayments and earnings.");
        }

        public async Task<GenericResponse<bool>> WithdrawFromPoolAsync(WithdrawFromPoolDto dto)
        {
            try
            {
                // Get available balance for user in the pool
                var available = await _poolRepository.GetAvailableBalanceAsync(dto.PoolId, dto.UserId);

                if (available < dto.Amount)
                    return GenericResponse<bool>.FailResponse("Insufficient balance in the pool.", 400);

                // Step 1: Credit wallet first
                var creditSuccess = await _walletService.CreditAsync(dto.UserWalletId, dto.Amount);
                if (!creditSuccess)
                {
                    return GenericResponse<bool>.FailResponse("Failed to credit user wallet.", 500);
                }

                // Step 2: Record withdrawal in pool
                var withdrawSuccess = await _poolRepository.RecordWithdrawalAsync(dto.PoolId, dto.UserId, dto.Amount);
                if (!withdrawSuccess.Data)
                {
                    // Rollback wallet credit if withdrawal recording fails
                    await _walletService.DebitAsync(dto.UserWalletId, dto.Amount);
                    return GenericResponse<bool>.FailResponse("Failed to record withdrawal in pool.", 500);
                }

                return GenericResponse<bool>.SuccessResponse(true, 200, "Withdrawal successful.");
            }
            catch (Exception ex)
            {
                // In case of any unexpected error, attempt to rollback wallet credit
                try
                {
                    await _walletService.DebitAsync(dto.UserWalletId, dto.Amount);
                }
                catch (Exception rollbackEx)
                {
                    // Log rollback failure but don't throw - original exception is more important
                    // In a production system, you'd want to log this to a monitoring system
                }

                return GenericResponse<bool>.FailResponse($"Withdrawal failed with error: {ex.Message}. Attempted rollback.", 500);
            }
        }

        public async Task<GenericResponse<PoolSummaryDto>> GetPoolSummaryAsync(string poolId)
        {
            var summary = await _poolRepository.GetPoolSummaryAsync(poolId);

            if (summary == null || summary.Data == null)
            {
                return GenericResponse<PoolSummaryDto>.FailResponse("Summary not found for this pool", 400);
            }

         

            return GenericResponse<PoolSummaryDto>.SuccessResponse(summary.Data, 200, "Pool summary retrieved successfully");
        }

        public async Task<GenericResponse<LendPool.Domain.DTOs.LenderPoolDto>> GetPoolById(string poolId)
        {
           var pool = await _poolRepository.GetPoolById(poolId);

            var poolData = pool.Data;

            if (poolData == null)
            {
                return GenericResponse<LendPool.Domain.DTOs.LenderPoolDto>.FailResponse("Lender pool not found", 404);
            }
            var poolDto = new LendPool.Domain.DTOs.LenderPoolDto
            {
                Name = poolData.Name,
                Description = poolData.Description,
                InterestRate = poolData.InterestRate,
                MaximumAmount = poolData.MaximumAmount,
                MinimumAmount = poolData.MinimumAmount,
            };
            return GenericResponse<LendPool.Domain.DTOs.LenderPoolDto>.SuccessResponse(poolDto, 200, "Successfully fetched pool");
        }
    }
}
