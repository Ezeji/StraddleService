using StraddleCore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace StraddleCore.Services.Common.Interfaces
{
    public interface IPasswordService
    {
        string GenerateRandomCode();

        string GenerateClientId(bool isProductionId);

        string GenerateClientSecret();

        string GetPasswordHash(string password);

        bool VerifyPassword(string passwordHash, string password);

        string GenerateToken(List<Claim> claims, JwtConfig jwtConfig);
    }
}
