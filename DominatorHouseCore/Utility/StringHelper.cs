#region

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.ComponentModel;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

#endregion

namespace DominatorHouseCore.Utility
{
    [Localizable(false)]
    public static class StringHelper
    {
        public static string Base64Decode(this string base64EncodedData)
        {
            return Encoding.UTF8.GetString(Convert.FromBase64String(base64EncodedData));
        }

        public static string Base64Encode(this string plainText)
        {
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(plainText));
        }

        public static string GenerateGuid(bool keepDashes = true)
        {
            var str = Guid.NewGuid().ToString();
            return !keepDashes ? str.Replace('-', char.MinValue) : str;
        }

        public static string GetRegexPatern(string patern, string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return null;
            var match = Regex.Match(value, patern);
            return !match.Success ? null : match.Value;
        }

        public static byte[] GetSha256Raw(string randomString, byte[] key = null)
        {
            using (var hmacshA256 = key == null ? new HMACSHA256() : new HMACSHA256(key))
            {
                hmacshA256.ComputeHash(Encoding.UTF8.GetBytes(randomString));
                return hmacshA256.Hash;
            }
        }

        public static bool IsValidJson(this string strInput)
        {
            if (string.IsNullOrWhiteSpace(strInput))
                return false;
            strInput = strInput.Trim();
            if ((!strInput.StartsWith("{") || !strInput.EndsWith("}")) &&
                (!strInput.StartsWith("[") || !strInput.EndsWith("]")))
                return false;
            try
            {
                JToken.Parse(strInput);
                return true;
            }
            catch (JsonReaderException ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return false;
            }
        }

        public static bool IsBase64String(string input)
        {
            try
            {
                if (string.IsNullOrEmpty(input)) return false;
                input = input.Trim().Replace(" ", String.Empty);
                // Check the length of the string is a multiple of 4.
                if ((input.Length % 4) != 0) return false;
                // Use a regular expression to check if the string contains only valid Base64 characters.
                //var RegexToMatchBase64String = @"^[a-zA-Z0-9\+/]*={0,3}$";
                var RegexToMatchBase64String = @"^(?:[A-Za-z0-9+/]{4})*(?:[A-Za-z0-9+/]{2}==|[A-Za-z0-9+/]{3}=)?$";
                return Regex.IsMatch(input, RegexToMatchBase64String, RegexOptions.None);
            }
            catch
            {
                return false;
            }
        }
    }
}