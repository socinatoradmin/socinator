using CefSharp;
using DominatorHouseCore;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using EmbeddedBrowser;
using LinkedDominatorCore.DetailedInfo;
using LinkedDominatorCore.LDLibrary;
using LinkedDominatorCore.LDModel;
using LinkedDominatorCore.Utility;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Security.Policy;
using System.Text;
using System.Text.RegularExpressions;
using Cookie = CefSharp.Cookie;

namespace LinkedDominatorCore.LDUtility
{
    public class LdDataHelper
    {
        private static readonly object Lock = new object();
        public static volatile LdDataHelper Instance;
        public ILdFunctions ldFunctions { get; set; }
        private JsonJArrayHandler jsonHandler = JsonJArrayHandler.GetInstance;
        public static LdDataHelper GetInstance
        {
            get
            {
                if (Instance == null)
                {
                    lock (Lock)
                    {
                        if (Instance == null)
                            Instance = new LdDataHelper();
                    }
                }
                return Instance;
            }
        }
        public string GetInstanceIdFromUserProfilePageSource(string userProfilePageSource)
        {
            return string.IsNullOrEmpty(userProfilePageSource)
                ? ""
                : Utils.GetBetween(userProfilePageSource, "clientPageInstanceId\" content=\"", "\"");
        }

        public string GetPublicInstanceFromProfileUrl(string profileUrl)
        {
            return string.IsNullOrEmpty(profileUrl)
                ? "":Utils.GetBetween(profileUrl + "**", "in/", "**").Replace("/", "");
        }
        public string GetProfileIdFromUserProfilePageSource(string userProfilePageSource)
        {
            var profileId = string.IsNullOrEmpty(userProfilePageSource)? "": Utils.GetBetween(userProfilePageSource, "profileId\":\"", "\"");
            profileId = string.IsNullOrEmpty(profileId) ? Utils.GetBetween(UserJsonDataFromProfileResponse(userProfilePageSource), "fs_miniProfile:","\""): profileId;
            profileId = string.IsNullOrEmpty(profileId) ? Utils.GetBetween(userProfilePageSource, "/miniprofiles/", "\""): profileId;
            profileId = string.IsNullOrEmpty(profileId) ? Utils.GetBetween(userProfilePageSource, "authorProfileId\":\"", "\",\"") : profileId;
            profileId = string.IsNullOrEmpty(profileId) ?Utils.GetBetween(userProfilePageSource, "fs_miniProfile:", "\""): profileId;
            return profileId;
        }
        public string GetProfileId(string PublicIdentifier)
        {
            var profileId = string.Empty;
            try
            {
                if (!string.IsNullOrEmpty(PublicIdentifier) && ldFunctions != null && jsonHandler!=null)
                {
                    var ProfileDetails = ldFunctions.GetInnerLdHttpHelper().GetRequest(LdConstants.GetLDUserDetailsAPI(PublicIdentifier)).Response;
                    var failedCount = 0;
                    while(failedCount++<2&&string.IsNullOrEmpty(ProfileDetails))
                        ProfileDetails = ldFunctions.GetInnerLdHttpHelper().GetRequest(LdConstants.GetLDUserDetailsAPI(PublicIdentifier)).Response;
                    var jsonObject = jsonHandler.ParseJsonToJObject(ProfileDetails);
                    profileId = jsonHandler.GetJTokenValue(jsonObject, "elements",0, "entityUrn")?.Replace("urn:li:fsd_profile:", "");
                }
            }catch(Exception ex){ex.DebugLog();}
            return profileId;
        }
        public string GetCompanyIdFromUserProfilePageSource(string companyUrl)
        {
            return string.IsNullOrEmpty(companyUrl) ? "" : companyUrl[companyUrl.Length - 1] == '/' ? Utils.GetBetween(companyUrl, "company/", "/") : Utils.GetBetween(companyUrl + "**", "company/", "**");
        }

        public string GetCsrfTokenFromCookies(IRequestParameters requestParameters)
        {
            return requestParameters.Cookies["JSESSIONID"]?.Value.Replace("\"", "").Replace("ajax:", "");
        }

        public string GetFullCsrfTokenFromCookies(IRequestParameters requestParameters)
        {
            return requestParameters.Cookies["JSESSIONID"]?.Value.Replace("\"", "");
        }

