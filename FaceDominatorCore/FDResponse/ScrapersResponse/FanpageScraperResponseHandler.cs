using DominatorHouseCore;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Utility;
using FaceDominatorCore.FDLibrary.FdClassLibrary;
using FaceDominatorCore.FDLibrary.FdFunctions;
using FaceDominatorCore.FDResponse.BaseResponse;
using FaceDominatorCore.Interface;
using FaceDominatorCore.Utility;
using HtmlAgilityPack;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;

namespace FaceDominatorCore.FDResponse.ScrapersResponse
{
    public class FanpageScraperResponseHandler : FdResponseHandler, IResponseHandler
    {

        public string EntityId { get; set; }

        public string PageletData { get; set; }

        public bool HasMoreResults { get; set; }

        public bool Status { get; set; }

        public FdScraperResponseParameters ObjFdScraperResponseParameters { get; set; }
            = new FdScraperResponseParameters();

        public FanpageScraperResponseHandler(IResponseParameter responseParameter, FanpageDetails fanpageDetails)
            : base(responseParameter)
        {
            if (responseParameter.HasError)
                return;

            ObjFdScraperResponseParameters.FanpageDetails = fanpageDetails ?? (fanpageDetails = new FanpageDetails());

            string decodedResponse = FdFunctions.GetNewPrtialDecodedResponse(responseParameter.Response);

            GetFanpageDetails(decodedResponse, responseParameter.Response);
        }

