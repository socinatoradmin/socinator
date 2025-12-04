using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using DominatorHouseCore;
using DominatorHouseCore.Models;
using DominatorHouseCore.Request;
using DominatorHouseCore.Utility;
using Newtonsoft.Json;
using QuoraDominatorCore.Models;
using QuoraDominatorCore.QdLibrary;
using QuoraDominatorCore.Request;

namespace QuoraDominatorCore.QdUtility
{
    public class QdUtilities
    {
        public static Random ObjRandom { get; } = new Random(Guid.NewGuid().GetHashCode());
        public static string GetDecodedResponse(string response)
        {
            if (string.IsNullOrEmpty(response))
                return response;
            try
            {
                response = Regex.Replace(response, "\\\\([^u])", "\\\\$1").Replace("\\", "");
                response = WebUtility.HtmlDecode(response)?.Replace("u003C", "<")?.Replace("u00252C", ",");
            }
            catch (Exception)
            {
                response = WebUtility.HtmlDecode(response).Replace("u003C", "<")?.Replace("u00252C", ",");
            }

            return response?.Replace("<!--", string.Empty)?.Replace("--!>", string.Empty);
        }

        public static string CalculateMD5Hash(string input, string image)
        {
            var adId = string.Empty;
            var combinedText = input + image;
            try
            {
                var md5 = MD5.Create();
                var inputBytes = Encoding.ASCII.GetBytes(combinedText);
                adId = GetMd5HashCode(input, md5, inputBytes);
                //1C69F0697DBBDA89AF130D25F234EBF5
                //1C69F0697DBBDA89AF130D25F234EBF5
                //byte[] bArray = new byte[inputBytes.Length + 1];
                //inputBytes.CopyTo(bArray, 0);
                //bArray[bArray.Length - 1] = Convert.ToByte('\0');
                //newImage = input.Split('.')[0] + "_hash.jpg";
                //File.WriteAllBytes(newImage, bArray);
                //inputBytes = File.ReadAllBytes(newImage);
                //string newHashCode = GetMd5HashCode(newImage, md5, inputBytes);
                //if (File.Exists(input))
                //    File.Delete(input);
            }
            catch (Exception)
            {
            }

            return adId;
        }

        public static string GetMd5HashCode(string input, MD5 md5, byte[] inputBytes)
        {
            var sb = new StringBuilder();
            // byte[] inputBytess = File.ReadAllBytes(input);
            var hash1 = md5.ComputeHash(inputBytes);

            // step 2, convert byte array to hex string

            for (var i = 0; i < hash1.Length; i++) sb.Append(hash1[i].ToString("X2"));
            return sb.ToString();
        }

        public static string GetNewPrtialDecodedResponse(string response, bool ispostProcess = false)
        {
            var decodedResponse = string.Empty;

            try
            {
                if (response.StartsWith("<!DOCTYPE html>"))
                {
                    return GetDecodedResponse(response);
                }

                decodedResponse = Regex.Unescape(response).Replace("\\", "");
                decodedResponse = Regex.Replace(decodedResponse, "\\\\([^u])", "\\\\$1");
                decodedResponse = WebUtility.HtmlDecode(decodedResponse).Replace("\\u003C\\", "<")
                    .Replace("\\u003C", "<").Replace("u00252C", ",").Replace("<//", "<");
                return decodedResponse.Replace("<!--", string.Empty).Replace("--!>", string.Empty);
            }
            catch (Exception ex)
            {
                if (ispostProcess == false)
                    ex.DebugLog();
                else
                    Console.WriteLine(ex.StackTrace);

                return GetDecodedResponse(response);
            }
        }

        public static int GetDateDifferenceFromTimeStamp(double TimeStamp)
        {
            try
            {
                // here we taking tweets within day
                // therefore we using Math.Ceiling
                var currentDate = DateTime.UtcNow;
                var date1 = TimeStamp.EpochToDateTimeUtc();
                var ts = currentDate - date1;
                //  return (int)(ts.TotalDays);
                return (int)Math.Ceiling(ts.TotalDays);
            }
            catch (Exception)
            {
                return 0;
            }
        }

