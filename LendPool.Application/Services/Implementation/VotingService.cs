using LendPool.Application.Services.Interfaces;
using LendPool.Domain.DTOs;
using LendPool.Domain.Enums;
using LendPool.Domain.Models;
using LendPool.Domain.Responses;
using LendPool.Infrastructure.Repositories.Interfaces;
using Microsoft.Extensions.Logging;

namespace LendPool.Application.Services.Implementation
{
    public class VotingService : IVotingService
    {
        private readonly IVotingRepository _votingRepository;
        private readonly ILoanRepository _loanRepository;
        private readonly ILendpoolRepository _lenderpoolRepository;
        private readonly ILogger<VotingService> _logger;

        public VotingService(
            IVotingRepository votingRepository,
            ILoanRepository loanRepository,
            ILendpoolRepository lenderpoolRepository,
            ILogger<VotingService> logger)
        {
            _votingRepository = votingRepository;
            _loanRepository = loanRepository;
            _lenderpoolRepository = lenderpoolRepository;
            _logger = logger;
        }

        public async Task<GenericResponse<string>> CastVoteAsync(string lenderId, CastVoteDto dto)
        {
            try
            {
                // For LoanApproval, check if operation exists and pool membership
                string poolId = null;
                if (dto.OperationType == VoteOperationType.LoanApproval)
                {
                    var loanRequest = await _loanRepository.GetLoanRequestByIdAsync(dto.OperationId);
                    if (loanRequest.Data == null)
                        return GenericResponse<string>.FailResponse("Loan request not found", 404);
                    poolId = loanRequest.Data.MatchedPoolId;
                    var isPoolMember = await _lenderpoolRepository.IsUserInPoolAsync(lenderId, poolId);
                    if (!isPoolMember)
                        return GenericResponse<string>.FailResponse("You are not a member of this pool", 403);
                }
                // For other operation types, you may want to add similar checks

                // Check if already voted
                var hasVoted = await _votingRepository.HasVotedAsync(lenderId, dto.OperationId, dto.OperationType);
                if (hasVoted.Data)
                    return GenericResponse<string>.FailResponse("You have already voted on this operation", 400);

                // Create vote
                var vote = new Vote
                {
                    Id = Guid.NewGuid().ToString(),
                    OperationId = dto.OperationId,
                    OperationType = dto.OperationType,
                    LenderId = lenderId,
                    VoteType = dto.VoteType,
                    Comment = dto.Comment,
                    VotedAt = DateTime.UtcNow
                };

                await _votingRepository.AddVoteAsync(vote);

                // Check if voting is complete and determine result
                var voteResult = await GetVoteResultAsync(dto.OperationId, dto.OperationType);
                if (voteResult.Success && voteResult.Data.IsApproved && dto.OperationType == VoteOperationType.LoanApproval)
                {
                    // Auto-approve the loan request and create the loan
                    var loanRequest = await _loanRepository.GetLoanRequestByIdAsync(dto.OperationId);
                    if (loanRequest.Data != null)
                    {
                        loanRequest.Data.RequestStatus = LoanRequestStatus.Approved.ToString();
                        loanRequest.Data.AdminComment = dto.Comment;
                        var pool = await _lenderpoolRepository.GetByIdAsync(loanRequest.Data.MatchedPoolId);
                        if (pool.Data == null)
                            return GenericResponse<string>.FailResponse("Matched pool not found.", 404);
                        var loan = new Loan
                        {
                            Id = Guid.NewGuid().ToString(),
                            UserId = loanRequest.Data.BorrowerId,
                            PoolId = loanRequest.Data.MatchedPoolId,
                            Amount = loanRequest.Data.Amount,
                            InterestRate = pool.Data.InterestRate,
                            StartDate = DateTime.UtcNow,
                            DueDate = DateTime.UtcNow.AddDays(loanRequest.Data.TenureInDays),
                            LoanStatus = LoanStatus.Active.ToString(),
                            TotalRepaid = 0,
                            IsActive = true,
                            LoanRequestId = dto.OperationId,
                        };
                        await _loanRepository.AddLoanAsync(loan);
                        await _loanRepository.SaveChangesAsync();
                        return GenericResponse<string>.SuccessResponse("Vote recorded. Loan request approved by majority vote and loan created.", 200);
                    }
                }
                else if (voteResult.Success && voteResult.Data.IsRejected && dto.OperationType == VoteOperationType.LoanApproval)
                {
                    var loanRequest = await _loanRepository.GetLoanRequestByIdAsync(dto.OperationId);
                    if (loanRequest.Data != null)
                    {
                        loanRequest.Data.RequestStatus = LoanRequestStatus.Rejected.ToString();
                        loanRequest.Data.AdminComment = dto.Comment;
                        await _loanRepository.SaveChangesAsync();
                        return GenericResponse<string>.SuccessResponse("Vote recorded. Loan request rejected by majority vote.", 200);
                    }
                }

                return GenericResponse<string>.SuccessResponse("Vote recorded successfully", 200);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error casting vote for lender {LenderId} on operation {OperationId}", lenderId, dto.OperationId);
                return GenericResponse<string>.FailResponse("Error recording vote", 500);
            }
        }

