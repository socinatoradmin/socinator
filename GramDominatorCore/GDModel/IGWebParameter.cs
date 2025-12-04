using System;
using System.Net;
using System.Net.Http;

namespace GramDominatorCore.GDModel
{
    public class IGWebParameter
    {
        public CookieContainer cookieContainer { get; set; } = new CookieContainer();
        public string X_IG_Claim { get; set; } = string.Empty;
        public string CsrfToken { get; set; } = string.Empty;
        public string Jazoest { get; set; } = string.Empty;
        public string Authorization { get; set; } = string.Empty;
        public string DsUserId { get; set; } = string.Empty;
        public string MID { get; set; } = string.Empty;
        public HttpClientHandler httpClient { get; set; } = new HttpClientHandler()
        {
            UseCookies = true,
            CookieContainer = new CookieContainer(),
            AllowAutoRedirect = true,
            AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
        };
        public CookieCollection GetCookies()
        {
            var collection = new CookieCollection();
            foreach (Cookie cookie in cookieContainer.GetCookies(new Uri("https://www.instagram.com/")))
            {
                collection.Add(cookie);
            }
            return collection;
        }
    }
}