        public static int GetDateDifferenceFromTimeStamp(DateTime DateTime)
        {
            try
            {
                var currentDate = DateTime.UtcNow;
                var ts = currentDate - DateTime;

                return (int)Math.Ceiling(ts.TotalDays);
            }
            catch (Exception)
            {
                return 0;
            }
        }
        public static string ConvertDecimalToNumber(string number)
        {
            if (string.IsNullOrEmpty(number) ? true : !number.Contains("."))
                return number;
            var Base = number.Contains("k") ? 10 : number.Contains("M") ? 100 : 1000;
            number = number?.Replace("k", "")?.Replace("M", "")?.Replace("K","")?.Trim();
            var decimalLength = number.Split('.').LastOrDefault().Length;
            int.TryParse(number?.Replace(".", ""), out int originalNumber);
            return (originalNumber * Math.Pow(Base, decimalLength)).ToString();
        }
        public static byte[] GeneratePostBodyForMediaUpload(List<string> MediaList,HttpWebRequest RequestParameter,BasePostData basePost)
        {
            var multipartBoundary = "----WebKitFormBoundary" + GetRandomString(16);
            var strMultipartBoundary = $"--{multipartBoundary}";
            var stringBuilder = new StringBuilder();
            using (var memoryStream = new MemoryStream())
            {
                foreach (var file in MediaList)
                {
                    var FileContentType = MediaUtilites.GetMimeTypeByFilePath(file);
                    var FileName = new FileInfo(file).Name;
                    var fileData = GetFileData(file, FileName);
                    stringBuilder.AppendLine(strMultipartBoundary);
                    stringBuilder.AppendLine("Content-Disposition: form-data; name=\"kind\"");
                    stringBuilder.AppendLine();
                    stringBuilder.AppendLine("qtext");
                    stringBuilder.AppendLine(strMultipartBoundary);
                    stringBuilder.AppendLine($"Content-Disposition: form-data; name=\"file\"; filename=\"{FileName}\"");
                    stringBuilder.AppendLine($"Content-Type: {FileContentType}");
                    stringBuilder.AppendLine();
                    var bytes = Encoding.UTF8.GetBytes(stringBuilder.ToString());
                    memoryStream.Write(bytes, 0, bytes.Length);
                    stringBuilder.Clear();
                    if (fileData != null)
                        memoryStream.Write(fileData.Contents, 0, fileData.Contents.Length);
                    stringBuilder.AppendLine(Environment.NewLine + strMultipartBoundary);
                    stringBuilder.AppendLine("Content-Disposition: form-data; name=\"nid\"");
                    stringBuilder.AppendLine();
                    stringBuilder.AppendLine("0");
                    stringBuilder.AppendLine(strMultipartBoundary);
                    stringBuilder.AppendLine("Content-Disposition: form-data; name=\"revision\"");
                    stringBuilder.AppendLine();
                    stringBuilder.AppendLine("e4d44ec1abba41a3164e8c3112dedb420243af2e");
                    stringBuilder.AppendLine(strMultipartBoundary);
                    stringBuilder.AppendLine("Content-Disposition: form-data; name=\"postkey\"");
                    stringBuilder.AppendLine();
                    stringBuilder.AppendLine(basePost.PostKey);
                    stringBuilder.AppendLine(strMultipartBoundary);
                    stringBuilder.AppendLine("Content-Disposition: form-data; name=\"is_canary_revision\"");
                    stringBuilder.AppendLine();
                    stringBuilder.AppendLine("false");
                    stringBuilder.AppendLine(strMultipartBoundary);
                    stringBuilder.AppendLine("Content-Disposition: form-data; name=\"formkey\"");
                    stringBuilder.AppendLine();
                    stringBuilder.AppendLine("ca75672a7dbf2ca09938f0f25f182ce7");
                    stringBuilder.AppendLine(strMultipartBoundary);
                    stringBuilder.AppendLine("Content-Disposition: form-data; name=\"window_id\"");
                    stringBuilder.AppendLine();
                    stringBuilder.AppendLine(basePost.WindowId);
                    stringBuilder.AppendLine(strMultipartBoundary);
                    stringBuilder.AppendLine("Content-Disposition: form-data; name=\"referring_controller\"");
                    stringBuilder.AppendLine();
                    stringBuilder.AppendLine(basePost.ReferringController);
                    stringBuilder.AppendLine(strMultipartBoundary);
                    stringBuilder.AppendLine("Content-Disposition: form-data; name=\"referring_action\"");
                    stringBuilder.AppendLine();
                    stringBuilder.AppendLine(basePost.ReferringAction);
                    bytes = Encoding.UTF8.GetBytes(stringBuilder.ToString());
                    memoryStream.Write(bytes, 0, bytes.Length);
                    stringBuilder.Clear();
                }
                var bytes1 = Encoding.UTF8.GetBytes(strMultipartBoundary + "--");
                memoryStream.Write(bytes1, 0, bytes1.Length);
                RequestParameter.ContentType = $"multipart/form-data; boundary={multipartBoundary}";
                return memoryStream.ToArray();
            }
        }

