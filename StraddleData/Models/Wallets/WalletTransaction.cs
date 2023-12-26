using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StraddleData.Models.Wallets
{
    public partial class WalletTransaction
    {
        public Guid TransactionId { get; set; }
        public Guid? SourceAccountId { get; set; }
        public Guid? DestinationAccountId { get; set; }
        public string? TransactionRequest { get; set; }
        public string? TransactionResponse { get; set; }
        public int TransactionStatus { get; set; }
        public int TransactionType { get; set; }
        public decimal TransactionAmount { get; set; }
        public decimal? TransactionFee { get; set; }
        public decimal? GrossTransactionAmount { get; set; }
        public decimal? ExchangeRate { get; set; }
        public decimal? ConvertedTransactionAmount { get; set; }
        public string TransactionReference { get; set; } = null!;
        public string TransactionDetails { get; set; } = null!;
        public string? PaymentReason { get; set; }
        public bool IsAmountLiened { get; set; }
        public DateTime? DateCreated { get; set; }
        public DateTime? DateUpdated { get; set; }
    }
}
