using DominatorHouseCore;
using DominatorHouseCore.Interfaces;
using FaceDominatorCore.FDLibrary.FdClassLibrary;
using FaceDominatorCore.FDLibrary.FdFunctions;
using FaceDominatorCore.FDResponse.BaseResponse;
using FaceDominatorCore.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace FaceDominatorCore.FDResponse.CommonResponse
{
    public class PostScraperForKeywordResponseHandler : FdResponseHandler, IResponseHandler
    {
        public bool HasMoreResults { get; set; }

        public string EntityId { get; set; }

        public string PageletData { get; set; }

        public bool Status { get; set; }

        public FdScraperResponseParameters ObjFdScraperResponseParameters { get; set; } = new FdScraperResponseParameters();

        public PostScraperForKeywordResponseHandler(IResponseParameter newsFeedResponse,
            FdScraperResponseParameters objFdScraperResponseParameters, List<KeyValuePair<string, string>> listPostReaction)
            : base(newsFeedResponse)
        {
            if (newsFeedResponse.HasError)
                return;

            ObjFdScraperResponseParameters = objFdScraperResponseParameters;
            ObjFdScraperResponseParameters.ListPostReaction = listPostReaction;
            ObjFdScraperResponseParameters.ListPostDetails = new List<FacebookPostDetails>();
            string decodedResponse = FdFunctions.GetDecodedResponse(newsFeedResponse.Response);

            GetPostIdListNew(decodedResponse);

            //GetPostIdList(decodedResponse);

            if (!ObjFdScraperResponseParameters.IsPagination)
                GetPagelet(newsFeedResponse.Response);

            UpadetePaginationData(newsFeedResponse);

            Status = ObjFdScraperResponseParameters.ListPostDetails.Count > 0;

        }

        private void GetPostIdListNew(string decodedResponse)
        {
            string[] postValues = Regex.Split(decodedResponse, "\"_19_p\"");

            foreach (var post in postValues)
            {
                FacebookPostDetails objFacebookPostDetails = new FacebookPostDetails();
                try
                {
                    List<string> value = Regex.Split(post, "_6-co").ToList();
                    if (value.Count > 1)
                    {
                        objFacebookPostDetails.PostUrl = $"{FdConstants.FbHomeUrl}{FdRegexUtility.FirstMatchExtractor(value?[1], "href=\"/(.*?)\"")}";

                        ObjFdScraperResponseParameters.ListPostDetails.Add(objFacebookPostDetails);
                    }


                }
                catch (Exception)
                {

                }

            }

        }

        private void GetPagelet(string decodedResponse)
        {
            try
            {
                string pageletSection;
                pageletSection = FdRegexUtility.FirstMatchExtractor(decodedResponse,
                    ",\"globalData\":(.*?)\\}\\},\"prefetchPixels\":");

                if (string.IsNullOrEmpty(pageletSection))
                {
                    pageletSection = FdRegexUtility.FirstMatchExtractor(decodedResponse,
                        ",globalData:(.*?)\\},prefetchPixels:");
                }

                var ajaxTokenSection = FdRegexUtility.FirstMatchExtractor(decodedResponse, FdConstants.AjaxPipeTokenRegex);

                ObjFdScraperResponseParameters.AjaxToken = ajaxTokenSection;
                ObjFdScraperResponseParameters.FinalEncodedQuery = pageletSection;


            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

        }

        private void UpadetePaginationData(IResponseParameter responseParameter)
        {
            try
            {
                string[] cursorDataArray = Regex.Split(responseParameter.Response, "BrowseScrollingPager");

                string cursorData;
                if (!ObjFdScraperResponseParameters.IsPagination)
                {
                    cursorData = Regex.Split(cursorDataArray[2], "{cursor:(.*?),display_params", RegexOptions.Singleline)[1];
                    cursorData = cursorData.Replace(",", ",\"").Replace(":", "\":");
                    ObjFdScraperResponseParameters.IsPagination = true;
                }
                else
                    cursorData = Regex.Split(cursorDataArray[1], "{\"cursor\":(.*?),\"display_params", RegexOptions.Singleline)[1];

                string fullQueryParameters;
                if (string.IsNullOrEmpty(cursorData))
                {
                    HasMoreResults = false;
                    fullQueryParameters = string.Empty;
                }
                else
                {
                    HasMoreResults = true;
                    fullQueryParameters = $"{ObjFdScraperResponseParameters.FinalEncodedQuery},\"cursor\":{cursorData}}}";
                }

                PageletData = fullQueryParameters;
            }
            catch (Exception ex)
            {
                ex.DebugLog(ex.Message);
            }
        }
    }
}
