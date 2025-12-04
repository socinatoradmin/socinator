using DominatorHouseCore;
using DominatorHouseCore.Interfaces;
using FaceDominatorCore.FDLibrary.FdClassLibrary;
using FaceDominatorCore.FDLibrary.FdFunctions;
using FaceDominatorCore.FDResponse.BaseResponse;
using FaceDominatorCore.Interface;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace FaceDominatorCore.FDResponse.ScrapersResponse
{
    public class MobileUserFanPageScraperResponseHandler : FdResponseHandler, IResponseHandler
    {

        public string EntityId { get; set; }
        public string PageletData { get; set; }
        public bool HasMoreResults { get; set; }
        public bool Status { get; set; }
        public FdScraperResponseParameters ObjFdScraperResponseParameters { get; set; }
        = new FdScraperResponseParameters();

        //public static string AjaxValue = string.Empty;
        //public List<FanpageDetails> fanpageDetailsList = new List<FanpageDetails>();
        //public List<string> fanapageNameList = new List<string>();

        public MobileUserFanPageScraperResponseHandler(IResponseParameter responseParameter)
            : base(responseParameter)
        {
            if (responseParameter.HasError)
                return;

            ObjFdScraperResponseParameters = new FdScraperResponseParameters();
            ObjFdScraperResponseParameters.ListPage = new List<FanpageDetails>();
            if (!responseParameter.Response.Contains("for (;;)"))
            {
                GetPageCurser(responseParameter.Response);

            }
            else
            {
                try
                {
                    JObject jObject = JObject.Parse(responseParameter.Response.Replace("for (;;);", string.Empty));

                    var curser = jObject["payload"]["actions"][3]["code"].ToString();

                    PaginationPageCurser(curser);
                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                }
            }

        }

        public void GetPageList(string htmlValue)
        {
            string[] valuesStrings = Regex.Split(htmlValue, "style=\"-webkit-line-clamp: 2;\"");

            foreach (var values in valuesStrings)
            {
                if (values.StartsWith("<!DOCTYPE"))
                    continue;
                ObjFdScraperResponseParameters.ListPage.Add(new FanpageDetails()
                {
                    FanPageName = FdRegexUtility.FirstMatchExtractor(values, ">(.*?)<"),
                    FanPageUrl = $"{FdConstants.FbHomeUrl}{FdRegexUtility.FirstMatchExtractor(values, "href=\"/(.*?)\"")}"
                });
            }

            if (string.IsNullOrEmpty(ObjFdScraperResponseParameters.AjaxToken))
                ObjFdScraperResponseParameters.AjaxToken = FdRegexUtility.FirstMatchExtractor(htmlValue, "encrypted\":\"(.*?)\"");

        }

        public void GetPageCurser(string htmlValue)
        {

            PageletData = "timeline/app_collection/more/" + FdRegexUtility.FirstMatchExtractor(htmlValue, "href:\"/timeline/app_collection/more/(.*?)\",");

            PageletData = PageletData == "timeline/app_collection/more/" ? string.Empty : PageletData;
        }

        public void PaginationPageCurser(string htmlValue)
        {
            htmlValue = FdFunctions.GetNewPrtialDecodedResponse(htmlValue);
            PageletData = FdRegexUtility.FirstMatchExtractor(htmlValue, "\"href\":\"(.*?)\",");
        }

    }
}
