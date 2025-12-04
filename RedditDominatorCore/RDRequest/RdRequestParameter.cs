using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Models;
using DominatorHouseCore.Request;
using Newtonsoft.Json;
using RedditDominatorCore.RDLibrary;
using System;
using System.IO;
using System.Net;
using System.Text;

namespace RedditDominatorCore.RDRequest
{
    public class RequestParameter : RequestParameters
    {
        private readonly IRdHttpHelper _httpHelper;
        public IRequestParameters _req = new RequestParameters();

        public RequestParameter()
        {
        }

        public RequestParameter(IRdHttpHelper rdHttpHelper)
        {
            _httpHelper = rdHttpHelper;
            HeaderInitialize(rdHttpHelper);
        }

        /// <summary>
        ///     To generate the post based on already stored items from
        ///     <see cref="DominatorHouseCore.Request.RequestParameters.PostDataParameters" />
        /// </summary>
        /// <returns>post data in bytes of sequence</returns>
        public byte[] GetPostDataFromParameters(string contentType)
        {
            return !IsMultiPart ? GeneratePostData() : CreateMultipartBody(contentType);
        }

        // Initialize Header Parameters
        public bool HeaderInitialize(IRdHttpHelper rdHttpHelper)
        {
            try
            {
                var userAgent = RdConstants.UserAgentValue;
                _req = rdHttpHelper.GetRequestParameter();
                _req.Headers = new WebHeaderCollection();
                _req.Headers["Host"] = "www.reddit.com";
                _req.Headers["User-Agent"] = userAgent;
                _req.UserAgent = userAgent;
                _req.Accept = RdConstants.AcceptType;
                _req.KeepAlive = RdConstants.KeepAlive;
                _req.ContentType = RdConstants.ContentType;
                rdHttpHelper.SetRequestParameter(_req);
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }

        // Add Headers with authorization parameter
        public bool AddExtraHeaders(DominatorAccountModel account, RdParameters requestParameter)
        {
            try
            {
                _req = _httpHelper.GetRequestParameter();
                _req.Headers[RdConstants.HostKey] = requestParameter.RdHostValue;
                _req.Headers[RdConstants.AuthorizationKey] = requestParameter.AuthorizationValue;
                _req.Headers[RdConstants.OriginKey] = requestParameter.OriginValue;
                _req.Headers[RdConstants.LoidKey] = requestParameter.LoidValue;
                _req.Headers[RdConstants.RdUserIdKey] = requestParameter.RdUserIdValue;
                _req.Headers[RdConstants.RdSessionKey] = requestParameter.SessionValue;
                _req.ContentType = "application/json";
                _req.Accept = RdConstants.AcceptType;
                _req.Headers["Accept-Language"] = "en-US,en;q=0.9";
                _req.Headers["sec-ch-ua"] = "\"Chromium\";v=\"92\", \" Not A; Brand\";v=\"99\", \"Google Chrome\";v=\"92\"";
                _req.Headers["sec-ch-ua-mobile"] = "?0";
                _req.Referer = requestParameter.RefererValue;
                _httpHelper.SetRequestParameter(_req);
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }

        public override byte[] CreateMultipartBody()
        {
            var multipartBoundary = GenerateFdMultipartBoundary();

            var strMultipartBoundary = $"--{multipartBoundary}";

            var stringBuilder = new StringBuilder();

            using (var memoryStream = new MemoryStream())
            {
                //stringBuilder.AppendLine(strMultipartBoundary);

                foreach (var postItem in PostDataParameters)
                {
                    stringBuilder.AppendLine(strMultipartBoundary);
                    stringBuilder.AppendLine($"Content-Disposition: form-data; name=\"{postItem.Key as object}\"");
                    stringBuilder.AppendLine();
                    stringBuilder.AppendLine(postItem.Value);
                }

                foreach (var file in FileList)
                {
                    stringBuilder.AppendLine(strMultipartBoundary);


                    if (file.Value.Headers != null)
                        foreach (string header in file.Value.Headers)
                        {
                            stringBuilder.AppendLine(
                                $"Content-Disposition: form-data; name=\"{(object)header}\"");
                            stringBuilder.AppendLine();
                            stringBuilder.AppendLine($"{file.Value.Headers[header] as object}");
                            stringBuilder.AppendLine(strMultipartBoundary);
                        }

                    stringBuilder.AppendLine(
                        $"Content-Disposition: form-data; name=\"file\"; filename=\"{file.Value.FileName as object}\" Content-Type: image/jpeg");

                    stringBuilder.AppendLine();
                    var bytes = Encoding.UTF8.GetBytes(stringBuilder.ToString());
                    memoryStream.Write(bytes, 0, bytes.Length);
                    stringBuilder.Clear();
                    if (file.Value.Contents != null)
                        memoryStream.Write(file.Value.Contents, 0, file.Value.Contents.Length);
                }

                var bytes1 = Encoding.UTF8.GetBytes(Environment.NewLine + strMultipartBoundary);
                memoryStream.Write(bytes1, 0, bytes1.Length);

                //AddHeader("Content-Type", $"multipart/form-data; boundary={multipartBoundary}");

                ContentType = $"multipart/form-data; boundary={multipartBoundary}";
                return memoryStream.ToArray();
            }
        }

        public override byte[] CreateMultipartBody(string contentType)
        {
            var multipartBoundary = GenerateFdMultipartBoundary();

            var strMultipartBoundary = $"--{multipartBoundary}";

            var stringBuilder = new StringBuilder();

            using (var memoryStream = new MemoryStream())
            {
                //stringBuilder.AppendLine(strMultipartBoundary);

                foreach (var postItem in PostDataParameters)
                {
                    stringBuilder.AppendLine(strMultipartBoundary);
                    stringBuilder.AppendLine($"Content-Disposition: form-data; name=\"{postItem.Key as object}\"");
                    stringBuilder.AppendLine();
                    stringBuilder.AppendLine(postItem.Value);
                }

                foreach (var file in FileList)
                {
                    stringBuilder.AppendLine(strMultipartBoundary);


                    if (file.Value.Headers != null)
                        foreach (string header in file.Value.Headers)
                        {
                            stringBuilder.AppendLine(
                                $"Content-Disposition: form-data; name=\"{(object)header}\"");
                            stringBuilder.AppendLine();
                            stringBuilder.AppendLine($"{file.Value.Headers[header] as object}");
                            stringBuilder.AppendLine(strMultipartBoundary);
                        }

                    stringBuilder.AppendLine(
                        $"Content-Disposition: form-data; name=\"file\"; filename=\"{file.Value.FileName as object}\" Content-Type: {contentType}");


                    stringBuilder.AppendLine();
                    var bytes = Encoding.UTF8.GetBytes(stringBuilder.ToString());
                    memoryStream.Write(bytes, 0, bytes.Length);
                    stringBuilder.Clear();
                    if (file.Value.Contents != null)
                        memoryStream.Write(file.Value.Contents, 0, file.Value.Contents.Length);
                }

                var bytes1 = Encoding.UTF8.GetBytes(Environment.NewLine + strMultipartBoundary);
                memoryStream.Write(bytes1, 0, bytes1.Length);

                //AddHeader("Content-Type", $"multipart/form-data; boundary={multipartBoundary}");

                ContentType = $"multipart/form-data; boundary={multipartBoundary}";
                return memoryStream.ToArray();
            }
        }

        private static string GenerateFdMultipartBoundary()
        {
            return "----WebKitFormBoundary" + DateTime.Now.Ticks.ToString("x");
        }

        //To get post data in bytes
        public byte[] GetPostDataFromJson<T>(T jsonElements) where T : class, new()
        {
            var jsonString = GetJsonString(jsonElements);

            if (string.IsNullOrEmpty(jsonString)) return null;

            return !IsMultiPart ? GeneratePostDataFromJson(jsonString) : CreateMultipartBody(jsonString);
        }

        //For Serialization
        public string GetJsonString<T>(T jsonElements) where T : class, new()
        {
            return jsonElements == null
                ? null
                : JsonConvert.SerializeObject(
                    jsonElements,
                    new JsonSerializerSettings
                    {
                        NullValueHandling = NullValueHandling.Ignore
                    });
        }
    }
}