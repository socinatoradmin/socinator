using System;
using System.Net;
using System.Text;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Request;

namespace TwtDominatorCore.Requests
{
    public interface ITdHttpHelper : IHttpHelper
    {
    }

    public class TdHttpHelper : HttpHelper, ITdHttpHelper
    {
        public TdHttpHelper() : base(new TdRequestParameters())
        {
        }

        protected int RequestCount { get; set; }

        private string Url { get; set; }

        private byte[] PostData { get; set; }

        public override IResponseParameter PostRequest(string url, byte[] postData)
        {
            Url = url;
            PostData = postData;
            RequestCount = 0;
            return base.PostRequest(url, postData);
        }

        protected override void WritePostData(ref HttpWebRequest webRequest, string postData)
        {
            try
            {
                //string postdata = string.Format(postData);
                var postBuffer = Encoding.GetEncoding(1252).GetBytes(postData);
                WritePostData(ref webRequest, postBuffer);
            }
            catch (Exception)
            {
            }
        }

        protected override void WritePostData(ref HttpWebRequest webRequest, byte[] postBuffer)
        {
            try
            {
                //ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls;

                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls
                                                       | SecurityProtocolType.Tls11
                                                       | SecurityProtocolType.Tls12;

                webRequest.UseDefaultCredentials = true;
                webRequest.Method = "POST";
                webRequest.Timeout = 10000;
                webRequest.ContentLength = postBuffer.Length;
                using (var postDataStream = webRequest.GetRequestStreamAsync().Result)
                {
                    postDataStream.Write(postBuffer, 0, postBuffer.Length);
                    postDataStream.Close();
                }
            }
            catch (Exception)
            {
            }
        }


        protected override void SetRequestParametersToWebRequest(ref HttpWebRequest webRequest,
            IRequestParameters requestParameter)
        {
            try
            {
                if (requestParameter == null)
                    return;

                #region Set the Headers

                //ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls;


                //  ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls
                                                       | SecurityProtocolType.Tls11
                                                       | SecurityProtocolType.Tls12;

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
                        catch (Exception)
                        {
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
                        catch (Exception)
                        {
                        }
                }

                #endregion

                #region Set the Proxy

                webRequest.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
                if (!string.IsNullOrEmpty(requestParameter.Proxy?.ProxyIp))
                    SetProxy(ref webRequest, requestParameter);

                #endregion
            }
            catch (Exception)
            {
            }
        }
    }
}