using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StraddleCore.Models.DTO.Shared
{
    public class DisburseTransactionDTO
    {
        public string? TransactionReference { get; set; }

        public Guid SourceAccountId { get; set; }
    }
}
