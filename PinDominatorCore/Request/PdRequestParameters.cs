using DominatorHouseCore.Request;
using DominatorHouseCore.Utility;
using Newtonsoft.Json;
using PinDominatorCore.PDUtility;
using PinDominatorCore.Utility;
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace PinDominatorCore.Request
{
    public class PdRequestParameters : RequestParameters
    {
        /// <summary>
        ///     To store the post data in the json elements
        /// </summary>
        public PdJsonElement PdPostElements { get; set; }

        /// <summary>
        ///     To generate the post based on already stored elements from
        ///     <see cref="PinDominatorCore.Request.PdRequestParameters.PdPostElements" />
        /// </summary>
        /// <returns>post data in bytes of sequence</returns>
        public byte[] GetPostDataFromJson()
        {
            var jsonString = GetJsonString();

            if (string.IsNullOrEmpty(jsonString)) return null;
            return !IsMultiPart ? GeneratePostDataFromJson(jsonString) : CreateMultipartBodyFromJson(jsonString);
        }

        /// <summary>
        ///     To generate the post based on already stored itmes in
        ///     <see cref="DominatorHouseCore.Request.RequestParameters.PostDataParameters" /> and
        ///     <see cref="PinDominatorCore.Request.PdRequestParameters.PdPostElements" />
        /// </summary>
        /// <returns>post data in bytes of sequence</returns>
        public byte[] GetPostData()
        {
            var jsonString = GetJsonString();

            if (string.IsNullOrEmpty(jsonString)) return null;

            return !IsMultiPart ? GeneratePostData(jsonString) : CreateMultipartBody(jsonString);
        }

        /// <summary>
        ///     To convert the <see cref="PinDominatorCore.Request.PdRequestParameters.PdPostElements" /> elements to string
        /// </summary>
        /// <returns>json string</returns>
        internal string GetJsonString()
        {
            return PdPostElements == null
                ? null
                : JsonConvert.SerializeObject(
                    PdPostElements,
                    new JsonSerializerSettings
                    {
                        NullValueHandling = NullValueHandling.Ignore
                    });
        }

        /// <summary>
        ///     To convert the <see cref="PinDominatorCore.Request.PdRequestParameters" /> object to string
        /// </summary>
        /// <param name="pdJsonElement"><see cref="PdJsonElement" /> object </param>
        /// <returns>json string</returns>
        internal string GetJsonString(PdJsonElement pdJsonElement)
        {
            return pdJsonElement == null
                ? null
                : JsonConvert.SerializeObject(
                    pdJsonElement,
                    new JsonSerializerSettings
                    {
                        NullValueHandling = NullValueHandling.Ignore
                    });
        }


        //Used for uploading pin
        public byte[] GetPostDataFromParameters(string contentType)
            => !IsMultiPart ? GeneratePostData() : CreateMultipartBody(contentType);
        public byte[] GeneratePostDataForPinUpload(string fileContentType,string response)
        {
            if (string.IsNullOrEmpty(response) || !response.Contains("\"status\":\"success\""))
                return new byte[0];
            var jsonHandler = new JsonHandler(response);
            var requestParameter = jsonHandler.GetJToken("resource_response", "data", "upload_parameters");
            if(requestParameter is null || !requestParameter.HasValues)
            {
                var Id = jsonHandler.GetJToken("resource_response", "data")?.First;
                requestParameter = jsonHandler.GetJToken("resource_response", "data",Id?.Path?.Split('.')?.LastOrDefault(x=>x!=string.Empty), "upload_parameters");
            }
            var x_amz_date = jsonHandler.GetJTokenValue(requestParameter, "x-amz-date");
            var x_amz_signature = jsonHandler.GetJTokenValue(requestParameter, "x-amz-signature");
            var x_amz_security_token = jsonHandler.GetJTokenValue(requestParameter, "x-amz-security-token");
            var x_amz_algorithm = jsonHandler.GetJTokenValue(requestParameter, "x-amz-algorithm");
            var x_amz_key = jsonHandler.GetJTokenValue(requestParameter, "key");
            var x_amz_policy = jsonHandler.GetJTokenValue(requestParameter, "policy");
            var x_amz_credential = jsonHandler.GetJTokenValue(requestParameter, "x-amz-credential");
            var x_amz_contentType = jsonHandler.GetJTokenValue(requestParameter, "Content-Type");
            var multipartBoundary = GenerateFdMultipartBoundary();
            var strMultipartBoundary = $"--{multipartBoundary}";
            var stringBuilder = new StringBuilder();
            using (var memoryStream = new MemoryStream())
            {
                foreach (var file in FileList)
                {
                    stringBuilder.AppendLine(strMultipartBoundary);
                    stringBuilder.AppendLine("Content-Disposition: form-data; name=\"x-amz-date\"");
                    stringBuilder.AppendLine();
                    stringBuilder.AppendLine(x_amz_date);
                    stringBuilder.AppendLine(strMultipartBoundary);
                    stringBuilder.AppendLine("Content-Disposition: form-data; name=\"x-amz-signature\"");
                    stringBuilder.AppendLine();
                    stringBuilder.AppendLine(x_amz_signature);
                    stringBuilder.AppendLine(strMultipartBoundary);
                    stringBuilder.AppendLine("Content-Disposition: form-data; name=\"x-amz-security-token\"");
                    stringBuilder.AppendLine();
                    stringBuilder.AppendLine(x_amz_security_token);
                    stringBuilder.AppendLine(strMultipartBoundary);
                    stringBuilder.AppendLine("Content-Disposition: form-data; name=\"x-amz-algorithm\"");
                    stringBuilder.AppendLine();
                    stringBuilder.AppendLine(x_amz_algorithm);
                    stringBuilder.AppendLine(strMultipartBoundary);
                    stringBuilder.AppendLine("Content-Disposition: form-data; name=\"key\"");
                    stringBuilder.AppendLine();
                    stringBuilder.AppendLine(x_amz_key);
                    stringBuilder.AppendLine(strMultipartBoundary);
                    stringBuilder.AppendLine("Content-Disposition: form-data; name=\"policy\"");
                    stringBuilder.AppendLine();
                    stringBuilder.AppendLine(x_amz_policy);
                    stringBuilder.AppendLine(strMultipartBoundary);
                    stringBuilder.AppendLine("Content-Disposition: form-data; name=\"x-amz-credential\"");
                    stringBuilder.AppendLine();
                    stringBuilder.AppendLine(x_amz_credential);
                    stringBuilder.AppendLine(strMultipartBoundary);
                    stringBuilder.AppendLine("Content-Disposition: form-data; name=\"Content-Type\"");
                    stringBuilder.AppendLine();
                    stringBuilder.AppendLine(x_amz_contentType);
                    stringBuilder.AppendLine(strMultipartBoundary);
                    stringBuilder.AppendLine($"Content-Disposition: form-data; name=\"file\"; filename=\"{"blob" as object}\"");
                    stringBuilder.AppendLine($"Content-Type: {fileContentType}");
                    stringBuilder.AppendLine();
                    var bytes = Encoding.UTF8.GetBytes(stringBuilder.ToString());
                    memoryStream.Write(bytes, 0, bytes.Length);
                    stringBuilder.Clear();
                    if (file.Value.Contents != null)
                    {
                        memoryStream.Write(file.Value.Contents, 0, file.Value.Contents.Length);
                    }
                }

                var bytes1 = Encoding.UTF8.GetBytes(Environment.NewLine + strMultipartBoundary + "--");
                memoryStream.Write(bytes1, 0, bytes1.Length);

                ContentType = $"{x_amz_contentType}; boundary={multipartBoundary}";
                return memoryStream.ToArray();
            }
        }
        public override byte[] CreateMultipartBody(string contentType)
        {
            if (contentType.Equals("video/mp4"))
            {
                var multipartBoundaryVideo = GenerateFdMultipartBoundary();

                var strMultipartBoundaryVideo = $"--{multipartBoundaryVideo}";

                var stringBuilder = new StringBuilder();

                using (var memoryStream = new MemoryStream())
                {
                    foreach (var file in FileList)
                    {
                        if (file.Value.Headers != null)
                        {
                            foreach (string header in file.Value.Headers)
                            {
                                stringBuilder.AppendLine(strMultipartBoundaryVideo);
                                stringBuilder.AppendLine(
                                    $"Content-Disposition: form-data; name=\"{(object)header}\"");
                                stringBuilder.AppendLine();
                                stringBuilder.AppendLine($"{file.Value.Headers[header] as object}");
                            }
                        }
                        stringBuilder.AppendLine(strMultipartBoundaryVideo);
                        stringBuilder.AppendLine($"Content-Disposition: form-data; name=\"file\"; filename=\"{file.Value.FileName as object}\"");
                        stringBuilder.AppendLine($"Content-Type: {contentType}");
                        stringBuilder.AppendLine();
                        var bytes = Encoding.UTF8.GetBytes(stringBuilder.ToString());
                        memoryStream.Write(bytes, 0, bytes.Length);
                        stringBuilder.Clear();
                        if (file.Value.Contents != null)
                        {
                            memoryStream.Write(file.Value.Contents, 0, file.Value.Contents.Length);
                        }
                    }

                    var bytes1 = Encoding.UTF8.GetBytes(Environment.NewLine + strMultipartBoundaryVideo + "--");
                    memoryStream.Write(bytes1, 0, bytes1.Length);

                    ContentType = $"multipart/form-data; boundary={multipartBoundaryVideo}";
                    return memoryStream.ToArray();
                }
            }
            else
            {
                var multipartBoundary = GenerateFdMultipartBoundary();

                var strMultipartBoundary = $"--{multipartBoundary}";

                var stringBuilder = new StringBuilder();

                using (var memoryStream = new MemoryStream())
                {
                    foreach (var file in FileList)
                    {
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
                        stringBuilder.AppendLine(strMultipartBoundary);
                        stringBuilder.AppendLine($"Content-Disposition: form-data; name=\"img\"; filename=\"{"blob" as object}\"");
                        stringBuilder.AppendLine($"Content-Type: {contentType}");
                      
                        stringBuilder.AppendLine();
                        var bytes = Encoding.UTF8.GetBytes(stringBuilder.ToString());
                        memoryStream.Write(bytes, 0, bytes.Length);
                        stringBuilder.Clear();
                        if (file.Value.Contents != null)
                        {
                            memoryStream.Write(file.Value.Contents, 0, file.Value.Contents.Length);
                        }
                    }

                    var bytes1 = Encoding.UTF8.GetBytes(Environment.NewLine + strMultipartBoundary + "--");
                    memoryStream.Write(bytes1, 0, bytes1.Length);

                    ContentType = $"multipart/form-data; boundary={multipartBoundary}";
                    return memoryStream.ToArray();
                }
            }
        }

        private static string GenerateFdMultipartBoundary()
        {
            return "----WebKitFormBoundary" + DateTime.Now.Ticks.ToString("x");
        }
        public byte[] GeneratePostDataForVideoUpload(string Response,string boardId,string description,string title,string url,string imageSignature,string videoSignature,string trackingId,string filePath,string SectionId)
        {
            var postData = string.Empty;
            if (string.IsNullOrEmpty(Response) ? true : !Response.Contains("\"status\":\"success\""))
                return new byte[0];
            var jsonHandler = JsonJArrayHandler.GetInstance;
            var jsonObject = jsonHandler.ParseJsonToJObject(Response);
            var details = jsonHandler.GetJTokenOfJToken(jsonObject, "resource_response", "data",0, "pages",0);
            var CanvasAspectRatio = jsonHandler.GetJTokenValue(jsonObject, "resource_response", "data", 0, "metadata", "canvas_aspect_ratio");
            var blockHeight = jsonHandler.GetJTokenValue(details, "blocks",0, "block_style", "height");
            var blockWidth = jsonHandler.GetJTokenValue(details, "blocks",0, "block_style", "width");
            var x_coord = jsonHandler.GetJTokenValue(details, "blocks",0, "block_style", "x_coord");
            var y_coord = jsonHandler.GetJTokenValue(details, "blocks",0, "block_style", "y_coord");
            var blockType = jsonHandler.GetJTokenValue(details, "blocks",0, "block_type");
            var backGroundColor = jsonHandler.GetJTokenValue(details,"style", "background_color");
            var MediaResolution =Regex.Split(PdUtility.GetImageResolution(filePath),"<:>");var sourceMediaWidth = MediaResolution.FirstOrDefault();var sourceMediaHeight = MediaResolution.LastOrDefault();
            CanvasAspectRatio = string.IsNullOrEmpty(CanvasAspectRatio) ?PdUtility.GetAspectRatioOfAnImage(Convert.ToInt32(sourceMediaWidth),Convert.ToInt32(sourceMediaHeight)): CanvasAspectRatio;
            backGroundColor = string.IsNullOrEmpty(backGroundColor) ? "#FFFFFF" : backGroundColor;blockType = string.IsNullOrEmpty(blockType) ?"3": blockType;
            y_coord = string.IsNullOrEmpty(y_coord) ?"0": y_coord;x_coord = string.IsNullOrEmpty(x_coord) ?"0": x_coord;
            blockWidth = string.IsNullOrEmpty(blockWidth) ?"100": blockWidth;blockHeight = string.IsNullOrEmpty(blockHeight) ?"100": blockHeight;
            postData = $"source_url=/pin-creation-tool/&data={{\"options\":{{\"allow_shopping_rec\":true,\"description\":\"{description}\",\"is_comments_allowed\":true,\"is_removable\":false,\"is_unified_builder\":true,\"link\":\"{url}\",\"orbac_subject_id\":\"\",\"story_pin\":\"{{\\\"metadata\\\":{{\\\"pin_title\\\":\\\"{title}\\\",\\\"pin_image_signature\\\":\\\"{imageSignature}\\\",\\\"canvas_aspect_ratio\\\":{CanvasAspectRatio},\\\"diy_data\\\":null,\\\"recipe_data\\\":null,\\\"template_type\\\":null}},\\\"pages\\\":[{{\\\"blocks\\\":[{{\\\"block_style\\\":{{\\\"height\\\":{blockHeight},\\\"width\\\":{blockWidth},\\\"x_coord\\\":{x_coord},\\\"y_coord\\\":{y_coord}}},\\\"tracking_id\\\":\\\"{trackingId}\\\",\\\"video_signature\\\":\\\"{videoSignature}\\\",\\\"type\\\":{blockType}}}],\\\"clips\\\":[{{\\\"clip_type\\\":1,\\\"end_time_ms\\\":-1,\\\"is_converted_from_image\\\":false,\\\"source_media_height\\\":{sourceMediaHeight},\\\"source_media_width\\\":{sourceMediaWidth},\\\"start_time_ms\\\":-1}}],\\\"layout\\\":0,\\\"style\\\":{{\\\"background_color\\\":\\\"{backGroundColor}\\\"}}}}]}}\",\"user_mention_tags\":\"[]\"}},\"context\":{{}}}}";
            return Encoding.UTF8.GetBytes(postData);
        }
        public byte[] GeneratePostDataForPinLike(string PinID, string AggregatedPinID, bool IsHelpful = false) =>Encoding.UTF8.GetBytes(IsHelpful ? $"source_url=%2Fpin%2F{PinID}%2F&data=%7B%22options%22%3A%7B%22url%22%3A%22%2Fv3%2Fhelpful%2F1%2F{AggregatedPinID}%2F%22%7D%2C%22context%22%3A%7B%7D%7D" : $"source_url=%2Fpin%2F{PinID}%2F&data=%7B%22options%22%3A%7B%22url%22%3A%22%2Fv3%2Faggregated_comments%2F{AggregatedPinID}%2Freact%2F%22%2C%22data%22%3A%7B%22reaction_type%22%3A1%7D%7D%2C%22context%22%3A%7B%7D%7D");
    }
}