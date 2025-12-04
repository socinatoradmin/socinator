using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Tumblr.classes
{
    public class GlobusHttpHelper
    {
        public CookieCollection gCookies = new CookieCollection();
        public HttpWebRequest gRequest;
        HttpWebResponse gResponse;
        public string responseURI = string.Empty;
        public static string valueURl = string.Empty;

        public static string path_AppDataFolder = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\PinDominator3.5";


        public static string Path_ResposeloginError = path_AppDataFolder + "\\ResponseText.txt";


        string UserAgent = "Mozilla/5.0 (Windows NT 6.1; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/57.0.2987.133 Safari/537.36";

        int Timeout = 90000;

        public Uri GetResponseData()
        {
            return gResponse.ResponseUri;
        }

        public string getHtmlfromUrl(Uri url)
        {
            string responseString = string.Empty;

            try
            {
                gRequest = (HttpWebRequest)WebRequest.Create(url);
                //setExpect100Continue();

                gRequest.UserAgent = "Mozilla/5.0 (Windows NT 6.1; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/51.0.2704.103 Safari/537.36";
                gRequest.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";
                // gRequest.Headers["Accept-Charset"] = "ISO-8859-1,utf-8;q=0.7,*;q=0.7";
                gRequest.Headers["Accept-Language"] = "en-us,en;q=0.5";
                // gRequest.Headers.Add("")
                gRequest.KeepAlive = true;

                gRequest.AllowAutoRedirect = true;

                gRequest.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;

                gRequest.CookieContainer = new CookieContainer(); //gCookiesContainer;
                gRequest.Headers.Add("Javascript-enabled", "true");

                gRequest.Method = "GET";

                #region CookieManagment

                if (this.gCookies != null && this.gCookies.Count > 0)
                {
                    setExpect100Continue();
                    gRequest.CookieContainer.Add(gCookies);

                }

                if (this.gCookies == null)
                {
                    this.gCookies = new CookieCollection();
                }



                //Get Response for this request url

                setExpect100Continue();
                gResponse = (HttpWebResponse)gRequest.GetResponse();

                valueURl = gResponse.ResponseUri.ToString();
                //check if the status code is http 200 or http ok
                if (gResponse.StatusCode == HttpStatusCode.OK)
                {
                    //get all the cookies from the current request and add them to the response object cookies
                    setExpect100Continue();
                    gResponse.Cookies = gRequest.CookieContainer.GetCookies(gRequest.RequestUri);


                    //check if response object has any cookies or not
                    if (gResponse.Cookies.Count > 0)
                    {
                        //check if this is the first request/response, if this is the response of first request gCookies
                        //will be null
                        if (this.gCookies == null)
                        {
                            gCookies = gResponse.Cookies;
                        }
                        else
                        {
                            foreach (Cookie oRespCookie in gResponse.Cookies)
                            {
                                bool bMatch = false;
                                foreach (Cookie oReqCookie in this.gCookies)
                                {
                                    if (oReqCookie.Name == oRespCookie.Name)
                                    {
                                        oReqCookie.Value = oRespCookie.Value;
                                        bMatch = true;
                                        break; // 
                                    }
                                }
                                if (!bMatch)
                                    this.gCookies.Add(oRespCookie);
                            }
                        }
                    }
                    #endregion

                    responseURI = gResponse.ResponseUri.AbsoluteUri;

                    StreamReader reader = new StreamReader(gResponse.GetResponseStream());
                    responseString = reader.ReadToEnd();
                    reader.Close();
                    return responseString;
                }
                else
                {
                    return "Error";
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace);
            }
            return responseString;
        }


        public string getHtmlfromUrl(Uri url, string Referes, string Token, string AccountUserAgent)
        {
            string responseString = string.Empty;

            try
            {
                setExpect100Continue();
                gRequest = (HttpWebRequest)WebRequest.Create(url);
                if (!string.IsNullOrEmpty(AccountUserAgent))
                {
                    gRequest.UserAgent = AccountUserAgent;
                }
                else
                {
                    gRequest.UserAgent = UserAgent;
                }
                gRequest.Accept = "application/json, text/javascript, */*; q=0.01";
                gRequest.Headers["Accept-Language"] = "en-US,en;q=0.8";
                gRequest.Timeout = Timeout;
                gRequest.KeepAlive = true;
                gRequest.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;

                gRequest.CookieContainer = new CookieContainer(); //gCookiesContainer;
                gRequest.Headers.Add("Javascript-enabled", "true");
                gRequest.Method = "GET";
                if (!string.IsNullOrEmpty(Referes))
                {
                    gRequest.Referer = Referes;
                }
                //if (!string.IsNullOrEmpty(Token))
                {
                    //gRequest.Headers.Add("X-CSRFToken", Token);
                    gRequest.Headers.Add("X-Requested-With", "XMLHttpRequest");
                }
                gRequest.Headers["X-NEW-APP"] = "1";

                if (gCookies != null)
                {
                    foreach (Cookie item in gCookies)
                    {
                        if (item.Name == "csrftoken")
                        {
                            string csrftokenValue = item.Value;
                            gRequest.Headers["X-CSRFToken"] = csrftokenValue;
                            break;
                        }
                    }
                }

                #region CookieManagment

                if (this.gCookies != null && this.gCookies.Count > 0)
                {
                    setExpect100Continue();
                    gRequest.CookieContainer.Add(gCookies);

                    try
                    {
                        //gRequest.CookieContainer.Add(url, new Cookie("__qca", "P0 - 2078004405 - 1321685323158", "/"));
                        //gRequest.CookieContainer.Add(url, new Cookie("__utma", "101828306.1814567160.1321685324.1322116799.1322206824.9", "/"));
                        //gRequest.CookieContainer.Add(url, new Cookie("__utmz", "101828306.1321685324.1.1.utmcsr=(direct)|utmccn=(direct)|utmcmd=(none)", "/"));
                        //gRequest.CookieContainer.Add(url, new Cookie("__utmb", "101828306.2.10.1321858563", "/"));
                        //gRequest.CookieContainer.Add(url, new Cookie("__utmc", "101828306", "/"));
                    }
                    catch (Exception ex)
                    {
                    }
                }
                //Get Response for this request url

                //setExpect100Continue();
                //gResponse = (HttpWebResponse)gRequest.GetResponse();
                //valueURl = gResponse.ResponseUri.ToString();
                setExpect100Continue();
                gResponse = (HttpWebResponse)gRequest.GetResponse();
                valueURl = gResponse.ResponseUri.ToString();
                //check if the status code is http 200 or http ok
                if (gResponse.StatusCode == HttpStatusCode.OK)
                {
                    //get all the cookies from the current request and add them to the response object cookies
                    setExpect100Continue();
                    gResponse.Cookies = gRequest.CookieContainer.GetCookies(gRequest.RequestUri);


                    //check if response object has any cookies or not
                    if (gResponse.Cookies.Count > 0)
                    {
                        //check if this is the first request/response, if this is the response of first request gCookies
                        //will be null
                        if (this.gCookies == null)
                        {
                            gCookies = gResponse.Cookies;
                        }
                        else
                        {
                            foreach (Cookie oRespCookie in gResponse.Cookies)
                            {
                                bool bMatch = false;
                                foreach (Cookie oReqCookie in this.gCookies)
                                {
                                    if (oReqCookie.Name == oRespCookie.Name)
                                    {
                                        oReqCookie.Value = oRespCookie.Value;
                                        bMatch = true;
                                        break; // 
                                    }
                                }
                                if (!bMatch)
                                    this.gCookies.Add(oRespCookie);
                            }
                        }
                    }
                    #endregion

                    StreamReader reader = new StreamReader(gResponse.GetResponseStream());
                    responseString = reader.ReadToEnd();
                    reader.Close();
                    return responseString;
                }
                else
                {
                    return "Error";
                }

            }
            catch (Exception ex)
            {
            }
            return responseString;
        }

        public string postFormData(Uri formActionUrl, string postData, string referer) //AQeU1YnAYI90JJ5oCa5e3MFTJpxN4JhP
        {

            gRequest = (HttpWebRequest)WebRequest.Create(formActionUrl);
            gRequest.UserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/35.0.1916.153 Safari/537.36";//UserAgent;";//"Mozilla/4.0 (compatible; MSIE 8.0; Windows NT 6.1; Trident/4.0; SLCC2; .NET CLR 2.0.50727; .NET CLR 3.5.30729; .NET CLR 3.0.30729; Media Center PC 6.0;";//UserAgent;
                                                                                                                                                 // gRequest.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8";//"text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";: 
            gRequest.Headers["X-tumblr-form-key"] = "885cSdgUPaFBhHNrqLAdycmf2Dc";
            gRequest.Headers["Accept-Language"] = "en-US,en;q=0.9,fr;q=0.8";
            gRequest.KeepAlive = true;
            gRequest.Host = "www.tumblr.com";
            gRequest.ContentType = @"application/x-www-form-urlencoded; charset=UTF-8";
            gRequest.Timeout = Timeout;
            gRequest.Method = "POST";
            gRequest.Referer = referer;

            gRequest.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;

            gRequest.CookieContainer = new CookieContainer();

            if (!string.IsNullOrEmpty(referer))
            {
                gRequest.Referer = referer;
            }

            ///Modified BySumit 18-11-2011


            #region CookieManagement
            if (this.gCookies != null && this.gCookies.Count > 0)
            {
                setExpect100Continue();
                gRequest.CookieContainer.Add(gCookies);
            }

            //logic to postdata to the form
            try
            {
                setExpect100Continue();
                //string postdata = string.Format(postData);
                byte[] postBuffer = System.Text.Encoding.UTF8.GetBytes(postData);
                gRequest.ContentLength = postBuffer.Length;
                Stream postDataStream = gRequest.GetRequestStream();
                postDataStream.Write(postBuffer, 0, postBuffer.Length);
                postDataStream.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                // Logger.LogText("Internet Connectivity Exception : "+ ex.Message,null);
            }
            //post data logic ends

            //Get Response for this request url
            try
            {
                gResponse = (HttpWebResponse)gRequest.GetResponse();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                //Logger.LogText("Response from "+formActionUrl + ":" + ex.Message,null);
            }

            //string valueURl =  gResponse.ResponseUri.ToString();

            //check if the status code is http 200 or http ok

            if (gResponse.StatusCode == HttpStatusCode.OK)
            {
                //get all the cookies from the current request and add them to the response object cookies
                setExpect100Continue();
                gResponse.Cookies = gRequest.CookieContainer.GetCookies(gRequest.RequestUri);
                //check if response object has any cookies or not
                //Added by sandeep pathak
                //gCookiesContainer = gRequest.CookieContainer;  

                if (gResponse.Cookies.Count > 0)
                {
                    //check if this is the first request/response, if this is the response of first request gCookies
                    //will be null
                    if (this.gCookies == null)
                    {
                        gCookies = gResponse.Cookies;
                    }
                    else
                    {
                        foreach (Cookie oRespCookie in gResponse.Cookies)
                        {
                            bool bMatch = false;
                            foreach (Cookie oReqCookie in this.gCookies)
                            {
                                if (oReqCookie.Name == oRespCookie.Name)
                                {
                                    oReqCookie.Value = oRespCookie.Value;
                                    bMatch = true;
                                    break; // 
                                }
                            }
                            if (!bMatch)
                                this.gCookies.Add(oRespCookie);
                        }
                    }
                }
                #endregion



                StreamReader reader = new StreamReader(gResponse.GetResponseStream());
                string responseString = reader.ReadToEnd();
                reader.Close();
                //Console.Write("Response String:" + responseString);
                return responseString;
            }
            else
            {
                return "Error in posting data";
            }
        }

        public string getBetween(string strSource, string strStart, string strEnd)
        {
            int Start, End;
            if (strSource.Contains(strStart) && strSource.Contains(strEnd))
            {
                Start = strSource.IndexOf(strStart, 0) + strStart.Length;
                End = strSource.IndexOf(strEnd, Start);
                return strSource.Substring(Start, End - Start);
            }
            else
            {
                return "";
            }
        }

        public void setExpect100Continue()
        {
            if (ServicePointManager.Expect100Continue == true)
            {
                ServicePointManager.Expect100Continue = false;
            }
        }
    }
}
