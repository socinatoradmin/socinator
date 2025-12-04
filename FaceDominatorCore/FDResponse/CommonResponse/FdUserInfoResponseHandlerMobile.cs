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
using System.Linq;
using System.Text.RegularExpressions;

namespace FaceDominatorCore.FDResponse.CommonResponse
{
    public class FdUserInfoResponseHandlerMobile : FdResponseHandler, IResponseHandler
    {
        public string EntityId { get; set; }
        public string PageletData { get; set; }
        public bool HasMoreResults { get; set; }
        public bool Status { get; set; }
        public FdScraperResponseParameters ObjFdScraperResponseParameters { get; set; }

        public FdUserInfoResponseHandlerMobile(IResponseParameter responseParameter, FacebookUser facebookUser)
        : base(responseParameter)
        {

            if (responseParameter.HasError)
                return;
            ObjFdScraperResponseParameters = new FdScraperResponseParameters();
            ObjFdScraperResponseParameters.FacebookUser = facebookUser;

            var decodedResponse = FdFunctions.GetDecodedResponse(responseParameter.Response);

            GetBasicUserDetail(decodedResponse);

            GetFullUserDetails(responseParameter);
        }
        public FdUserInfoResponseHandlerMobile(IResponseParameter responseParameter, List<string> jsonResponseList, FacebookUser facebookUser)
            : base(responseParameter)
        {
            ObjFdScraperResponseParameters = new FdScraperResponseParameters();
            ObjFdScraperResponseParameters.FacebookUser = facebookUser;
            try
            {
                ObjFdScraperResponseParameters.FacebookUser.IsAllDetailsScrapped = true;
                foreach (var postResponse in jsonResponseList)
                {
                    JObject jObject = null;
                    JArray fdjArray = new JArray();
                    var elements = new JArray();
                    try
                    {
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
                                ObjFdScraperResponseParameters.FacebookUser.UserId = parser.GetJTokenValue(userNode, "id");
                                ObjFdScraperResponseParameters.FacebookUser.Username = parser.GetJTokenValue(userNode, "name");
                                ObjFdScraperResponseParameters.FacebookUser.FullName = parser.GetJTokenValue(userNode, "name");
                                ObjFdScraperResponseParameters.FacebookUser.Familyname = parser.GetJTokenValue(userNode, "name");
                                ObjFdScraperResponseParameters.FacebookUser.ProfileUrl = parser.GetJTokenValue(userNode, "url");
                                ObjFdScraperResponseParameters.FacebookUser.ProfilePicUrl = parser.GetJTokenValue(userNode, "profilePhoto", "url");
                                ObjFdScraperResponseParameters.FacebookUser.Gender = parser.GetJTokenValue(userNode, "gender");
                                ObjFdScraperResponseParameters.FacebookUser.IsPrivateUser = JsonSearcher.FindStringValueByKey(userNode, "private_sharing_enabled").ToLower().Contains("true") ? true : false;
                                ObjFdScraperResponseParameters.FacebookUser.IsverifiedUser = JsonSearcher.FindStringValueByKey(userNode, "is_verified").ToLower().Contains("true") ? true : false;
                                ObjFdScraperResponseParameters.FacebookUser.ProfileId = !ObjFdScraperResponseParameters.FacebookUser.ProfileUrl.Contains(ObjFdScraperResponseParameters.FacebookUser.UserId) ?
                                    Utilities.GetBetween(ObjFdScraperResponseParameters.FacebookUser.ProfileUrl + "/", FdConstants.FbHomeUrl, "/") : ObjFdScraperResponseParameters.FacebookUser.UserId;
                                var socialContext = JsonSearcher.FindStringValueByKey(userNode, "profile_social_context");
                                var mutualFriendCount = FdFunctions.GetIntegerOnlyString(socialContext);
                                ObjFdScraperResponseParameters.FacebookUser.HasMutualFriends = socialContext.Contains("mutual friends") && mutualFriendCount != "0" ? true : false;
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
                                var key = JsonSearcher.FindStringValueByKey(elementNode, "group_key");
                                if (string.IsNullOrEmpty(key))
                                    key = JsonSearcher.FindStringValueByKey(elementNode, "field_type");
                                if (string.IsNullOrEmpty(key) || key == "null_state") continue;
                                var text = parser.GetJTokenValue(elementNode, "title", "text");
                                if (!string.IsNullOrEmpty(key) && !string.IsNullOrEmpty(text)
                                    && key.ToLower() == text.ToLower())
                                    text = parser.GetJTokenValue(elementNode, "renderer", "field", "text_content", "text");

                                switch (key)
                                {
                                    case "website":
                                        ObjFdScraperResponseParameters.FacebookUser.WebsiteUrl = text;
                                        break;
                                    case "email":
                                    case "profile_email":
                                        ObjFdScraperResponseParameters.FacebookUser.Email = text;
                                        break;
                                    case "gender":
                                        ObjFdScraperResponseParameters.FacebookUser.Gender = text;
                                        break;
                                    case "birthday":
                                        {
                                            var typeText = parser.GetJTokenValue(elementNode, "list_item_groups", 0, "list_items", 0, "text", "text");
                                            if (typeText.ToLower() == "birth date")
                                                ObjFdScraperResponseParameters.FacebookUser.DateOfBirth = text;
                                            else ObjFdScraperResponseParameters.FacebookUser.DateOfBirth = ObjFdScraperResponseParameters.FacebookUser.DateOfBirth + " " + text;
                                            break;
                                        }
                                    case "hometown":
                                        ObjFdScraperResponseParameters.FacebookUser.Hometown = text?.Replace("From ", "");
                                        break;
                                    case "work":
                                        ObjFdScraperResponseParameters.FacebookUser.WorkPlace = text?.Replace("Works at ", "");
                                        break;
                                    case "education":
                                        ObjFdScraperResponseParameters.FacebookUser.University = text?.Replace("Studied at ", "");
                                        break;
                                    case "current_city":
                                        ObjFdScraperResponseParameters.FacebookUser.Currentcity = text?.Replace("Lives in ", "");
                                        break;
                                    case "relationship":
                                        ObjFdScraperResponseParameters.FacebookUser.RelationShip = text;
                                        break;
                                    case "languages":
                                        ObjFdScraperResponseParameters.FacebookUser.OtherDetails = text;
                                        break;
                                    case "contact_no":
                                        ObjFdScraperResponseParameters.FacebookUser.ContactNo = text;
                                        break;
                                    case "other_phone":
                                    case "profile_phone":
                                        ObjFdScraperResponseParameters.FacebookUser.PhoneNumber = text;
                                        break;

                                }
                            }
                        }

                    }
                    catch (Exception e) { e.DebugLog(); }
                }
                ObjFdScraperResponseParameters.FacebookUser.ContactNo = !string.IsNullOrEmpty(ObjFdScraperResponseParameters.FacebookUser.ContactNo) ?
                    ObjFdScraperResponseParameters.FacebookUser.ContactNo : ObjFdScraperResponseParameters.FacebookUser.PhoneNumber;

