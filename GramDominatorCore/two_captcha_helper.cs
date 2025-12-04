using DominatorHouseCore.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace GramDominatorCore.CaptchaSolver
{
    public class two_captcha_helper
    {
        private readonly string _token;
        public two_captcha_helper(string token)
        {
            _token = "df33aabde6b1eb09269bfa746c17b6c8";
        }
        public async Task<string> SubmitSiteKey(string siteKey, string siteUrl)
        {
            var url = $"https://2captcha.com/in.php?key={_token}&method=userrecaptcha&googlekey={siteKey}&pageurl={siteUrl}&json=1";
            WebClient client = new WebClient();
            var res = await client.DownloadStringTaskAsync(url);
            var json = new JsonHandler(res);
            return json.GetElementValue("request")??"";
        }

        public async Task<string> GetCaptchaResponse(string capthaId)
        {
            string gResponse = string.Empty;
            var url = $"https://2captcha.com/res.php?key={_token}&action=get&id={capthaId}&json=1";
            do
            {
                await Task.Delay(5000);
                WebClient client = new WebClient();
                gResponse = client.DownloadString(url);

            } while (gResponse != null && gResponse.Contains("CAPCHA_NOT_READY"));
            var json = new JsonHandler(gResponse);
            gResponse = json.GetElementValue("request");

            return gResponse;

        }
    }
}
