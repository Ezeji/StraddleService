using StraddleData.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StraddleCore.Models.DTO.Wallets
{
    public class WalletAccountDTO
    {
        public Guid AccountId { get; set; }

        public Guid CustomerId { get; set; }

        public string? BankAccountName { get; set; }

        [EnumDataType(typeof(AccountType))]
        public AccountType BankAccountType { get; set; }

        [EnumDataType(typeof(AccountStatus))]
        public AccountStatus AccountStatus { get; set; }

        public string? BankAccountNumber { get; set; }

        public decimal WalletBalance { get; set; }

        [EnumDataType(typeof(AccountCurrency))]
        public AccountCurrency BankAccountCurrency { get; set; }

        [EnumDataType(typeof(TierLevel))]
        public TierLevel BankAccountTierLevel { get; set; }
    }
}
