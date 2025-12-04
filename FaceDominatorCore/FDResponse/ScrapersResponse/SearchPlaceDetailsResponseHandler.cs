using DominatorHouseCore;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Utility;
using FaceDominatorCore.FDLibrary.FdClassLibrary;
using FaceDominatorCore.FDLibrary.FdFunctions;
using FaceDominatorCore.FDResponse.BaseResponse;
using FaceDominatorCore.Interface;
using HtmlAgilityPack;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

namespace FaceDominatorCore.FDResponse.ScrapersResponse
{

    public class SearchFanpageDetailsResponseHandler : FdResponseHandler, IResponseHandler
    {
        public bool HasMoreResults { get; set; }

        public string EntityId { get; set; }

        public string PageletData { get; set; }

        public bool Status { get; set; }

        public FdScraperResponseParameters ObjFdScraperResponseParameters { get; set; }
        = new FdScraperResponseParameters();

        public SearchFanpageDetailsResponseHandler(IResponseParameter responseParameter, bool isUpdate = false)
            : base(responseParameter)
        {

            if (responseParameter.HasError)
                return;

            ObjFdScraperResponseParameters.ListPage = new List<FanpageDetails>();

            //if Class called for update then if condition will execute.
            if (isUpdate)
            {
                string pagesLiked = string.Empty;
                //for Pagination Value
                if (responseParameter.Response.StartsWith("for (;;);"))
                {
                    JObject jObject = JObject.Parse(responseParameter.Response.Replace("for (;;);", string.Empty));
                    pagesLiked = jObject["payload"].ToString();
                }
                else
                    pagesLiked = Utilities.GetBetween(responseParameter.Response, "<div class=\"_7wjh\">", "</code>");

                PageletData = Utilities.GetBetween(responseParameter.Response, "PageBrowserAllLikedPagesPagelet\",", "}") + "}";

                PageletData = PageletData == "}" ? string.Empty : PageletData;

                string[] pageDetailSplit = Regex.Split(pagesLiked, "aria-label=");

                foreach (var pageDetail in pageDetailSplit)
                {
                    if (!pageDetail.StartsWith("\"")) continue;
                    ObjFdScraperResponseParameters.ListPage.Add(new FanpageDetails()
                    {
                        FanPageName = HttpUtility.HtmlDecode(Utilities.GetBetween(pageDetail, "\"", "\"")),
                        FanPageID = Utilities.GetBetween(pageDetail, "page.php?id=", "\""),
                        FanPageUrl = $"{FdConstants.FbHomeUrl}{Utilities.GetBetween(pageDetail, "page.php?id=", "\"")}"
                    });
                }
                Status = ObjFdScraperResponseParameters.ListPage.Count > 0;

                return;
            }
            //if search method is called
            string decodedResponse = FdFunctions.GetNewPrtialDecodedResponse(responseParameter.Response);

            HtmlDocument objHtmlDocument = new HtmlDocument();

            objHtmlDocument.LoadHtml(decodedResponse);

            HtmlNodeCollection objNodeCollection =
                objHtmlDocument.DocumentNode.SelectNodes("//div[starts-with(@class, '_3u1 _gli')]");

