using DominatorHouseCore;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Request;
using DominatorHouseCore.Utility;
using RedditDominatorCore.RDLibrary;
using RedditDominatorCore.Response;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace RedditDominatorCore.RDUtility
{
    public class Utils
    {
        public static string GetBetween(string strSource, string strStart, string strEnd)
        {
            try
            {
                if (!strSource.Contains(strStart) || !strSource.Contains(strEnd)) return string.Empty;
                var start = strSource.IndexOf(strStart, 0, StringComparison.Ordinal) + strStart.Length;
                var end = strSource.IndexOf(strEnd, start, StringComparison.Ordinal);
                return strSource.Substring(start, end - start);
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }

        public static string GetLastWordFromUrl(string inputString)
        {
            if (!inputString.Contains("/"))
                return inputString;

            if (inputString.Contains("?"))
            {
                var lstText = inputString.Split('?').ToList();
                var text = lstText.FirstOrDefault();
                var lstNewText = text.TrimEnd('/').Split('/').ToList();
                inputString = lstNewText.Last();
            }
            else
            {
                var text = inputString.TrimEnd('/').Split('/').ToList();
                inputString = text.Last();
            }

            return inputString;
        }

        public static string GetRedditId(string inputString)
        {
            if (!inputString.Contains("_")) return inputString;
            var text = inputString.Split('_').ToList();
            inputString = text.Last();
            return inputString;
        }
        public static bool IsMutualWordPresent(List<string> specificwords, string userLastReply)
        {
            foreach (var eachWord in specificwords)
            {
                if (eachWord.ToLower().Contains(userLastReply) || userLastReply.ToLower().Contains(eachWord.ToLower()))
                    return true;
            }
            return false;
        }
        public static string RemoveHtmlTags(string Response)
        {
            if (string.IsNullOrEmpty(Response))
                return Response;
            var HtmlResponse = Regex.Replace(Response, "<(.*)?>", "");
            HtmlResponse = Regex.Replace(HtmlResponse, "<(.*)?", "");
            HtmlResponse = Regex.Replace(HtmlResponse, "(.*)?>", "");
            return HtmlResponse;
        }
        public static FileData GetFileData(string MediaPath, string FileName)
        {
            using (var fileReader = new FileStream(MediaPath, FileMode.Open, FileAccess.Read))
            {
                var br = new BinaryReader(fileReader);
                var buffer1 = br.ReadBytes((int)br.BaseStream.Length);
                fileReader.Close();
                br.Close();
                var nvc = new NameValueCollection();
                return new FileData(nvc, FileName, buffer1);
            }
        }
        public static byte[] GeneratePostDataForMediaUpload(ObservableCollection<string> MediaList, IRequestParameters RequestParameter, PublisherPostUploadResponseHandler UploadParameter, bool PublishOnGroup = false)
        {
            var multipartBoundary = "----WebKitFormBoundary" + DateTime.Now.Ticks.ToString("X");
            var strMultipartBoundary = $"--{multipartBoundary}";
            var stringBuilder = new StringBuilder();
            using (var memoryStream = new MemoryStream())
            {
                foreach (var file in MediaList)
                {
                    var FileContentType = MediaUtilites.GetMimeTypeByFilePath(file);
                    var FileName = new FileInfo(file).Name;
                    FileName = PublishOnGroup ? "poster.png" : FileName;
                    var fileData = GetFileData(file, FileName);
                    stringBuilder.AppendLine(strMultipartBoundary);
                    stringBuilder.AppendLine("Content-Disposition: form-data; name=\"x-amz-storage-class\"");
                    stringBuilder.AppendLine();
                    stringBuilder.AppendLine(UploadParameter.XAmzStorageClass);
                    stringBuilder.AppendLine(strMultipartBoundary);
                    stringBuilder.AppendLine("Content-Disposition: form-data; name=\"success_action_status\"");
                    stringBuilder.AppendLine();
                    stringBuilder.AppendLine(UploadParameter.SuccessActionStatus);
                    stringBuilder.AppendLine(strMultipartBoundary);
                    stringBuilder.AppendLine("Content-Disposition: form-data; name=\"bucket\"");
                    stringBuilder.AppendLine();
                    stringBuilder.AppendLine(UploadParameter.Bucket);
                    stringBuilder.AppendLine(strMultipartBoundary);
                    stringBuilder.AppendLine("Content-Disposition: form-data; name=\"acl\"");
                    stringBuilder.AppendLine();
                    stringBuilder.AppendLine(UploadParameter.Acl);
                    stringBuilder.AppendLine(strMultipartBoundary);
                    stringBuilder.AppendLine("Content-Disposition: form-data; name=\"x-amz-meta-ext\"");
                    stringBuilder.AppendLine();
                    stringBuilder.AppendLine(UploadParameter.XAmzMetaExt);
                    stringBuilder.AppendLine(strMultipartBoundary);
                    stringBuilder.AppendLine("Content-Disposition: form-data; name=\"x-amz-date\"");
                    stringBuilder.AppendLine();
                    stringBuilder.AppendLine(UploadParameter.XAmzDate);
                    stringBuilder.AppendLine(strMultipartBoundary);
                    stringBuilder.AppendLine("Content-Disposition: form-data; name=\"x-amz-signature\"");
                    stringBuilder.AppendLine();
                    stringBuilder.AppendLine(UploadParameter.XAmzSignature);
                    stringBuilder.AppendLine(strMultipartBoundary);
                    stringBuilder.AppendLine("Content-Disposition: form-data; name=\"x-amz-security-token\"");
                    stringBuilder.AppendLine();
                    stringBuilder.AppendLine(UploadParameter.XAmzSecurityToken);
                    stringBuilder.AppendLine(strMultipartBoundary);
                    stringBuilder.AppendLine("Content-Disposition: form-data; name=\"x-amz-algorithm\"");
                    stringBuilder.AppendLine();
                    stringBuilder.AppendLine(UploadParameter.XAmzAlgorithm);
                    stringBuilder.AppendLine(strMultipartBoundary);
                    stringBuilder.AppendLine("Content-Disposition: form-data; name=\"key\"");
                    stringBuilder.AppendLine();
                    stringBuilder.AppendLine(UploadParameter.Key);
                    stringBuilder.AppendLine(strMultipartBoundary);
                    stringBuilder.AppendLine("Content-Disposition: form-data; name=\"policy\"");
                    stringBuilder.AppendLine();
                    stringBuilder.AppendLine(UploadParameter.Policy);
                    stringBuilder.AppendLine(strMultipartBoundary);
                    stringBuilder.AppendLine("Content-Disposition: form-data; name=\"x-amz-credential\"");
                    stringBuilder.AppendLine();
                    stringBuilder.AppendLine(UploadParameter.XAmzCredential);
                    stringBuilder.AppendLine(strMultipartBoundary);
                    stringBuilder.AppendLine("Content-Disposition: form-data; name=\"Content-Type\"");
                    stringBuilder.AppendLine();
                    stringBuilder.AppendLine(UploadParameter.ContentType);
                    stringBuilder.AppendLine(strMultipartBoundary);
                    stringBuilder.AppendLine($"Content-Disposition: form-data; name=\"file\"; filename=\"{FileName}\"");
                    stringBuilder.AppendLine($"Content-Type: {FileContentType}");
                    stringBuilder.AppendLine();
                    var bytes = Encoding.UTF8.GetBytes(stringBuilder.ToString());
                    memoryStream.Write(bytes, 0, bytes.Length);
                    stringBuilder.Clear();
                    if (fileData != null)
                        memoryStream.Write(fileData.Contents, 0, fileData.Contents.Length);
                }
                var bytes1 = Encoding.UTF8.GetBytes(Environment.NewLine + strMultipartBoundary + "--");
                memoryStream.Write(bytes1, 0, bytes1.Length);
                RequestParameter.ContentType = $"multipart/form-data; boundary={multipartBoundary}";//for post header.
                return memoryStream.ToArray();
            }
        }
        public static string GenerateRichText(string Description)
        {
            var RichText = string.Empty;
            try
            {
                var TextArray = Regex.Split(Description, "\n");
                foreach (var Text in TextArray)
                {
                    var Url = Regex.Match(Text, "http(s)?://([\\w+?\\.\\w+])+([a-zA-Z0-9\\~\\!\\@\\#\\$\\%\\^\\&amp;\\*\\(\\)_\\-\\=\\+\\\\\\/\\?\\.\\:\\;\\'\\,]*)?", RegexOptions.IgnoreCase).Value;
                    if (!string.IsNullOrEmpty(Url))
                    {
                        RichText += $"{{\"e\":\"link\",\"t\":\"{Url}\",\"u\":\"{Url}\"}}";
                        if (!string.IsNullOrEmpty(Text.Replace(Url, "")))
                            RichText += $",{{\"e\":\"text\",\"t\":\"{Text.Replace(Url, "")}\"}}";
                    }
                    else
                        RichText += $"{{\"e\":\"par\",\"c\":[{{\"e\":\"text\",\"t\":\"{Text}\"}}]}}";
                    if (Text != TextArray.LastOrDefault())
                        RichText += ",";
                }
            }
            catch (Exception ex) { ex.DebugLog(); }
            return RichText;
        }
        public static string GetCommunityJson(string Response)
            => Utils.GetBetween(RdConstants.GetDecodedResponse(Response, true, true), "initialStateJSON=\"", "></left-nav-communities-controller>")?.Replace(" ", "")?.Replace("\r\n", "")?.Replace("\n", "")?.TrimEnd('\"');
        public static string UpdateDomain(string Url)
        {
            return Url;
            //if (string.IsNullOrEmpty(Url))
            //return Url;
            //var Splitted = Url.Split('/')?.ToList();
            //Splitted.RemoveAll(x => string.IsNullOrEmpty(x));
            //return RdConstants.NewRedditHomePageAPI + "/" + string.Join("/", Splitted.Skip(2));
        }
    }
}