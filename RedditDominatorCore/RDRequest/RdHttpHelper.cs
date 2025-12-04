using DominatorHouseCore;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Request;
using System;
using System.Net;

namespace RedditDominatorCore.RDRequest
{
    public interface IRdHttpHelper : IHttpHelper
    {
    }

    public class RdHttpHelper : HttpHelper, IRdHttpHelper
    {
        private static readonly object LockObject = new object();

        public RdHttpHelper() : base(new RequestParameter())
        {
        }

        protected int RequestCount { get; set; }

        private string Url { get; set; }

        private byte[] PostData { get; set; }
        protected override void SetRequestParametersToWebRequest(ref HttpWebRequest webRequest,
            IRequestParameters requestParameter)
        {
            try
            {
                if (requestParameter == null)
                    return;

                #region Set the Headers

                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                webRequest.Headers = new WebHeaderCollection();

                if (requestParameter.Headers != null)
                    foreach (var eachHeader in requestParameter.Headers)
                        try
                        {
                            var headerName = eachHeader.ToString();

                            var headerValue = requestParameter.Headers[headerName];

                            if (headerName == "X-CSRFToken")
                            {
                                var token = requestParameter.Cookies["csrftoken"]?.Value;
                                webRequest.Headers.Add(eachHeader.ToString(), token);
                            }
                            else
                            {
                                if (!WebHeaderCollection.IsRestricted(headerName))
                                    webRequest.Headers.Add(headerName, headerValue);
                            }
                        }
                        catch (Exception ex)
                        {
                            ex.DebugLog();
                        }

                webRequest.Host = webRequest.RequestUri.Host;
                webRequest.KeepAlive = requestParameter.KeepAlive;
                webRequest.UserAgent = requestParameter.UserAgent;
                webRequest.ContentType = requestParameter.ContentType;
                webRequest.Referer = requestParameter.Referer;
                webRequest.Accept = requestParameter.Accept;

                #endregion

                if (ServicePointManager.Expect100Continue) ServicePointManager.Expect100Continue = false;

                #region Set the Cookies

                if (requestParameter.Cookies != null)
                {
                    webRequest.CookieContainer = new CookieContainer();

                    foreach (Cookie eachCookie in RequestParameters.Cookies)
                        try
                        {
                            var cookieData = new Cookie(eachCookie.Name, eachCookie.Value, "/", eachCookie.Domain);
                            webRequest.CookieContainer.Add(cookieData);
                        }
                        catch (Exception ex)
                        {
                            ex.DebugLog();
                        }
                }

                #endregion

                #region Set the Proxy

                webRequest.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
                if (!string.IsNullOrEmpty(requestParameter.Proxy?.ProxyIp))
                    SetProxy(ref webRequest, requestParameter);

                #endregion
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }
    }
}