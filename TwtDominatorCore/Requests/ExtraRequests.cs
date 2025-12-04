using System;
using System.Collections.Specialized;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using DominatorHouseCore;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Utility;
using TwtDominatorCore.TDUtility;

namespace TwtDominatorCore.Requests
{
    public class ExtraRequests
    {
        private HttpWebRequest Request;
        private HttpWebResponse Response;

        public ExtraRequests()
        {
        }

        public ExtraRequests(ITdHttpHelper tdHttpHelper)
        {
            requestParameter = tdHttpHelper.GetRequestParameter();
        }

        private IRequestParameters requestParameter { get; }

        public string PostRequest(string url, string PostUrl, string postData, bool CompressionAllowed,
            string Referer = null)
        {
            try
            {
                Request = (HttpWebRequest) WebRequest.Create(url);
                Request.Headers.Add("Accept-Language", TdConstants.AcceptLanguage);
                if (CompressionAllowed)
                    Request.Headers.Add("Accept-Encoding", TdConstants.AcceptEnocoding);
                Request.Headers.Add("Upgrade-Insecure-Requests", "1");
                Request.KeepAlive = true;
                Request.UserAgent = TdConstants.NewUserAgent;
                Request.Accept = TdConstants.AcceptHtml;
                var host = url.Replace("https", "").Replace("/", "").Replace("http", "").Replace(":", "");
                Request.Host = host;
                Request.Referer = Referer == null ? url : Referer;
                if (CompressionAllowed)
                    Request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
                if (ServicePointManager.Expect100Continue) ServicePointManager.Expect100Continue = false;
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls;
                string firstResponse;
                Response = (HttpWebResponse)Request.GetResponse();
                var cookies = Response.Headers["Set-Cookie"];
                var responseStream = Response.GetResponseStream();

                using (var streamReader = new StreamReader(responseStream))
                {
                    firstResponse = streamReader.ReadToEnd();
                }
           //     var firstResponse = GetFinalResponse((HttpWebResponse) Request.GetResponse());
                if (string.IsNullOrEmpty(firstResponse))
                    return "";
                var csrf_token = HtmlParseUtility.GetAttributeValueFromTagName(firstResponse, "input", "name", "csrf_token",
                    "value");
                postData = $"csrf_token={csrf_token}&" + postData;
                Request = (HttpWebRequest) WebRequest.Create(PostUrl);
                Request.ContentType = TdConstants.ContentType;
                Request.Referer = Referer == null ? url : Referer;
                Request.Headers["Cookie"] = cookies;
                var postBuffer = Encoding.UTF8.GetBytes(postData);
                WritePostData(postBuffer);
                var PostResponse = GetFinalResponse((HttpWebResponse) Request.GetResponse());
                return PostResponse;
            }
            catch (Exception ex)
            {
                ex.ErrorLog();
                return null;
            }
        }

        private string GetFinalResponse(HttpWebResponse webResponse)
        {
            try
            {
                string finalResponse;
                Response = webResponse;
                var responseStream = Response.GetResponseStream();

                using (var streamReader = new StreamReader(responseStream))
                {
                    finalResponse = streamReader.ReadToEnd();
                }

                return finalResponse;
            }
            catch (Exception)
            {
                return null;
            }
        }

        private void WritePostData(byte[] postBuffer)
        {
            try
            {
                Request.Method = "POST";
                Request.ContentLength = postBuffer.Length;
                using (var postDataStream = Request.GetRequestStream())
                {
                    postDataStream.Write(postBuffer, 0, postBuffer.Length);
                }
            }
            catch (Exception ex)
            {
                ex.ErrorLog();
            }
        }