        public static FileData GetFileData(string MediaPath, string FileName)
        {
            using (var fileReader = new FileStream(MediaPath, FileMode.Open, FileAccess.Read))
            {
                var br = new BinaryReader(fileReader);
                var buffer1 = br.ReadBytes((int)br.BaseStream.Length);
                fileReader.Close();
                br.Close();
                return new FileData(new NameValueCollection(), FileName, buffer1);
            }
        }
        public static string GetRandomString(int outputStringLength) =>
            new string (Enumerable.Repeat("ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789", outputStringLength)
                .Select(s => s[ObjRandom.Next(s.Length)]).ToArray());
        public static ReportOptionsModel GetReportDetails(string ReportString)
        {
            var ReportDetails=new ReportOptionsModel();
            try
            {
                if(!string.IsNullOrEmpty(ReportString))
                {
                    ReportDetails=JsonConvert.DeserializeObject<ReportOptionsModel>(ReportString);
                }
            }catch(Exception ex) { ex.DebugLog(); }
            return ReportDetails;
        }
        public static string GetReportReason(string ReportTitle)
        {
            string Reason;
            switch (ReportTitle)
            {
                case "Sexual exploitation and abuse (child safety)":
                    Reason = "sexual_exploitation_minors";
                    break;
                case "Sexual exploitation and abuse (adults and animals)":
                    Reason = "sexual_exploitation_adults_or_non_humans";
                    break;
                case "Hate Speech":
                    Reason = "hatespeech";
                    break;
                case "Plagiarism":
                    Reason = "cite_sources";
                    break;
                case "Contains adult content":
                    Reason = "name_adult";
                    break;
                case "Contains profanity or obscenity":
                    Reason = "name_explicit";
                    break;
                case "Contains abuse or hate speech":
                    Reason = "name_abuse";
                    break;
                case "Adult content (Consensual)":
                    Reason = "adult_content";
                    break;
                case "Doesn't answer the question":
                    Reason = "not_an_answer";
                    break;
                case "Joke answer":
                    Reason = "joke";
                    break;
                default:
                    Reason=ReportTitle.ToLower().Replace(" ","_");
                    break;
            }
            return Reason;
        }
        public static string GetDecodedWebResponse(string WebResponse, bool HtmlDecode = false, bool UrlDecode = false) =>
            string.IsNullOrEmpty(WebResponse) ? WebResponse : HtmlDecode && UrlDecode ? WebUtility.HtmlDecode(WebUtility.UrlDecode(WebResponse)) :
            HtmlDecode ? WebUtility.HtmlDecode(WebResponse) : WebUtility.UrlDecode(WebResponse);
        public static string EncodeGermanSpecialCharacterToUTF8(string Response)=>
            string.IsNullOrEmpty(Response) ? Response
            :Response.Replace("\\\\u00fc", "%C3%BC")
                .Replace("\\\\u00dc", "%C3%9C")
                .Replace("\\\\u00df", "%C3%9F")
                .Replace("\\\\u00e4", "%C3%A4")
                .Replace("\\\\u00c4", "%C3%84")
                .Replace("\\\\u00f6", "%C3%B6")
                .Replace("\\\\u00d6", "%C3%96");
        public static string GetJsonPostBody(string PostText,string MediaUrl="")
        {
            if (string.IsNullOrEmpty(PostText)) return string.Empty;
            var textlist=PostText.Replace("\r","").Split('\n');
            var sections=new List<PostBodyJsonElement>();
            textlist.ForEach(text =>
            {
                var element = new PostBodyJsonElement();
                if (text.StartsWith("[") && text.EndsWith("]"))
                {
                    text = text.Substring(1, text.Length - 2);
                    var AnswerUrlList = Regex.Split(text, ",");
                    foreach (var url in AnswerUrlList)
                        if (!string.IsNullOrEmpty(url) && !url.Contains("main-qimg") && (url.Trim().StartsWith("https://") || url.StartsWith("http")))
                        {
                            element = new PostBodyJsonElement()
                            {
                                Type = "hyperlink_embed",
                                Indent = 0,
                                Quoted = false,
                                Spans = new List<Spans>
                                {
                                  new Spans
                                  {
                                    Modifiers=new Modifiers
                                    {
                                        Embed=new Embed
                                        {
                                            Url=url.Trim()
                                        }
                                    }
                                  }
                                }
                            };
                            sections.Add(element);
                        }
                }
                else
                {
                    if (!string.IsNullOrEmpty(text) && !text.Contains("main-qimg") && (text.Trim().StartsWith("https://") || text.StartsWith("http")))
                    {
                        element = new PostBodyJsonElement()
                        {
                            Type = "hyperlink_embed",
                            Indent = 0,
                            Quoted = false,
                            Spans = new List<Spans>
                        {
                            new Spans
                            {
                                Modifiers=new Modifiers
                                {
                                    Embed=new Embed
                                    {
                                        Url=text.Trim()
                                    }
                                }
                            }
                        }
                        };
                    }
                    else
                    {
                        element = new PostBodyJsonElement()
                        {
                            Type = "plain",
                            Indent = 0,
                            Quoted = text.Contains("\"") || text.Contains("\'"),
                            IsRtl = false,
                            Spans = new List<Spans>
                    {
                        new Spans()
                        {
                            Modifiers=new Modifiers{},
                            Text=text
                        }
                    }
                        };
                    }
                    sections.Add(element);
                }
            });
            if (!string.IsNullOrEmpty(MediaUrl))
            {
                sections.Add(new PostBodyJsonElement()
                {
                    Type="image",
                    Indent=0,
                    Quoted=false,
                    IsRtl=false,
                    Spans=new List<Spans> { new Spans()
                    {
                        Modifiers=new Modifiers
                        {
                            Image=MediaUrl
                        },
                        Text=string.Empty
                    } }
                });
                sections.Add(new PostBodyJsonElement()
                {
                    Type = "plain",
                    Indent = 0,
                    Quoted = false,
                    IsRtl = false,
                    Spans = new List<Spans>{ new Spans()
                    {
                        Modifiers=new Modifiers{},
                        Text=string.Empty
                    } }
                });
            }
            return sections.Count > 0? JsonConvert.SerializeObject(new PostBody{ Sections = sections },new JsonSerializerSettings { NullValueHandling=NullValueHandling.Include})?.Replace("\"","\\\""):PostText;
        }
        public static void RemoveUnUsedCookies(ref DominatorAccountModel dominatorAccount)
        {
            try
            {
                var cookies = dominatorAccount.Cookies;
                var collection = new CookieCollection();
                foreach (Cookie cookie in cookies)
                {
                    if (cookie != null && (cookie.Name == "m-login" || cookie.Name == "m-b" || cookie.Name == "m-b_lax" ||
                        cookie.Name == "m-b_strict" || cookie.Name == "m-s" || cookie.Name == "m-uid"
                        || cookie.Name == "m-lat"))
                    {
                        bool exists = collection.Cast<Cookie>().Any(c => c.Name == cookie.Name);
                        if (!exists)
                            collection.Add(cookie);
                    }
                }
                dominatorAccount.Cookies = collection;
            }
            catch { }
        }
    }
}