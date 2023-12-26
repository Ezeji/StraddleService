using StraddleData.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StraddleCore.Models.DTO.Wallets
{
    public class WalletTransactionCreateDTO
    {
        [Required]
        public string? SourcePhoneNumber { get; set; }

        [Required]
        public string? SourceAccountNumber { get; set; }

        [Required]
        public string? DestinationAccountNumber { get; set; }

        [Required]
        public decimal TransactionAmount { get; set; }

        public string? PaymentReason { get; set; }
    }

    public class WalletTransactionDTO : WalletTransactionCreateDTO
    {
        public decimal? TransactionFee { get; set; }

        public DateTime? TransactionDate { get; set; }

        public decimal? GrossTransactionAmount { get; set; }

        public decimal? ExchangeRate { get; set; }

        public decimal? ConvertedTransactionAmount { get; set; }

        [EnumDataType(typeof(TransactionStatus))]
        public TransactionStatus TransactionStatus { get; set; }
    }
}