        public async Task<GenericResponse<VoteResultDto>> GetVoteResultAsync(string operationId, VoteOperationType operationType)
        {
            try
            {
                // For LoanApproval, get poolId for member count
                string poolId = null;
                if (operationType == VoteOperationType.LoanApproval)
                {
                    var loanRequest = await _loanRepository.GetLoanRequestByIdAsync(operationId);
                    if (loanRequest.Data == null)
                        return GenericResponse<VoteResultDto>.FailResponse("Loan request not found", 404);
                    poolId = loanRequest.Data.MatchedPoolId;
                }
                // For other operation types, you may want to add logic to get poolId

                // Get all votes for this operation
                var votes = await _votingRepository.GetVotesByOperationAsync(operationId, operationType);
                if (!votes.Success)
                    return GenericResponse<VoteResultDto>.FailResponse("Error retrieving votes", 500);

                // Get total pool members (for LoanApproval) or set to votes.Data.Count for others
                int totalMembers = votes.Data.Count;
                if (operationType == VoteOperationType.LoanApproval && poolId != null)
                {
                    var memberCount = await _votingRepository.GetPoolMemberCountAsync(poolId);
                    if (!memberCount.Success)
                        return GenericResponse<VoteResultDto>.FailResponse("Error getting pool member count", 500);
                    totalMembers = memberCount.Data;
                }

                var voteCounts = votes.Data.GroupBy(v => v.VoteType)
                    .ToDictionary(g => g.Key, g => g.Count());

                var approveVotes = voteCounts.GetValueOrDefault("Approve", 0);
                var rejectVotes = voteCounts.GetValueOrDefault("Reject", 0);
                var abstainVotes = voteCounts.GetValueOrDefault("Abstain", 0);
                var totalVotes = votes.Data.Count;

                var approvalPercentage = totalMembers > 0 ? (decimal)approveVotes / totalMembers * 100 : 0;
                var rejectionPercentage = totalMembers > 0 ? (decimal)rejectVotes / totalMembers * 100 : 0;

                // Determine if approved or rejected (need >50% of total pool members)
                var isApproved = approvalPercentage > 50;
                var isRejected = rejectionPercentage > 50;
                var isPending = !isApproved && !isRejected;

                var voteDetails = votes.Data.Select(v => new VoteDetailDto
                {
                    LenderId = v.LenderId,
                    LenderName = v.Lender?.FullName ?? "Unknown",
                    VoteType = v.VoteType,
                    Comment = v.Comment,
                    VotedAt = v.VotedAt
                }).ToList();

                var result = new VoteResultDto
                {
                    OperationId = operationId,
                    OperationType = operationType,
                    TotalVotes = totalVotes,
                    ApproveVotes = approveVotes,
                    RejectVotes = rejectVotes,
                    AbstainVotes = abstainVotes,
                    TotalPoolMembers = totalMembers,
                    ApprovalPercentage = approvalPercentage,
                    RejectionPercentage = rejectionPercentage,
                    IsApproved = isApproved,
                    IsRejected = isRejected,
                    IsPending = isPending,
                    Votes = voteDetails
                };

                return GenericResponse<VoteResultDto>.SuccessResponse(result, 200, "Vote result retrieved successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting vote result for operation {OperationId}", operationId);
                return GenericResponse<VoteResultDto>.FailResponse("Error retrieving vote result", 500);
            }
        }

