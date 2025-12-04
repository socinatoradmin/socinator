using DominatorHouseCore;
using DominatorHouseCore.Interfaces;
using FaceDominatorCore.FDLibrary.FdFunctions;
using FaceDominatorCore.FDResponse.BaseResponse;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace FaceDominatorCore.FDResponse.CommonResponse
{
    public class ScrapVideoFromPageResponseHandler : FdResponseHandler
    {
        public List<string> ListVideoIds { get; set; }

        public string Pagelet { get; set; }

        public string AjaxToken { get; set; }

        public bool HasMoreResults { get; set; }



        public ScrapVideoFromPageResponseHandler(IResponseParameter responseParameter, List<string> videoIds)
            : base(responseParameter)
        {
            ListVideoIds = videoIds;

            string decodedResponse = FdFunctions.GetPrtialDecodedResponse(responseParameter.Response);

            GetPostIdList(decodedResponse);
        }


        private void GetPostIdList(string decodedResponse)
        {
            try
            {
                var response = Regex.Split(decodedResponse, "div id=\"pages_video_hub_all_videos_pagelet\"");

                using (FdHtmlParseUtility objHtmlParseUtility = new FdHtmlParseUtility())
                {
                    var allVideoDetails = objHtmlParseUtility.GetInnerHtmlFromTagName(response[1], "table", "class",
                        "uiGrid _51mz");

                    var listVideoDetails = objHtmlParseUtility.GetListInnerHtmlFromTagName(allVideoDetails, "div", "class",
                        "_5asl");

                    listVideoDetails.ForEach(x =>
                    {
                        var videoId = FdRegexUtility.FirstMatchExtractor(x, "/videos/(.*?)/");
                        videoId = FdFunctions.GetIntegerOnlyString(videoId);
                        if (!ListVideoIds.Contains(videoId))
                            ListVideoIds.Add(videoId);
                    });
                }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }
    }
}