            if (objNodeCollection != null)
            {
                List<string> fanpageResponseList = new List<string>();

                objNodeCollection.ForEach(objNode =>
                    fanpageResponseList.Add(objNode.InnerHtml));

                GetFanpageId(fanpageResponseList, responseParameter.Response);
            }
        }

        private void GetFanpageId(List<string> fanpageResponseList, string partialDecodedResponse)
        {
            double fanpageLikeCount = 0;

            HtmlDocument objHtmlDocument = new HtmlDocument();

            var nameDetails = Regex.Split(partialDecodedResponse, "_3u1 _gli");

            foreach (string response in fanpageResponseList)
            {
                try
                {
                    FanpageDetails objFanpageDetails = new FanpageDetails();

                    string fanpageName = string.Empty;

                    objHtmlDocument.LoadHtml(response);
                    var fanpageId = objHtmlDocument.DocumentNode.SelectNodes("//button")[0].OuterHtml;

                    if (!fanpageId.Contains("data-profileid=\""))
                        continue;

                    fanpageId = FdRegexUtility.FirstMatchExtractor(fanpageId, FdConstants.DataProfileIdRegx);

                    objFanpageDetails.FanPageID = fanpageId;

                    objFanpageDetails.FanPageUrl = $"{FdConstants.FbHomeUrl}{fanpageId}";

                    var fanpageLikeDetails = objHtmlDocument.DocumentNode.SelectNodes("(//div[@class=\"_pac\"])")[0].InnerHtml;

                    objHtmlDocument.LoadHtml(fanpageLikeDetails);

                    if (objHtmlDocument.DocumentNode.SelectNodes("//a") != null)
                    {
                        fanpageLikeDetails = objHtmlDocument.DocumentNode.SelectNodes("//a")[0].InnerHtml;

                        if (fanpageLikeDetails.Contains("<span"))
                            fanpageLikeDetails = objHtmlDocument.DocumentNode.SelectNodes("//a")[1].InnerHtml;

                        fanpageLikeDetails = Regex.Split(fanpageLikeDetails, " ")[0];


                        if (fanpageLikeDetails.ToLower().Contains("k"))
                        {
                            fanpageLikeCount = double.Parse(FdFunctions.GetDouleOnlyString(fanpageLikeDetails));
                            fanpageLikeCount = fanpageLikeCount * 1000;
                        }
                        else if (fanpageLikeDetails.ToLower().Contains("m"))
                        {
                            fanpageLikeCount = double.Parse(FdFunctions.GetDouleOnlyString(fanpageLikeDetails));
                            fanpageLikeCount = fanpageLikeCount * 1000000;
                        }
                    }

                    objFanpageDetails.FanPageLikerCount = fanpageLikeCount.ToString();


                    objFanpageDetails.IsVerifiedPage = response.Contains("_56_f _5dzy _5dz- _3twv") ? "True" : "False";

                    objFanpageDetails.IsLikedByUser =
                        response.Contains("_42ft _4jy0 PageLikedButton _52nf PageLikeButton _4jy3 _517h _51sy")
                            ? true
                            : false;


                    objFanpageDetails.IsLikedByFriend =
                        response.Contains("img sp_kdXIGLzRSM_ sx_27e48a") ||
                        response.Contains("img sp_kdXIGLzRSM__1_5x sx_786b04")
                            ? "True"
                            : "False";

                    objHtmlDocument.LoadHtml(response);

                    var fanpageDescriptionNode = objHtmlDocument.DocumentNode.SelectNodes("(//div[@class=\"_52eh\"])");

                    if (fanpageDescriptionNode != null)
                    {
                        foreach (HtmlNode node in fanpageDescriptionNode)
                        {
                            try
                            {
                                if (node.InnerHtml.Contains("_52eh _5bcu"))
                                {
                                    objHtmlDocument.LoadHtml(node.InnerHtml);

                                    var fanpageDetails = objHtmlDocument.DocumentNode.SelectNodes("(//div[@class=\"_52eh _5bcu\"])")[0].InnerHtml;

                                    var fanpageDescription = Regex.Replace(fanpageDetails, "<span(.*?)>", string.Empty).Replace("</span(.*?)>", string.Empty).Replace("\"", string.Empty);

                                    objFanpageDetails.FanPageDescription = fanpageDescription;

                                    break;
                                }
                            }
                            catch (Exception ex)
                            {
                                ex.DebugLog();
                            }
                        }
                    }


                    var detailsArray = nameDetails.FirstOrDefault(x => x.Contains(fanpageId));

                    try
                    {
                        var details = Regex.Split(detailsArray, "_2mch _gll");

                        if (details.Length >= 2)
                        {

                            var nameArray = Regex.Split(details[1].Replace("\\u003C", "<"), "<a");

                            if (nameArray.Length >= 2)
                            {
                                //var match = FdRegexUtility.FirstMatchExtractor(nameArray[1], FdConstants.FanpageNameModRegx);

                                fanpageName = FdFunctions.GetPrtialDecodedResponse(fanpageName);

                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        ex.DebugLog();
                    }

                    if (string.IsNullOrEmpty(fanpageName))
                    {
                        objHtmlDocument.LoadHtml(response);

                        try
                        {
                            if (response.Contains("_52eh _5bcu"))
                                if (objHtmlDocument.DocumentNode.SelectNodes("(//div[@class=\"_52eh _5bcu\"])") != null)
                                {
                                    var fanpageNameDetails = objHtmlDocument.DocumentNode.SelectNodes("(//div[@class=\"_52eh _5bcu\"])")[0].InnerHtml;

                                    fanpageName = FdRegexUtility.FirstMatchExtractor(fanpageNameDetails, FdConstants.UserNameModRegx);

                                    if (fanpageName.Contains("<span"))
                                        fanpageName = Regex.Split(fanpageName, "<span")[0];
                                }

                        }
                        catch (Exception ex)
                        {
                            ex.DebugLog();
                        }
                    }

                    objFanpageDetails.FanPageName = fanpageName;

                    try
                    {
                        if (response.Contains("<span> · </span>"))
                        {
                            var category = FdRegexUtility.FirstMatchExtractor(response, FdConstants.CategoryRegx);

                            if (string.IsNullOrEmpty(category))
                            {
                                category = FdRegexUtility.FirstMatchExtractor(response, FdConstants.CategoryModRegx);
                            }

                            //var category = FdRegexUtility.FirstMatchExtractor(response, "<span> · </span>(.*?)/a>");
                            objFanpageDetails.FanPageCategory = FdRegexUtility.FirstMatchExtractor(category, ">(.*?)<");

                        }

                        if (string.IsNullOrEmpty(objFanpageDetails.FanPageCategory))
                        {
                            try
                            {

                                if (response.Contains("<span>u00a0u00b7u00a0</span>"))
                                {
                                    var pageCategory = FdRegexUtility.FirstMatchExtractor(response, "<span>u00a0u00b7u00a0</span>(.*?)/a>");

                                    if (string.IsNullOrEmpty(pageCategory))
                                    {
                                        pageCategory = FdRegexUtility.FirstMatchExtractor(response, "<span>u00a0u00b7u00a0</span>(.*?)/span>");
                                    }

                                    objFanpageDetails.FanPageCategory = FdRegexUtility.FirstMatchExtractor(pageCategory, ">(.*?)<");
                                }
                            }
                            catch (Exception e)
                            {
                                e.DebugLog();
                            }
                        }
                    }
                    catch (ArgumentException)
                    {

                    }
                    catch (Exception)
                    {

                    }
                    if (ObjFdScraperResponseParameters.ListPage.FirstOrDefault(x => x.FanPageID == fanpageId) == null)
                        ObjFdScraperResponseParameters.ListPage.Add(objFanpageDetails);
                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                }
            }

            Status = ObjFdScraperResponseParameters.ListPage.Count > 0;

        }
    }
}
