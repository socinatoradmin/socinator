using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.Text.RegularExpressions;

namespace ProtectedCommon
{
    public static class Signature
    {
        static string _xml_parameters;
        public static void Configure(string xml_parameters)
        {
            _xml_parameters = xml_parameters;
        }
        public static string Sign(string text)
        {
            if (text.Contains('/'))
            {
                throw new ArgumentOutOfRangeException("text", "The text cannot contain a slash");
            }
            // append the date
            var signed_at = DateTime.UtcNow.ToString("o");
            text += "/" + signed_at;
            var rsa = RSA.Create();
            rsa.FromXmlString(_xml_parameters);
            var bytes = rsa.SignData(Encoding.Unicode.GetBytes(text), HashAlgorithmName.SHA1, RSASignaturePadding.Pkcs1);
            return signed_at + "/" + Convert.ToBase64String(bytes);
        }
        public static string GetVerifiedText(string original)
        {
            string result = null;
            // there will be some text then / then the time then / then the signature
            var m = Regex.Match(original, "([^/]*)/([^/]*)/(.*)$");
            if (m != null && m.Success)
            {
                var proposed_result = m.Groups[1].Value;
                var date_time = m.Groups[2].Value;
                var signature = m.Groups[3].Value;

                DateTime serverTime;
                // is the date valid?
                if (DateTime.TryParse(date_time, null, System.Globalization.DateTimeStyles.RoundtripKind, out serverTime))
                {
                    // is the server time within 90 seconds?
                    if (Math.Abs((serverTime - DateTime.UtcNow).TotalSeconds) < 90) {
                        var signature_bytes = Convert.FromBase64String(signature);
                        var trial_bytes = Encoding.Unicode.GetBytes(proposed_result + "/" + date_time);
                        var rsa = RSA.Create();
                        rsa.FromXmlString(_xml_parameters);
                        if (rsa.VerifyData(trial_bytes, signature_bytes, HashAlgorithmName.SHA1, RSASignaturePadding.Pkcs1))
                        {
                            result = proposed_result;
                        }
                    }
                }
            }
            return result;
        }
        public static string CreateClientConfig()
        {
            var rsa = RSA.Create();
            rsa.FromXmlString(_xml_parameters);
            return "Configure a client with the following data: \n" + rsa.ToXmlString(false);
        }
        public static string CreateKeyPair()
        {
            var rsa = RSA.Create();
            string result = "For clients use ";
            result += rsa.ToXmlString(false);
            result += "\nand for the server use ";
            result += rsa.ToXmlString(true);
            return result;
        }
    }
}
