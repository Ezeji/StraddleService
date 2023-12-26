using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StraddleData.Enums
{
    public enum TransactionType
    {
        Intra = 1,
        Inter,
        Own
    }

    public enum TransactionStatus
    {
        Pending = 1,
        Cancelled,
        Refunded,
        Processed
    }
}
