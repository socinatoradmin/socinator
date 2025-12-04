#region

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using DominatorHouseCore.Annotations;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using Newtonsoft.Json.Linq;

#endregion

namespace DominatorHouseCore.Request
{
    public class RequestParameters : IRequestParameters
    {
        public RequestParameters(string url)
        {
            Url = url;
        }

        public RequestParameters()
        {
        }

        /// <summary>
        ///     To assign the Cookies for request
        /// </summary>
        public virtual CookieCollection Cookies { get; set; }

        /// <summary>
        ///     To assign the Http header
        /// </summary>
        public WebHeaderCollection Headers { get; set; } = new WebHeaderCollection();

        /// <summary>
        ///     To assign proxy for the http request
        /// </summary>
        public Proxy Proxy { get; set; }

        /// <summary>
        ///     If its true, allows the same tcp connection for upcoming http connections
        /// </summary>
        public bool KeepAlive { get; set; }

        /// <summary>
        ///     To specify media types which are acceptable for the response
        /// </summary>
        public string Accept { get; set; }

        /// <summary>
        ///     To specify the address of the previous pages of the http request
        /// </summary>
        public string Referer { get; set; }

        /// <summary>
        ///     To specify the media type of the body of the http request
        /// </summary>
        public string ContentType { get; set; }

        /// <summary>
        ///     To specify the user agent
        /// </summary>
        public string UserAgent { get; set; }

        /// <summary>
        ///     To specify the get or post request url
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        ///     To specify the post data in bytes of sequences
        /// </summary>
        public byte[] PostData { get; set; }

        /// <summary>
        ///     To specify whether request for media type or not, such as images
        /// </summary>
        public bool IsMultiPart { get; set; }

        public bool IsMultiPartForBroadCast { get; set; }

        /// <summary>
        ///     AddHeader is used to add the header key and values to <see cref="RequestParameters.Headers" />
        /// </summary>
        /// <param name="key">The header to add to the collection</param>
        /// <param name="value">The content of the header</param>
        public void AddHeader(string key, string value)
        {
            try
            {
                Headers.Add(key, value);
            }
            catch (Exception ex)
            {
                ex.DebugLog(ex.StackTrace);
            }
        }

        /// <summary>
        ///     To add media files with their respective <see cref="DominatorHouseCore.Request.FileData" /> details
        /// </summary>
        public Dictionary<string, FileData> FileList { get; set; } = new Dictionary<string, FileData>();

        /// <summary>
        ///     To append the extra parameter to url
        ///     For example : https://webaddress.com/?paramkey1=paramvalue1&paramkey2=paramvalue2
        ///     Here url : https://webaddress.com/
        ///     So UrlParameters contains two items
        ///     UrlParameters  = new Dictionary &lt;string, string &gt;() { { "paramkey1", "paramvalue1" } , { "paramkey2",
        ///     "paramvalue2" } };
        /// </summary>
        public Dictionary<string, string> UrlParameters { get; set; }
            = new Dictionary<string, string>();

        /// <summary>
        ///     To Add the extra parameter to post data
        ///     For Example posturl =  https://webaddress.com/ and
        ///     postdata = paramkey1=paramvalue1&paramkey2=paramvalue2
        ///     So PostDataParameters contains two items
        ///     PostDataParameters  = new Dictionary &lt;string, string &gt;() { { "paramkey1", "paramvalue1" } , { "paramkey2",
        ///     "paramvalue3" } };
        /// </summary>
        public Dictionary<string, string> PostDataParameters { get; set; }
            = new Dictionary<string, string>();


        /// <summary>
        ///     To add the extra parameter along with url.
        ///     The key and values will add in  <see cref="DominatorHouseCore.Request.RequestParameters.UrlParameters" />
        /// </summary>
        /// <param name="key">parameter title</param>
        /// <param name="value">parameter value</param>
        public void AddUrlParameters(string key, string value)
        {
            UrlParameters.Add(key, value);
        }