        public void UploadVideoByMultipartFormData(string URL, string Referer, NameValueCollection nvc,
            string LastKey, string AppKey)
        {
            try
            {
                var boundary = "------WebKitFormBoundary" + DateTime.Now.Ticks.ToString("x");
                var FirstBoundaryBytes = Encoding.ASCII.GetBytes(boundary + "\r\n");
                var boundarybytes = Encoding.ASCII.GetBytes("\r\n" + boundary + "\r\n");
                Request = (HttpWebRequest) WebRequest.Create(URL);
                Request.KeepAlive = true;
                Request.Accept = "*/*";
                Request.UserAgent = requestParameter.UserAgent;
                Request.ContentType = "multipart/form-data; boundary=" + boundary.Replace("------", "----");
                Request.Referer = Referer;
                Request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
                Request.Headers["Accept-Language"] = "en-US,en;q=0.9";
                Request.Method = "POST";
                Request.Credentials = CredentialCache.DefaultCredentials;
                Request.AllowAutoRedirect = true;
                Request.CookieContainer = new CookieContainer();
                SetProxy(requestParameter);

                #region CookieManagment

                if (requestParameter.Cookies != null && requestParameter.Cookies.Count > 0)
                    Request.CookieContainer.Add(requestParameter.Cookies);

                #endregion

                using (var rs = Request.GetRequestStream())
                {
                    rs.Write(FirstBoundaryBytes, 0, FirstBoundaryBytes.Length);
                    var file = string.Empty;
                    var ContentType = string.Empty;
                    var formdataTemplate = "Content-Disposition: form-data; name=\"{0}\"\r\n\r\n{1}";
                    foreach (string key in nvc.Keys)
                        try
                        {
                            if (key.Equals(LastKey))
                            {
                                ContentType = Regex.Split(nvc[key], "<:><:><:>")[1];
                                file = Regex.Split(nvc[key], "<:><:><:>")[0];
                                if (key.Equals(LastKey))
                                {
                                    var headerTemplate =
                                        "Content-Disposition: form-data; name=\"{0}\"; filename=\"{1}\"\r\nContent-Type: {2}\r\n\r\n";
                                    var header = string.Format(headerTemplate, key, AppKey, ContentType);
                                    var headerbytes = Encoding.UTF8.GetBytes(header);
                                    rs.Write(headerbytes, 0, headerbytes.Length);
                                    try
                                    {
                                        var fileStream = new FileStream(file, FileMode.Open, FileAccess.Read);
                                        var buffer = new byte[4096];
                                        var bytesRead = 0;
                                        while ((bytesRead = fileStream.Read(buffer, 0, buffer.Length)) != 0)
                                            rs.Write(buffer, 0, bytesRead);
                                        fileStream.Close();
                                    }
                                    catch (Exception ex)
                                    {
                                        ex.DebugLog();
                                    }
                                }
                                else
                                {
                                    ContentType = Regex.Split(nvc[key], "<:><:><:>")[1];
                                    file = Regex.Split(nvc[key], "<:><:><:>")[0];
                                    var headerTemplate =
                                        "Content-Disposition: form-data; name=\"{0}\"; filename=\"{1}\"\r\nContent-Type: {2}\r\n\r\n";
                                    var filename = Regex.Split(file, "\\\\");
                                    var header = string.Format(headerTemplate, key, filename[filename.Length - 1],
                                        ContentType);
                                    var headerbytes = Encoding.UTF8.GetBytes(header);
                                    rs.Write(headerbytes, 0, headerbytes.Length);
                                    try
                                    {
                                        var fileStream = new FileStream(file, FileMode.Open, FileAccess.Read);
                                        var buffer = new byte[1000];
                                        int bytesRead;
                                        while ((bytesRead = fileStream.Read(buffer, 0, buffer.Length)) != 0)
                                            rs.Write(buffer, 0, bytesRead);
                                        fileStream.Close();
                                    }
                                    catch (Exception ex)
                                    {
                                        ex.ErrorLog();
                                    }

                                    rs.Write(boundarybytes, 0, boundarybytes.Length);
                                }
                            }
                            else
                            {
                                var formitem = string.Format(formdataTemplate, key, nvc[key]);
                                var formitembytes = Encoding.UTF8.GetBytes(formitem);
                                rs.Write(formitembytes, 0, formitembytes.Length);
                                if (key.Equals(LastKey))
                                {
                                }
                                else
                                {
                                    rs.Write(boundarybytes, 0, boundarybytes.Length);
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            ex.DebugLog();
                        }

                    var trailer = Encoding.ASCII.GetBytes("\r\n" + boundary + "--\r\n");
                    rs.Write(trailer, 0, trailer.Length);
                }

                WebResponse wresp = null;
                try
                {
                    wresp = Request.GetResponse();
                    var stream2 = wresp.GetResponseStream();
                    using (var reader2 = new StreamReader(stream2))
                    {
                        reader2.ReadToEnd();
                    }
                }
                catch (Exception ex)
                {
                    ex.ErrorLog();
                    wresp?.Dispose();
                }
                finally
                {
                    Request = null;
                }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        public void UploadVideoByMultipartFormDataTest(string URL, string Referer, NameValueCollection nvc,
            string LastKey, string AppKey, byte[] bufferbytes)
        {
            try
            {
                var boundary = "------WebKitFormBoundary" + DateTime.Now.Ticks.ToString("x");
                var FirstBoundaryBytes = Encoding.ASCII.GetBytes(boundary + "\r\n");
                var boundarybytes = Encoding.ASCII.GetBytes("\r\n" + boundary + "\r\n");
                Request = (HttpWebRequest) WebRequest.Create(URL);
                Request.KeepAlive = true;
                Request.Accept = "*/*";
                Request.UserAgent = requestParameter.UserAgent;
                Request.ContentType = "multipart/form-data; boundary=" + boundary.Replace("------", "----");
                Request.Referer = Referer;
                Request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
                Request.Headers["Accept-Language"] = "en-US,en;q=0.9";
                Request.Method = "POST";
                Request.Credentials = CredentialCache.DefaultCredentials;
                Request.AllowAutoRedirect = true;
                Request.CookieContainer = new CookieContainer();
                SetProxy(requestParameter);

                #region CookieManagment

                if (requestParameter.Cookies != null && requestParameter.Cookies.Count > 0)
                    Request.CookieContainer.Add(requestParameter.Cookies);

                #endregion

                using (var rs = Request.GetRequestStream())
                {
                    rs.Write(FirstBoundaryBytes, 0, FirstBoundaryBytes.Length);
                    var file = string.Empty;
                    string ContentType;
                    var formdataTemplate = "Content-Disposition: form-data; name=\"{0}\"\r\n\r\n{1}";
                    foreach (string key in nvc.Keys)
                        try
                        {
                            if (key.Equals(LastKey))
                            {
                                ContentType = Regex.Split(nvc[key], "<:><:><:>")[1];
                                file = Regex.Split(nvc[key], "<:><:><:>")[0];
                                if (key.Equals(LastKey))
                                {
                                    var headerTemplate =
                                        "Content-Disposition: form-data; name=\"{0}\"; filename=\"{1}\"\r\nContent-Type: {2}\r\n\r\n";
                                    var header = string.Format(headerTemplate, key, AppKey, ContentType);
                                    var headerbytes = Encoding.UTF8.GetBytes(header);
                                    rs.Write(headerbytes, 0, headerbytes.Length);
                                    try
                                    {
                                        rs.Write(bufferbytes, 0, bufferbytes.Length);
                                    }
                                    catch (Exception ex)
                                    {
                                        ex.DebugLog();
                                    }
                                }
                                else
                                {
                                    ContentType = Regex.Split(nvc[key], "<:><:><:>")[1];
                                    file = Regex.Split(nvc[key], "<:><:><:>")[0];
                                    var headerTemplate =
                                        "Content-Disposition: form-data; name=\"{0}\"; filename=\"{1}\"\r\nContent-Type: {2}\r\n\r\n";
                                    var filename = Regex.Split(file, "\\\\");
                                    var header = string.Format(headerTemplate, key, filename[filename.Length - 1],
                                        ContentType);
                                    var headerbytes = Encoding.UTF8.GetBytes(header);
                                    rs.Write(headerbytes, 0, headerbytes.Length);
                                    rs.Write(boundarybytes, 0, boundarybytes.Length);
                                }
                            }
                            else
                            {
                                var formitem = string.Format(formdataTemplate, key, nvc[key]);
                                var formitembytes = Encoding.UTF8.GetBytes(formitem);
                                rs.Write(formitembytes, 0, formitembytes.Length);
                                if (key.Equals(LastKey))
                                {
                                }
                                else
                                {
                                    rs.Write(boundarybytes, 0, boundarybytes.Length);
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            ex.DebugLog();
                        }

                    var trailer = Encoding.ASCII.GetBytes("\r\n" + boundary + "--\r\n");
                    rs.Write(trailer, 0, trailer.Length);
                }

                WebResponse wresp = null;
                try
                {
                    wresp = Request.GetResponse();
                    var stream2 = wresp.GetResponseStream();
                    using (var reader2 = new StreamReader(stream2))
                    {
                        reader2.ReadToEnd();
                    }

                    return;
                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                    wresp?.Dispose();
                }
                finally
                {
                    Request = null;
                }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        protected void SetProxy(IRequestParameters requestParameter)
        {
            try
            {
                var webProxy = new WebProxy(requestParameter.Proxy.ProxyIp, int.Parse(requestParameter.Proxy.ProxyPort))
                {
                    BypassProxyOnLocal = true
                };

                if (!string.IsNullOrEmpty(requestParameter.Proxy.ProxyUsername)
                    && !string.IsNullOrEmpty(requestParameter.Proxy.ProxyPassword))
                    webProxy.Credentials = new NetworkCredential(requestParameter.Proxy.ProxyUsername,
                        requestParameter.Proxy.ProxyPassword);
                Request.Proxy = webProxy;
            }
            catch (Exception ex)
            {
                ex.ErrorLog();
            }
        }
    }
}