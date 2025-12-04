using DominatorHouseCore;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Request;
using DominatorHouseCore.Utility;
using GramDominatorCore.GDUtility;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace GramDominatorCore.Request
{
    public interface IGdHttpHelper : IHttpHelper
    {
        string GetResponseHeader(string header);
    }
    public class IgHttpHelper : HttpHelper, IGdHttpHelper
    {
        public IgHttpHelper() : base(new IgRequestParameters())
        {

        }

        private const int MaxRequestRetry = 1;

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

        public string GetResponseHeader(string header)
        {
            return Response.Headers[header];
        }
        public IResponseParameter PostRequestIteration(string url, byte[] postData)
        {
            RequestCount++;
            return base.PostRequest(url, postData);
        }

        protected override IResponseParameter GetFinalResponse()
        {
            // string finalResponse = string.Empty;
            try
            {
                //var sw = new Stopwatch();
                //sw.Start();
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls |
                    SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
                return GetReponse((HttpWebResponse)_request.GetResponse());

                //var elapsed = sw.Elapsed.TotalSeconds;
                //var nics = System.Net.NetworkInformation.NetworkInterface.GetAllNetworkInterfaces();
                //var nic = nics.Single(n => n.Name == "Local Area Connection");
                //var lastBr = nic.GetIPv4Statistics().BytesReceived;
            }
            catch (WebException ex1)
            {
                try
                {

                    if (RequestCount >= MaxRequestRetry)
                    {
                        return new ResponseParameter()
                        {
                            HasError = true,
                            Exception = ex1
                        };
                    }

                    if (ex1.Status == WebExceptionStatus.ProtocolError)
                    {
                        // ReadCookies(ex1.Response);
                        int statusCode = (int)((HttpWebResponse)ex1.Response).StatusCode;
                        // GlobusLogHelper.log.Debug($"Response protocol error: {statusCode}");
                        ex1.DebugLog($"Response protocol error: {statusCode}");
                        using (Stream responseStream = ex1.Response.GetResponseStream())
                        {
                            // ReadCookies(ex1.Response);
                            using (StreamReader streamReader = new StreamReader(responseStream))
                            {
                                string end = streamReader.ReadToEnd();
                                if (end.IsValidJson() || end == "Oops, an error occurred.")
                                {
                                    GlobusLogHelper.log.Debug($"Response error received: {end}");
                                    if (statusCode == 500)
                                        return PostRequestIteration(Url, PostData);
                                    return new ResponseParameter()
                                    {
                                        Response = end
                                    };
                                }

                                if (statusCode == 502 || statusCode == 503 || statusCode == 500 || statusCode == 404)
                                    return new ResponseParameter()
                                    {
                                        HasError = true,
                                        Exception = ex1
                                    };
                                ex1.DebugLog($"Unhandled status code: {statusCode} - Message: {ex1.Message}");
                                throw;
                            }
                        }
                    }
                    else
                    {
                        ex1.DebugLog("Response non-protocol error");
                        return new ResponseParameter()
                        {
                            HasError = true,
                            Exception = ex1
                        };
                    }
                }
                catch (WebException ex2)
                {
                    if (RequestCount >= MaxRequestRetry)
                    {
                        return new ResponseParameter()
                        {
                            HasError = true,
                            Exception = ex2
                        };
                    }

                    GlobusLogHelper.log.Debug(
                        $"An error occured in exception handler for the webrequest. Code: {(int)((HttpWebResponse)ex2.Response).StatusCode}");
                    return PostRequestIteration(Url, PostData);
                }
            }
            catch (Exception Ex)
            {
                return new ResponseParameter()
                {
                    HasError = true,
                    Exception = Ex
                };
            }

        }

        protected override async Task<IResponseParameter> GetFinalResponseAsync(CancellationToken cancellationToken)
        {
            if (cancellationToken == default(CancellationToken))
            {
                return await DoGetFinalResponseAsync();
            }

            using (cancellationToken.Register(() => _request.Abort()))
            {
                return await DoGetFinalResponseAsync(() => cancellationToken.IsCancellationRequested);
            }

        }

        private async Task<IResponseParameter> DoGetFinalResponseAsync(Func<bool> wasCancelled = null)
        {
            try
            {
                //_elapsedTime = DateTimeUtilities.GetCurrentEpochTimeMilliSeconds();
                
                // Get the reponse from request
                return GetReponse((HttpWebResponse)await _request.GetResponseAsync());
            }
            catch (WebException ex)
            {
                if (wasCancelled != null && wasCancelled())
                {
                    throw new OperationCanceledException();
                }
                try
                {
                    // Get error message from the response
                    return ex.Response != null
                        ? GetReponse((HttpWebResponse)ex.Response)
                        : new ResponseParameter
                        {
                            HasError = true,
                            Exception = ex
                        };
                }
                catch (WebException exception)
                {
                    // return the exceptions of response in ResponseParameter
                    return new ResponseParameter()
                    {
                        HasError = true,
                        Exception = exception
                    };
                }
            }
            catch (Exception ex)
            {
                // return the actual exceptions in ResponseParameter
                return new ResponseParameter()
                {
                    HasError = true,
                    Exception = ex
                };
            }
        }

        protected override IResponseParameter GetReponse(HttpWebResponse webResponse)
        {
            // pointing same address Response and webResponse
            Response = webResponse;

            // Read the cookies from webresponse to RequestParameter
            ReadCookies(Response);
            // Get the streams from Response
            var responseStream = Response.GetResponseStream();
            return base.GetDecodedResponse(responseStream);
        }


        protected override void ReadCookies(WebResponse webResponse)
        {
            try
            {
                if (webResponse == null) return;

                var response = webResponse as HttpWebResponse;

                if (RequestParameters.Cookies == null)
                {
                    RequestParameters.Cookies = new CookieCollection();
                }

                if (response == null) return;
                    CookieCollection cookies = new CookieCollection();

                if (_request.CookieContainer != null)
                {
                    cookies = _request.CookieContainer.GetCookies(_request.RequestUri);
                   // RequestParameters.Cookies = new CookieCollection();
                }

                foreach (Cookie cookie in cookies)
                {
                    // check the current cookie is any already present in RequestParameter
                    var isPresent =
                       RequestParameters.Cookies.Cast<Cookie>()
                           .Any(requestParameterCookie => requestParameterCookie.Name == cookie.Name);
                    // If its present read then overwrite otherwise add to RequestParameter
                    if (isPresent)
                    {
                        if ((!string.IsNullOrEmpty(RequestParameters.Cookies[cookie.Name]?.Value) && (RequestParameters.Cookies[cookie.Name]?.Value != cookie.Value)))
                            RequestParameters.Cookies[cookie.Name].Value = cookie.Value;
                    }
                    else
                        RequestParameters.Cookies.Add(cookie);
                }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        protected override void SetRequestParametersToWebRequest(ref HttpWebRequest webRequest, IRequestParameters requestParameter)
        {
            try
            {
                if (requestParameter == null)
                    return;

                #region Set the Headers

                webRequest.Headers = new WebHeaderCollection();

                if (requestParameter.Headers != null)
                {
                    foreach (var eachHeader in requestParameter.Headers)
                    {
                        try
                        {
                            var headerName = eachHeader.ToString();
                           
                            var headerValue = requestParameter.Headers[headerName];

                            if (headerName == "X-CSRFToken")
                            {
                                var token = requestParameter.Cookies["csrftoken"]?.Value;
                                webRequest.Headers.Add(eachHeader.ToString(), token);
                            }
                            else if (headerName == "X-Pigeon-Rawclienttime")
                            {
                                string Rawclienttime = GdUtilities.GetRowClientTime();
                                webRequest.Headers["X-Pigeon-Rawclienttime"] = Rawclienttime;
                            }
                            else
                            {
                                if (!WebHeaderCollection.IsRestricted(headerName))
                                    webRequest.Headers.Add(headerName, headerValue);
                            }
                        }
                        catch (Exception ex)
                        {
                            ex.DebugLog(ex.StackTrace);
                        }
                    }
                }

                webRequest.Host = requestParameter.Headers["Host"] ?? webRequest.Host;
                webRequest.KeepAlive = requestParameter.KeepAlive;
                webRequest.UserAgent = requestParameter.UserAgent;
                webRequest.ContentType = requestParameter.ContentType;
                // webRequest.ContentType =  requestParameter.ContentType;
                // webRequest.Connection ="Keep-Alive";
                //webRequest.Accept = requestParameter.Headers["Accept"]??requestParameter.Accept;
               // webRequest.Referer = "https://i.instagram.com/challenge/35352206758/BL6qsxkcw5/";
                // webRequest.Timeout =300000;
                // webRequest.Date = DateTime.Now;
                // webRequest.Connection = requestParameter.Headers["Keep-Alive"];
                //webRequest.Expect = null;
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
                #endregion

                //if (ServicePointManager.Expect100Continue)
                //{
                //    ServicePointManager.Expect100Continue = true;
                //}

                #region Set the Cookies

                if (requestParameter.Cookies != null)
                {
                    webRequest.CookieContainer = new CookieContainer();

                    foreach (Cookie eachCookie in RequestParameters.Cookies)
                    {
                        try
                        {
                            if (string.IsNullOrEmpty(eachCookie.Domain))
                            {
                                eachCookie.Domain = ".instagram.com";
                                eachCookie.Path = "/";
                            }
                            //var cookieData = new Cookie(eachCookie.Name, eachCookie.Value, "/", eachCookie.Domain);
                            if (eachCookie.Expires < DateTime.Now && (eachCookie.Name == "shbid" || eachCookie.Name == "shbts"))
                            {
                                DateTime date = DateTime.Now;
                                eachCookie.Expires = date.AddDays(6).AddHours(18).AddMinutes(30);
                            }
                            //cookieData.Expires = eachCookie.Expires;
                            webRequest.CookieContainer.Add(eachCookie);
                        }
                        catch (Exception ex)
                        {
                            ex.DebugLog(ex.StackTrace);
                        }
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
                ex.DebugLog(ex.StackTrace);
            }
        }
    }
}