        /// <summary>
        ///     To add the extra parameter in post data.
        ///     The key and values will add in  <see cref="DominatorHouseCore.Request.RequestParameters.PostDataParameters" />
        /// </summary>
        /// <param name="key">parameter title</param>
        /// <param name="value">parameter value</param>
        public void AddPostDataParameters(string key, string value)
        {
            PostDataParameters.Add(key, value);
        }

        /// <summary>
        ///     To add the file description along with title, image byte sequences and file name
        /// </summary>
        /// <param name="title">File title</param>
        /// <param name="data">image byte sequences</param>
        /// <param name="fileName">file name </param>
        public void AddFileList(string title, byte[] data, string fileName)
        {
            var headers = new NameValueCollection
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

        /// <summary>
        ///     To Generate the request url alone with <see cref="DominatorHouseCore.Request.RequestParameters.Url" /> and stored
        ///     <see cref="DominatorHouseCore.Request.RequestParameters.UrlParameters" /> items
        /// </summary>
        /// <returns>returns the valid url</returns>
        public string GenerateUrl()
        {
            var array = GetUrlParameterValues();
            return $"{Url}{(array.Length != 0 ? "?" : string.Empty)}{string.Join("&", array)}";
        }

        /// <summary>
        ///     To Generate the request url alone with given url and stored
        ///     <see cref="DominatorHouseCore.Request.RequestParameters.UrlParameters" /> items
        /// </summary>
        /// <param name="url">the actual url</param>
        /// <returns>returns the valid url</returns>
        public string GenerateUrl([NotNull] string url)
        {
            if (url == null)
                throw new ArgumentNullException(nameof(url));

            var array = GetUrlParameterValues();

            return $"{url}{(array.Length != 0 ? "?" : string.Empty)}{string.Join("&", array)}";
        }

        /// <summary>
        ///     To retrieve the stored <see cref="DominatorHouseCore.Request.RequestParameters.UrlParameters" /> items as bytes
        ///     sequences
        /// </summary>
        /// <returns>stored items in <see cref="DominatorHouseCore.Request.RequestParameters.UrlParameters" /></returns>
        private string[] GetUrlParameterValues()
        {
            return UrlParameters.Select(x =>
                $"{WebUtility.UrlEncode(x.Key)}={WebUtility.UrlEncode(x.Value)}").ToArray();
        }


        /// <summary>
        ///     To generate the normal post data from already stored in
        ///     <see cref="DominatorHouseCore.Request.RequestParameters.PostDataParameters" />
        /// </summary>
        /// <returns>post data in sequences of bytes</returns>
        public byte[] GeneratePostData()
        {
            var stringBuilder = new StringBuilder();
            foreach (var postItem in PostDataParameters)
            {
                stringBuilder.Append(postItem.Key);
                stringBuilder.Append("=");
                stringBuilder.Append(postItem.Value);
                stringBuilder.Append("&");
            }

            --stringBuilder.Length;
            return Encoding.UTF8.GetBytes(stringBuilder.ToString());
        }


        /// <summary>
        ///     To generate the normal post data from json string
        /// </summary>
        /// <param name="jsonString">post data which will pass as bytes</param>
        /// <returns>post data in sequences of bytes</returns>
        public byte[] GeneratePostDataFromJson(string jsonString)
        {
            var jobject = JObject.Parse(jsonString);
            var stringBuilder = new StringBuilder();
            foreach (var keyValuePair in jobject)
            {
                stringBuilder.Append(keyValuePair.Key);
                stringBuilder.Append("=");
                stringBuilder.Append(keyValuePair.Value);
                stringBuilder.Append("&");
            }

            --stringBuilder.Length;
            return Encoding.UTF8.GetBytes(stringBuilder.ToString());
        }


        /// <summary>
        ///     To generate the normal post data from json and already stored in
        ///     <see cref="DominatorHouseCore.Request.RequestParameters.PostDataParameters" />
        /// </summary>
        /// <param name="jsonString">post data which will pass as bytes</param>
        /// <returns>post data in sequences of bytes</returns>
        protected byte[] GeneratePostData(string jsonString, bool isDecode = false)
        {
            var jobject = JObject.Parse(jsonString);
            var stringBuilder = new StringBuilder();
            foreach (var keyValuePair in jobject)
            {
                
                stringBuilder.Append(keyValuePair.Key.ToString());
                stringBuilder.Append("=");
                var data1 = keyValuePair.Value.ToString();
                if (isDecode)
                    stringBuilder.Append(WebUtility.UrlEncode(keyValuePair.Value.ToString()));
                else
                    stringBuilder.Append(keyValuePair.Value);
                stringBuilder.Append("&");
            }

            foreach (var postItem in PostDataParameters)
            {
                stringBuilder.Append(postItem.Key);
                stringBuilder.Append("=");
                stringBuilder.Append(postItem.Value);
                stringBuilder.Append("&");
            }

            --stringBuilder.Length;

            var data = stringBuilder.ToString();
            return Encoding.UTF8.GetBytes(data);
        }


        /// <summary>
        ///     Generate multipart boundary values for multipart initial values
        /// </summary>
        /// <returns></returns>
        private static string GenerateMultipartBoundary()
        {
            var stringBuilder = new StringBuilder();
            var max = "-_1234567890abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ".Length - 1;
            for (var index = 0; index < 30; ++index)
                stringBuilder.Append(
                    "-_1234567890abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ"[
                        RandomUtilties.GetRandomNumber(max)]);
            return stringBuilder.ToString();
        }


        /// <summary>
        ///     To generate the multi part post data for media files and rest of the post data are generated from stored
        ///     <see cref="DominatorHouseCore.Request.RequestParameters.PostDataParameters" /> items
        /// </summary>
        /// <returns>post data in sequences of bytes</returns>
        public virtual byte[] CreateMultipartBody()
        {
            var multipartBoundary = GenerateMultipartBoundary();

            var strMultipartBoundary = $"--{multipartBoundary}";

            var stringBuilder = new StringBuilder();

            using (var memoryStream = new MemoryStream())
            {
                stringBuilder.AppendLine(strMultipartBoundary);

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
                        stringBuilder.AppendLine($"{(object)header}: {file.Value.Headers[header] as object}");

                    stringBuilder.AppendLine();
                    var bytes = Encoding.UTF8.GetBytes(stringBuilder.ToString());
                    memoryStream.Write(bytes, 0, bytes.Length);
                    stringBuilder.Clear();
                    memoryStream.Write(file.Value.Contents, 0, file.Value.Contents.Length);
                }

                var bytes1 = Encoding.UTF8.GetBytes(Environment.NewLine + strMultipartBoundary);
                memoryStream.Write(bytes1, 0, bytes1.Length);

                //AddHeader("Content-Type", $"multipart/form-data; boundary={multipartBoundary}");

                ContentType = $"multipart/form-data; boundary={multipartBoundary}";
                return memoryStream.ToArray();
            }
        }

