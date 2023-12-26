using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StraddleData.Models.Partners
{
    public partial class ApiClient
    {
        public Guid ApiClientId { get; set; }
        public string ClientId { get; set; } = null!;
        public string ClientSecret { get; set; } = null!;
        public string ClientCode { get; set; } = null!;
        public string ClientName { get; set; } = null!;
        public bool? IsEnabled { get; set; }
        public DateTime? DateCreated { get; set; }
        public DateTime? DateUpdated { get; set; }
    }
}
