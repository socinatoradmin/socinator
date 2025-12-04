using System;
using System.IO;
using System.Net;
using DominatorHouseCore;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Request;
using DominatorHouseCore.Utility;

namespace QuoraDominatorCore.Request
{
    public interface IQdHttpHelper : IHttpHelper
    {
    }

    public class QdHttpHelper : HttpHelper, IQdHttpHelper
    {
        private const int MaxRequestRetry = 3;

        public QdHttpHelper() : base(new QdRequestParameters())
        {
        }

        protected int RequestCount { get; set; }

        public string Url { get; set; }

        private byte[] PostData { get; set; }


        public override IResponseParameter PostRequest(string url, byte[] postData)
        {
            Url = url;
            PostData = postData;
            RequestCount = 0;
            return base.PostRequest(url, postData);
        }


        public IResponseParameter PostRequestIteration(string url, byte[] postData)
        {
            RequestCount++;
            return base.PostRequest(url, postData);
        }


        protected override IResponseParameter GetFinalResponse()
        {
            try
            {
                return GetReponse((HttpWebResponse) Request.GetResponse());
            }
            catch (WebException ex1)
            {
                try
                {
                    if (RequestCount >= MaxRequestRetry)
                        return new ResponseParameter
                        {
                            HasError = true,
                            Exception = ex1
                        };

                    if (ex1.Status == WebExceptionStatus.ProtocolError)
                    {
                        ReadCookies(ex1.Response);
                        var statusCode = (int) ((HttpWebResponse) ex1.Response).StatusCode;
                        ex1.DebugLog();
                        using (var responseStream = ex1.Response.GetResponseStream())
                        {
                            ReadCookies(ex1.Response);
                            using (var streamReader = new StreamReader(responseStream))
                            {
                                var end = streamReader.ReadToEnd();
                                if (end.IsValidJson() || end == "Oops, an error occurred.")
                                {
                                    ex1.DebugLog();
                                    if (statusCode == 500)
                                        return PostRequestIteration(Url, PostData);
                                    return new ResponseParameter
                                    {
                                        Response = end
                                    };
                                }

                                if (statusCode == 502 || statusCode == 503 || statusCode == 500)
                                    return new ResponseParameter
                                    {
                                        HasError = true,
                                        Exception = ex1
                                    };
                                ex1.DebugLog();
                                throw;
                            }
                        }
                    }

                    ex1.DebugLog();
                    return new ResponseParameter
                    {
                        HasError = true,
                        Exception = ex1
                    };
                }
                catch (WebException ex2)
                {
                    if (RequestCount >= MaxRequestRetry)
                        return new ResponseParameter
                        {
                            HasError = true,
                            Exception = ex2
                        };

                    ex2.DebugLog();
                    return PostRequestIteration(Url, PostData);
                }
            }
            catch (Exception ex)
            {
                return new ResponseParameter
                {
                    HasError = true,
                    Exception = ex
                };
            }
        }
    }
}