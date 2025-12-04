using System;
using System.IO;
using System.Net;
using System.Threading;
using System.Web;
using DominatorHouseCore;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Request;

namespace LinkedDominatorCore.Request
{
    public interface ILdHttpHelper : IHttpHelper
    {
        string WebClientGetRequest(string url);
        IResponseParameter PutRequest(string url, byte[] mediaByte);
        IResponseParameter DeleteRequest(string url);
        IResponseParameter HandleGetResponse(string url);
        IResponseParameter HandlePostResponse(string url, string postData);
        bool DownloadFile(string url, string filePath);
    }

    public class LdHttpHelper : HttpHelper, ILdHttpHelper
    {
        public LdHttpHelper() : base(new LdRequestParameters())
        {
        }

        protected int RequestCount { get; set; }


        public override IResponseParameter PostRequest(string url, byte[] postData)
        {
            RequestCount = 0;
            return base.PostRequest(url, postData);
        }

        public IResponseParameter DeleteRequest(string url)
        {
            try
            {
                Request = (HttpWebRequest) WebRequest.Create(url);
                Request.Method = "DELETE";
                var httpWebRequest = Request;
                SetRequestParametersToWebRequest(ref httpWebRequest, RequestParameters);
                return GetFinalResponse();
            }
            catch (Exception ex)
            {
                ex.ErrorLog();
                return null;
            }
        }

        public IResponseParameter PutRequest(string url, byte[] mediaByte)
        {
            try
            {
                Request = (HttpWebRequest) WebRequest.Create(url);
                Request.Method = "PUT";
                Request.Timeout = 600000;
                var httpWebRequest = Request;
                SetRequestParametersToWebRequest(ref httpWebRequest, RequestParameters);
                try
                {
                    Request.ContentLength = mediaByte.Length;
                    using (var postDataStream = Request.GetRequestStream())
                    {
                        postDataStream.Write(mediaByte, 0, mediaByte.Length);
                    }
                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                }

                return GetFinalResponse();
            }
            catch (Exception ex)
            {
                ex.ErrorLog();
            }
            finally
            {
                try
                {
                    Request.Timeout = 100000;
                }
                catch (Exception)
                {
                }
            }

            return GetFinalResponse();
        }

        public string WebClientGetRequest(string url)
        {
            try
            {
                var dataResult = string.Empty;
                //SetRequestParametersToWebRequest(ref Request, RequestParameters);
                try
                {
                    var webClient = new WebClient();
                    webClient.Headers.Clear();
                    webClient.Headers.Add("Host", "www.linkedin.com");
                    webClient.Headers.Add("Connection", "Keep-Alive");
                    webClient.Headers.Add("User-Agent", "ANDROID OS");
                    webClient.Headers.Add("X-UDID", "fc61c979-3c11-4cc0-9537-b08a3dbfcc87");
                    webClient.Headers.Add("X-RestLi-Protocol-Version", "2.0.0");
                    webClient.Headers.Add("Accept-Language", "en-US");
                    try
                    {
                        webClient.Headers.Add("Csrf-Token",
                            GetRequestParameter().Cookies["JSESSIONID"].Value.Replace("\"", ""));
                    }
                    catch (Exception ex)
                    {
                        ex.DebugLog();
                    }

                    webClient.Headers.Add("ContentType", @"application/x-www-form-urlencoded");
                    webClient.Headers.Add("X-LI-Track",
                        "{\"model\":\"samsung_SM-G925F\",\"appId\":\"com.linkedin.android\",\"osVersion\":\"4.4.2\",\"mpName\":\"voyager-android\",\"timezoneOffset\":5,\"mpVersion\":\"0.225.25\",\"clientMinorVersion\":106032,\"deviceType\":\"android\",\"isAdTrackingLimited\":false,\"dpi\":\"hdpi\",\"storeId\":\"us_googleplay\",\"clientVersion\":\"4.1.152\",\"deviceId\":\"fc61c979-3c11-4cc0-9537-b08a3dbfcc87\",\"osName\":\"Android OS\"}");
                    webClient.Headers.Add("X-LI-User-Agent",
                        "LIAuthLibrary:0.0.3 com.linkedin.android:4.1.152 samsung_SM-G925F:android_4.4.2");
                    webClient.Headers.Add("X-LI-Lang", "en-US");
                    if (!string.IsNullOrEmpty(RequestParameters.Proxy.ProxyIp))
                    {
                        var webProxy = new WebProxy(RequestParameters.Proxy.ProxyIp,
                            int.Parse(RequestParameters.Proxy.ProxyPort))
                        { BypassProxyOnLocal=true};
                        if (!string.IsNullOrEmpty(RequestParameters.Proxy.ProxyUsername))
                            webProxy.Credentials = new NetworkCredential(RequestParameters.Proxy.ProxyUsername,
                                RequestParameters.Proxy.ProxyPassword);
                        webClient.Proxy = webProxy;
                    }

                    var cookieData = string.Empty;
                    foreach (Cookie itemCookie in RequestParameters.Cookies)
                        cookieData = cookieData + itemCookie.Name + "=" + itemCookie.Value + ";";

                    webClient.Headers.Add("Cookie", cookieData);

                    try
                    {
                        dataResult = webClient.DownloadString(url);
                    }
                    catch (Exception ex)
                    {
                        ex.DebugLog();
                        try
                        {
                            dataResult = webClient.DownloadString(url);
                        }
                        catch
                        {
                            dataResult = "Please Check Headers";
                        }
                    }

                    dataResult = HttpUtility.HtmlDecode(dataResult);
                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                }

                return dataResult;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
                return null;
            }
        }

        public IResponseParameter HandleGetResponse(string postUrl)
        {
            try
            {
                IResponseParameter reqParameter = null;
                for (var index = 0;
                    index < 3 && (reqParameter == null || string.IsNullOrEmpty(reqParameter.Response));
                    index++)
                {
                    if (index > 0) Thread.Sleep(new Random().Next(5000, 8000));
                    reqParameter = GetRequest(postUrl);
                }

                return reqParameter;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
                return null;
            }
        }

        public bool DownloadFile(string url, string filePath)
        {
            try
            {
                byte[] b = null;

                var webRequest = (HttpWebRequest) WebRequest.Create(url);
                SetRequestParametersToWebRequest(ref webRequest, RequestParameters);

                using (var webResponse = (HttpWebResponse) webRequest.GetResponse())
                {
                    using (var input = webResponse.GetResponseStream())
                    {
                        using (var ms = new MemoryStream())
                        {
                            var count = 0;
                            do
                            {
                                var buf = new byte[1024];
                                count = input.Read(buf, 0, 1024);
                                ms.Write(buf, 0, count);
                            } while (input.CanRead && count > 0);

                            b = ms.ToArray();
                        }
                    }
                }


                using (var fileStream = File.OpenWrite(filePath))
                {
                    fileStream.Write(b, 0, b.Length);
                }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
                return false;
            }

            return true;
        }

        public IResponseParameter HandlePostResponse(string actionUrl, string postString)
        {
            try
            {
                return PostRequest(actionUrl, postString);
            }
            catch (Exception ex)
            {
                ex.DebugLog();
                return null;
            }
        }


        protected override IResponseParameter GetFinalResponse()
        {
            try
            {
                // sometimes we getting 'Request' null   after prism changes
                // therefore we using '_request'
                var response = GetReponse((HttpWebResponse) _request.GetResponse());
                return response;
            }
            catch (WebException exception)
            {
                return new ResponseParameter
                {
                    HasError = true,
                    Exception = exception
                };
            }
        }
    }
}