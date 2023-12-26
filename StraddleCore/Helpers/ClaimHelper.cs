using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace StraddlePaymentCore.Helpers
{
    public class ClaimHelper : IClaimHelper
    {
        private readonly IHttpContextAccessor _context;

        public ClaimHelper(IHttpContextAccessor context)
        {
            _context = context;
        }

        public string GetUserEmail()
        {
            return GetClaimValue("Name");
        }

        public string GetUserPhoneNumber()
        {
            return GetClaimValue("PhoneNumber");
        }

        public string GetClaimValue(string claimType) //e.g LoginID or ClaimType.Role
        {
            ClaimsIdentity identity = _context.HttpContext.User.Identity as ClaimsIdentity;

            if (identity == null)
            {
                return null;
            }

            Claim claim = identity.FindFirst(claimType);

            if (claim == null)
            {
                return null;
            }

            return string.IsNullOrEmpty(claim.Value) ? null : claim.Value;
        }
    }
}
