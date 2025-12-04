using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Utility;
using FaceDominatorCore.FDLibrary.FdClassLibrary;
using FaceDominatorCore.FDLibrary.FdFunctions;
using FaceDominatorCore.FDResponse.BaseResponse;
using FaceDominatorCore.Interface;
using FaceDominatorCore.Utility;
using Newtonsoft.Json.Linq;
using System;
using System.Linq;

namespace FaceDominatorCore.FDResponse.BrowserResponseHandler.Publisher
{

    public class PublisherBrowserResponseHandler : FdResponseHandler, IResponseHandler
    {
        public string PostUrl { get; set; } = string.Empty;

        public string EntityId { get; set; }

        public string PageletData { get; set; }

        public bool HasMoreResults { get; set; }

        public bool Status { get; set; }

        public FdScraperResponseParameters ObjFdScraperResponseParameters { get; set; }
            = new FdScraperResponseParameters();

        public PublisherBrowserResponseHandler(IResponseParameter responseParameter, string paginationData,
            bool isStory = false) : base(responseParameter)
        {
            try
            {
                var postId = string.Empty;
                JObject jObject = new JObject();
                JArray fdjArray = new JArray();
                if (!paginationData.IsValidJson())
                {
                    var decodedResponse = "";
                    if (paginationData.Contains("for (;;);"))
                    {
                        decodedResponse = paginationData.Replace("for (;;);", string.Empty);
                    }
                    else
                        decodedResponse = "[" + paginationData.Replace("}}}}\r\n{\"label\":", "}}}},\r\n{\"label\":") + "]";
                    if (decodedResponse.IsValidJson())
                        jObject = parser.ParseJsonToJObject(decodedResponse);
                    if (jObject == null && decodedResponse.IsValidJson())
                    {
                        fdjArray = parser.GetJArrayElement(decodedResponse);
                        if (fdjArray != null && fdjArray.Count > 0)
                            jObject = fdjArray.FirstOrDefault() as JObject;
                    }
                }
                else
                {
                    jObject = parser.ParseJsonToJObject(paginationData);
                    if (jObject.Count == 0)
                    {
                        fdjArray = parser.GetJArrayElement(paginationData);
                        jObject = fdjArray.FirstOrDefault() as JObject;
                    }

                }
                //  if (!paginationData.IsValidJson())
                {



                    if (jObject != null && jObject.Count > 0)
                    {
                        try
                        {
                            PostUrl = JsonSearcher.FindStringValueByKey(JsonSearcher.FindByKey(jObject, "story_create"), "url");
                            if (string.IsNullOrEmpty(PostUrl))
                                PostUrl = JsonSearcher.FindStringValueByKey(JsonSearcher.FindByKey(jObject, "story"), "url");
                            postId = JsonSearcher.FindStringValueByKey(JsonSearcher.FindByKey(jObject, "story_create"), "id");
                            if (string.IsNullOrEmpty(postId))
                                postId = JsonSearcher.FindStringValueByKey(JsonSearcher.FindByKey(jObject, "story_create"), "story_id");
                        }
                        catch (Exception)
                        {
                        }
                    }
                    else
                    {

                        if (paginationData.Contains("share_fbid"))
                        {
                            postId = FdRegexUtility.FirstMatchExtractor(paginationData, FdConstants.SharePostIdRegex);
                            if (string.IsNullOrEmpty(postId))
                            {
                                postId = FdRegexUtility.FirstMatchExtractor(paginationData, "\"id\": \"(.*?)\"");
                            }
                        }
                        else if (paginationData.Contains("story_fbid") || paginationData.Contains("contentID"))
                        {
                            postId = FdRegexUtility.FirstMatchExtractor(paginationData, "story_fbid\":(.*?)}");

                            if (string.IsNullOrEmpty(postId))
                                postId = FdRegexUtility.FirstMatchExtractor(paginationData, "contentID\":\"(.*?)\"");
                            if (paginationData.Contains("story_create"))
                                postId = FdRegexUtility.FirstMatchExtractor(paginationData, ":{\"id\":\"(.*?)\"");
                        }
                        else if (paginationData.Contains("story_create"))
                        {
                            //        JsonHandler jobject = new JsonHandler(paginationData);
                            //        postId = jobject.GetElementValue("data", "story_create", "story", "id");
                            postId = FdRegexUtility.FirstMatchExtractor(paginationData, ":{\"id\":\"(.*?)\"");
                        }
                    }
                    if (!string.IsNullOrEmpty(paginationData) && string.IsNullOrEmpty(postId))
                    {
                        if (paginationData.Contains("story_id"))
                        {
                            postId = FdRegexUtility.FirstMatchExtractor(paginationData, "story_id\":\"(.*?)\"");
                        }
                        else if (paginationData.Contains("logging_token"))
                        {
                            postId = FdRegexUtility.FirstMatchExtractor(paginationData, "logging_token\":\"(.*?)\"");
                        }

                        if (string.IsNullOrEmpty(postId) && paginationData.Contains("logging_token"))
                        {
                            postId = FdRegexUtility.FirstMatchExtractor(paginationData, "logging_token\":\"(.*?)\"");
                        }
                        if (string.IsNullOrEmpty(postId) && paginationData.Contains("\"story\":"))
                        {
                            postId = FdRegexUtility.FirstMatchExtractor(paginationData, "\"id\":\"(.*?)\"");
                        }
                    }

                    if (!FdFunctions.IsIntegerOnly(postId) && StringHelper.IsBase64String(postId))
                    {
                        postId = StringHelper.Base64Decode(postId);
                        var postIdList = postId.Split(':').ToList();
                        postIdList.RemoveAll(x => !FdFunctions.IsIntegerOnly(x));
                        postId = postIdList.LastOrDefault();
                        if (!string.IsNullOrEmpty(PostUrl) && !PostUrl.Contains(postId))
                            postId = postIdList.FirstOrDefault();
                    }

                    if (postId != "0" && string.IsNullOrEmpty(PostUrl))
                    {
                        PostUrl = isStory
                                    ? $"{FdConstants.FbHomeUrl}stories/{postId}"
                                    : $"{FdConstants.FbHomeUrl}{postId}";
                    }

                    ObjFdScraperResponseParameters.PostDetails = new FacebookPostDetails
                    {
                        PostUrl = PostUrl,
                        Id = postId
                    };

                    Status = !string.IsNullOrEmpty(postId) && postId != "0" && FdFunctions.IsIntegerOnly(postId);

                    if (string.IsNullOrEmpty(PostUrl) || PostUrl == FdConstants.FbHomeUrl)
                    {
                        FbErrorDetails = new FdErrorDetails
                        {
                            Description = "Unknown Error"
                        };
                    }
                }
            }
            catch (Exception)
            {
                FbErrorDetails = new FdErrorDetails
                {
                    Description = "Unknown Error"
                };
            }
        }

        public PublisherBrowserResponseHandler(IResponseParameter responseParameter)
          : base(responseParameter)
        {
            try
            {
                PostUrl = FdRegexUtility.FirstMatchExtractor(responseParameter.Response, "a href=\"(.*?)\"");
                ObjFdScraperResponseParameters.PostDetails = new FacebookPostDetails()
                {
                    PostUrl = PostUrl.Contains(FdConstants.FbHomeUrl)
                                ? PostUrl
                                : FdConstants.FbHomeUrl + PostUrl
                };
                Status = !string.IsNullOrEmpty(PostUrl);

            }
            catch (Exception)
            {
                Status = false;
            }
        }

    }
}
