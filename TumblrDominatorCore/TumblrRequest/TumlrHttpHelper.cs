using DominatorHouseCore;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Request;
using System;
using System.Net;

namespace TumblrDominatorCore.TumblrRequest
{
    public interface ITumblrHttpHelper : IHttpHelper
    {
        string GetXCsrfTokenFromResp();
    }

    public class TumblrHttpHelper : HttpHelper, ITumblrHttpHelper
    {
        // public TumblrRequestParameter TumblrRequestparameter { get; set; }


        public TumblrHttpHelper() : base(new TumblrRequestParameter())
        {
        }

        //public TumblrHttpHelper(TumblrRequestParameter tumblrRequestParameter) : base(tumblrRequestParameter)
        //{
        //    try
        //    {
        //        TumblrRequestparameter = tumblrRequestParameter;
        //    }
        //    catch (Exception ex)
        //    {
        //        ex.DebugLog();
        //    }
        //}

        public string TumblrSecurityKey { get; set; }
        public string TumblrCsrfKey { get; set; }

        protected override IResponseParameter GetReponse(HttpWebResponse webResponse)
        {
            var response = base.GetReponse(webResponse);
            TumblrSecurityKey = string.Empty;
            if (Response != null && string.Equals(Response.GetResponseHeader("X-Tumblr-Secure-Form-Key"), null,
                    StringComparison.Ordinal)) return response;
            if (Response != null)
            {

                TumblrSecurityKey = Response.GetResponseHeader("X-Tumblr-Secure-Form-Key");
                TumblrCsrfKey = "csrf :" + Response.GetResponseHeader("X-Csrf") + "\"";
                response.Response += TumblrCsrfKey;
            }
            if (string.IsNullOrEmpty(TumblrSecurityKey)) return response;
            SeRequestParameters();
            return response;
        }

        public IRequestParameters SeRequestParameters()
        {
            RequestParameters.Headers.Remove("X-tumblr-puppies");
            if (string.IsNullOrEmpty(TumblrSecurityKey)) return RequestParameters;
            RequestParameters.AddHeader("X-tumblr-puppies", TumblrSecurityKey);
            return RequestParameters;
        }
        protected override void SetRequestParametersToWebRequest(ref HttpWebRequest webRequest,
            IRequestParameters requestParameter)
        {
            try
            {
                if (requestParameter == null)
                    return;

                #region Set the Headers

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

                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls
                                                       | SecurityProtocolType.Tls11
                                                       | SecurityProtocolType.Tls12
                                                       | SecurityProtocolType.Ssl3;

                if (ServicePointManager.Expect100Continue) ServicePointManager.Expect100Continue = false;

                #region Set the Cookies

                if (requestParameter.Cookies != null)
                {
                    webRequest.CookieContainer = new CookieContainer();

                    foreach (Cookie eachCookie in RequestParameters.Cookies)
                        try
                        {
                            if (eachCookie.Name.Contains("Etag")) continue;
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

                if (!string.IsNullOrEmpty(requestParameter.Proxy?.ProxyIp))
                    SetProxy(ref webRequest, requestParameter);

                #endregion
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }
        public string GetXCsrfTokenFromResp()
        {
            return Response.Headers["X-Csrf"];
        }
    }
}