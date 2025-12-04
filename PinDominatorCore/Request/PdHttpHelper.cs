using DominatorHouseCore;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Request;
using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;

namespace PinDominatorCore.Request
{
    public interface IPdHttpHelper : IHttpHelper
    {
        IResponseParameter LockedPostRequest(string url, byte[] postData);
    }

    public class PdHttpHelper : HttpHelper, IPdHttpHelper
    {
        public static readonly object LockObject = new object();

        protected int RequestCount { get; set; }


        public override IResponseParameter PostRequest(string url, byte[] postData)
        {
            RequestCount = 0;
            return base.PostRequest(url, postData);
        }


        public IResponseParameter LockedPostRequest(string url, byte[] postData)
        {
            lock (LockObject)
            {
                return base.PostRequest(url, postData);
            }
        }


        public IResponseParameter PostRequestIteration(string url, byte[] postData)
        {
            RequestCount++;
            return base.PostRequest(url, postData);
        }

        /// <summary>
        ///     Get the final response data from <see cref="System.Net.HttpWebResponse" /> objects to
        ///     <see cref="IResponseParameter" />
        /// </summary>
        /// <returns></returns>
        protected override IResponseParameter GetReponse(HttpWebResponse webResponse)
        {
            // pointing same address Response and webResponse
            Response = webResponse;

            // Read the cookies from webresponse to RequestParameter
            ReadCookies(Response);

            // Get the streams from Response
            var responseStream = Response.GetResponseStream();

            if (Response.ContentEncoding == "gzip")
                using (var decompress = new GZipStream(responseStream, CompressionMode.Decompress))
                using (var sr = new StreamReader(decompress))
                {
                    return new ResponseParameter
                    {
                        Response = sr.ReadToEnd()
                    };
                }

            // Check null integrity
            if (responseStream == null)
                return new ResponseParameter {Response = string.Empty};

            // return as proper ResponseParameter with appropriate reponse
            using (var streamReader = new StreamReader(responseStream))
            {
                return new ResponseParameter
                {
                    Response = streamReader.ReadToEnd()
                };
            }
        }

        /// <summary>
        ///     Read the cookies from <see cref="System.Net.WebResponse" /> object to RequestParameters
        /// </summary>
        /// <param name="webResponse">
        ///     <see cref="System.Net.WebResponse" />
        /// </param>
        protected override void ReadCookies(WebResponse webResponse)
        {
            try
            {
                if (webResponse == null) return;

                var response = webResponse as HttpWebResponse;

                if (RequestParameters.Cookies == null) 
                    RequestParameters.Cookies = new CookieCollection();

                if (response == null) return;
                var cookies = _request.CookieContainer.GetCookies(_request.RequestUri);
                var respcookies = _request.CookieContainer.GetCookies(webResponse.ResponseUri);
                cookies.Add(respcookies);

                    foreach (Cookie cookie in cookies)
                    {                   
                    // check the current cookie is any already present in RequestParameter
                    
                        var isPresent =
                            RequestParameters.Cookies.Cast<Cookie>()
                                .Any(requestParameterCookie => requestParameterCookie.Name == cookie.Name);
                    
                        // If its present read then overwrite otherwise add to RequestParameter
                        if (isPresent)
                        {
                            if (!string.IsNullOrEmpty(RequestParameters.Cookies[cookie.Name]?.Value))
                            {
                            if (cookie.Name != "_auth" || cookie.Name == "_auth" && cookie.Value != "0")
                                if (cookie.Name == "_pinterest_sess" && cookie.Value.Length > 400)
                                {
                                    RequestParameters.Cookies[cookie.Name].Value = cookie.Value;
                                }
                              if (cookie.Name != "_pinterest_sess")
                              {
                                RequestParameters.Cookies[cookie.Name].Value = cookie.Value;
                              }
                            RequestParameters.Cookies[cookie.Name].Domain = cookie.Domain;
                            }
                        }
                        else
                        {
                            RequestParameters.Cookies.Add(cookie);
                        }
                    }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }
    }
}