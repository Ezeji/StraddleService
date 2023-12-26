using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using StraddleCore.Models;
using StraddleCore.Services.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace StraddleCore.Services.Common
{
    public class PasswordService : IPasswordService
    {
        private readonly RandomNumberGenerator _rng;
        private readonly KeyDerivationPrf _prf;

        public PasswordService()
        {
            _rng = RandomNumberGenerator.Create();
            _prf = KeyDerivationPrf.HMACSHA256;
        }

        public string GetPasswordHash(string password)
        {
            var hashedPassword = HashPassword(
                password: password,
                rng: _rng,
                prf: _prf,
                iterCount: 10000,
                saltSize: 128 / 8,
                numBytesRequested: 256 / 8);

            return Convert.ToBase64String(hashedPassword);
        }

        public bool VerifyPassword(string passwordHash, string password)
        {
            var hashedBytes = Convert.FromBase64String(passwordHash);

            return VerifyHashedPassword(hashedBytes, password);
        }

        public string GenerateRandomCode()
        {
            return $"{GenerateRandomChars(128 / 16).ToLower(CultureInfo.InvariantCulture)}";
        }

        public string GenerateClientId(bool isProductionId)
        {
            if (!isProductionId)
            {
                return $"Test_SK{GenerateRandomChars(128 / 24)}";
            }
            return $"Prod_SK{GenerateRandomChars(128 / 24)}";
        }

        public string GenerateClientSecret()
        {
            return GenerateRandomChars(128 / 8);
        }

        public string GenerateToken(List<Claim> claims, JwtConfig jwtConfig)
        {
            SymmetricSecurityKey key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtConfig.Key));

            SigningCredentials credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            JwtSecurityToken token = new JwtSecurityToken(
                    jwtConfig.Issuer,
                    jwtConfig.Issuer,
                    claims,
                    expires: DateTime.UtcNow.AddHours(1),
                    signingCredentials: credentials
                );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private static byte[] HashPassword(string password, RandomNumberGenerator rng, KeyDerivationPrf prf, int iterCount, int saltSize, int numBytesRequested)
        {
            // Produce a version 3 (see comment above) text hash.
            byte[] salt = new byte[saltSize];
            rng.GetBytes(salt);
            byte[] subkey = KeyDerivation.Pbkdf2(password, salt, prf, iterCount, numBytesRequested);

            byte[] outputBytes = new byte[13 + salt.Length + subkey.Length];
            outputBytes[0] = 0x01; // format marker
            WriteNetworkByteOrder(outputBytes, 1, (uint)prf);
            WriteNetworkByteOrder(outputBytes, 5, (uint)iterCount);
            WriteNetworkByteOrder(outputBytes, 9, (uint)saltSize);
            Buffer.BlockCopy(salt, 0, outputBytes, 13, salt.Length);
            Buffer.BlockCopy(subkey, 0, outputBytes, 13 + saltSize, subkey.Length);
            return outputBytes;
        }

        private static bool VerifyHashedPassword(byte[] hashedPassword, string password)
        {
            int iterCount = 10000;
            try
            {
                // Read header information
                KeyDerivationPrf prf = (KeyDerivationPrf)ReadNetworkByteOrder(hashedPassword, 1);
                iterCount = (int)ReadNetworkByteOrder(hashedPassword, 5);
                int saltLength = (int)ReadNetworkByteOrder(hashedPassword, 9);

                // Read the salt: must be >= 128 bits
                if (saltLength < 128 / 8)
                {
                    return false;
                }
                byte[] salt = new byte[saltLength];
                Buffer.BlockCopy(hashedPassword, 13, salt, 0, salt.Length);

                // Read the subkey (the rest of the payload): must be >= 128 bits
                int subkeyLength = hashedPassword.Length - 13 - salt.Length;
                if (subkeyLength < 128 / 8)
                {
                    return false;
                }
                byte[] expectedSubkey = new byte[subkeyLength];
                Buffer.BlockCopy(hashedPassword, 13 + salt.Length, expectedSubkey, 0, expectedSubkey.Length);

                // Hash the incoming password and verify it
                byte[] actualSubkey = KeyDerivation.Pbkdf2(password, salt, prf, iterCount, subkeyLength);

                return CryptographicOperations.FixedTimeEquals(actualSubkey, expectedSubkey);
            }
            catch (IndexOutOfRangeException)
            {
                // This should never occur except in the case of a malformed payload, where
                // we might go off the end of the array. Regardless, a malformed payload
                // implies verification failed.

                return false;
            }
        }

        private static uint ReadNetworkByteOrder(byte[] buffer, int offset)
        {
            return ((uint)(buffer[offset + 0]) << 24)
                | ((uint)(buffer[offset + 1]) << 16)
                | ((uint)(buffer[offset + 2]) << 8)
                | ((uint)(buffer[offset + 3]));
        }

        private static void WriteNetworkByteOrder(byte[] buffer, int offset, uint value)
        {
            buffer[offset + 0] = (byte)(value >> 24);
            buffer[offset + 1] = (byte)(value >> 16);
            buffer[offset + 2] = (byte)(value >> 8);
            buffer[offset + 3] = (byte)(value >> 0);
        }

        private string GenerateRandomChars(int saltSize)
        {
            byte[] randomBytes = new byte[saltSize];
            _rng.GetBytes(randomBytes);
            var randomString = Convert.ToBase64String(randomBytes);
            var cleanString = randomString.Replace("=", "").Replace("/", "").Replace("+", "");
            return cleanString;
        }
    }
}
