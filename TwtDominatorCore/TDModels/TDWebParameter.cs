using System;
using System.Net;
using System.Net.Http;

namespace TwtDominatorCore.TDModels
{
    public class TDWebParameter
    {
        public CookieContainer cookieContainer { get; set; } = new CookieContainer();
        public string CsrfToken {  get; set; }
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
            foreach (Cookie cookie in cookieContainer.GetCookies(new Uri("https://x.com/")))
            {
                collection.Add(cookie);
            }
            return collection;
        }
    }
}
