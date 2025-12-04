using DominatorHouseCore;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Models;
using DominatorHouseCore.Request;
using DominatorHouseCore.Utility;
using GramDominatorCore.GDModel;
using GramDominatorCore.Request;
using GramDominatorCore.Response;
using IGEncrypt;
using Newtonsoft.Json.Linq;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace GramDominatorCore.Utility
{
    public class PasswordEncryptionProcess
    {
        private static readonly object DownloadMainExeObj = new object();
        public static readonly object startProcess = new object();
        /// <summary>
        /// Method Request to server for plain password encryption and get encrypted password.
        /// </summary>
        
        public static void EncrptLoginPassword(DominatorAccountModel dominatorAccountModel, AccountModel accountModel)
        {
            var unix = Convert.ToInt32(Math.Floor(DateTime.Now.Subtract(new DateTime(1970, 1, 1)).TotalSeconds));
            var unixTimestampSeconds = unix.ToString();
            //var time = Math.Floor(DateTime.Now();
            var encryptedPwd = EncryptPwd(accountModel.publicKey, accountModel.publicId, unixTimestampSeconds, dominatorAccountModel.AccountBaseModel.Password);
            var fullPwd =
                $"#PWD_INSTAGRAM:4:{unixTimestampSeconds}:{encryptedPwd}";
            accountModel.EncPwd = fullPwd;
        }

        public static string EncryptPwd(string publicKey, string keyId, string unixTimestampSeconds, string plainPwd)
        {
            var encryptedPwd = string.Empty;
            try
            {
                IHttpHelper helper = new IgHttpHelper();
                var requestParameter = helper.GetRequestParameter();
                requestParameter.ContentType = "application/json";
                dynamic postElement = new JObject();
                postElement.element = Convert.ToBase64String(Encoding.UTF8.GetBytes(plainPwd));
                postElement.public_key = publicKey;
                postElement.public_id = keyId;
                postElement.epoch_time = unixTimestampSeconds;
                var postRaw = postElement.ToString();
                byte[] bytes = Encoding.UTF8.GetBytes(postRaw);
                var response = helper.PostRequest("https://enc.socinator.com/api/v2/enc", bytes);
                //var response = helper.PostRequest("https://4li63yyaymukz2yguriuraz34q0bjlld.lambda-url.eu-west-2.on.aws/api/v2/enc", bytes);
                if(response.HasError)
                {
                    response = helper.PostRequest("https://4li63yyaymukz2yguriuraz34q0bjlld.lambda-url.eu-west-2.on.aws/api/v2/enc", bytes);
                }
                if (!string.IsNullOrEmpty(response.Response))
                {
                    JsonHandler json = new JsonHandler(response.Response);
                    var pwd = json.GetElementValue("result");
                    encryptedPwd = pwd;
                }
            }
            catch (Exception ex)
            {
                ex.DebugLog();

            }

            return encryptedPwd;
        }

        public static async Task<string> EncryptInstagramPassword(string Password,PreLoginData preLoginData=null, bool IsEncrypted=true)
        {
            var encryptedPwd = string.Empty;
            try
            {
                var ts = PasswordGenerator.GetTimeStamp();
                if(preLoginData is null)
                    preLoginData =  await GetPreLoginData();
                var generated = PasswordGenerator.Encrypt(Password, preLoginData.KeyID, preLoginData.PublicKey, ts,false);
                encryptedPwd = $"#PWD_INSTAGRAM_BROWSER:{preLoginData.Version}:{ts}:{generated}";
            }
            catch(Exception) { }
            return IsEncrypted ? Uri.EscapeDataString(encryptedPwd) : encryptedPwd;
        }
        public static async Task<PreLoginData> GetPreLoginData()
        {
            var loginData = new PreLoginData();
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    var request = new HttpRequestMessage(HttpMethod.Get, "https://www.instagram.com/api/v1/web/data/shared_data/");

                    // Add headers
                    request.Headers.Add("Host", "www.instagram.com");
                    request.Headers.Add("Connection", "keep-alive");
                    request.Headers.Add("Cache-Control", "max-age=0");
                    request.Headers.Add("dpr", "1.1");
                    request.Headers.Add("viewport-width", "1745");
                    request.Headers.Add("sec-ch-ua", "\"Not)A;Brand\";v=\"8\", \"Chromium\";v=\"138\", \"Google Chrome\";v=\"138\"");
                    request.Headers.Add("sec-ch-ua-mobile", "?0");
                    request.Headers.Add("sec-ch-ua-platform", "\"Windows\"");
                    request.Headers.Add("sec-ch-ua-platform-version", "\"10.0.0\"");
                    request.Headers.Add("sec-ch-ua-model", "\"\"");
                    request.Headers.Add("sec-ch-ua-full-version-list", "\"Not)A;Brand\";v=\"8.0.0.0\", \"Chromium\";v=\"138.0.7204.96\", \"Google Chrome\";v=\"138.0.7204.96\"");
                    request.Headers.Add("sec-ch-prefers-color-scheme", "light");
                    request.Headers.Add("Upgrade-Insecure-Requests", "1");
                    request.Headers.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/138.0.0.0 Safari/537.36");
                    request.Headers.Add("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/avif,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.7");
                    request.Headers.Add("Sec-Fetch-Site", "none");
                    request.Headers.Add("Sec-Fetch-Mode", "navigate");
                    request.Headers.Add("Sec-Fetch-User", "?1");
                    request.Headers.Add("Sec-Fetch-Dest", "document");
                    request.Headers.Add("Accept-Encoding", GramStatic.AcceptEncoding);
                    request.Headers.Add("Accept-Language", "en-GB,en;q=0.9");
                    // Send request
                    var response = await client.SendAsync(request);
                    var ResponseData = await HttpHelper.Decode(response);
                    loginData = new PreLoginData(ResponseData);
                }
            }
            catch { }
            return loginData;
        }
    }
}
