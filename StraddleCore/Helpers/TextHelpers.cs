using StraddleCore.Constants;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace StraddleCore.Helpers
{
    public static class TextHelpers
    {
        public static string GenerateUniqueCode(int seedId)
        {
            string initialId = seedId.ToString();

            while (initialId.Length < 6)
            {
                initialId = "0" + initialId;
            }

            string dateTime = DateTime.UtcNow.ToString("mmssfff"); //get minute, second and ticks
            // return dateTime;
            string tempPatientId = $"{dateTime}{initialId}";
            long uniqueCode = Convert.ToInt64(tempPatientId);
            return uniqueCode.ToString("x2").ToLower(CultureInfo.InvariantCulture);
        }

        public static string CleanNumber(string value)
        {
            Regex digitsOnly = new Regex(@"[^\d]");
            return digitsOnly.Replace(value, "");
        }

        public static string FormatPhoneNumber(string phoneNumber, string code = "1")
        {
            if (string.IsNullOrEmpty(phoneNumber))
            {
                return string.Empty;
            }

            if (phoneNumber.StartsWith(code) || phoneNumber.StartsWith($"+{code}"))
            {
                return phoneNumber;
            }

            string phone = CleanNumber(phoneNumber);

            if (phone.StartsWith("0"))
            {
                phone = $"{code}{phone.Remove(0, 1)}";
            }
            else
            {
                phone = $"{code}{phone}";
            }

            return phone;
        }

        public static string FormatPhoneNumberReverse(string phoneNumber, string code = "1")
        {
            if (string.IsNullOrEmpty(phoneNumber))
            {
                return string.Empty;
            }

            if (phoneNumber.StartsWith("0"))
            {
                return phoneNumber;
            }

            string phone = CleanNumber(phoneNumber);

            if (phone.StartsWith(code))
            {
                int codeLength = code.Length;
                phone = $"0{phone.Remove(0, codeLength)}";
            }
            else if (phone.StartsWith($"+{code}"))
            {
                int codeLength = code.Length + 1;
                phone = $"0{phone.Remove(0, codeLength)}";
                return phone;
            }
            else
            {
                phone = $"0{phone}";
            }

            return phone;
        }

        public static string RandomNumberString(int characters = 6)
        {
            Random random = new Random((int)DateTime.Now.Ticks);
            string code = string.Empty;

            string chars = ServicesConstants.NumberSeed;
            code = new string(Enumerable.Repeat(chars, characters).Select(s => s[random.Next(s.Length)]).ToArray());

            return code;
        }

        public static string RandomString(int characters = 6)
        {
            Random random = new Random((int)DateTime.Now.Ticks);
            string code = string.Empty;

            string chars = ServicesConstants.LetterSeed;
            code = new string(Enumerable.Repeat(chars, characters).Select(s => s[random.Next(s.Length)]).ToArray());

            return code;
        }

        public static string Base64Encode(string plainText)
        {
            byte[] codedbytes = Encoding.UTF8.GetBytes(plainText);
            string encodedText = Convert.ToBase64String(codedbytes);

            return encodedText;
        }

        public static string Base64Decode(string encodedText)
        {
            byte[] decodedBytes = Convert.FromBase64String(encodedText);

            string decodedText = Encoding.UTF8.GetString(decodedBytes);

            return decodedText;
        }

        public static string EncodeUserAndPassword(string userId, string password)
        {
            string combined = $"{userId}:{password}";

            return Base64Encode(combined);
        }

        public static List<string> SplitName(string name)
        {
            List<string> namesToReturn = new List<string>(2);

            if (string.IsNullOrEmpty(name))
            {
                return namesToReturn;
            }

            name = name.Trim();

            if (name.StartsWith("Mrs") || name.StartsWith("mrs") || name.StartsWith("Mr.") || name.StartsWith("MRS"))
            {
                name = name.Remove(0, 3);
            }
            else if (name.StartsWith("Mr") || name.StartsWith("mr") || name.StartsWith("MR"))
            {
                name = name.Remove(0, 2);
            }
            else if (name.StartsWith("Mrs."))
            {
                name = name.Remove(0, 4);
            }

            name = name.Trim();

            List<string> names = name.Split(' ').ToList();

            if (names.Count == 1)
            {
                namesToReturn.Add(names[0]);
                namesToReturn.Add(" ");
            }
            else if (names.Count == 2)
            {
                namesToReturn.Add(names[0]);
                namesToReturn.Add(names[1]);
            }
            else if (names.Count == 3)
            {
                namesToReturn.Add(names[0]);
                var name2 = names[1] + " " + names[2];
                namesToReturn.Add(name2);
            }
            else if (names.Count > 3)
            {
                namesToReturn.Add(names[0]);
                var name2 = names[1] + " " + names[2];
                namesToReturn.Add(name2);
            }

            return namesToReturn;
        }

        public static DateTime FormatDateOfBirth(string dateOfBirth)
        {
            string[] formats = new string[] { "dd-MM-yyyy", "dd/MM/yyyy", "d/M/yyyy", "d/MM/yyyy", "dd/M/yyyy",
                "d-M-yyyy", "d-MM-yyyy", "dd-M-yyyy", "yyyy-MM-dd", "yyyy/MM/dd", "yyyy/M/d", "yyyy/MM/d", "yyyy/M/dd",
                "yyyy-M-d", "yyyy-MM-d", "yyyy-M-dd" };

            if (DateTime.TryParseExact(dateOfBirth, formats, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime formattedDoB))
            {
                return formattedDoB;
            }

            return Convert.ToDateTime("2000-01-01");
        }

        public static string GenerateRandomUsername()
        {
            //e.g User1232923
            string randomString = RandomNumberString(3);
            string secondStamp = DateTime.UtcNow.ToString("ssfff");

            return $"User{randomString}{secondStamp}";
        }

        public static DateTime UnixTimeStampToDateTime(string date)
        {
            double unixTimeStamp = double.Parse(date);

            // Unix timestamp is seconds past epoch
            DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            dateTime = dateTime.AddSeconds(unixTimeStamp).ToUniversalTime();
            return dateTime;
        }

        public static string MaskEmail(string email)
        {
            string pattern = @"(?<=[\w]{1})[\w-\._\+%]*(?=[\w]{2}@)";
            return Regex.Replace(email, pattern, m => new string('*', m.Length));
        }

        public static string MaskPhoneNumber(string phoneNumber, char maskXter, int startIndex, int maskLength)
        {
            startIndex = startIndex < 1 ? 1 : startIndex;
            while (startIndex > maskLength)
            {
                startIndex--;
            }

            return string.Format("{0}{1}{2}", phoneNumber.Substring(0, startIndex), "".PadRight(maskLength, maskXter),
                phoneNumber.Substring(startIndex + maskLength));
        }
    }
}
