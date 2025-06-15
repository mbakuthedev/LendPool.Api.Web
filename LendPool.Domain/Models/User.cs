using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LendPool.Domain.Models
{
    public class User : BaseEntity
    {
        public User()
        {
            FullName = FirstName + " " +  LastName;
        }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }

        // KYC fields
        public string? BVN { get; set; }
        public string? NIN { get; set; }
        public string? Address { get; set; }

        // Lender KYC
        public string? BusinessName { get; set; }
        public string? RegistrationNumber { get; set; }
        public decimal? InvestmentCapacity { get; set; }

        public DateTime? DateOfBirth { get; set; }

        public string Role { get; set; }
        public bool IsKycVerified { get; set; } = false;
        public string? DocumentId { get; set; }
        public string? DocumentUrl { get; set; }

        public Wallet Wallet { get; set; }
        public ICollection<Transaction> Transactions { get; set; }

        public ICollection<LenderPoolMembership> LenderPoolMemberships { get; set; }

    }
}
