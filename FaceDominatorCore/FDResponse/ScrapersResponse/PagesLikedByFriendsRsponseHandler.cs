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

namespace FaceDominatorCore.FDResponse.ScrapersResponse
{

    public class PagesLikedByFriendsRsponseHandler : FdResponseHandler, IResponseHandler
    {
        public bool HasMoreResults { get; set; }

        public string EntityId { get; set; }

        public string PageletData { get; set; }

        public bool Status { get; set; }

        public FdScraperResponseParameters ObjFdScraperResponseParameters { get; set; }
        = new FdScraperResponseParameters();

        public PagesLikedByFriendsRsponseHandler(IResponseParameter responseParameter, bool isPagination = false,
            string paginationData = "")
            : base(responseParameter)
        {
            if (responseParameter.HasError || responseParameter.Response == null)
                return;

            ObjFdScraperResponseParameters.ListPage = new List<FanpageDetails>();

            string decodedResponse = FdFunctions.GetDecodedResponse(responseParameter.Response);

            GetPlaceId(decodedResponse);

            GetPaginationData(decodedResponse, isPagination, paginationData);
        }

        private void GetPaginationData(string decodedResponse, bool isPagination = false, string pagination = "")
        {
            try
            {
                if (!isPagination)
                {
                    var lst = Uri.UnescapeDataString(FdRegexUtility.FirstMatchExtractor(decodedResponse, "\\?lst=(.*?)\""));
                    var profileId = FdRegexUtility.FirstMatchExtractor(lst, ":(.*?):");
                    var pageletToken = FdRegexUtility.FirstMatchExtractor(decodedResponse, "pagelet_token:\"(.*?)\"");
                    ObjFdScraperResponseParameters.CollectionToken = FdRegexUtility.FirstMatchExtractor(decodedResponse, "pagelet_timeline_app_collection_(.*?)\"");
                    ObjFdScraperResponseParameters.Cursor = Regex.Split(decodedResponse, "\"TimelineAppCollection\",\"enableContentLoader\"")[1];
                    ObjFdScraperResponseParameters.Cursor = FdRegexUtility.FirstMatchExtractor(ObjFdScraperResponseParameters.Cursor, "},\"(.*?)\"]]");
                    PageletData = "{\"collection_token\":\"" + ObjFdScraperResponseParameters.CollectionToken + "\",\"cursor\":\""
                        + ObjFdScraperResponseParameters.Cursor + "\",\"disablepager\":false,\"overview\":false,\"profile_id\":\""
                        + profileId + "\",\"pagelet_token\":\"" + pageletToken
                        + "\",\"tab_key\":\"likes_all\",\"lst\":\""
                        + lst + "\",\"order\":null,\"sk\":\"likes\",\"importer_state\":null}";
                }
                else
                {
                    var previousCursor = FdRegexUtility.FirstMatchExtractor(pagination, "cursor\":\"(.*?)\"");
                    ObjFdScraperResponseParameters.Cursor = Regex.Split(decodedResponse, "\"TimelineAppCollection\",\"enableContentLoader\"")[1];
                    ObjFdScraperResponseParameters.Cursor = FdRegexUtility.FirstMatchExtractor(ObjFdScraperResponseParameters.Cursor, "},\"(.*?)\"]]");
                    PageletData = Regex.Replace(pagination, previousCursor, ObjFdScraperResponseParameters.Cursor);
                }

                HasMoreResults = !string.IsNullOrEmpty(ObjFdScraperResponseParameters.Cursor);
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        public void GetPlaceId(string partialDecodedResponse)
        {
            try
            {
                var splitData = Regex.Split(partialDecodedResponse, "class=\"_5rz _5k3a _5rz3 _153f\"").Skip(1).ToList();

                foreach (var pageDetails in splitData)
                {
                    var url = FdRegexUtility.FirstMatchExtractor(pageDetails, FdConstants.ScrapedUrlRegx);

                    var name = FdRegexUtility.FirstMatchExtractor(pageDetails, "aria-label=\"(.*?)\"");

                    var pageId = FdRegexUtility.FirstMatchExtractor(pageDetails, "page.php\\?id=(.*?)&");

                    var pageCategory = FdRegexUtility.FirstMatchExtractor(pageDetails, "class=\"fsm fwn fcg\">(.*?)<");

                    bool isVerifiedPage = false;

                    if (pageDetails.Contains("_56_f _5dzy _5dzz _3twv"))
                        isVerifiedPage = true;

                    FanpageDetails objFanpageDetails = new FanpageDetails()
                    {
                        FanPageID = pageId,
                        FanPageCategory = pageCategory,
                        FanPageUrl = url,
                        FanPageName = name,
                        IsVerifiedPage = isVerifiedPage.ToString()
                    };

                    if (ObjFdScraperResponseParameters.ListPage.FirstOrDefault(x => x.FanPageID == objFanpageDetails.FanPageID) == null)
                        ObjFdScraperResponseParameters.ListPage.Add(objFanpageDetails);
                }

                Status = ObjFdScraperResponseParameters.ListPage.Count > 0;

            }
            catch (Exception)
            {

            }
        }

    }
}
