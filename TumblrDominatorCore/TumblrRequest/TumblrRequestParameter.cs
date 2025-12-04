using DominatorHouseCore;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Models;
using DominatorHouseCore.Request;
using DominatorHouseCore.Utility;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Specialized;
using System.IO;
using System.Text;

namespace TumblrDominatorCore.TumblrRequest
{
    public class TumblrRequestParameter : RequestParameters
    {
        private readonly IRequestParameters _req = new RequestParameters();

        private TumblrHttpHelper httpHelper;
        public TumblrRequestParameter()
        {
            // DominbatorModel = dominatorAccountModel;
            InitializeHeders();
        }

        public void InitializeHeders()
        {
            try
            {
                Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,image/avif,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.7";
                UserAgent =
                    "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/118.0.0.0 Safari/537.36";
                ContentType = "text/html; charset=utf-8";
                Referer = "https://www.tumblr.com/login";
                AddHeader("Host", "https://www.tumblr.com");
                AddHeader("sec-ch-ua", "\"Not_A Brand\";v=\"8\", \"Chromium\";v=\"120\", \"Google Chrome\";v=\"120\"");
                AddHeader("Accept-Language", "en-US,en;q=0.9");
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        public void SetHeaders(DominatorAccountModel _dominatorAccountModel, string headerName, string HeaderValue)
        {
            _req.Headers[headerName] = HeaderValue;
            httpHelper.SetRequestParameter(_req);
        }

        /// <summary>
        ///     To generate the post based on already stored items from
        ///     <see cref="DominatorHouseCore.Request.RequestParameters.PostDataParameters" />
        /// </summary>
        /// <returns>post data in bytes of sequence</returns>
        public byte[] GetPostDataFromParameters()
        {
            return !IsMultiPart ? GeneratePostData() : CreateMultipartBody();
        }

        public void TumblrAddFileList(string title, byte[] data, string fileName)
        {
            NameValueCollection headers;
            if (title.Equals("video"))
                headers = new NameValueCollection
            {
                {"Content-Type", "video/mp4"},
          //      {"Content-Transfer-Encoding", "binary"}
            };
            else if (title.Equals("photo"))
                headers = new NameValueCollection
            {
                {"Content-Type", "image/jpeg"},
          //      {"Content-Transfer-Encoding", "binary"}
            };
            else if (title.Equals("audio"))
                headers = new NameValueCollection
            {
                {"Content-Type", "audio/mpeg"},
          //      {"Content-Transfer-Encoding", "binary"}
            };
            else
                headers = new NameValueCollection
            {
                {"Content-Type", "application/octet-stream"},
                {"Content-Transfer-Encoding", "binary"}
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
        public override byte[] CreateMultipartBody()
        {
            var multipartBoundary = GenerateMultipartBoundary();

            var strMultipartBoundary = $"------{multipartBoundary}";

            var stringBuilder = new StringBuilder();

            using (var memoryStream = new MemoryStream())
            {
                //     stringBuilder.AppendLine(strMultipartBoundary);

                foreach (var postItem in PostDataParameters)
                {
                    stringBuilder.AppendLine(strMultipartBoundary);
                    stringBuilder.AppendLine($"Content-Disposition: form-data; name=\"file\"");
                    stringBuilder.AppendLine();
                    stringBuilder.AppendLine(postItem.Value);
                }

                foreach (var file in FileList)
                {
                    stringBuilder.AppendLine(strMultipartBoundary);
                    stringBuilder.AppendLine(
                        $"Content-Disposition: form-data; name=\"file\"; filename=\"{file.Value.FileName as object}\"");

                    foreach (string header in file.Value.Headers)
                        stringBuilder.AppendLine($"{(object)header}: {file.Value.Headers[header] as object}");

                    stringBuilder.AppendLine();
                    var bytes = Encoding.UTF8.GetBytes(stringBuilder.ToString());
                    memoryStream.Write(bytes, 0, bytes.Length);
                    stringBuilder.Clear();
                    if (file.Value.Contents != null)
                        memoryStream.Write(file.Value.Contents, 0, file.Value.Contents.Length);
                }

                var bytes1 = Encoding.UTF8.GetBytes(Environment.NewLine + strMultipartBoundary + "--");
                memoryStream.Write(bytes1, 0, bytes1.Length);

                //AddHeader("Content-Type", $"multipart/form-data; boundary={multipartBoundary}");

                ContentType = $"multipart/form-data; boundary=----{multipartBoundary}";
                return memoryStream.ToArray();
            }
        }
        public override byte[] CreateMultipartBodyForBroadCastMessage(string jsonString)
        {
            var jobject = JObject.Parse(jsonString);
            var multipartBoundary = GenerateMultipartBoundary();

            var strMultipartBoundary = $"------{multipartBoundary}";

            var stringBuilder = new StringBuilder();

            using (var memoryStream = new MemoryStream())
            {
                // stringBuilder.AppendLine(strMultipartBoundary);
                foreach (var keyValuePair in jobject)
                {
                    if (keyValuePair.Key == "participants") continue;
                    stringBuilder.AppendLine(strMultipartBoundary);
                    stringBuilder.AppendLine($"Content-Disposition: form-data; name=\"{keyValuePair.Key as object}\"");
                    stringBuilder.AppendLine();
                    stringBuilder.AppendLine(keyValuePair.Value.ToString());
                }
                foreach (var file in FileList)
                {
                    stringBuilder.AppendLine(strMultipartBoundary);
                    stringBuilder.AppendLine(
                        $"Content-Disposition: form-data; name=\"file\"; filename=\"{file.Value.FileName as object}\"");

                    foreach (string header in file.Value.Headers)
                        stringBuilder.AppendLine($"{(object)header}: {file.Value.Headers[header] as object}");

                    stringBuilder.AppendLine();
                    var bytes = Encoding.UTF8.GetBytes(stringBuilder.ToString());
                    memoryStream.Write(bytes, 0, bytes.Length);
                    stringBuilder.Clear();
                    if (file.Value.Contents != null)
                        memoryStream.Write(file.Value.Contents, 0, file.Value.Contents.Length);
                }
                foreach (var keyValuePair in jobject)
                {
                    if (keyValuePair.Key == "participants")
                    {
                        stringBuilder.AppendLine(strMultipartBoundary);
                        //  stringBuilder.AppendLine("Content-Type: text/plain; charset=utf-8");
                        stringBuilder.AppendLine($"Content-Disposition: form-data; name=\"{keyValuePair.Key as object}\"");
                        stringBuilder.AppendLine();
                        stringBuilder.AppendLine(keyValuePair.Value.ToString());
                    }
                }
                var bytess = Encoding.UTF8.GetBytes(stringBuilder.ToString());
                memoryStream.Write(bytess, 0, bytess.Length);
                var bytes1 = Encoding.UTF8.GetBytes(Environment.NewLine + strMultipartBoundary + "--");
                memoryStream.Write(bytes1, 0, bytes1.Length);
                //AddHeader("Content-Type", $"multipart/form-data; boundary={multipartBoundary}");
                ContentType = $"multipart/form-data; boundary=----{multipartBoundary}";
                return memoryStream.ToArray();
            }
        }
        private static string GenerateMultipartBoundary()
        {
            var stringBuilder = new StringBuilder();
            var max = "abcdefghijklmnopqrstuvwxyz0123459876ABCDEFGHIJKLMNOPQRSTUVWXYZ".Length - 1;
            for (var index = 0; index < 16; ++index)
                stringBuilder.Append(
                    "abcdefghijklmnopqrstuvwxyz0123459876ABCDEFGHIJKLMNOPQRSTUVWXYZ"[
                        RandomUtilties.GetRandomNumber(max)]);
            if (string.IsNullOrEmpty(stringBuilder.ToString()))
            {
                var newstrBuilder = new StringBuilder();
                stringBuilder = newstrBuilder.Append(DateTime.Now.Ticks.ToString("x"));
            }

            return "WebKitFormBoundary" + stringBuilder;
        }

        public string AuthorizationHeader { get; set; }
        public string CsrfToken { get; set; }
    }
}