                var DoB = new DateTime();
                if (!string.IsNullOrEmpty(facebookUser.DateOfBirth) && DateTime.TryParse(facebookUser.DateOfBirth, out DoB))
                    ObjFdScraperResponseParameters.FacebookUser.Age = (Math.Abs((DoB - DateTime.Now.Date).Days) / 365).ToString();
                if (!string.IsNullOrEmpty(responseParameter.Response))
                {
                    ObjFdScraperResponseParameters.FacebookUser.CanSendMessage = responseParameter.Response.Contains("aria-label=\"Message\"") ? true : false;
                    ObjFdScraperResponseParameters.FacebookUser.CanSendFriendRequest = responseParameter.Response.Contains("aria-label=\"Add friend\"")
                        || responseParameter.Response.Contains($"Add Friend {ObjFdScraperResponseParameters?.FacebookUser?.FullName}")? "true" : "false";
                    ObjFdScraperResponseParameters.FacebookUser.CanFollow = responseParameter.Response.Contains("aria-label=\"Follow\"") ? "true" : "false";
                }
                if (string.IsNullOrEmpty(ObjFdScraperResponseParameters.FacebookUser.ScrapedProfileUrl) && !string.IsNullOrEmpty(ObjFdScraperResponseParameters.FacebookUser.ProfileUrl))
                    ObjFdScraperResponseParameters.FacebookUser.ScrapedProfileUrl = ObjFdScraperResponseParameters.FacebookUser.ProfileUrl;
                if (string.IsNullOrEmpty(ObjFdScraperResponseParameters.FacebookUser.Address) && !string.IsNullOrEmpty(ObjFdScraperResponseParameters.FacebookUser.Currentcity))
                    ObjFdScraperResponseParameters.FacebookUser.Address = ObjFdScraperResponseParameters.FacebookUser.Currentcity;
                if (string.IsNullOrEmpty(ObjFdScraperResponseParameters.FacebookUser.Address) && !string.IsNullOrEmpty(ObjFdScraperResponseParameters.FacebookUser.Hometown))
                    ObjFdScraperResponseParameters.FacebookUser.Address = ObjFdScraperResponseParameters.FacebookUser.Hometown;
            }
            catch (Exception e) { e.DebugLog(); }

        }


        public void GetBasicUserDetail(string decodedResponse)
        {
            string workPlaceResponse = string.Empty;
            string educationResponse = string.Empty;
            string locationResponse = string.Empty;
            string contactNoResponse = string.Empty;
            string emailResponse = string.Empty;
            string birthDayResponse = string.Empty;
            string genderResponse = string.Empty;
            string relationShipResponse = string.Empty;
            string addressResponse = string.Empty;
            string regionResponse = string.Empty;

            HtmlDocument objHtmlDocument = new HtmlDocument();
            try
            {
                string profileId = FdRegexUtility.FirstMatchExtractor(decodedResponse, "currentProfileID\":(.*?),");
                if (string.IsNullOrEmpty(profileId?.Trim()))
                    profileId = FdRegexUtility.FirstMatchExtractor(decodedResponse, "currentProfileID:(.*?),");
                if (string.IsNullOrEmpty(profileId) || profileId == "null")
                    profileId = FdRegexUtility.FirstMatchExtractor(decodedResponse, "entity_id:(.*?),");


                ObjFdScraperResponseParameters.FacebookUser.UserId = string.IsNullOrEmpty(profileId)
                    ? ObjFdScraperResponseParameters.FacebookUser.UserId
                    : FdFunctions.GetIntegerOnlyString(profileId);

                objHtmlDocument.LoadHtml(decodedResponse);

                try
                {
                    ObjFdScraperResponseParameters.FacebookUser.Familyname = FdRegexUtility.FirstMatchExtractor(decodedResponse, "<title>(.*?)</title>");
                    //ObjFdScraperResponseParameters.FacebookUser.Familyname = name;
                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                }

                try
                {
                    locationResponse = objHtmlDocument.DocumentNode.SelectNodes("(//div[@id=\"living\"])") != null
                        ? objHtmlDocument.DocumentNode.SelectNodes("(//div[@id=\"living\"])")[0].InnerHtml
                        : string.Empty;

                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                }
                try
                {
                    if (objHtmlDocument.DocumentNode.SelectNodes("(//div[@id=\"education\"])") != null)
                    {
                        var educationResponseCollection = objHtmlDocument.DocumentNode.SelectNodes("(//ul[@id=\"education\"])");

                        if (educationResponseCollection == null)
                        {
                            educationResponse =
                                objHtmlDocument.DocumentNode.SelectNodes("(//div[@id=\"education\"])") != null
                                    ? objHtmlDocument.DocumentNode.SelectNodes("(//div[@id=\"education\"])")[0].InnerHtml
                                    : string.Empty;

                        }
                        else
                            educationResponse = educationResponseCollection[0].InnerHtml;
                    }
                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                }
                try
                {
                    workPlaceResponse = objHtmlDocument.DocumentNode.SelectNodes("(//div[@id=\"work\"])") != null
                        ? objHtmlDocument.DocumentNode.SelectNodes("(//div[@id=\"work\"])")[0].InnerHtml
                        : string.Empty;
                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                }

                try
                {
                    genderResponse = birthDayResponse =
                        objHtmlDocument.DocumentNode.SelectNodes("(//div[@id=\"basic-info\"])") != null
                            ? objHtmlDocument.DocumentNode.SelectNodes("(//div[@id=\"basic-info\"])")[0].InnerHtml
                            : string.Empty;

                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                }

                try
                {
                    if (objHtmlDocument.DocumentNode.SelectNodes("(//div[@id=\"contact-info\"])") != null)
                    {
                        contactNoResponse =
                            objHtmlDocument.DocumentNode.SelectNodes("(//div[@title=\"Mobile\"])") != null
                                ? objHtmlDocument.DocumentNode.SelectNodes("(//div[@title=\"Mobile\"])")[0].InnerHtml
                                : string.Empty;

                        emailResponse = objHtmlDocument.DocumentNode.SelectNodes("(//div[@title=\"Email\"])") != null
                            ? objHtmlDocument.DocumentNode.SelectNodes("(//div[@title=\"Email\"])")[0].InnerHtml
                            : string.Empty;

                        addressResponse =
                            objHtmlDocument.DocumentNode.SelectNodes("(//div[@title=\"Address\"])") != null
                                ? objHtmlDocument.DocumentNode.SelectNodes("(//div[@title=\"Address\"])")[0].InnerHtml
                                : string.Empty;

                        regionResponse =
                            objHtmlDocument.DocumentNode.SelectNodes("(//div[@title=\"Neighborhood\"])") != null
                                ? objHtmlDocument.DocumentNode.SelectNodes("(//div[@title=\"Neighborhood\"])")[0].InnerHtml
                                : string.Empty;

                    }
                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                }

                try
                {
                    relationShipResponse =
                       objHtmlDocument.DocumentNode.SelectNodes("(//div[@id=\"relationship\"])") != null
                           ? objHtmlDocument.DocumentNode.SelectNodes("(//div[@id=\"relationship\"])")[0].InnerHtml
                           : string.Empty;

                    //if (objHtmlNodeCollection != null)
                    //    relationShipResponse = objHtmlNodeCollection[0].InnerHtml;
                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                }


                if (!string.IsNullOrEmpty(regionResponse))
                {
                    objHtmlDocument.LoadHtml(regionResponse);

                    ObjFdScraperResponseParameters.FacebookUser.NeighBorHood = objHtmlDocument.DocumentNode.SelectNodes("(//div[@class=\"_5cdv r\"])") != null
                        ? objHtmlDocument.DocumentNode.SelectNodes("(//div[@class=\"_5cdv r\"])")[0].InnerHtml
                        : string.Empty;
                }

                if (!string.IsNullOrEmpty(addressResponse))
                {
                    objHtmlDocument.LoadHtml(addressResponse);
                    var objHtmlAddressNodeCollection = objHtmlDocument.DocumentNode.SelectNodes("//a");

                    if (objHtmlAddressNodeCollection != null)
                    {
                        ObjFdScraperResponseParameters.FacebookUser.Address = objHtmlAddressNodeCollection[0].InnerHtml;

                        ObjFdScraperResponseParameters.FacebookUser.AddressMapUrl = FdRegexUtility.FirstMatchExtractor(objHtmlAddressNodeCollection[0].OuterHtml,
                            "href=\"(.*?)\"");
                    }
                }


                if (!string.IsNullOrEmpty(birthDayResponse))
                {
                    objHtmlDocument.LoadHtml(birthDayResponse);
                    var objHtmlBirthdayNodeCollection = objHtmlDocument.DocumentNode.SelectNodes("(//div[@title=\"Birthday\"])");

                    if (objHtmlBirthdayNodeCollection != null)
                    {
                        objHtmlDocument.LoadHtml(objHtmlBirthdayNodeCollection[0].InnerHtml);

                        ObjFdScraperResponseParameters.FacebookUser.DateOfBirth =
                            objHtmlDocument.DocumentNode.SelectNodes("(//div[@class=\"_5cdv r\"])") != null
                                ? objHtmlDocument.DocumentNode.SelectNodes("(//div[@class=\"_5cdv r\"])")[0].InnerText : string.Empty;

                    }
                }

                if (!string.IsNullOrEmpty(genderResponse))
                {
                    objHtmlDocument.LoadHtml(genderResponse);
                    var objHtmlGenderNodeCollection = objHtmlDocument.DocumentNode.SelectNodes("(//div[@title=\"Gender\"])");

                    if (objHtmlGenderNodeCollection != null)
                    {
                        objHtmlDocument.LoadHtml(objHtmlGenderNodeCollection[0].InnerHtml);
                        ObjFdScraperResponseParameters.FacebookUser.Gender =
                            objHtmlDocument.DocumentNode.SelectNodes("(//div[@class=\"_5cdv r\"])") != null
                                ? objHtmlDocument.DocumentNode.SelectNodes("(//div[@class=\"_5cdv r\"])")[0].InnerText
                                : string.Empty;
                    }
                }

                if (!string.IsNullOrEmpty(relationShipResponse))
                {
                    objHtmlDocument.LoadHtml(relationShipResponse);
                    ObjFdScraperResponseParameters.FacebookUser.RelationShip =
                        objHtmlDocument.DocumentNode.SelectNodes("(//div[@class=\"_52ja _5cds _5cdt\"])") != null
                            ? objHtmlDocument.DocumentNode.SelectNodes("(//div[@class=\"_52ja _5cds _5cdt\"])")[0].InnerText
                            : string.Empty;
                }

                if (!string.IsNullOrEmpty(contactNoResponse))
                {
                    objHtmlDocument.LoadHtml(contactNoResponse);
                    ObjFdScraperResponseParameters.FacebookUser.ContactNo =
                        objHtmlDocument.DocumentNode.SelectNodes("(//span[@dir=\"ltr\"])") != null
                            ? objHtmlDocument.DocumentNode.SelectNodes("(//span[@dir=\"ltr\"])")[0].InnerText
                            : string.Empty;
                }

                if (!string.IsNullOrEmpty(emailResponse))
                {
                    objHtmlDocument.LoadHtml(emailResponse);
                    ObjFdScraperResponseParameters.FacebookUser.Email = objHtmlDocument.DocumentNode.SelectNodes("(//a)") != null
                        ? objHtmlDocument.DocumentNode.SelectNodes("(//a)")[0].InnerText
                        : string.Empty;
                }


                if (!string.IsNullOrEmpty(workPlaceResponse))
                {
                    try
                    {
                        objHtmlDocument.LoadHtml(workPlaceResponse);
                        ObjFdScraperResponseParameters.FacebookUser.WorkPlace =
                            objHtmlDocument.DocumentNode.SelectNodes("(//span[@class=\"_52jd _52jb _52jh _3-8_\"])") != null
                                ? objHtmlDocument.DocumentNode.SelectNodes("(//span[@class=\"_52jd _52jb _52jh _3-8_\"])")[0].InnerText
                                : string.Empty;
                    }
                    catch (Exception ex)
                    {
                        ex.DebugLog();
                    }
                }

                if (!string.IsNullOrEmpty(educationResponse))
                {
                    try
                    {
                        objHtmlDocument.LoadHtml(educationResponse);
                        ObjFdScraperResponseParameters.FacebookUser.University =
                            objHtmlDocument.DocumentNode.SelectNodes("(//span[@class=\"_52jd _52jb _52jh _3-8_\"])") != null
                                ? objHtmlDocument.DocumentNode.SelectNodes("(//span[@class=\"_52jd _52jb _52jh _3-8_\"])")[0].InnerText
                                : string.Empty;
                    }
                    catch (Exception ex)
                    {
                        ex.DebugLog();
                    }
                }
                if (!string.IsNullOrEmpty(locationResponse))
                {
                    try
                    {
                        objHtmlDocument.LoadHtml(locationResponse);
                        var objHtmlNodeCollection = objHtmlDocument.DocumentNode.SelectNodes("(//div[@class=\"_2swz _2lcw\"])");

                        if (objHtmlNodeCollection != null)
                        {
                            if (objHtmlNodeCollection[0].InnerText.Contains("Current City"))
                            {
                                var document = new HtmlDocument();
                                document.LoadHtml(objHtmlNodeCollection[0].InnerHtml);
                                if (document.DocumentNode.SelectNodes("(//h4)") != null)
                                    ObjFdScraperResponseParameters.FacebookUser.Currentcity = document.DocumentNode.SelectNodes("(//h4)")[0].InnerText;
                            }
                            else
                            {
                                var document = new HtmlDocument();
                                document.LoadHtml(objHtmlNodeCollection[0].InnerHtml);
                                if (document.DocumentNode.SelectNodes("(//h4)") != null)
                                    ObjFdScraperResponseParameters.FacebookUser.Hometown = document.DocumentNode.SelectNodes("(//h4)")[0].InnerText;
                            }

                            if (objHtmlNodeCollection.Count > 1)
                            {
                                var document = new HtmlDocument();
                                document.LoadHtml(objHtmlNodeCollection[1].InnerHtml);
                                if (document.DocumentNode.SelectNodes("(//h4)") != null)
                                    ObjFdScraperResponseParameters.FacebookUser.Hometown = document.DocumentNode.SelectNodes("(//h4)")[0].InnerText;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        ex.DebugLog();
                    }
                }

                string regexExpression =
                    "_42ft _4jy0 _55pi _2agf _4o_4 FriendRequestFriends friendButton enableFriendListFlyout _4jy4 _517h _9c6";
                ObjFdScraperResponseParameters.FacebookUser.IsFriendAccount = decodedResponse.Contains(regexExpression);
                try
                {

                    var listDetails = DominatorHouseCore.Utility.HtmlParseUtility.GetListInnerTextFromPartialTagNamecontains(decodedResponse, "div", "class", "_5cds _2lcw _5cdu");

                    if (string.IsNullOrEmpty(ObjFdScraperResponseParameters.FacebookUser.ContactNo))
                    {
                        ObjFdScraperResponseParameters.FacebookUser.ContactNo
                            = FdFunctions.GetIntegerOnlyString(listDetails.FirstOrDefault(x => x.Contains("Mobile")));
                    }
                    if (string.IsNullOrEmpty(ObjFdScraperResponseParameters.FacebookUser.Email) || !ObjFdScraperResponseParameters.FacebookUser.Email.Contains("@"))
                    {
                        ObjFdScraperResponseParameters.FacebookUser.Email
                            = listDetails.FirstOrDefault(x => x.Contains("Email address"));

                        ObjFdScraperResponseParameters.FacebookUser.Email
                            = ObjFdScraperResponseParameters.FacebookUser.Email?.Replace("Email address", string.Empty);

                    }

                    if (string.IsNullOrEmpty(ObjFdScraperResponseParameters.FacebookUser.DateOfBirth))
                    {
                        ObjFdScraperResponseParameters.FacebookUser.DateOfBirth =
                             listDetails.FirstOrDefault(x => x.Contains("Date of birth") || x.Contains("Birthday"));

                        ObjFdScraperResponseParameters.FacebookUser.DateOfBirth =
                            ObjFdScraperResponseParameters.FacebookUser.DateOfBirth?.Replace("Date of birth", string.Empty).Replace("Birthday", string.Empty);
                    }
                    if (string.IsNullOrEmpty(ObjFdScraperResponseParameters.FacebookUser.Gender))
                    {
                        ObjFdScraperResponseParameters.FacebookUser.Gender =
                             listDetails.FirstOrDefault(x => x.Contains("Gender"));

                        ObjFdScraperResponseParameters.FacebookUser.Gender =
                            ObjFdScraperResponseParameters.FacebookUser.Gender?.Replace("Gender", string.Empty);
                    }
                }
                catch (Exception)
                {
                }

            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        public void GetFullUserDetails(IResponseParameter responseParameter)
        {
            try
            {
                ObjFdScraperResponseParameters.FacebookUser.ProfilePicUrl =
                    string.IsNullOrEmpty(ObjFdScraperResponseParameters.FacebookUser.ProfilePicUrl)
                        ? $"https://graph.facebook.com/{ObjFdScraperResponseParameters.FacebookUser.UserId}/picture?type=large&redirect=true&width=400&height=400"
                        : ObjFdScraperResponseParameters.FacebookUser.ProfilePicUrl;

                ObjFdScraperResponseParameters.FacebookUser.ProfileUrl = $"{FdConstants.FbHomeUrl}{ObjFdScraperResponseParameters.FacebookUser.UserId}";
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }



    }
}

