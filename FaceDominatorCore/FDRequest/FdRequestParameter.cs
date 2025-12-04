using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Request;
using FaceDominatorCore.FDLibrary.FdClassLibrary;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace FaceDominatorCore.FDRequest
{
    public class FdRequestParameter : RequestParameters
    {
        public FdRequestParameter()
        {
            HeaderInitialize();
        }

        public FdRequestParameter(DominatorHouseCore.Models.Proxy objProxy)
        {
            HeaderInitializeDefault(objProxy);
        }

        public FdRequestParameter(IRequestParameters requestParameter)
        {
            HeaderInitializeFullInfo(requestParameter);
        }

        private void HeaderInitializeFullInfo(IRequestParameters requestParameter)
        {
            try
            {
                Accept = FdConstants.AcceptType;
                Headers[FdConstants.AcceptCharsetKey] = FdConstants.AcceptCharset;
                Headers[FdConstants.AcceptLanguageKey] = FdConstants.AcceptLanguage;
                Headers[FdConstants.HostKey] = FdConstants.FullInfoHost;
                Headers[FdConstants.UpgradeInsecureRequestkey] = FdConstants.UpgradeInSecureRequest;
                UserAgent = "Mozilla/5.0 (Windows NT 6.1; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/69.0.3497.100 Safari/537.36";
                KeepAlive = FdConstants.KeepAlive;
                Proxy = requestParameter.Proxy;

            }
            catch (Exception)
            {
                //ignored
            }
        }

        /// <summary>
        /// To store the post data in the json elements
        /// </summary>
        public FdJsonElement FdPostElements { private get; set; }

        public bool IsMultiPartVideo { get; set; }

        public bool IsMultiPartForMessage { get; set; }

        /// <summary>
        /// To generate the post based on already stored items from 
        /// <see cref="DominatorHouseCore.Request.RequestParameters.PostDataParameters"/>
        /// </summary>
        /// <returns>post data in bytes of sequence</returns>
        public byte[] GetPostDataFromParameters()
            => !IsMultiPart ? GeneratePostData() : (!IsMultiPartVideo ? (!IsMultiPartForMessage ? CreateMultipartBody() : CreateMultipartBodyMessage()) : CreateMultipartBodyVideo());

        /// <summary>
        /// To generate the post based on already stored elements from 
        /// <see cref="FaceDominatorCore.FDRequest.FdRequestParameter.FdPostElements"/>
        /// </summary>
        /// <returns>post data in bytes of sequence</returns>
        public byte[] GetPostDataFromJson()
        {
            var jsonString = GetJsonString();

            if (string.IsNullOrEmpty(jsonString)) return null;

            return !IsMultiPart ?
                GeneratePostDataFromJson(jsonString) : CreateMultipartBodyFromJson(jsonString);
        }

        /*
                /// <summary>
                /// To generate the post based on already stored itmes in
                /// <see cref="DominatorHouseCore.Request.RequestParameters.PostDataParameters"/> and 
                /// <see cref="FaceDominatorCore.FDRequest.FdRequestParameter.FdPostElements"/>
                /// </summary>
                /// <returns>post data in bytes of sequence</returns>
                public byte[] GetPostData()
                {
                    var jsonString = GetJsonString();

                    if (string.IsNullOrEmpty(jsonString)) return null;

                    return !IsMultiPart ? GeneratePostData(jsonString) : CreateMultipartBody(jsonString);
                }
        */

        /// <summary>
        /// To convert the <see cref="FaceDominatorCore.FDRequest.FdRequestParameter.FdPostElements"/> elements to string
        /// </summary>
        /// <returns>json string</returns>
        internal string GetJsonString()
        {
            return FdPostElements == null ? null :
                JsonConvert.SerializeObject(
                    value: FdPostElements,
                 settings: new JsonSerializerSettings()
                 {
                     NullValueHandling = NullValueHandling.Ignore
                 });
        }

        internal string GetJsonString(FdPublisherJsonElement objElement)
        {
            return objElement == null ? null :
                JsonConvert.SerializeObject(
                    value: objElement,
                 settings: new JsonSerializerSettings()
                 {
                     NullValueHandling = NullValueHandling.Ignore
                 });
        }

        internal string GetJsonString(FdMarketplaceJsonElements objElement)
        {
            return objElement == null ? null :
                JsonConvert.SerializeObject(
                    value: objElement,
                 settings: new JsonSerializerSettings()
                 {
                     NullValueHandling = NullValueHandling.Ignore
                 });
        }

        /*
                /// <summary>
                /// To convert the <see cref="FaceDominatorCore.FDRequest.FdRequestParameter"/> object to string
                /// </summary>
                /// <param name="fdPostElements"><see cref="FaceDominatorCore.FDRequest.FdJsonElement"/> object </param>
                /// <returns>json string</returns>
                internal string GetJsonString(FdJsonElement fdPostElements)
                {
                    return FdPostElements == null ? null :
                        JsonConvert.SerializeObject(
                            value: fdPostElements,
                            settings: new JsonSerializerSettings()
                            {
                                NullValueHandling = NullValueHandling.Ignore
                            });
                }
        */

        internal string GetJsonString(FdAdsScraperJsonElement fdPostElements)
        {
            return fdPostElements == null ? null :
                JsonConvert.SerializeObject(
                    value: fdPostElements,
                    settings: new JsonSerializerSettings()
                    {
                        NullValueHandling = NullValueHandling.Ignore
                    });
        }

        /// <summary>
        /// Initialize the neccessary header for facebook to <see cref="DominatorHouseCore.Request.RequestParameters.Headers"/>
        /// </summary>
        /// <returns>returns true if initialize success, else false</returns>
        public bool HeaderInitialize()
        {
            try
            {
                Accept = FdConstants.AcceptType;
                Headers[FdConstants.AcceptCharsetKey] = FdConstants.AcceptCharset;
                Headers[FdConstants.AcceptLanguageKey] = FdConstants.AcceptLanguage;
                Headers[FdConstants.UpgradeInsecureRequestkey] = FdConstants.UpgradeInSecureRequest;
                Headers[FdConstants.HostKey] = FdConstants.Host;
                KeepAlive = FdConstants.KeepAlive;
                Referer = FdConstants.Referer;
                UserAgent =
                    "Mozilla/5.0 (Windows NT 6.1; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/71.0.3578.98 Safari/537.36";
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }

        public bool HeaderInitializeDefault(DominatorHouseCore.Models.Proxy objProxy)
        {
            try
            {
                Accept = FdConstants.AcceptType;
                Headers[FdConstants.AcceptCharsetKey] = FdConstants.AcceptCharset;
                Headers[FdConstants.AcceptLanguageKey] = FdConstants.AcceptLanguage;
                Headers[FdConstants.UpgradeInsecureRequestkey] = FdConstants.UpgradeInSecureRequest;
                UserAgent = "Mozilla/5.0 (Windows NT 6.1; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/69.0.3497.100 Safari/537.36";
                KeepAlive = FdConstants.KeepAlive;
                Proxy = objProxy;
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }

        private static string GenerateFdMultipartBoundary()
        {
            return "----WebKitFormBoundary" + DateTime.Now.Ticks.ToString("x");
        }

        public override byte[] CreateMultipartBody()
        {

            var multipartBoundary = GenerateFdMultipartBoundary();

            var strMultipartBoundary = $"--{multipartBoundary}";

            var stringBuilder = new StringBuilder();

            using (var memoryStream = new MemoryStream())
            {
                //stringBuilder.AppendLine(strMultipartBoundary);

                foreach (KeyValuePair<string, string> postItem in PostDataParameters)
                {
                    stringBuilder.AppendLine(strMultipartBoundary);
                    stringBuilder.AppendLine($"Content-Disposition: form-data; name=\"{postItem.Key as object}\"");
                    stringBuilder.AppendLine();
                    stringBuilder.AppendLine(postItem.Value);
                }
                foreach (KeyValuePair<string, FileData> file in FileList)
                {
                    stringBuilder.AppendLine(strMultipartBoundary);


                    if (file.Value.Headers != null)
                    {
                        foreach (string header in file.Value.Headers)
                        {
                            stringBuilder.AppendLine(
                                $"Content-Disposition: form-data; name=\"{(object)header}\"");
                            stringBuilder.AppendLine();
                            stringBuilder.AppendLine($"{file.Value.Headers[header] as object}");
                            stringBuilder.AppendLine(strMultipartBoundary);
                        }
                    }

                    stringBuilder.AppendLine(
                        $"Content-Disposition: form-data; name=\"farr\"; filename=\"{file.Value.FileName as object}\" Content-Type: image/jpeg");

                    stringBuilder.AppendLine();
                    var bytes = Encoding.UTF8.GetBytes(stringBuilder.ToString());
                    memoryStream.Write(bytes, 0, bytes.Length);
                    stringBuilder.Clear();
                    if (file.Value.Contents != null)
                    {
                        memoryStream.Write(file.Value.Contents, 0, file.Value.Contents.Length);
                    }
                }

                byte[] bytes1 = Encoding.UTF8.GetBytes(Environment.NewLine + strMultipartBoundary);
                memoryStream.Write(bytes1, 0, bytes1.Length);

                //AddHeader("Content-Type", $"multipart/form-data; boundary={multipartBoundary}");

                ContentType = $"multipart/form-data; boundary={multipartBoundary}";
                return memoryStream.ToArray();
            }
        }

        public byte[] CreateMultipartBodyMessage()
        {
            var multipartBoundary = GenerateFdMultipartBoundary();

            var strMultipartBoundary = $"--{multipartBoundary}";

            var stringBuilder = new StringBuilder();

            using (var memoryStream = new MemoryStream())
            {
                //stringBuilder.AppendLine(strMultipartBoundary);

                foreach (KeyValuePair<string, string> postItem in PostDataParameters)
                {
                    stringBuilder.AppendLine(strMultipartBoundary);
                    stringBuilder.AppendLine($"Content-Disposition: form-data; name=\"{postItem.Key as object}\"");
                    stringBuilder.AppendLine();
                    stringBuilder.AppendLine(postItem.Value);
                }
                foreach (KeyValuePair<string, FileData> file in FileList)
                {
                    stringBuilder.AppendLine(strMultipartBoundary);


                    if (file.Value.Headers != null)
                    {
                        foreach (string header in file.Value.Headers)
                        {
                            stringBuilder.AppendLine(
                                $"Content-Disposition: form-data; name=\"{(object)header}\"");
                            stringBuilder.AppendLine();
                            stringBuilder.AppendLine($"{file.Value.Headers[header] as object}");
                            stringBuilder.AppendLine(strMultipartBoundary);
                        }
                    }

                    stringBuilder.AppendLine(
                        $"Content-Disposition: form-data; name=\"upload_1029\"; filename=\"{file.Value.FileName as object}\"");

                    stringBuilder.AppendLine("Content-Type: image/png");

                    stringBuilder.AppendLine();
                    var bytes = Encoding.UTF8.GetBytes(stringBuilder.ToString());
                    memoryStream.Write(bytes, 0, bytes.Length);
                    stringBuilder.Clear();
                    if (file.Value.Contents != null)
                    {
                        memoryStream.Write(file.Value.Contents, 0, file.Value.Contents.Length);
                    }
                }

                var endBoundary = strMultipartBoundary + "--";

                byte[] bytes1 = Encoding.UTF8.GetBytes(Environment.NewLine + endBoundary);
                memoryStream.Write(bytes1, 0, bytes1.Length);

                //AddHeader("Content-Type", $"multipart/form-data; boundary={multipartBoundary}");

                ContentType = $"multipart/form-data; boundary={multipartBoundary}";
                return memoryStream.ToArray();
            }
        }

        public byte[] CreateMultipartBodyVideo()
        {

            var multipartBoundary = GenerateFdMultipartBoundary();

            var strMultipartBoundary = $"--{multipartBoundary}";

            var stringBuilder = new StringBuilder();

            using (var memoryStream = new MemoryStream())
            {
                foreach (KeyValuePair<string, FileData> file in FileList)
                {
                    stringBuilder.AppendLine(strMultipartBoundary);

                    stringBuilder.AppendLine(
                        $"Content-Disposition: form-data; name=\"video_file_chunk\"; filename=\"{file.Value.FileName as object}\" Content-Type: video/*");

                    stringBuilder.AppendLine();
                    var bytes = Encoding.UTF8.GetBytes(stringBuilder.ToString());
                    memoryStream.Write(bytes, 0, bytes.Length);
                    stringBuilder.Clear();
                    if (file.Value.Contents != null)
                    {
                        memoryStream.Write(file.Value.Contents, 0, file.Value.Contents.Length);
                    }
                }

                byte[] bytes1 = Encoding.UTF8.GetBytes(Environment.NewLine + strMultipartBoundary);
                memoryStream.Write(bytes1, 0, bytes1.Length);

                ContentType = $"multipart/form-data; boundary={multipartBoundary}";
                return memoryStream.ToArray();
            }
        }

        public CookieCollection RefreshCookies(CookieCollection cookieCollection, ref bool isCookieChanged)
        {
            CookieCollection objcookieCollection = new CookieCollection();
            foreach (Cookie cookie in cookieCollection)
            {
                if (string.IsNullOrEmpty(cookie.Domain))
                {
                    cookie.Domain = ".facebook.com";
                    isCookieChanged = true;
                }
                objcookieCollection.Add(cookie);
            }

            return objcookieCollection;
        }

        public string GenerateUrlByEscapeDataString(string url)
        {
            var array = UrlParameters.Select(x => $"{x.Key}={WebUtility.UrlEncode(x.Value)}").ToArray();

            return $"{url}{(array.Length != 0 ? "?" : string.Empty)}{string.Join("&", array)}";
        }
    }
}
