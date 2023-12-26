using StraddleData.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StraddleCore.Models.DTO.Wallets
{
    public class WalletCustomerDTO
    {
        [Required]
        public string? FirstName { get; set; }

        [Required]
        public string? LastName { get; set; }

        public string? MiddleName { get; set; }

        [Required]
        public string? PhoneNumber { get; set; }

        [EnumDataType(typeof(CustomerStatus))]
        public CustomerStatus Status { get; set; }

        [Required]
        [EnumDataType(typeof(IdentityType))]
        public IdentityType IdentificationType { get; set; }

        [Required]
        public string? IdentificationNumber { get; set; }

        public string? BankAccountNumber { get; set; }

        public string? Email { get; set; }

        public Guid CustomerId { get; set; }
    }
}