        public string GetbcookieFromCookies(IRequestParameters requestParameters)
        {
            return requestParameters.Cookies["bcookie"]?.Value.Replace("\"", "");
        }

        public string GetbscookieFromCookies(IRequestParameters requestParameters)
        {
            return requestParameters.Cookies["bscookie"]?.Value.Replace("\"", "");
        }

        public string GetlidcFromCookies(IRequestParameters requestParameters)
        {
            return requestParameters.Cookies["lidc"]?.Value.Replace("\"", "");
        }

        public string GetConversationIdFromConversationApi(string conversationApiResponse)
        {
            return string.IsNullOrEmpty(conversationApiResponse)
                ? ""
                : Utils.GetBetween(conversationApiResponse, "urn:li:fs_conversation:", "\"");
        }
        public string GetConversationId(LinkedinUser objLinkedinUser,ILdFunctions ldFunctions)
        {
            var ldDataHelper = GetInstance;
            var getConversationIdUrl = $"https://www.linkedin.com/voyager/api/messaging/conversations?keyVersion=LEGACY_INBOX&q=participants&recipients=List({objLinkedinUser.ProfileId})";
            var getConversationIdResponse =
                            ldFunctions.GetInnerLdHttpHelper().HandleGetResponse(getConversationIdUrl);
            var conversationId = ldDataHelper.GetConversationIdFromConversationApi(getConversationIdResponse.Response);
            return conversationId;
        }
        public string GetSalesNavUserApi(ILdFunctions ldFunctions, string desktopUrl)
        {
            var apiUrl = string.Empty;
            try
            {
                var responseParameter = ldFunctions.GetInnerHttpHelper().GetRequest(desktopUrl);
                var pageText = responseParameter.Response;
                var data = Utils.GetBetween(pageText, " {\"request\":\"/sales-api/salesApiPeopleSearch?q", "\",\"");
                var decodedUrl = Regex.Unescape(Regex.Replace(data, "\\\\([^u])", "\\\\$1"));
                if (!string.IsNullOrEmpty(decodedUrl))
                    apiUrl = $"https://www.linkedin.com/sales-api/salesApiPeopleSearch?q{decodedUrl}";
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            return apiUrl;
        }


        public string ActionUrl(string constructedActionUrl, string start)
        {
            return constructedActionUrl.Contains("count=")
                ? $"{constructedActionUrl.Replace("&start=0", $"&start={start}")}"
                : $"{constructedActionUrl}&count=40&start={start}";
        }

        public string DataReplacer(string inputData)
        {
            return string.IsNullOrEmpty(inputData)
                ? inputData
                : inputData.Replace(",", "").Replace("\n", "").Replace("\r", "").Replace("\t", "").Replace("\\n", "")
                    .Replace("\\r", "").Replace("\\t", "");
        }

        public void ReplaceAllPropertiesOfClass(UserScraperDetailedInfo detailedInfo)
        {
            try
            {
                var bindingFlags = BindingFlags.Public |
                                   BindingFlags.Instance |
                                   BindingFlags.Static;
                var allProp = detailedInfo.GetType().GetProperties(bindingFlags);
                foreach (var field in allProp)
                    try
                    {
                        var businessObjectPropValue = field.GetValue(detailedInfo, null);
                        if (!field.PropertyType.Name.Equals("String"))
                            continue;
                        field.SetValue(detailedInfo,
                            string.IsNullOrWhiteSpace(businessObjectPropValue.ToString())
                                ? Utils.AssignNa(businessObjectPropValue.ToString())
                                : DataReplacer(businessObjectPropValue.ToString()), null);
                    }
                    catch (Exception)
                    {
                    }
            }
            catch (Exception exception)
            {
                exception.DebugLog();
            }
        }

        public string GetPublicIdentifierFromSalesProfileUrl(ILdFunctions ldFunctions, string salesProfileUrl)
        {
            if (string.IsNullOrEmpty(salesProfileUrl) ? true : salesProfileUrl.Contains("sales/"))
                return string.IsNullOrEmpty(salesProfileUrl)?salesProfileUrl:salesProfileUrl.Contains("sales/people")?Utils.GetBetween(salesProfileUrl, "sales/people/", ","): Utils.GetBetween(salesProfileUrl, "sales/lead/", ",");
            var profilePageSource = ldFunctions.GetRequestUpdatedUserAgent(salesProfileUrl, true);
            return Utils.GetBetween(profilePageSource, "https://www.linkedin.com/in/", "\"");
        }
        public string GetAuthTokenFromSalesProfileUrl(string salesProfileUrl)
        {
            var authToken = Utils.GetBetween(salesProfileUrl, "NAME_SEARCH,", "?_ntb=");
            authToken = string.IsNullOrEmpty(authToken) ? Utils.GetBetween(salesProfileUrl + "**", "NAME_SEARCH,", "**") : authToken;
            authToken = string.IsNullOrEmpty(authToken) ? Utils.GetBetween(salesProfileUrl,",", ",NAME_SEARCH") : authToken;
            return authToken;
        }
        public bool IsSalesProfile(string ProfileUrl) => 
            string.IsNullOrEmpty(ProfileUrl) ? false : ProfileUrl.Contains("/sales")||ProfileUrl.Contains("/lead");
        public string GetPublicIdentifierFromProfileUrl(string ProfileUrl)
        {
            var publicIdentifier = Utils.GetBetween(ProfileUrl + "**", "in/", "**").Replace("/", "");
            if (string.IsNullOrEmpty(publicIdentifier) ? false : publicIdentifier.Contains("?miniProfileUrn"))
                publicIdentifier = Regex.Replace(publicIdentifier, "\\?miniProfileUrn(.*)", "");
            return publicIdentifier;
        }
        public void UpdateSalesUserProfileDetails(ILdFunctions ldFunctions,string authToken,ref LinkedinUser linkedinUser)
        {
            try
            {
                var ProfileUrl = LdConstants.GetSalesUserProfileAPI(linkedinUser.PublicIdentifier, authToken);
                var ProfilePageResponse=ldFunctions.TryAndGetResponse(ProfileUrl);
                ProfilePageResponse = Utilities.ValidateJsonString(ProfilePageResponse);
                var JsonObject=jsonHandler.ParseJsonToJObject(ProfilePageResponse);
                linkedinUser.AuthToken= string.IsNullOrEmpty(linkedinUser.AuthToken)?authToken:linkedinUser.AuthToken;
                linkedinUser.UserId = string.IsNullOrEmpty(linkedinUser.UserId) ? jsonHandler.GetJTokenValue(JsonObject, "objectUrn")?.Replace("urn:li:member:", "") :linkedinUser.UserId ;
                linkedinUser.NumberOfSharedConnections = string.IsNullOrEmpty(linkedinUser.NumberOfSharedConnections) ? jsonHandler.GetJTokenValue(JsonObject, "numOfConnections") : linkedinUser.NumberOfSharedConnections;
                linkedinUser.Firstname=string.IsNullOrEmpty(linkedinUser.Firstname)?jsonHandler.GetJTokenValue(JsonObject, "firstName") :linkedinUser.Firstname;
                linkedinUser.Lastname=string.IsNullOrEmpty(linkedinUser.Lastname)?jsonHandler.GetJTokenValue(JsonObject, "lastName") :linkedinUser.Lastname;
                linkedinUser.FullName=string.IsNullOrEmpty(linkedinUser.FullName)?jsonHandler.GetJTokenValue(JsonObject, "fullName") :linkedinUser.FullName;
                var linkedInProfile = jsonHandler.GetJTokenValue(JsonObject, "flagshipProfileUrl");
                if(!string.IsNullOrEmpty(linkedInProfile))
                    linkedinUser.PublicIdentifier = linkedInProfile?.Split('/')?.LastOrDefault(x=>x!=string.Empty);
            }
            catch (Exception exception) { exception.DebugLog(); }
        }
        public string GetApiData(string url, string getString)
        {
            var apiData = Utils.GetBetween(url, getString, "&");
            if (string.IsNullOrEmpty(apiData))
                apiData = Utils.GetBetween(url + "**", getString, "**");
            return apiData;
        }


        public string GetIdFromUrl(string url)=>Utils.GetBetweenLast(url.Trim('/') + "&&", "/", "&&").Trim('/');

        public void GetCompanyAddress(StringBuilder stringBuilder, int count,
            JsonJArrayHandler jArrayHandler,
            JToken jData)
        {
            try
            {
                stringBuilder.Append($"{count}) " + jArrayHandler.GetJTokenValue(jData, "line1").Trim());
                stringBuilder.Append(jArrayHandler.GetJTokenValue(jData, "city") + " ");
                stringBuilder.Append(jArrayHandler.GetJTokenValue(jData, "geographicArea") + " ");
                stringBuilder.Append(jArrayHandler.GetJTokenValue(jData, "postalCode") + " ");
                stringBuilder.Append(jArrayHandler.GetJTokenValue(jData, "country") + " ");
                stringBuilder.Append(jArrayHandler.GetJTokenValue(jData, "city") + "\n");
            }
            catch (Exception)
            {
                //
            }
        }

        public CookieCollection GetCookieCollectionFromEmbeddedBrowser(BrowserWindow browserWindow,
            out List<Cookie> lstCookies)
        {
            lstCookies = browserWindow.Browser.RequestContext.GetCookieManager(new TaskCompletionCallback())
                .VisitAllCookiesAsync().Result;

            var cookieCollection = new CookieCollection();

            foreach (var item in lstCookies)
                try
                {
                    var cookie = new System.Net.Cookie
                    {
                        Name = item.Name,
                        Value = item.Value,
                        Domain = item.Domain,
                        Path = item.Path,
                        Secure = item.Secure
                    };
                    if (item.Expires != null)
                        cookie.Expires = (DateTime)item.Expires;
                    cookieCollection.Add(cookie);
                }
                catch (Exception ex)
                {
                    ex.DebugLog(ex.StackTrace);
                }

            return cookieCollection;
        }

        public bool IsValidUrl(string input, string mustContain)
        {
            if (string.IsNullOrEmpty(input) || !input.Contains(mustContain))
                return false;
            return true;
        }

        public string GetUserPageResponse(string userPageResponse, LinkedinUser linkedinUser)
        {
            try
            {
                var jArrayHandler = JsonJArrayHandler.GetInstance;
                userPageResponse = WebUtility.HtmlDecode(userPageResponse);
                var jsonApiResponse = "{\"objectUrn\":\"urn:li:member:" +
                              Utils.GetBetween(userPageResponse, "{\"objectUrn\":\"urn:li:member:", "<").Trim();

                var jObject = jArrayHandler.ParseJsonToJObject(jsonApiResponse);
                return jsonApiResponse;
            }
            catch (Exception)
            {
                return userPageResponse;
            }
        }

        public string GetCompanyDetailsApi(string UniversalName)
        {
            return
                $"https://www.linkedin.com/voyager/api/graphql?includeWebMetadata=true&variables=(universalName:{UniversalName})&&queryId=voyagerOrganizationDashCompanies.63bc6f9c62b9e5543191a67a828f07b4";
        }

        public string GetSalesCompanyDetailsApi(string companyId)
        {
            return
                $"https://www.linkedin.com/sales-api/salesApiCompanies/{companyId}?decoration=%28entityUrn%2Cname%2Caccount%28saved%2CnoteCount%2ClistCount%2CcrmStatus%2Cstarred%29%2CpictureInfo%2CcompanyPictureDisplayImage%2Cdescription%2Cindustry%2Clocation%2Cheadquarters%2Cwebsite%2CrevenueRange%2CcrmOpportunities%2CflagshipCompanyUrl%2CemployeeGrowthPercentages%2Cemployees*~fs_salesProfile%28entityUrn%2CfirstName%2ClastName%2CfullName%2CpictureInfo%2CprofilePictureDisplayImage%29%2Cspecialties%2Ctype%2CyearFounded%29";
        }

        public string ApiForContactInfo(string profileId)
        {
            //return $"https://www.linkedin.com/voyager/api/identity/profiles/{profileId}/profileContactInfo";
            return $"https://www.linkedin.com/voyager/api/graphql?includeWebMetadata=true&variables=(memberIdentity:{profileId})&queryId=voyagerIdentityDashProfiles.c7452e58fa37646d09dae4920fc5b4b9";
        }

        public string Flagship3ProfileViewBase(string profilePageSource)
        {
            return Utils.GetBetween(profilePageSource, "urn:li:page:d_flagship3_profile_view_base;", "\n")?.Trim();
        }

        public string Flagship3GroupsEntity(string profilePageSource)
        {
            return Utils.GetBetween(profilePageSource, "urn:li:page:d_flagship3_groups_entity;", "\n")?.Trim();
        }

        public string PersonalDescription(string personalDescription)
        {
            var PersonalDescription = string.Empty;
            var jObject = jsonHandler.ParseJsonToJObject(personalDescription);
            if (jObject != null)
                PersonalDescription = jsonHandler.GetJTokenValue(jObject, "summary");
            return PersonalDescription;

        }
        public string GetFeedPreviewResponse(ILdFunctions _ldFunction,string FeedUrl)
        {
            return _ldFunction.GetInnerHttpHelper().GetRequest(LdConstants.GetPostPreviewAPI(FeedUrl)).Response;
        }
        public string GetPublicIdentifierFromPageSource(string pageSource)
        {
            var publicIdentifier = Utils.GetBetween(pageSource, "href=\"/in/", "/overlay/about-this-profile/");
            publicIdentifier =string.IsNullOrEmpty(publicIdentifier)? Utils.GetBetween(pageSource, "href=\"/in/", "/overlay/contact-info"):publicIdentifier;
            publicIdentifier = string.IsNullOrEmpty(publicIdentifier) ?Utils.GetBetween(pageSource, "href=\"https://www.linkedin.com/in/", "?miniProfileUrn=") : publicIdentifier;
            publicIdentifier = string.IsNullOrEmpty(publicIdentifier) ?Utils.GetBetween(pageSource, "href=\"https://www.linkedin.com/in/", "\" data-test-app-aware-link") : publicIdentifier;
            publicIdentifier = string.IsNullOrEmpty(publicIdentifier) ?Utils.GetBetween(pageSource,"<a href=\"/in/","\"") : publicIdentifier;
            publicIdentifier = Normalize(publicIdentifier);
            return WebUtility.UrlDecode(publicIdentifier?.Replace("/detail/contact-info/", "")?.Replace("/", ""));
        }

        private string Normalize(string publicIdentifier)
        {
            if (string.IsNullOrEmpty(publicIdentifier))
                return publicIdentifier;
            int index = publicIdentifier.IndexOf('/');
            if (index != -1)
                publicIdentifier = publicIdentifier.Substring(0, index);
            return publicIdentifier;
        }

        public string GetSalesUrlFromPageSource(string pageSource)
        {
            return Utils.GetBetween(pageSource, "href=\"/sales/", "\"");
        }

        public string GetSource(string pageSource)
        {
            var profilePicUrl = string.Empty;
            return
                !string.IsNullOrEmpty(profilePicUrl= Utils.GetBetween(pageSource, "<img src=\"", "\" loading="))?profilePicUrl:
                !string.IsNullOrEmpty(profilePicUrl=Utils.GetBetween(pageSource, "src=\"", "\" loading="))?profilePicUrl:string.Empty;
        }
        public string GetJsonDataFromPageSource(string pageSource, string startString, string endString)
        {
            return $"{startString}{Utils.GetBetween(pageSource, startString, endString)}";
        }


        public string GetInvitationIdFromPageSource(string pageSource)
        {
            return Utils.GetBetween(pageSource, "fs_relInvitationView:", "\"");
        }

        public string GetGroupIdFromGroupUrl(string groupUrl)
        {
            return Utils.GetBetween(groupUrl + "$$", "groups/", "$$")?.Replace("/", "");
        }

        public string GetPageIdFromPageUrl(string PageUrl)
        {
            return Utils.GetBetween(PageUrl + "$$", "/company/", "$$")?.Replace("/", "");
        }

        public string GetGroupIdFromPageSource(string pageSource)
        {
            return Utils.GetBetween(pageSource, "groups/", "\"")?.Replace("/", "");
        }

        public string GetAltFromPageSource(string pageSource)
        {
            return Utils.GetBetween(pageSource, "\n      \n        \n         ", "\n")?.Replace("/", "");
        }

        public string ConnectionType(string pageSource)
        {
            return Utils.RemoveHtmlTags(
                HtmlAgilityHelper.GetStringInnerTextFromClassName(pageSource, "distance-badge separator"));
        }

        public bool IsAlreadySendConnectionRequest(string pageSource)
        {
            return string.IsNullOrWhiteSpace(pageSource)
                ? false
                : pageSource.Contains("An invitation has been sent to");
        }


        public long GetTimeStamp(string pageSource)
        {
            try
            {
                var pattern = @"(\d+)";
                var match = Regex.Match(pageSource, pattern);

                if (match.Success)
                {
                    var matchedTime = 0;
                    int.TryParse(match.Groups[1].Value, out matchedTime);
                    var dateTime = DateTime.Now;
                    if (pageSource.Contains("day"))
                        dateTime = dateTime.AddDays(-matchedTime);
                    else if (pageSource.Contains("month"))
                        dateTime = dateTime.AddMonths(-matchedTime);
                    else if (pageSource.Contains("week"))
                        dateTime = dateTime.AddDays(-(matchedTime * 7));
                    else if (pageSource.Contains("hour"))
                        dateTime = dateTime.AddHours(-matchedTime);
                    else if (pageSource.Contains("minute"))
                        dateTime = dateTime.AddMinutes(-matchedTime);
                    else if (pageSource.Contains("seconds"))
                        dateTime = dateTime.AddSeconds(-matchedTime);

                    return dateTime.GetCurrentEpochTimeMilliSeconds();
                }
            }
            catch (Exception exception)
            {
                exception.DebugLog();
            }

            return 0;
        }

        /// <summary>
        ///     setting count in dominatorAccountModel instance property DisplayColumnValue1(ConnectionCount)
        /// </summary>
        /// <param name="dominatorAccountModel"></param>
        /// <param name="ldFunctions"></param>
        /// <returns></returns>
        public int GetAndSetConnectionCount(DominatorAccountModel dominatorAccountModel, ILdFunctions ldFunctions)
        {
            var connectionCount = 0;

            try
            {
                var viewConnectionsLink = dominatorAccountModel.IsRunProcessThroughBrowser
                    ? "https://www.linkedin.com/mynetwork/invite-connect/connections/"
                    : "https://www.linkedin.com/voyager/api/relationships/connectionsSummary/";
                var viewConnectionsPageSource = ldFunctions.GetRequestUpdatedUserAgent(viewConnectionsLink, true);

                if (dominatorAccountModel.IsRunProcessThroughBrowser)
                    int.TryParse(Utilities.GetIntegerOnlyString(Utils.RemoveHtmlTags(HtmlAgilityHelper
                        .GetStringInnerHtmlFromClassName(
                            viewConnectionsPageSource, "mn-connections__header")
                        ?.Replace("Connections", ""))), out connectionCount);
                else
                    int.TryParse(
                        Utilities.GetIntegerOnlyString(Utils.GetBetween(viewConnectionsPageSource, "numConnections\":",
                            "}")),
                        out connectionCount);

                dominatorAccountModel.DisplayColumnValue1 = connectionCount;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            return connectionCount;
        }


        /// <summary>
        ///     setting count in dominatorAccountModel instance property DisplayColumnValue3(SentConnectionCount)
        /// </summary>
        /// <param name="dominatorAccountModel"></param>
        /// <param name="ldFunctions"></param>
        /// <returns></returns>
        public int GetAndSetSentConnectionCount(DominatorAccountModel dominatorAccountModel, ILdFunctions ldFunctions)
        {
            var ConnectionCount = 0;
            if(dominatorAccountModel.IsRunProcessThroughBrowser)
            {
                var pageResponse = ldFunctions.GetRequestUpdatedUserAgent("https://www.linkedin.com/mynetwork/invitation-manager/sent/", true);
                var Nodes = HtmlAgilityHelper.GetListInnerHtmlOrInnerTextOrOuterHtmlFromIdOrClassName(pageResponse, "mn-invitation-manager__invitation-facet-pills", true, string.Empty, false);
                int.TryParse(Nodes.Count > 0 ?Regex.Replace(Nodes.FirstOrDefault(y=>y.Contains("People")),"[^0-9]",""):string.Empty, out ConnectionCount);
            }
            else
            {
                var jsonHandler = JsonJArrayHandler.GetInstance;
                var FetchTryCount = 0;
                Fetch:
                var SentInvitationUrl = $"https://www.linkedin.com/voyager/api/relationships/sentInvitationView?count=100&start={ConnectionCount}&type=SINGLES_ALL&q=sent";
                var Response = ldFunctions.GetHtmlFromUrlNormalMobileRequest(SentInvitationUrl);
                var jsonObject = jsonHandler.ParseJsonToJObject(Response);
                var Count = jsonHandler.GetJArrayElement(jsonHandler.GetJTokenValue(jsonObject, "elements")).Count;
                ConnectionCount += Count;
                while (Count > 0 && FetchTryCount++ <= 15)
                    goto Fetch;
            }
            dominatorAccountModel.DisplayColumnValue3 = ConnectionCount;
            return ConnectionCount;
        }

        public string UserJsonDataFromProfileResponse(string userProfileResponse)
        {
            return "{\"data\":{\"memorialized\":false,\"firstName\"" +
                   Utilities.GetBetween(userProfileResponse, "{\"data\":{\"memorialized\":false,\"firstName\"", "</code>");
        }

        public static bool IsMultiEqual(ActivityType ActivitiesComparer, params ActivityType[] ActivitiesComparing)
        {
            return ActivitiesComparing.Contains(ActivitiesComparer);
        }

        public static bool IsCompanyProcessor(ActivityType ActivityType)
        {
            return IsMultiEqual(ActivityType, ActivityType.CompanyScraper, ActivityType.SalesNavigatorCompanyScraper);
        }

        public static bool IsGroupProcessor(ActivityType ActivityType)
        {
            return IsMultiEqual(ActivityType, ActivityType.GroupJoiner, ActivityType.GroupUnJoiner);
        }

        public static bool IsPostProcessor(ActivityType ActivityType)
        {
            return IsMultiEqual(ActivityType, ActivityType.Like, ActivityType.Share, ActivityType.Comment);
        }

        public static bool IsPageProcessor(ActivityType ActivityType)
        {
            return IsMultiEqual(ActivityType, ActivityType.FollowPages);
        }
        public string GetDecodedResponse(string Response, bool htmlDecode = false, bool UrlDecode = false) => string.IsNullOrEmpty(Response) ? string.Empty : htmlDecode && UrlDecode ? WebUtility.UrlDecode(WebUtility.HtmlDecode(Response)) : htmlDecode ? WebUtility.HtmlDecode(Response) : WebUtility.UrlDecode(Response);
        public string ReplaceSpecialCharacter(string InputString)=>
            string.IsNullOrEmpty(InputString)?InputString:InputString?.Replace("\r\n", "\\n")?.Replace("\t", "\\t")?.Replace("\r","");
        public Tuple<bool,string> GetPostDataToSendMessage(ILdFunctions objLdFunctions, string profileId,
            string finalMessage, string imageSource, string memberId,string originToken,List<string> Medias=null)
        {
            try
            {
                var MediaIDs = objLdFunctions.GetMediaIDs(Medias);
                var postBody = string.Empty;
                finalMessage = finalMessage?.Replace("\r\n","\n")?.Replace("\n", "\\n");
                if (string.IsNullOrEmpty(memberId))
                {
                    postBody =
                        "{\"keyVersion\":\"LEGACY_INBOX\",\"conversationCreate\":{\"eventCreate\":{\"originToken\":\"" +
                        originToken +
                        "\",\"value\":{\"com.linkedin.voyager.messaging.create.MessageCreate\":{\"attributedBody\":{\"text\":\"" +
                        finalMessage?.Replace("\\","\\\\") + $"\",\"attributes\":[]}},\"attachments\":[{MediaIDs}]}}}}}},\"recipients\":[\"" + profileId + "\"],\"subtype\":\"MEMBER_TO_MEMBER\"}}";
                }
                else
                {
                    postBody =
                        "{\"keyVersion\":\"LEGACY_INBOX\",\"conversationCreate\":{\"eventCreate\":{\"originToken\":\"" +
                        originToken +
                        "\",\"value\":{\"com.linkedin.voyager.messaging.create.MessageCreate\":{\"body\":\"" +
                        finalMessage?.Replace("\\", "\\\\") + $"\",\"attachments\":[{MediaIDs}],\"attributedBody\":{{\"attributes\":[],\"text\":\"" + finalMessage?.Replace("\\", "\\\\")
                        + "\"},\"customContent\":{\"string\":\"urn:li:fs_miniGroup:" + memberId +
                        "\"}}}},\"recipients\":[\"" + profileId + "\"],\"subtype\":\"MEMBER_TO_GROUP_MEMBER\"}}";
                }
                return new Tuple<bool, string>(true, postBody);
                #region OLD Code for send message.
                //bool success;
                //#region PostData And PostDataResponse For Message Sending

                //#region PhotoUploading

                //var imageUploadUrl = LdConstants.GetLDMediaUploadAPI;
                //var objFileInfo = new FileInfo(imageSource);
                //var imageUploadResponse =
                //    objLdFunctions.UploadImageAndGetContentIdForMessaging(imageUploadUrl, objFileInfo);
                //if (imageUploadResponse == null || !imageUploadResponse.Contains("{\"value\":{\"urn\":\""))
                //    return (false, string.Empty);
                //success = true;
                //var mediaUrn = Utils.GetBetween(imageUploadResponse, "urn\":\"", "\"");
                //var singleUploadUrl = Utils.GetBetween(imageUploadResponse, "singleUploadUrl\":\"", "\"");

                //#endregion
                //var referer = "https://www.linkedin.com/messaging/compose/";
                //var singleUploadResponse =
                //    objLdFunctions.GetSingleUploadResponse(singleUploadUrl, objFileInfo, referer);
                //if (singleUploadResponse == null)
                //    return (success, string.Empty);
                //if (singleUploadResponse == string.Empty)
                //    success = true;
                //else
                //    return (success, string.Empty);
                //var contentType = Utils.GetMediaType(objFileInfo.Extension);
                //var randomId = Guid.NewGuid().ToString();
                //finalMessage = finalMessage.Replace("<:>", "").Replace(imageSource, "");
                //finalMessage = finalMessage.Replace("\r", "").Replace("\n", "\\n").Replace("\"", "\\\"");
                //var postData = "";

                //#region OLD Code for broadcast message with single images.
                //if (string.IsNullOrEmpty(memberId))
                //    postData =
                //        "{\"keyVersion\":\"LEGACY_INBOX\",\"conversationCreate\":{\"eventCreate\":{\"originToken\":\"" +
                //        originToken +
                //        "\",\"value\":{\"com.linkedin.voyager.messaging.create.MessageCreate\":{\"attributedBody\":{\"text\":\"" +
                //        finalMessage + "\",\"attributes\":[]},\"attachments\":[{\"id\":\"" + mediaUrn +
                //        "\",\"name\":\"" + objFileInfo.Name + "\",\"byteSize\":" + objFileInfo.Length +
                //        ",\"mediaType\":\"" + contentType +
                //        "\",\"reference\":{\"string\":\"blob:https://www.linkedin.com/" + randomId +
                //        "\"}}]}}},\"recipients\":[\"" + profileId + "\"],\"subtype\":\"MEMBER_TO_MEMBER\"}}";

                //else
                //    postData =
                //        "{\"keyVersion\":\"LEGACY_INBOX\",\"conversationCreate\":{\"eventCreate\":{\"originToken\":\"" +
                //        originToken +
                //        "\",\"value\":{\"com.linkedin.voyager.messaging.create.MessageCreate\":{\"body\":\"" +
                //        finalMessage + "\",\"attachments\":[{\"id\":\"" + mediaUrn + "\",\"originalId\":\"" + mediaUrn +
                //        "\",\"name\":\"" + objFileInfo.Name + "\",\"byteSize\":" + objFileInfo.Length +
                //        ",\"mediaType\":\"image/jpeg\",\"reference\":{\"string\":\"blob:https://www.linkedin.com/" +
                //        randomId + "\"}}],\"attributedBody\":{\"attributes\":[],\"text\":\"" + finalMessage
                //        + "\"},\"customContent\":{\"string\":\"urn:li:fs_miniGroup:" + memberId +
                //        "\"}}}},\"recipients\":[\"" + profileId + "\"],\"subtype\":\"MEMBER_TO_GROUP_MEMBER\"}}";
                //#endregion
                //return (success, postData);

                //#endregion
                #endregion
            }
            catch (OperationCanceledException)
            {
                throw new OperationCanceledException("Operation Cancelled!");
            }
            catch (Exception ex)
            {
                ex.DebugLog();
                return new Tuple<bool, string>(false,string.Empty);
            }
        }
    }
}