        public virtual byte[] CreateMultipartBodyForBroadCastMessage(string jsonString)
        {
            var jobject = JObject.Parse(jsonString);
            var multipartBoundary = GenerateMultipartBoundary();

            var strMultipartBoundary = $"--{multipartBoundary}";

            var stringBuilder = new StringBuilder();

            using (var memoryStream = new MemoryStream())
            {
                // stringBuilder.AppendLine(strMultipartBoundary);
                foreach (var keyValuePair in jobject)
                {
                    stringBuilder.AppendLine(strMultipartBoundary);
                    stringBuilder.AppendLine("Content-Type: text/plain; charset=utf-8");
                    stringBuilder.AppendLine($"Content-Disposition: form-data; name=\"{keyValuePair.Key as object}\"");
                    stringBuilder.AppendLine();
                    stringBuilder.AppendLine(keyValuePair.Value.ToString());
                }

                var bytess = Encoding.UTF8.GetBytes(stringBuilder.ToString());
                memoryStream.Write(bytess, 0, bytess.Length);
                var bytes1 = Encoding.UTF8.GetBytes(strMultipartBoundary);
                memoryStream.Write(bytes1, 0, bytes1.Length);

                //AddHeader("Content-Type", $"multipart/form-data; boundary={multipartBoundary}");
                ContentType = $"multipart/form-data; boundary={multipartBoundary}";
                return memoryStream.ToArray();
            }
        }