        public async Task<GenericResponse<List<VoteResultDto>>> GetVoteResultsByPoolAsync(string poolId)
        {
            try
            {
                var votes = await _votingRepository.GetVotesByPoolAsync(poolId);
                if (!votes.Success)
                    return GenericResponse<List<VoteResultDto>>.FailResponse("Error retrieving pool votes", 500);

                var voteResults = new List<VoteResultDto>();
                var groupedVotes = votes.Data.GroupBy(v => new { v.OperationId, v.OperationType });

                foreach (var group in groupedVotes)
                {
                    var result = await GetVoteResultAsync(group.Key.OperationId, group.Key.OperationType);
                    if (result.Success)
                    {
                        voteResults.Add(result.Data);
                    }
                }

                return GenericResponse<List<VoteResultDto>>.SuccessResponse(voteResults, 200, "Pool vote results retrieved successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting vote results for pool {PoolId}", poolId);
                return GenericResponse<List<VoteResultDto>>.FailResponse("Error retrieving pool vote results", 500);
            }
        }

        public async Task<GenericResponse<PoolVotingSummaryDto>> GetPoolVotingSummaryAsync(string poolId)
        {
            try
            {
                var totalMembers = await _votingRepository.GetPoolMemberCountAsync(poolId);
                if (!totalMembers.Success)
                    return GenericResponse<PoolVotingSummaryDto>.FailResponse("Error getting pool member count", 500);

                var pool = await _lenderpoolRepository.GetByIdAsync(poolId);
                if (!pool.Success || pool.Data == null)
                    return GenericResponse<PoolVotingSummaryDto>.FailResponse("Pool not found", 404);

                var votes = await _votingRepository.GetVotesByPoolAsync(poolId);
                if (!votes.Success)
                    return GenericResponse<PoolVotingSummaryDto>.FailResponse("Error retrieving pool votes", 500);

                var activeVoters = votes.Data.Select(v => v.LenderId).Distinct().Count();
                var participationRate = totalMembers.Data > 0 ? (decimal)activeVoters / totalMembers.Data * 100 : 0;

                var recentVotes = new List<VoteResultDto>();
                var groupedVotes = votes.Data.GroupBy(v => new { v.OperationId, v.OperationType }).Take(5); // Get last 5 votes

                foreach (var group in groupedVotes)
                {
                    var result = await GetVoteResultAsync(group.Key.OperationId, group.Key.OperationType);
                    if (result.Success)
                    {
                        recentVotes.Add(result.Data);
                    }
                }

                var summary = new PoolVotingSummaryDto
                {
                    PoolId = poolId,
                    PoolName = pool.Data.Name,
                    TotalMembers = totalMembers.Data,
                    ActiveVoters = activeVoters,
                    ParticipationRate = participationRate,
                    RecentVotes = recentVotes
                };

                return GenericResponse<PoolVotingSummaryDto>.SuccessResponse(summary, 200, "Pool voting summary retrieved successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting voting summary for pool {PoolId}", poolId);
                return GenericResponse<PoolVotingSummaryDto>.FailResponse("Error retrieving pool voting summary", 500);
            }
        }

        public async Task<GenericResponse<bool>> HasVotedAsync(string lenderId, string operationId, VoteOperationType operationType)
        {
            return await _votingRepository.HasVotedAsync(lenderId, operationId, operationType);
        }

        public async Task<GenericResponse<int>> GetPoolMemberCountAsync(string poolId)
        {
            return await _votingRepository.GetPoolMemberCountAsync(poolId);
        }

        public async Task<GenericResponse<int>> GetActiveVoterCountAsync(string operationId, VoteOperationType operationType)
        {
            return await _votingRepository.GetActiveVoterCountAsync(operationId, operationType);
        }
    }
} 