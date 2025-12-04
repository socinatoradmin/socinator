using System;
using System.Collections.Specialized;
using System.IO;
using System.Net;
using System.Text;
using DominatorHouseCore;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Request;
using DominatorHouseCore.Utility;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace QuoraDominatorCore.Request
{
    public class QdRequestParameters : RequestParameters
    {
        public QdRequestParameters()
        {
            SetupDefaultHeaders();
            Cookies = new CookieCollection();
        }

        public QdRequestParameters(IRequestParameters requestParameter) : this()
        {
            if (requestParameter?.Cookies != null && requestParameter?.Cookies?.Count != 0)
                Cookies = requestParameter.Cookies;
        }

        public QdJsonElements Body { private get; set; }

        public byte[] GetPostDataFromParameters()
        {
            return GeneratePostData();
        }

        public byte[] GenerateBody()
        {
            SetupDefaultHeaders();
            var str = Body == null
                ? null
                : JsonConvert.SerializeObject(Body, new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore
                });
            if (IsMultiPart)
                return CreateMultipartBody(str);
            if (str == null)
                return null;
            return GeneratePostData(str);
        }


        internal string GetJsonString(QdJsonElements jsonElements)
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

        internal string GetJsonStringNullInclude(QdJsonElements jsonElements)
        {
            return jsonElements == null
                ? null
                : JsonConvert.SerializeObject(
                    jsonElements,
                    new JsonSerializerSettings
                    {
                        NullValueHandling = NullValueHandling.Include
                    });
        }

        public byte[] GetPostDataFromJson()
        {
            var jsonString = GetJsonString();

            if (string.IsNullOrEmpty(jsonString)) return null;
            return !IsMultiPart ? GeneratePostDataFromJson(jsonString) : CreateMultipartBodyFromJson(jsonString);
        }

        internal string GetJsonString()
        {
            return Body == null
                ? null
                : JsonConvert.SerializeObject(
                    Body,
                    new JsonSerializerSettings
                    {
                        NullValueHandling = NullValueHandling.Ignore
                    });
        }


        /// <summary>
        ///     set default headers for mobile and web request
        /// </summary>
        private void SetupDefaultHeaders()
        {
            try
            {
                Headers.Clear();
                AddHeader("Accept-Language", "en-US,en;q=0.9");
                Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";
                KeepAlive = true;
                UserAgent =
                    "Mozilla/5.0 (Windows NT 6.1; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/75.0.3770.100 Safari/537.36";
                if (IsMultiPart)
                    return;
                ContentType = Constants.ContentTypeDefault;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }


        public void QuoraAddFileList(string title, byte[] data, string fileName)
        {
            var headers = new NameValueCollection
            {
                {"Content-Type", "image/jpeg"}
            };

            if (fileName != null)
            {
                fileName = Path.GetFileName(fileName);

                if (data != null)
                {
                    var fileData = new FileData(headers, fileName, data);
                    FileList.Add(title, fileData);
                }
            }

            IsMultiPart = true;
        }

        public byte[] GenerateBodyForMedia()
        {
            SetupDefaultHeaders();
            var str = Body == null
                ? null
                : JsonConvert.SerializeObject(Body, new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore
                });
            if (IsMultiPart)
                return CreateMultipartBody(str);
            if (str == null)
                return null;
            return GeneratePostData(str);
        }

        private static string GenerateMultipartBoundary()
        {
            var stringBuilder = new StringBuilder();
            var max = "1234567890abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ".Length - 1;
            for (var index = 0; index < 16; ++index)
                stringBuilder.Append(
                    "1234567890abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ"[
                        RandomUtilties.GetRandomNumber(max)]);
            return stringBuilder.ToString();
        }

        public virtual byte[] CreateMultipartBody(string jsonString)
        {
            var jobject = JObject.Parse(jsonString);
            var multipartBoundary = GenerateMultipartBoundary();

            var strMultipartBoundary = "------WebKitFormBoundary" + $"{multipartBoundary}";

            var stringBuilder = new StringBuilder();

            using (var memoryStream = new MemoryStream())
            {
                foreach (var keyValuePair in jobject)
                {
                    if (keyValuePair.Key.Contains("nid"))
                        continue;
                    stringBuilder.AppendLine(strMultipartBoundary);
                    stringBuilder.AppendLine($"Content-Disposition: form-data; name=\"{keyValuePair.Key as object}\"");
                    stringBuilder.AppendLine();
                    stringBuilder.AppendLine(keyValuePair.Value.ToString());
                }

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
                    stringBuilder.AppendLine(
                        $"Content-Disposition: form-data; name=\"{file.Key as object}\"; filename=\"{file.Value.FileName as object}\"");

                    foreach (string header in file.Value.Headers)
                        stringBuilder.AppendLine($"{(object) header}: {file.Value.Headers[header] as object}");

                    stringBuilder.AppendLine();

                    var bytes = Encoding.UTF8.GetBytes(stringBuilder.ToString());
                    memoryStream.Write(bytes, 0, bytes.Length);
                    stringBuilder.Clear();
                    memoryStream.Write(file.Value.Contents, 0, file.Value.Contents.Length);
                    foreach (var keyValuePair in jobject)
                    {
                        if (!keyValuePair.Key.Contains("nid"))
                            continue;
                        //stringBuilder.AppendLine("------WebKitFormBoundary" + $"{multipartBoundary}");
                        //stringBuilder.AppendLine(Environment.NewLine);
                        stringBuilder.AppendLine("\r\n" + strMultipartBoundary);
                        stringBuilder.AppendLine(
                            $"Content-Disposition: form-data; name=\"{keyValuePair.Key as object}\"");
                        stringBuilder.AppendLine();
                        stringBuilder.AppendLine(keyValuePair.Value.ToString());
                        stringBuilder.AppendLine(strMultipartBoundary + "--");
                    }

                    bytes = Encoding.UTF8.GetBytes(stringBuilder.ToString());
                    memoryStream.Write(bytes, 0, bytes.Length);
                    stringBuilder.Clear();
                }

                //byte[] bytes1 = Encoding.UTF8.GetBytes(Environment.NewLine + strMultipartBoundary);
                //memoryStream.Write(bytes1, 0, bytes1.Length);


                AddHeader("Content-Type",
                    "multipart/form-data; boundary=----WebKitFormBoundary" + $"{multipartBoundary}");
                ContentType = "multipart/form-data; boundary=----WebKitFormBoundary" + $"{multipartBoundary}";
                return memoryStream.ToArray();
            }
        }
    }
}