        /// <summary>
        ///     To generate the multi part post data for media files and rest of the post data are generated from given Jsonstring
        /// </summary>
        /// <param name="jsonString">post data which will pass as bytes</param>
        /// <returns>post data in sequences of bytes</returns>
        public virtual byte[] CreateMultipartBodyFromJson(string jsonString)
        {
            var jobject = JObject.Parse(jsonString);
            var multipartBoundary = GenerateMultipartBoundary();

            var strMultipartBoundary = $"--{multipartBoundary}";

            var stringBuilder = new StringBuilder();

            using (var memoryStream = new MemoryStream())
            {
                stringBuilder.AppendLine(strMultipartBoundary);
                foreach (var keyValuePair in jobject)
                {
                    stringBuilder.AppendLine(strMultipartBoundary);
                    stringBuilder.AppendLine($"Content-Disposition: form-data; name=\"{keyValuePair.Key as object}\"");
                    stringBuilder.AppendLine();
                    stringBuilder.AppendLine(keyValuePair.Value.ToString());
                }

                foreach (var file in FileList)
                {
                    stringBuilder.AppendLine(strMultipartBoundary);
                    stringBuilder.AppendLine(
                        $"Content-Disposition: form-data; name=\"{file.Key as object}\"; filename=\"{file.Value.FileName as object}\"");

                    foreach (string header in file.Value.Headers)
                        stringBuilder.AppendLine($"{(object)header}: {file.Value.Headers[header] as object}");

                    stringBuilder.AppendLine();
                    var bytes = Encoding.UTF8.GetBytes(stringBuilder.ToString());
                    memoryStream.Write(bytes, 0, bytes.Length);
                    stringBuilder.Clear();
                    memoryStream.Write(file.Value.Contents, 0, file.Value.Contents.Length);
                }

                var bytes1 = Encoding.UTF8.GetBytes(Environment.NewLine + strMultipartBoundary);
                memoryStream.Write(bytes1, 0, bytes1.Length);

                //   AddHeader("Content-Type", $"multipart/form-data; boundary={multipartBoundary}");
                ContentType = $"multipart/form-data; boundary={multipartBoundary}";
                return memoryStream.ToArray();
            }
        }


        /// <summary>
        ///     To generate the multi part post data for media files and rest of the post data are generated from given Jsonstring
        ///     and stored <see cref="DominatorHouseCore.Request.RequestParameters.PostDataParameters" /> items
        /// </summary>
        /// <param name="jsonString">post data which will pass as bytes</param>
        /// <returns>post data in sequences of bytes</returns>
        public virtual byte[] CreateMultipartBody(string jsonString)
        {
            var jobject = JObject.Parse(jsonString);
            var multipartBoundary = GenerateMultipartBoundary();

            var strMultipartBoundary = $"--{multipartBoundary}";

            var stringBuilder = new StringBuilder();

            using (var memoryStream = new MemoryStream())
            {
                stringBuilder.AppendLine(strMultipartBoundary);
                foreach (var keyValuePair in jobject)
                {
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
                        stringBuilder.AppendLine($"{(object)header}: {file.Value.Headers[header] as object}");

                    stringBuilder.AppendLine();
                    var bytes = Encoding.UTF8.GetBytes(stringBuilder.ToString());
                    memoryStream.Write(bytes, 0, bytes.Length);
                    stringBuilder.Clear();
                    memoryStream.Write(file.Value.Contents, 0, file.Value.Contents.Length);
                }

                var bytes1 = Encoding.UTF8.GetBytes(Environment.NewLine + strMultipartBoundary);
                memoryStream.Write(bytes1, 0, bytes1.Length);

                //AddHeader("Content-Type", $"multipart/form-data; boundary={multipartBoundary}");
                ContentType = $"multipart/form-data; boundary={multipartBoundary}";
                return memoryStream.ToArray();
            }
        }
    }
}