        public FanpageScraperResponseHandler(IResponseParameter responseParameter, FanpageDetails fanpageDetails,
            bool isLocationPage = false)
            : base(responseParameter)
        {
            if (responseParameter.HasError)
                return;

            ObjFdScraperResponseParameters.FanpageDetails = fanpageDetails ?? (fanpageDetails = new FanpageDetails());

            string decodedResponse = FdFunctions.GetDecodedResponse(responseParameter.Response);

            GetPlaceDetails(decodedResponse);
        }
        public FanpageScraperResponseHandler(IResponseParameter responseParameter, List<string> jsonResponseList, FanpageDetails fanpageDetails)
            : base(responseParameter)
        {

            ObjFdScraperResponseParameters.FanpageDetails = fanpageDetails ?? (fanpageDetails = new FanpageDetails());
            try
            {
                foreach (var postResponse in jsonResponseList)
                {
                    JObject jObject = null;
                    JArray fdjArray = new JArray();
                    var elements = new JArray();
                    if (!postResponse.IsValidJson())
                    {
                        var decodedResponse = "";
                        if (postResponse.StartsWith("<!DOCTYPE html>"))
                        {
                            var splittedArray = Utilities.GetBetween(Regex.Split(postResponse, "</script>").SingleOrDefault(x => x.Contains("\"data\":{\"user\":{\"profile_header_renderer\"")) + "/end", "{\"require\"", "/end");
                            decodedResponse = "{\"require\"" + splittedArray;
                            if (decodedResponse.IsValidJson())
                                jObject = parser.ParseJsonToJObject(decodedResponse);
                        }
                        else
                            decodedResponse = "[" + postResponse.Replace("}}}}\r\n{\"label\":", "}}}},\r\n{\"label\":") + "]";
                        fdjArray = parser.GetJArrayElement(decodedResponse);
                    }
                    else
                    {
                        fdjArray = parser.GetJArrayElement(postResponse);
                        if (fdjArray.Count() == 0)
                            jObject = parser.ParseJsonToJObject(postResponse);
                    }
                    if (fdjArray.Count() > 0)
                    {
                        var nodes = parser.GetJTokenOfJToken(fdjArray, 0, "data", "user", "about_app_sections", "nodes", 0, "activeCollections", "nodes", 0, "style_renderer", "profile_field_sections");
                        if (nodes == null || nodes.Count() == 0)
                            nodes = JsonSearcher.FindByKey(fdjArray, "profile_field_sections");
                        if (nodes != null || nodes.Count() > 0)
                        {
                            foreach (var nodeProperty in nodes)
                                elements.Add(nodeProperty);
                        }
                    }
                    if (jObject != null && jObject.Count > 0)
                    {
                        var userNode = JsonSearcher.FindByKey(jObject, "user");
                        var nodeObject = JsonSearcher.FindByKey(userNode, "user");
                        if (nodeObject != null && nodeObject.Count() > 0 && parser.GetJTokenValue(nodeObject, "__typename") == "User")
                        {
                            userNode = nodeObject;
                            ObjFdScraperResponseParameters.FanpageDetails.FanPageID = parser.GetJTokenValue(userNode, "id");
                            ObjFdScraperResponseParameters.FanpageDetails.FanPageName = parser.GetJTokenValue(userNode, "name");
                            ObjFdScraperResponseParameters.FanpageDetails.FanPageUrl = parser.GetJTokenValue(userNode, "url");
                            ObjFdScraperResponseParameters.FanpageDetails.MenuUrl = parser.GetJTokenValue(userNode, "profilePhoto", "url");
                            var detailsSec = JsonSearcher.FindByKey(JsonSearcher.FindByKey(userNode, "profile_social_context"), "content");
                            ObjFdScraperResponseParameters.FanpageDetails.IsVerifiedPage = JsonSearcher.FindStringValueByKey(nodeObject, "is_verified");
                            foreach (var details in detailsSec)
                            {
                                var text = parser.GetJTokenValue(details, "text", "text");
                                var countString = "0";
                                if (text.Contains("followers"))
                                    countString = text.Replace(" ", "").Replace("followers", "");
                                else if (text.Contains("likes"))
                                    countString = text.Replace(" ", "").Replace("likes", "");
                                if (countString.ToLower().Contains("m"))
                                    countString = (double.Parse(FdFunctions.GetDouleOnlyString(countString)) * 1000000).ToString(CultureInfo.InvariantCulture);
                                else if (countString.ToLower().Contains("k"))
                                    countString = (double.Parse(FdFunctions.GetDouleOnlyString(countString)) * 1000).ToString(CultureInfo.InvariantCulture);

                                if (text.Contains("followers"))
                                    ObjFdScraperResponseParameters.FanpageDetails.FanpageFollowerCount = countString;
                                else if (text.Contains("likes"))
                                    ObjFdScraperResponseParameters.FanpageDetails.FanPageLikerCount = countString;

                            }
                        }
                        else
                        {
                            var nodes = parser.GetJTokenOfJToken(jObject, "data", "user", "about_app_sections", "nodes", 0, "activeCollections", "nodes", 0, "style_renderer", "profile_field_sections");
                            if (nodes == null || nodes.Count() == 0)
                                nodes = JsonSearcher.FindByKey(jObject, "profile_field_sections");
                            if (nodes != null || nodes.Count() > 0)
                            {
                                foreach (var nodeProperty in nodes)
                                    elements.Add(nodeProperty);
                            }
                        }

                    }
                    foreach (var element in elements)
                    {
                        var elementNodes = JsonSearcher.FindByKey(element, "nodes");
                        foreach (var elementNode in elementNodes)
                        {
                            var key = JsonSearcher.FindStringValueByKey(elementNode, "field_type");
                            if (string.IsNullOrEmpty(key) || key == "null_state") continue;
                            var text = parser.GetJTokenValue(elementNode, "title", "text");
                            switch (key.ToLower())
                            {
                                case "category":
                                    ObjFdScraperResponseParameters.FanpageDetails.FanPageCategory = text;
                                    break;
                                case "address":
                                    ObjFdScraperResponseParameters.FanpageDetails.Address = text;
                                    break;
                                case "screenname":
                                    {
                                        var typeText = parser.GetJTokenValue(elementNode, "list_item_groups", 0, "list_items", 0, "text", "text");
                                        if (typeText.ToLower() == "instagram")
                                            ObjFdScraperResponseParameters.FanpageDetails.Instagram = JsonSearcher.FindStringValueByKey(elementNode, "link_url");
                                        else if (typeText.ToLower() == "twitter")
                                            ObjFdScraperResponseParameters.FanpageDetails.Twitter = JsonSearcher.FindStringValueByKey(elementNode, "link_url");
                                        break;
                                    }
                                case "ratings":
                                    ObjFdScraperResponseParameters.FanpageDetails.RatingValue = Utilities.GetBetween(text, "Rating · ", " (");
                                    ObjFdScraperResponseParameters.FanpageDetails.RatingCount = Utilities.GetBetween(text, "(", " reviews)");
                                    break;
                                case "creation_date":
                                    ObjFdScraperResponseParameters.FanpageDetails.CreationDate = text;
                                    break;
                                case "business_hour":
                                    ObjFdScraperResponseParameters.FanpageDetails.BusinessHour = text;
                                    break;
                                case "profile_email":
                                    ObjFdScraperResponseParameters.FanpageDetails.Email = text;
                                    break;
                                case "website":
                                    ObjFdScraperResponseParameters.FanpageDetails.WebAddresss = text;
                                    break;
                                case "profile_phone":
                                    ObjFdScraperResponseParameters.FanpageDetails.PhoneNumber = text;
                                    break;

                            }
                        }
                    }
                }

            }
            catch (Exception e)
            { e.DebugLog(); }
            if (!string.IsNullOrEmpty(responseParameter.Response))
                ObjFdScraperResponseParameters.FanpageDetails.CanSendMessage = responseParameter.Response.Contains("aria-label=\"Message\"") ? true : false;
            Status = jsonResponseList.Count > 0;
        }
        private void GetFanpageDetails(string decodedResponse, string response)
        {
            HtmlDocument objHtmlDocument = new HtmlDocument();

            FdFunctions objFdFunctions = new FdFunctions();

            try
            {
                var pageBasicDetails = new List<string>();

                var pageId = FdRegexUtility.FirstMatchExtractor(decodedResponse, FdConstants.PageIdModRegx);

                //Some time page Id is not coming
                if (!string.IsNullOrEmpty(pageId))
                    ObjFdScraperResponseParameters.FanpageDetails.FanPageID = pageId;

                ObjFdScraperResponseParameters.FanpageDetails.FanPageUrl = $"{FdConstants.FbHomeUrl}{ObjFdScraperResponseParameters.FanpageDetails.FanPageID}";

                string pageName = FdRegexUtility.FirstMatchExtractor(response, FdConstants.FanpageNameRegx);

                pageName = FdFunctions.GetNewPrtialDecodedResponse(pageName);

                if (!string.IsNullOrEmpty(pageName))
                    ObjFdScraperResponseParameters.FanpageDetails.FanPageName = pageName;

                objHtmlDocument.LoadHtml(decodedResponse);

                if (objHtmlDocument.DocumentNode.SelectNodes("(//div[@class=\"_4-u2 _6590 _3xaf _4-u8\"])") != null)
                {
                    var fanPageFollowLikeCountData = objHtmlDocument.DocumentNode.SelectNodes("(//div[@class=\"_4-u2 _6590 _3xaf _4-u8\"])")[0].InnerHtml;

                    objHtmlDocument.LoadHtml(fanPageFollowLikeCountData);

                    fanPageFollowLikeCountData = objHtmlDocument.DocumentNode.SelectNodes("(//div[@class=\"_4bl9\"])")[1].InnerHtml;
                    ObjFdScraperResponseParameters.FanpageDetails.FanPageLikerCount = FdFunctions.GetIntegerOnlyString(fanPageFollowLikeCountData);

                    fanPageFollowLikeCountData = objHtmlDocument.DocumentNode.SelectNodes("(//div[@class=\"_4bl9\"])")[2].InnerHtml;
                    ObjFdScraperResponseParameters.FanpageDetails.FanpageFollowerCount = FdFunctions.GetIntegerOnlyString(fanPageFollowLikeCountData);

                    objHtmlDocument.LoadHtml(decodedResponse);

                    if (objHtmlDocument.DocumentNode.SelectNodes("(//div[@class=\"_4-u2 _u9q _3xaf _4-u8\"])") != null)
                    {
                        var fanpageOtherDetails = objHtmlDocument.DocumentNode.SelectNodes("(//div[@class=\"_4-u2 _u9q _3xaf _4-u8\"])")[0].InnerHtml;

                        objHtmlDocument.LoadHtml(fanpageOtherDetails);

                        var objHtmlNodeCollection = objHtmlDocument.DocumentNode.SelectNodes("(//div[@class=\"_2pi9 _2pi2\"])");

                        var listHtml = objFdFunctions.GetInnerHtmlListFromNodeCollection(objHtmlNodeCollection);

                        pageBasicDetails = objFdFunctions.GetInnerTextListFromNodeCollection(objHtmlNodeCollection);

                        foreach (string item in listHtml)
                        {

                            if (item.Contains("src=\"https://static.xx.fbcdn.net/rsrc.php/v3/yb/r/vI94qcOy7qI.png\"")
                                || item.Contains("src=\"https://static.xx.fbcdn.net/rsrc.php/v3/yz/r/oXiCJHPgn3c.png\"")
                                || item.Contains("src=\"https://static.xx.fbcdn.net/rsrc.php/v3/yW/r/mYv88EsODOI.png\""))
                            {
                                objHtmlDocument.LoadHtml(item);
                                fanpageOtherDetails = objHtmlDocument.DocumentNode.SelectNodes("(//div[@class=\"_4bl9\"])")[0].InnerHtml;
                                fanpageOtherDetails = FdRegexUtility.FirstMatchExtractor(fanpageOtherDetails, ">(.*?)<");
                                ObjFdScraperResponseParameters.FanpageDetails.PhoneNumber = fanpageOtherDetails;
                            }

                            if (item.Contains("src=\"https://static.xx.fbcdn.net/rsrc.php/v3/y6/r/HEv3RWj26j6.png\"")
                                || item.Contains("src=\"https://static.xx.fbcdn.net/rsrc.php/v3/yx/r/xVA3lB-GVep.png\""))
                            {
                                objHtmlDocument.LoadHtml(item);
                                fanpageOtherDetails = objHtmlDocument.DocumentNode.SelectNodes("(//a[@rel=\"noopener nofollow\"])")[0].InnerHtml;
                                if (fanpageOtherDetails.Contains("<span"))
                                {
                                    objHtmlDocument.LoadHtml(fanpageOtherDetails);
                                    fanpageOtherDetails = objHtmlDocument.DocumentNode.SelectNodes("(//a[@rel=\"noopener nofollow\"])")[0].InnerHtml;
                                }
                                ObjFdScraperResponseParameters.FanpageDetails.WebAddresss = fanpageOtherDetails;
                            }

                            if (item.Contains("src=\"https://static.xx.fbcdn.net/rsrc.php/v3/yk/r/v5ivj8jObfJ.png\""))
                            {
                                objHtmlDocument.LoadHtml(item);

                                objHtmlNodeCollection = objHtmlDocument.DocumentNode.SelectNodes("//a");

                                foreach (HtmlNode node in objHtmlNodeCollection)
                                    ObjFdScraperResponseParameters.FanpageDetails.FanPageCategory += $" {node.InnerHtml}";

                            }

                            if (item.Contains("src=\"https://static.xx.fbcdn.net/rsrc.php/v3/y_/r/p_aTQryawVx.png\""))
                            {
                                objHtmlDocument.LoadHtml(item);
                                fanpageOtherDetails = objHtmlDocument.DocumentNode.SelectNodes("(//div[@class=\"_4bl9\"])")[0].InnerHtml;
                                fanpageOtherDetails = FdRegexUtility.FirstMatchExtractor(fanpageOtherDetails, ">(.*?)<");
                                ObjFdScraperResponseParameters.FanpageDetails.PriceRange = Regex.Replace(fanpageOtherDetails, "Price range ", string.Empty);
                            }

                            if (item.Contains("class=\"_2wzd\""))
                            {
                                objHtmlDocument.LoadHtml(item);

                                fanpageOtherDetails = objHtmlDocument.DocumentNode.SelectNodes("(//div[@class=\"_2wzd\"])")[0].InnerHtml;
                                ObjFdScraperResponseParameters.FanpageDetails.Address = Regex.Replace(fanpageOtherDetails, "<br>", string.Empty).Replace("\"", string.Empty);
                            }

                        }
                    }
                }

                if (string.IsNullOrEmpty(ObjFdScraperResponseParameters.FanpageDetails.FanPageCategory))
                {
                    ObjFdScraperResponseParameters.FanpageDetails.FanPageCategory =
                        FdRegexUtility.FirstMatchExtractor(decodedResponse, "\"categoryName\":\"(.*?)\"");
                }

                if (string.IsNullOrEmpty(ObjFdScraperResponseParameters.FanpageDetails.PhoneNumber))
                {
                    ObjFdScraperResponseParameters.FanpageDetails.PhoneNumber = pageBasicDetails.FirstOrDefault(x =>
                            !string.IsNullOrEmpty(FdFunctions.GetContactNo(x)));
                }

                if (string.IsNullOrEmpty(ObjFdScraperResponseParameters.FanpageDetails.WebAddresss))
                {
                    ObjFdScraperResponseParameters.FanpageDetails.WebAddresss = pageBasicDetails.FirstOrDefault(x =>
                            !string.IsNullOrEmpty(FdFunctions.CheckValidWebsite(x)));
                }

                var fanpageRatingValue = FdRegexUtility.FirstMatchExtractor(decodedResponse, FdConstants.AggregateRatingRegx);


                if (!string.IsNullOrEmpty(fanpageRatingValue))
                {
                    var fanpageRatingCount = Regex.Split(fanpageRatingValue, "ratingCount\":")[1];

                    ObjFdScraperResponseParameters.FanpageDetails.RatingCount = fanpageRatingCount;

                    fanpageRatingValue =
                        FdRegexUtility.FirstMatchExtractor(fanpageRatingValue, FdConstants.RatingValueRegx);

                    ObjFdScraperResponseParameters.FanpageDetails.RatingValue = fanpageRatingValue;
                }

            }
            catch (ArgumentException ex)
            {
                ex.DebugLog();
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            Status = ObjFdScraperResponseParameters.FanpageDetails != null;
        }

        private void GetPlaceDetails(string decodedResponse)
        {
            HtmlDocument objHtmlDocument = new HtmlDocument();

            try
            {
                objHtmlDocument.LoadHtml(decodedResponse);

                var fanPageFollowLikeCountData = objHtmlDocument.DocumentNode.SelectNodes("(//div[@class=\"_4-u2 _6590 _3xaf _4-u8\"])")[0].InnerHtml;

                objHtmlDocument.LoadHtml(fanPageFollowLikeCountData);

                fanPageFollowLikeCountData = objHtmlDocument.DocumentNode.SelectNodes("(//div[@class=\"_4bl9\"])")[1].InnerHtml;
                ObjFdScraperResponseParameters.FanpageDetails.FanPageLikerCount = FdFunctions.GetIntegerOnlyString(fanPageFollowLikeCountData);

                fanPageFollowLikeCountData = objHtmlDocument.DocumentNode.SelectNodes("(//div[@class=\"_4bl9\"])")[2].InnerHtml;
                ObjFdScraperResponseParameters.FanpageDetails.FanpageFollowerCount = FdFunctions.GetIntegerOnlyString(fanPageFollowLikeCountData);

                objHtmlDocument.LoadHtml(decodedResponse);

                if (objHtmlDocument.DocumentNode.SelectNodes("(//div[@class=\"_4-u2 _u9q _3xaf _4-u8\"])") != null)
                {
                    var fanpageOtherDetails = objHtmlDocument.DocumentNode.SelectNodes("(//div[@class=\"_4-u2 _u9q _3xaf _4-u8\"])")[0].InnerHtml;

                    objHtmlDocument.LoadHtml(fanpageOtherDetails);

                    var lstFanpageDetails = objHtmlDocument.DocumentNode.SelectNodes("(//div[@class=\"_4bl9 _v0m\"])");

                    ObjFdScraperResponseParameters.FanpageDetails.WebAddresss = lstFanpageDetails != null ? lstFanpageDetails[0].InnerText : string.Empty;

                }

            }
            catch (ArgumentException ex)
            {
                ex.DebugLog();
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            Status = ObjFdScraperResponseParameters.FanpageDetails != null;
        }
    }
}
