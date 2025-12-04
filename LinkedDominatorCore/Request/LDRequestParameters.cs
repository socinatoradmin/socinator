using DominatorHouseCore;
using DominatorHouseCore.Request;
using LinkedDominatorCore.LDModel;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace LinkedDominatorCore.Request
{
    public class LdRequestParameters : RequestParameters
    {
        public LdRequestParameters(string url) : base(url)
        {
            Url = url;
        }

        public LdRequestParameters()
        {
        }

        public bool IsLoginPostRequest { get; set; }

        public LdJsonElements Body { private get; set; }


        public void Login()
        {
            IsLoginPostRequest = true;
        }

        // passing body inside method safe if body we use body multiple times
        // we are assigning multiple times indirectly changing last element
        public string GenerateStringBody(LdJsonElements body)
        {
            var serializeObject = JsonConvert.SerializeObject(body, new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            });
            return serializeObject;
        }
        public string GenerateMediaUploadJson(MediaPostJsonElement jsonElement)
        {
            return JsonConvert.SerializeObject(jsonElement, new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            });
        }
        public byte[] GenerateBody()
        {
            var str = Body == null ? null : GenerateStringBody(Body);
            if (IsMultiPart)
                return base.CreateMultipartBody(str);
            if (str == null)
                return null;
            return IsLoginPostRequest ? GeneratePostData(str) : LdGeneratePostData(str);
        }

        public byte[] LdGeneratePostData(string jsonString)
        {
            var jObject = JObject.Parse(jsonString);
            var stringBuilder = new StringBuilder();
            foreach (var keyValuePair in jObject)
            {
                stringBuilder.Append("\"");
                stringBuilder.Append(keyValuePair.Key);
                stringBuilder.Append("\"");
                stringBuilder.Append(":");
                stringBuilder.Append("\"");
                stringBuilder.Append(keyValuePair.Value);
                stringBuilder.Append("\"");
            }

            foreach (var postItem in PostDataParameters)
            {
                stringBuilder.Append("\"");
                stringBuilder.Append(postItem.Key);
                stringBuilder.Append("\"");
                stringBuilder.Append(":");
                stringBuilder.Append("\"");
                stringBuilder.Append(postItem.Value);
                stringBuilder.Append("\"");
            }

            --stringBuilder.Length;
            return Encoding.UTF8.GetBytes(stringBuilder.ToString());
        }

        /// <summary>
        ///     To generate the multi part post data for media files and rest of the post data are generated from stored
        ///     <see cref="DominatorHouseCore.Request.RequestParameters.PostDataParameters" /> items
        /// </summary>
        /// <returns>post data in sequences of bytes</returns>
        public override byte[] CreateMultipartBody(string fileExtension)
        {
            var multipartBoundary = GenerateMultipartBoundary();

            var strMultipartBoundary = $"--WebKitFormBoundary{multipartBoundary}";

            var stringBuilder = new StringBuilder();

            using (var memoryStream = new MemoryStream())
            {
                //stringBuilder.AppendLine(strMultipartBoundary);

                foreach (var file in FileList)
                {
                    stringBuilder.AppendLine(strMultipartBoundary);

                    if (file.Value.Headers != null)
                        foreach (string header in file.Value.Headers)
                        {
                            stringBuilder.AppendLine(
                                $"Content-Disposition: form-data; name=\"{(object) header}\"");
                            stringBuilder.AppendLine();
                            stringBuilder.AppendLine($"{file.Value.Headers[header] as object}");
                            stringBuilder.AppendLine(strMultipartBoundary);
                        }

                    stringBuilder.AppendLine(
                        $"Content-Disposition: form-data; name=\"farr\"; filename=\"{file.Value.FileName as object}\" Content-Type: image/{fileExtension.Replace("jpg", "jpeg")}");

                    stringBuilder.AppendLine();
                    var bytes = Encoding.UTF8.GetBytes(stringBuilder.ToString());
                    memoryStream.Write(bytes, 0, bytes.Length);
                    stringBuilder.Clear();
                    if (file.Value.Contents != null)
                        memoryStream.Write(file.Value.Contents, 0, file.Value.Contents.Length);
                }

                var bytes1 = Encoding.UTF8.GetBytes(Environment.NewLine + strMultipartBoundary);
                memoryStream.Write(bytes1, 0, bytes1.Length);

                ContentType = $"multipart/form-data; boundary={multipartBoundary}";
                return memoryStream.ToArray();
            }
        }

        public byte[] CreateMultipartBody(Dictionary<string, string> postParameters, out string contentType)
        {
            var multipartBoundary = GenerateMultipartBoundaryResetPassword();

            var strMultipartBoundary = $"------WebKitFormBoundary{multipartBoundary}";

            var stringBuilder = new StringBuilder();

            using (var memoryStream = new MemoryStream())
            {
                try
                {
                    foreach (var postItem in postParameters)
                        try
                        {
                            stringBuilder.AppendLine(strMultipartBoundary);
                            stringBuilder.AppendLine(
                                $"Content-Disposition: form-data; name=\"{postItem.Key as object}\"");
                            stringBuilder.AppendLine();
                            stringBuilder.AppendLine($"{postItem.Value as object}");
                        }
                        catch (Exception ex)
                        {
                            ex.DebugLog();
                        }
                }
                catch (Exception exception)
                {
                    exception.DebugLog();
                }

                stringBuilder.AppendLine(strMultipartBoundary + "--");
                var bytes = Encoding.UTF8.GetBytes(stringBuilder.ToString());
                memoryStream.Write(bytes, 0, bytes.Length);

                contentType = $"multipart/form-data; boundary=----WebKitFormBoundary{multipartBoundary}";

                return memoryStream.ToArray();
            }
        }

        public byte[] UploadImageMultipartBody(string fileExtension)
        {
            try
            {
                var multipartBoundary = GenerateMultipartBoundary();

                var strMultipartBoundary = $"------WebKitFormBoundary{multipartBoundary.Replace("-", "")}";

                var stringBuilder = new StringBuilder();

                using (var memoryStream = new MemoryStream())
                {
                    foreach (var file in FileList)
                    {
                        stringBuilder.AppendLine(strMultipartBoundary);
                        stringBuilder.AppendLine(
                            $"Content-Disposition: form-data; name=\"file\"; filename=\"{file.Value.FileName}\"");
                        stringBuilder.AppendLine($"Content-Type: image/{fileExtension.Replace("jpg", "jpeg")}");
                        stringBuilder.AppendLine();

                        var bytes = Encoding.UTF8.GetBytes(stringBuilder.ToString());
                        memoryStream.Write(bytes, 0, bytes.Length);
                        stringBuilder.Clear();
                        try
                        {
                            var fileStream = new FileStream(file.Key, FileMode.Open, FileAccess.Read);
                            var buffer = new byte[4096];
                            var bytesRead = 0;
                            while ((bytesRead = fileStream.Read(buffer, 0, buffer.Length)) != 0)
                                memoryStream.Write(buffer, 0, bytesRead);

                            fileStream.Close();
                        }
                        catch (Exception ex)
                        {
                            ex.DebugLog();
                        }


                        stringBuilder.AppendLine();
                        stringBuilder.AppendLine(strMultipartBoundary);

                        if (file.Value.Headers != null)
                        {
                            var count = 0;
                            foreach (string header in file.Value.Headers)
                            {
                                if (file.Value.Headers.Count - 1 == count)
                                {
                                    stringBuilder.AppendLine(
                                        $"Content-Disposition: form-data; name=\"{(object) header}\"");
                                    stringBuilder.AppendLine();
                                    stringBuilder.AppendLine(file.Key);
                                    stringBuilder.AppendLine(strMultipartBoundary + "--");
                                }
                                else
                                {
                                    stringBuilder.AppendLine(
                                        $"Content-Disposition: form-data; name=\"{(object) header}\"");
                                    stringBuilder.AppendLine();
                                    stringBuilder.AppendLine($"{file.Value.Headers[header] as object}");
                                    stringBuilder.AppendLine(strMultipartBoundary);
                                }

                                count++;
                            }

                            var buffer = Encoding.UTF8.GetBytes(stringBuilder.ToString());
                            memoryStream.Write(buffer, 0, buffer.Length);
                            stringBuilder.Clear();
                        }
                    }

                    ContentType = $"multipart/form-data; boundary={strMultipartBoundary.Replace("------", "----")}";
                    return memoryStream.ToArray();
                }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            return null;
        }


        public byte[] CreateMultipartBodyForUploadingImageWhileMessaging(string fileExtension)
        {
            var multipartBoundary = GenerateMultipartBoundary();

            var strMultipartBoundary = $"--{multipartBoundary}";

            var stringBuilder = new StringBuilder();

            using (var memoryStream = new MemoryStream())
            {
                foreach (var file in FileList)
                {
                    stringBuilder.AppendLine(strMultipartBoundary);

                    stringBuilder.AppendLine(
                        $"Content-Disposition: form-data; name=\"file\"; filename=\"{file.Value.FileName as object}\"\r\nContent-Type: image/{fileExtension.Replace("jpg", "jpeg")}");
                    stringBuilder.AppendLine();
                    stringBuilder.AppendLine(strMultipartBoundary);
                    if (file.Value.Headers != null)
                    {
                        var count = 0;
                        foreach (string header in file.Value.Headers)
                            if (header != "file")
                            {
                                stringBuilder.AppendLine(
                                    $"Content-Disposition: form-data; name=\"{(object) header}\"");
                                stringBuilder.AppendLine();
                                stringBuilder.AppendLine($"{file.Value.Headers[header] as object}");
                                if (file.Value.Headers.Count - 2 == count)
                                    stringBuilder.AppendLine(Environment.NewLine + strMultipartBoundary + "--" +
                                                             Environment.NewLine);
                                else
                                    stringBuilder.AppendLine(strMultipartBoundary);
                                count++;
                            }
                    }

                    var bytes = Encoding.UTF8.GetBytes(stringBuilder.ToString());
                    memoryStream.Write(bytes, 0, bytes.Length);
                    stringBuilder.Clear();
                    if (file.Value.Contents != null)
                        memoryStream.Write(file.Value.Contents, 0, file.Value.Contents.Length);
                }

                ContentType = $"multipart/form-data; boundary={multipartBoundary}";
                return memoryStream.ToArray();
            }
        }

        private string GenerateMultipartBoundary()=> "----" + DateTime.Now.Ticks.ToString("x");
        private string GenerateMultipartBoundaryResetPassword()=> DateTime.Now.Ticks.ToString("x") + "x";
    }
}