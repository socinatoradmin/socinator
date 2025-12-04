#region

using DominatorHouseCore.Utility;
using System;
using System.Configuration;
using System.IO;
using System.Security.Cryptography;
using System.Text;

#endregion

namespace DominatorHouseCore.Diagnostics
{
    public class AesDecryption
    {
        public static byte[] Decrypt(byte[] bytes, string keyStr)
        {
            var sha256 = new SHA256CryptoServiceProvider();
            var ivBytes = new byte[] { 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0 };
            var keyBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(keyStr));
            return Decrypt(bytes, keyBytes, ivBytes);
        }
        public static byte[] Operation(byte[] bytes, bool IsEncrypt)
        {
            return PerformOperation(bytes, IsEncrypt);
        }
        private static byte[] Decrypt(byte[] bytes, byte[] keyBytes, byte[] ivBytes)
        {
            var aes = new AesCryptoServiceProvider
            {
                IV = ivBytes,
                Key = keyBytes
            };

            var transform = aes.CreateDecryptor();
            return transform.TransformFinalBlock(bytes, 0, bytes.Length);
        }
        private static byte[] PerformOperation(byte[] bytes, bool IsEncrypt)
        {
            var SKey = ConfigurationManager.AppSettings["Keykey"];
            var keyBytes = Encoding.UTF8.GetBytes(SKey);
            if (IsEncrypt)
            {
                using (Aes aesAlg = Aes.Create())
                {
                    aesAlg.Key = keyBytes;
                    aesAlg.IV = keyBytes;
                    aesAlg.Mode = CipherMode.CBC; // CBC is a common mode of operation
                    aesAlg.Padding = PaddingMode.PKCS7; // Default padding

                    using (ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV))
                    {
                        using (MemoryStream ms = new MemoryStream())
                        {
                            using (CryptoStream cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                            using (StreamWriter sw = new StreamWriter(cs))
                            {
                                sw.Write(Encoding.UTF8.GetString(bytes));
                            }
                            return ms.ToArray(); // Return the encrypted byte array
                        }
                    }
                }

            }
            else
            {
                using (Aes aesAlg = Aes.Create())
                {
                    aesAlg.Key = keyBytes;
                    aesAlg.IV = keyBytes;
                    aesAlg.Mode = CipherMode.CBC; // CBC mode for decryption
                    aesAlg.Padding = PaddingMode.PKCS7; // Default padding

                    using (ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV))
                    {
                        using (MemoryStream ms = new MemoryStream(bytes))
                        {
                            using (CryptoStream cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
                            using (StreamReader sr = new StreamReader(cs))
                            {
                                return Encoding.UTF8.GetBytes(sr.ReadToEnd()); // Return the decrypted string
                            }
                        }
                    }
                }
            }
        }
        public static string DecryptAes(string decryptedString)
        {
            try
            {
                var aesKey = ConfigurationManager.AppSettings["AesKey"];
                var decryptedKey = decryptedString.Replace("\\/", "/").Replace("\"", "");
                var bytes = Decrypt(Convert.FromBase64String(decryptedKey), aesKey);
                return Encoding.UTF8.GetString(bytes);
            }
            catch { return decryptedString; }
        }
        public static string EncryptKey(string key)
        {
            try
            {
                var bytes = Operation(Encoding.UTF8.GetBytes(key), true);
                return GenerateBase64(bytes, 6);
            }
            catch (Exception) { return key; }
        }

        private static string GenerateBase64(byte[] bytes, int v)
        {
            var str = Convert.ToBase64String(bytes);
            for (int i = 0; i < v; i++)
            {
                str = Convert.ToBase64String(Encoding.UTF8.GetBytes(str));
            }
            return str;
        }

        public static string DecryptKey(string Encrypted)
        {
            try
            {
                var bytes = Operation(CovertFromBase64(Encrypted, 6), false);
                return Encoding.UTF8.GetString(bytes);
            }
            catch (Exception) { return Encrypted; }
        }

        private static byte[] CovertFromBase64(string encrypted, int v)
        {
            var myBytes = Convert.FromBase64String(encrypted);
            try
            {
                for (int i = 0; i < v; i++)
                {
                    myBytes = Convert.FromBase64String(Encoding.UTF8.GetString(myBytes));
                }
                return myBytes;
            }
            catch { return myBytes; }
        }
    }
}