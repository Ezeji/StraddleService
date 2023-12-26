using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StraddleData.Models.Wallets
{
    public partial class WalletAccount
    {
        public Guid AccountId { get; set; }
        public Guid CustomerId { get; set; }
        public string BankName { get; set; } = null!;
        public string BankCode { get; set; } = null!;
        public string BankAccountNumber { get; set; } = null!;
        public string BankAccountName { get; set; } = null!;
        public int BankAccountType { get; set; }
        public int BankAccountCurrency { get; set; }
        public decimal WalletBalance { get; set; }
        public decimal LedgerBalance { get; set; }
        public decimal TotalCredits { get; set; }
        public decimal TotalLedgerCredits { get; set; }
        public decimal TotalDebits { get; set; }
        public decimal TotalLedgerDebits { get; set; }
        public DateTime? DateCreated { get; set; }
        public DateTime? DateUpdated { get; set; }
        public int BankAccountTierLevel { get; set; }
        public decimal? Discount { get; set; }
        public decimal LienAmount { get; set; }
        public int AccountStatus { get; set; }

        public virtual WalletCustomer Customer { get; set; } = null!;
    }
}
