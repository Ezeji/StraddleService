using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StraddleCore.Models
{
    public class JwtConfig
    {
        public const string ConfigName = nameof(JwtConfig);

        public string Key { get; set; }
        public string Issuer { get; set; }
        public string Issuer2 { get; set; }
        public string GrantType { get; set; }
    }
}
