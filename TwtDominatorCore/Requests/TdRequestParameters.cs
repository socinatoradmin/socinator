using DominatorHouseCore.Request;
using DominatorHouseCore.Utility;
using Newtonsoft.Json;
using System;
using System.Net;
using TwtDominatorCore.TDUtility;

namespace TwtDominatorCore.Requests
{
    public class TdRequestParameters : RequestParameters
    {
        public TdRequestParameters()
        {
            SetupHeaders();
            // in LoginBase of jobProcess class we checking cookies is null then going for login from DbCookies
            // therefore we are not initializing CookieCollection here
            // Cookies = new CookieCollection();
        }

        public JsonElementsForPostReq Body { private get; set; }

        public byte[] GeneratePostBody(JsonElementsForPostReq elements)
        {
            Body = elements;
            var str = Body == null
                ? null
                : JsonConvert.SerializeObject(Body, new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore
                });
            return GeneratePostData(str);
        }

        /// <summary>
        ///     set default headers
        /// </summary>
        public void SetupHeaders(string TypeOfReq = null, string Token = null,bool IsJsonRequest=false,SearchType type=SearchType.None, string Response = "", string Path = "",string Method="",string GuestID="")
        {
            Headers.Clear();
            var objWebHeaderCollection = new WebHeaderCollection();
            //objWebHeaderCollection.Add("Origin", $"https://{TdConstants.Domain}");
            //objWebHeaderCollection.Add("User-Agent", TdConstants.NewUserAgent2);
            objWebHeaderCollection.Add("Accept-Language", TdConstants.AcceptLanguage);
            Accept = TdConstants.AcceptAll;
            Referer = TdConstants.MainUrl;
            UserAgent = TdConstants.NewUserAgent;
            KeepAlive = true;
            try
            {
                switch (TypeOfReq)
                {
                    case "Json":
                        objWebHeaderCollection.Add("x-csrf-token", Token);
                        objWebHeaderCollection.Add("Content-Type", "application/x-www-form-urlencoded; charset=UTF-8");
                        objWebHeaderCollection.Add("authorization",
                            "Bearer AAAAAAAAAAAAAAAAAAAAANRILgAAAAAAnNwIzUejRCOuH5E6I8xnZz4puTs%3D1Zv7ttfk8LF81IUq16cHjhLTvJu4FA33AGWWjCpTnA");
                        objWebHeaderCollection.Add("Sec-Fetch-Dest", "empty");
                        objWebHeaderCollection.Add("Sec-Fetch-Site", "same-origin");
                        objWebHeaderCollection.Add("Sec-Fetch-Mode", "cors");
                        objWebHeaderCollection.Add("x-twitter-active-user", "yes");
                        objWebHeaderCollection.Add("X-Client-UUID",Guid.NewGuid().ToString());
                        objWebHeaderCollection.Add("x-twitter-auth-type", "OAuth2Session");
                        //objWebHeaderCollection.Add("sec-ch-ua-full-version-list", "\"(Not(A:Brand\";v=\"99.0.0.0\", \"Google Chrome\";v=\"127\", \"Chromium\";v=\"127\"");
                        objWebHeaderCollection.Add("x-twitter-client-language", "en");
                        objWebHeaderCollection.Add("sec-ch-ua-mobile", "?0");
                        //objWebHeaderCollection.Add("x-twitter-polling", "true");
                        objWebHeaderCollection.Add("sec-ch-ua-platform", "\"Windows\"");
                        objWebHeaderCollection.Add("Origin", "https://x.com");
                        objWebHeaderCollection.Add("sec-ch-ua", "\"Google Chrome\";v=\"141\", \"Not?A_Brand\";v=\"8\", \"Chromium\";v=\"141\"");
                        ContentType = IsJsonRequest ? "application/json":"application/x-www-form-urlencoded"; //"application/json";
                        break;
                    case "XML":
                        objWebHeaderCollection.Add("X-Requested-With", "XMLHttpRequest");
                        objWebHeaderCollection.Add("X-Twitter-Active-User", "yes");
                        objWebHeaderCollection.Add("X-Asset-Version", "2f2f06");
                        ContentType = "application/x-www-form-urlencoded; charset=UTF-8";
                        objWebHeaderCollection.Add("x-csrf-token", Token);
                        break;

                    default:
                        objWebHeaderCollection.Add("Upgrade-Insecure-Requests", "1");
                        objWebHeaderCollection.Add("Content-Type", "application/x-www-form-urlencoded");
                        objWebHeaderCollection.Add("Cache-Control", "max-age=0");
                        ContentType = TdConstants.ContentType;
                        Accept = TdConstants.AcceptHtml;
                        break;
                }
                { 
                    var id = string.Empty;
                    try
                    {
                        if (!string.IsNullOrEmpty(Method) && !string.IsNullOrEmpty(Path))
                        {
                            //id = Generator.GenerateTransactionID(Method, Path, Response).Result;
                            id = TdUtility.GetXClientTransactionID(Method, Path);
                            id = string.IsNullOrEmpty(id) ? TdUtility.GetTransactionID(type) : id;
                        }
                        else if (type != SearchType.None)
                            id = TdUtility.GetTransactionID(type);
                    }
                    catch (Exception)
                    {
                        id = type != SearchType.None ? TdUtility.GetTransactionID(type) : string.Empty;
                    }
                    if (!string.IsNullOrEmpty(id))
                        objWebHeaderCollection.Add("x-client-transaction-id", id);
                    }
                    if (!string.IsNullOrEmpty(GuestID))
                    {
                        var token = TdUtility.GetXForwardedFor(GuestID);
                        if (!string.IsNullOrEmpty(token))
                            objWebHeaderCollection.Add("x-xp-forwarded-for", token);
                }
            Headers = objWebHeaderCollection;
        }
            catch (Exception)
            {
            }
        }
    }
}