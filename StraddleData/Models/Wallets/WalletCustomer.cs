using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StraddleData.Models.Wallets
{
    public partial class WalletCustomer
    {
        public WalletCustomer()
        {
            WalletAccounts = new HashSet<WalletAccount>();
        }

        public Guid CustomerId { get; set; }
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string? MiddleName { get; set; }
        public string PhoneNumber { get; set; } = null!;
        public string? Email { get; set; }
        public int CustomerType { get; set; }
        public int Status { get; set; }
        public string AddressLine { get; set; } = null!;
        public string City { get; set; } = null!;
        public string State { get; set; } = null!;
        public string Country { get; set; } = null!;
        public int IdentificationType { get; set; }
        public string IdentificationNumber { get; set; } = null!;
        public DateTime? DateCreated { get; set; }
        public DateTime? DateUpdated { get; set; }

        public virtual ICollection<WalletAccount> WalletAccounts { get; set; }
    }
}
