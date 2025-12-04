using DominatorHouseCore;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using FaceDominatorCore.FDEnums;
using FaceDominatorCore.FDLibrary.FdClassLibrary;
using FaceDominatorCore.FDLibrary.FdFunctions;
using FaceDominatorCore.FDResponse.BaseResponse;
using FaceDominatorCore.Interface;
using HtmlAgilityPack;
using System;
using System.Text.RegularExpressions;


namespace FaceDominatorCore.FDResponse.Publisher
{
    public class PublisherResponseHandler : FdResponseHandler, IResponseHandler
    {
        private string PostUrl { get; set; } = string.Empty;

        public string EntityId { get; set; }

        public string PageletData { get; set; }

        public bool HasMoreResults { get; set; }

        public bool Status { get; set; }

        public FdScraperResponseParameters ObjFdScraperResponseParameters { get; set; }
            = new FdScraperResponseParameters();

        public PublisherResponseHandler(IResponseParameter responseParameter, string postUrl
           )
            : base(responseParameter)
        {
            PostUrl = postUrl;
            Status = true;
            ObjFdScraperResponseParameters.PostDetails = new FacebookPostDetails()
            {
                PostUrl = postUrl
            };
        }

        public PublisherResponseHandler(DominatorAccountModel account, IResponseParameter responseParameter)
           : base(responseParameter)
        {

            foreach (var value in Regex.Split(responseParameter.Response, "_5pcr userContentWrapper"))
            {
                if (value.Contains($"title=\"{account.AccountBaseModel.UserFullName}\""))
                {

                    if (Convert.ToInt32(FdRegexUtility.FirstMatchExtractor(value, FdConstants.DateTimeRegex)) > DateTime.Now.AddMinutes(-3).ConvertToEpoch())
                    {
                        PostUrl = FdRegexUtility.FirstMatchExtractor(value, "feed_subtitle_(.*?):");

                        if (!string.IsNullOrEmpty(PostUrl))
                        {
                            PostUrl = $"{FdConstants.FbHomeUrl}{PostUrl}";
                            ObjFdScraperResponseParameters.PostDetails = new FacebookPostDetails()
                            {
                                PostUrl = PostUrl
                            };
                        }

                        break;
                    }
                }
            }
            Status = true;
        }

        public PublisherResponseHandler(IResponseParameter responseParameter, FacebookErrors error)
            : base(responseParameter)
        {
            try
            {
                PostUrl = string.Empty;
                ObjFdScraperResponseParameters.PostDetails = new FacebookPostDetails()
                {
                    PostUrl = PostUrl
                };
                Status = false;
                FbErrorDetails = new FdErrorDetails
                {
                    FacebookErrors = error,
                    Description = error.GetDescriptionAttr()
                };
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        public PublisherResponseHandler(IResponseParameter responseParameter, bool isPostOnFriendWall = false, string friendId = "")
            : base(responseParameter)
        {
            var decodedResponse = FdFunctions.GetDecodedResponse(responseParameter.Response);

            try
            {
                HtmlDocument objNewDoc = new HtmlDocument();

                objNewDoc.LoadHtml(decodedResponse);

                var node = objNewDoc.DocumentNode.SelectNodes("//span[@class=\"_5shl fss\"]")[0].InnerHtml;

                PostUrl = FdRegexUtility.FirstMatchExtractor(node, FdConstants.ScrapedUrlRegx);

                if (!string.IsNullOrEmpty(PostUrl))
                {
                    PostUrl = $"{FdConstants.FbHomeUrl}{PostUrl}";
                    ObjFdScraperResponseParameters.PostDetails = new FacebookPostDetails()
                    {
                        PostUrl = PostUrl
                    };
                }

                Status = !string.IsNullOrEmpty(PostUrl);
            }
            catch (Exception)
            {
                FbErrorDetails = new FdErrorDetails
                {
                    Description = FdRegexUtility.FirstMatchExtractor(decodedResponse, FdConstants.DescriptionRegex)
                };
            }
        }

        public PublisherResponseHandler(IResponseParameter responseParameter, string pageUrl, string type = "")
            : base(responseParameter)
        {
            var decodedResponse = FdFunctions.GetDecodedResponse(responseParameter.Response);

            try
            {
                PostUrl = FdRegexUtility.FirstMatchExtractor(decodedResponse, FdConstants.EntIdentifierPostIdRegex);

                if (!string.IsNullOrEmpty(PostUrl))
                {
                    PostUrl = $"{FdConstants.FbHomeUrl}{PostUrl}";
                    ObjFdScraperResponseParameters.PostDetails = new FacebookPostDetails()
                    {
                        PostUrl = PostUrl
                    };
                }

                Status = !string.IsNullOrEmpty(PostUrl);
            }
            catch (Exception)
            {
                FbErrorDetails = new FdErrorDetails
                {
                    Description = FdRegexUtility.FirstMatchExtractor(decodedResponse, FdConstants.DescriptionRegex)
                };
            }
        }

        public PublisherResponseHandler(IResponseParameter responseParameter, string groupId, string productName, string location = "")
            : base(responseParameter)
        {
            var decodedResponse = FdFunctions.GetDecodedResponse(responseParameter.Response);

            try
            {
                HtmlDocument htmlDocument = new HtmlDocument();

                htmlDocument.LoadHtml(decodedResponse);

                var lastPublishedPost = string.Empty;

                var postDetails = htmlDocument.DocumentNode.SelectNodes("//div[@class=\"pam _5shk uiBoxWhite bottomborder\"]");

                if (postDetails != null)
                {
                    lastPublishedPost = postDetails[0].InnerHtml;
                }

                if (lastPublishedPost.Contains(groupId) && lastPublishedPost.Contains(productName) && lastPublishedPost.Contains("sale_post_id"))

                    PostUrl = FdRegexUtility.FirstMatchExtractor(decodedResponse, "sale_post_id=(.*?)\"");

                if (!string.IsNullOrEmpty(PostUrl))
                {

                    PostUrl = $"{FdConstants.FbHomeUrl}{PostUrl}";
                    ObjFdScraperResponseParameters.PostDetails = new FacebookPostDetails()
                    {
                        PostUrl = PostUrl
                    };
                }

                Status = !string.IsNullOrEmpty(PostUrl);

                if (string.IsNullOrEmpty(PostUrl))
                {
                    FbErrorDetails = new FdErrorDetails
                    {
                        Description = "Unknown Error"
                    };
                }
            }
            catch (Exception)
            {
                FbErrorDetails = new FdErrorDetails
                {
                    Description = FdRegexUtility.FirstMatchExtractor(decodedResponse, FdConstants.DescriptionRegex)
                };
            }
        }



    }

}
