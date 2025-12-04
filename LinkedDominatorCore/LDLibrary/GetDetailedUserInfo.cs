using DominatorHouseCore;
using DominatorHouseCore.DatabaseHandler.LdTables.Account;
using DominatorHouseCore.Enums;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using DominatorHouseCore.Process;
using DominatorHouseCore.Utility;
using HtmlAgilityPack;
using LinkedDominatorCore.DetailedInfo;
using LinkedDominatorCore.LDLibrary.DAL;
using LinkedDominatorCore.LDModel;
using LinkedDominatorCore.LDUtility;
using LinkedDominatorCore.Utility;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using ThreadUtils;

namespace LinkedDominatorCore.LDLibrary
{
    public class GetDetailedUserInfo
    {
        private readonly IDelayService _delayService;
        private readonly LdDataHelper _ldDataHelper = LdDataHelper.GetInstance;
        private readonly QueryInfo _queryInfo;

        public GetDetailedUserInfo(IDelayService delayService)
        {
            _delayService = delayService;
        }

        public GetDetailedUserInfo(QueryInfo queryInfo, IDelayService delayService)
        {
            _queryInfo = queryInfo;
            _delayService = delayService;
        }


        /// <summary>
        /// </summary>
        /// <param name="profileUrl"></param>
        /// <param name="dominatorAccountModel"></param>
        /// <param name="objLdFunctions"></param>
        /// <returns></returns>
        public Tuple<bool, string> ScrapeProfileDetails(string profileUrl, DominatorAccountModel dominatorAccountModel,
            ILdFunctions objLdFunctions, bool isVisiting)
        {
            Tuple<bool, string> resultScrapeProfileDetails;
            try
            {
                dominatorAccountModel.Token.ThrowIfCancellationRequested();
                var ldDataHelper = LdDataHelper.GetInstance;
                var detailedInfo = new UserScraperDetailedInfo();
                var companyInfo = new CompanyInfo();
                var lastPartUrl = string.Empty;
                var flagship3ProfileViewBase = string.Empty;
                var thisResponseCompanyList = string.Empty;
                const bool isUserFilter = false;
                detailedInfo.username = profileUrl?.Split('/')?.LastOrDefault(x=>x!=string.Empty);
                // Unix timeStamp
                var unixTimestamp = DateTime.Now.GetCurrentEpochTime();

                // GetProfilePageSource
                var profilePageSource =
                    WebUtility.HtmlDecode(GetProfilePageSource(profileUrl, objLdFunctions));


                detailedInfo = GetFullNameDetailedInfo(profilePageSource, objLdFunctions, detailedInfo.username);
                if(string.IsNullOrEmpty(detailedInfo.ProfileId))
                    detailedInfo.ProfileId = ldDataHelper.GetProfileIdFromUserProfilePageSource(profilePageSource);
                detailedInfo.ProfileType = "public";
                detailedInfo.IsVisiting = isVisiting;

                #region FirstConnection Email Id

                try
                {
                    // dob and twitter details
                    GetPersonalDetails(profilePageSource, objLdFunctions, detailedInfo);
                    if (!string.IsNullOrEmpty(detailedInfo.TwitterUserName))
                        detailedInfo.TwitterUrl = $"https://twitter.com/{detailedInfo.TwitterUserName}";
                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                }

                #endregion

                #region LDS_PastTitles

                try
                {
                    companyInfo = GetPastTitles(isUserFilter, profilePageSource, objLdFunctions,
                        detailedInfo.ProfileId, detailedInfo.TitleCurrent);
                    detailedInfo.PastTitles = companyInfo.PastTitles;
                    thisResponseCompanyList = companyInfo.CompanyResponse;
                    detailedInfo.Experience = companyInfo.Experience;
                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                }

                #endregion

                //GetPastCompanyWithOtherDetails(profilePageSource, objLdFunctions, unixTimestamp, detailedInfo);
                MapCurrentAndPastCompany(companyInfo, detailedInfo);

                #region HeadlineTitle

                if (string.IsNullOrEmpty(detailedInfo.HeadlineTitle))
                    detailedInfo.HeadlineTitle = Utils.GetBetween(profilePageSource, "\"headline\":\"", "\"");
                detailedInfo.HeadlineTitle = string.IsNullOrEmpty(detailedInfo.HeadlineTitle) ?Utils.RemoveHtmlTags(Utils.GetBetween(profilePageSource, "<div class=\"text-body-medium break-words\"", "</div>")): detailedInfo.HeadlineTitle;
                #endregion

                #region Current Company Details

                if (!string.IsNullOrEmpty(detailedInfo.CurrentCompanyUrl) &&
                    !detailedInfo.CurrentCompanyUrl.Contains("N/A"))
                {
                    try
                    {
                        var companyDetails = new CompanyScraperDetailedInfo();
                        var universalName = detailedInfo.CurrentCompanyUrl?.Split('/').Last(x => x.ToString() != string.Empty);
                        ScrapeCompanyDetails(objLdFunctions, detailedInfo.CurrentCompanyUrl, ref companyDetails,
                            dominatorAccountModel,universalName);
                        //ScrapeCompanyDetails(detailedInfo.CurrentCompanyUrl, ldFunctions, dominatorAccountModel);

                        #region Current Company Website

                        if (companyDetails != null)
                        {
                            detailedInfo.CurrentCompanyWebsite = companyDetails.Website;
                            detailedInfo.CompanyDescription = companyDetails.CompanyDescription;
                            detailedInfo.CompanyLocation = companyDetails.Headquarter;
                            detailedInfo.Industry = companyDetails.Industry;
                        }
                    }
                    catch (Exception ex)
                    {
                        ex.DebugLog();
                    }

                    #endregion
                }

                #endregion

                detailedInfo.EducationCollection = GetEducationCollection(isUserFilter, profilePageSource,
                    objLdFunctions, detailedInfo.ProfileId);

                detailedInfo.Recommendation = GetRecommendation(profilePageSource, objLdFunctions,
                    flagship3ProfileViewBase,detailedInfo.ProfileId, detailedInfo.PublicIdentifier);

                #region Total Connections And Other Count Details

                try
                {
                    // Connection
                    detailedInfo.Connection = Utils.GetBetween(profilePageSource, "\"connectionsCount\":", ",");
                    if (string.IsNullOrEmpty(detailedInfo.Connection))
                    {
                        //detailedInfo.Connection = Utils.GetBetween(profilePageSource, "<span class=\"t-bold\">", "</span> connections");
                        detailedInfo.Connection = Utils.GetBetween(profilePageSource, "<ul class=\"pv-top-card--list pv-top-card--list-bullet\">", "</ul>");
                    }
                    if(!objLdFunctions.IsBrowser && string.IsNullOrEmpty(detailedInfo.Connection))
                    {
                        var ConnectionAndFollower = GetConnectionAndFollower(objLdFunctions, detailedInfo.PublicIdentifier);
                        var Array = Regex.Split(ConnectionAndFollower, "<:>");
                        long.TryParse(Array.First(), out long Connection);
                        var follower = Array.Length > 2 ? !string.IsNullOrEmpty(Array[1])?Array[1]:"0": "0";
                        detailedInfo.Connection = $"[Connection : {Connection} ][Total Followers : {follower} ] and [Degree Connection : {Array.Last()} ]";
                        goto GetSkills;
                    }
                    if (!string.IsNullOrEmpty(detailedInfo.Connection) && detailedInfo.Connection.Contains("followers") && detailedInfo.Connection.Contains("connections"))
                    {
                        detailedInfo.Connection = Utils.GetBetween(detailedInfo.Connection, "<li class=\"text-body-small\">", "connections");
                        detailedInfo.Connection = Utils.GetBetween(detailedInfo.Connection, "<span class=\"t-bold\">", "</span>");
                    }
                    else
                        detailedInfo.Connection = Utils.GetBetween(profilePageSource, "<span class=\"t-bold\">", "</span> connections");

                    if (!string.IsNullOrEmpty(detailedInfo.Connection))
                    {
                        if (detailedInfo.Connection == "500+")
                            detailedInfo.Connection = "500 Plus";
                        detailedInfo.Connection = "[Connection : " + detailedInfo.Connection + "]";
                    } 
                    detailedInfo.Connection += "[Total Followers : " +StringHelper.GetRegexPatern(@"<span aria-hidden=""true""><!---->{1}(.*) followers<!----></span><span class=""visually-hidden"">", profilePageSource).Replace("<span aria-hidden=\"true\"><!---->", "").Replace("followers<!----></span><span class=\"visually-hidden\">", "")+ "]";
                    var followingFirstSecondOrHigher = Utils.GetBetween(profilePageSource,
                        "\"$type\":\"com.linkedin.voyager.common.FollowingInfo\"},{\"$deletedFields\":[],\"value\":\"DISTANCE_",
                        "\"");
                    if (!string.IsNullOrEmpty(followingFirstSecondOrHigher))
                        detailedInfo.Connection += " and [Degree Connection : " + followingFirstSecondOrHigher + "]";

                    //distance-badge separator
                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                }

                #endregion
                GetSkills:
                detailedInfo.Skill = GetSkills(isUserFilter, profilePageSource, objLdFunctions,!string.IsNullOrEmpty(detailedInfo.ProfileId)? detailedInfo.ProfileId:detailedInfo.PublicIdentifier);

                // Experience
                //detailedInfo.Experience = GetExperience(isUserFilter, thisResponseCompanyList,
                //    detailedInfo.CompanyCurrent, detailedInfo.TitleCurrent);


                #region Replace firstname with Linkedin Member if Empty

                if (string.IsNullOrEmpty(detailedInfo.Firstname))
                    detailedInfo.Firstname = "Linkedin Member";

                #endregion

                // Replace Comma with null

                ldDataHelper.ReplaceAllPropertiesOfClass(detailedInfo);

                // Account Info
                detailedInfo.AccountEmail = dominatorAccountModel.AccountBaseModel.UserName;
                detailedInfo.AccountUserFullName = dominatorAccountModel.AccountBaseModel.UserFullName;
                detailedInfo.AccountUserProfileUrl = dominatorAccountModel.AccountBaseModel.ProfilePictureUrl;


                #region Replacing IF empty with N/A

                try
                {
                    if (detailedInfo.Firstname == string.Empty) detailedInfo.Firstname = "LinkedIn";
                    if (string.IsNullOrEmpty(detailedInfo.Lastname)) detailedInfo.Lastname = "Member";
                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                }

                #endregion

                var detailedUserInfoJsonString = JsonConvert.SerializeObject(detailedInfo);
                if (detailedInfo.Firstname.Equals("Linkedin Member"))
                    return resultScrapeProfileDetails = new Tuple<bool, string>(false, detailedUserInfoJsonString);
                return resultScrapeProfileDetails = new Tuple<bool, string>(true, detailedUserInfoJsonString);
            }
            catch (OperationCanceledException)
            {
                throw new OperationCanceledException("Operation Cancelled!");
            }
            catch (Exception ex)
            {
                resultScrapeProfileDetails = new Tuple<bool, string>(false, null);
                ex.DebugLog();
            }

            return resultScrapeProfileDetails;
        }

        private void MapCurrentAndPastCompany(CompanyInfo companyInfo, UserScraperDetailedInfo detailedInfo)
        {
            var company = companyInfo?.CurrentAndPastCompanyInfo;
            if(company != null)
            {
                detailedInfo.CompanyCurrent = company.CurrentCompany;
                detailedInfo.CurrentCompanyUrl = company.CurrentCompanyUrl;
                detailedInfo.PastCompany = company.PastCompany;
                detailedInfo.TitleCurrent = company.TitleCurrent;
                detailedInfo.CurrentCompanyWebsite = company.CurrentCompanyWebsite;
            }
        }

        public string GetProfilePageSource(string profileUrl, ILdFunctions objLdFunctions)
        {
            var profilePageSource = string.Empty;
            try
            {
                profilePageSource =
                    _queryInfo?.QueryType != null && _queryInfo.QueryType.Equals("Joined Group Url") &&
                    objLdFunctions.IsBrowser
                        ? objLdFunctions.GetSecondaryBrowserResponse(profileUrl, 5000)
                        : objLdFunctions.GetHtmlFromUrlForMobileRequest(profileUrl,string.Empty /*"LinkedIn Corporation ©"*/);

                if (string.IsNullOrEmpty(profilePageSource))
                {
                    _delayService.ThreadSleep(2000);
                    profilePageSource = objLdFunctions.GetHtmlFromUrlForMobileRequest(profileUrl, "");
                }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            return profilePageSource;
        }
        private string GetConnectionAndFollower(ILdFunctions ldFunctions, string publicidentifier)
        {
            string connectioncount = string.Empty;
            try
            {
                var jsonHandler = JsonJArrayHandler.GetInstance;
                var connectioncountresponse = ldFunctions.GetHtmlFromUrlNormalMobileRequest(LdConstants.GetLDUserDetailsAPI(publicidentifier));
                var jObject = jsonHandler.ParseJsonToJObject(connectioncountresponse);
                connectioncount = jsonHandler.GetJTokenValue(jObject, "elements", 0, "connections", "paging", "total");
                connectioncount += "<:>" + jsonHandler.GetJTokenValue(jObject, "elements", 0, "followingState", "followerCount");
                connectioncount += "<:>" + Utils.GetBetween(connectioncountresponse, "\"memberDistance\":\"", "\"")?.Replace("DISTANCE_", "");
            }
            catch (Exception)
            {
            }
            return connectioncount;
        }
        public string GetPersonalDetailsForExportConnection(string profilePageSource, ILdFunctions objLdFunctions,
            string profileId)
        {
            #region Variable Declaration Required For This Method

            var firstConnectionPersonalDetails = string.Empty;
            var personalWebsites = string.Empty;
            var emailId = string.Empty;
            var personalPhoneNumber = string.Empty;
            var twitterUserName = string.Empty;

            var birthdate = string.Empty;

            #endregion

            try
            {
                var flagship3ProfileViewBase = _ldDataHelper.Flagship3ProfileViewBase(profilePageSource);
                var apiForContactInfo = objLdFunctions.IsBrowser
                    ? $"https://www.linkedin.com/in/{_ldDataHelper.GetPublicInstanceFromProfileUrl(profilePageSource)}/detail/contact-info/"
                    : _ldDataHelper.ApiForContactInfo(profileId);

                var contactInfoResponse =
                    objLdFunctions.GetHtmlFromUrlForMobileRequest(apiForContactInfo, flagship3ProfileViewBase);
                var jsonJArrayHandler = JsonJArrayHandler.GetInstance;
                var jobject = jsonJArrayHandler.ParseJsonToJObject(contactInfoResponse);
                if (jobject != null && jobject.Children().Contains("data"))
                    jobject = jsonJArrayHandler.ParseJsonToJObject(jsonJArrayHandler.GetJTokenValue(jobject, "data"));

                if (!string.IsNullOrEmpty(contactInfoResponse))
                    try
                    {
                        var arr = Regex.Split(contactInfoResponse, "deletedFields\":");

                        foreach (var item in arr)
                            if (item.Contains(".TwitterHandle") &&
                                string.IsNullOrEmpty(twitterUserName = Utils.GetBetween(item, "name\":\"", "\",\"")))
                                twitterUserName =
                                    jsonJArrayHandler.GetJTokenValue(jobject, "twitterHandles", 0, "name");

                            else if (item.Contains("twitterHandles\":[{\"name\":\""))
                                twitterUserName = Utils.GetBetween(item, "twitterHandles\":[{\"name\":\"", "\"");


                        if (string.IsNullOrEmpty(emailId =
                            Utils.GetBetween(contactInfoResponse, "emailAddress\":\"", "\",\"")))
                            emailId = jsonJArrayHandler.GetJTokenValue(jobject, "emailAddress");

                        if (string.IsNullOrEmpty(personalPhoneNumber =
                            Utils.GetBetween(contactInfoResponse, "\"number\":\"", "\",\"")))
                            personalPhoneNumber =
                                jsonJArrayHandler.GetJTokenValue(jobject, "phoneNumbers", 0, "number");
                        if (personalPhoneNumber.Contains("\"}]"))
                            personalPhoneNumber = Utils.GetBetween(personalPhoneNumber, "", "\"}]");
                        var arrayWebsites = Regex.Split(contactInfoResponse, "url\":\"").Skip(1).ToArray();
                        if (arrayWebsites.Count() != 0)
                            personalWebsites = GetWebSites(arrayWebsites, personalWebsites, jsonJArrayHandler, jobject);

                        var day = Utils.GetBetween(contactInfoResponse, "day\":", ",");

                        string month;
                        if (!string.IsNullOrEmpty(month = Utils.GetBetween(contactInfoResponse, "month\":", ",")))
                            month = Utils.GetMonth(month);
                        Utils.GetBetween(contactInfoResponse, "year\":", ",");
                        birthdate = month + " " + day;
                    }
                    catch (Exception ex)
                    {
                        ex.DebugLog();
                    }

                firstConnectionPersonalDetails = flagship3ProfileViewBase + "<:>" + emailId + "<:>" +
                                                 personalPhoneNumber + "<:>" + personalWebsites + "<:>" + birthdate +
                                                 "<:>" + twitterUserName;
            }
            catch (Exception ex)
            {
                Console.Write(ex);
            }

            return firstConnectionPersonalDetails;
        }

        private static string GetWebSites(string[] arrayWebsites, string personalWebsites,
            JsonJArrayHandler jsonJArrayHandler, JObject jobject)
        {
            foreach (var item in arrayWebsites)
            {
                var contactUrl = Utils.GetBetween("**" + item, "**", "\"");
                personalWebsites = personalWebsites + "[" + contactUrl + "] ";
            }

            personalWebsites = personalWebsites.Replace("[]", "").Replace("[ ]", "");
            if (string.IsNullOrEmpty(personalWebsites?.Trim()))
            {
                //GetWebSites
                var websites = jsonJArrayHandler.GetTokenElement(jobject, "websites");
                foreach (var website in websites)
                {
                    var contactUrl = jsonJArrayHandler.GetJTokenValue(website, "url");
                    personalWebsites = personalWebsites + "[" + contactUrl + "] ";
                }
            }

            return personalWebsites;
        }

        public void GetPersonalDetails(string profilePageSource, ILdFunctions ldFunctions,
            UserScraperDetailedInfo detailedInfo)
        {
            try
            {
                detailedInfo.Flagship3ProfileViewBase = _ldDataHelper.Flagship3ProfileViewBase(profilePageSource);

                if (ldFunctions.BrowserWindow != null)
                {
                    var automationExtension = new BrowserAutomationExtension(ldFunctions.BrowserWindow);
                    automationExtension.ExecuteScript(string.Format(LDClassesConstant.ScriptConstant.ScrollWindowByXXPixel,0,0));
                    automationExtension.ExecuteScript(string.Format(LDClassesConstant.ScriptConstant.ScrollWindowToXXPixel, 0, 0));
                    var isSuccess =
                        automationExtension.LoadAndClick("a", AttributeIdentifierType.Id, "ember44");

                    if (!isSuccess)
                        automationExtension.ExecuteScript(AttributeIdentifierType.Xpath, $"//{HTMLTags.Anchor}[text()='Contact info']");

                    _delayService.ThreadSleep(5000);
                    profilePageSource = ldFunctions.BrowserWindow.GetPageSource();
                    automationExtension.LoadAndClick(HTMLTags.Button, AttributeIdentifierType.Id, "Dismiss");

                    //pv-contact-info__ci-container
                    var htmlDoc = new HtmlDocument();
                    htmlDoc.LoadHtml(WebUtility.HtmlDecode(profilePageSource));

                    //pv-contact-info__contact-icon
                    //pv-contact-info__ci-container
                    var userDetails =
                        HtmlAgilityHelper.GetListNodesFromClassName("", "pv-contact-info__contact-type", htmlDoc);
                    foreach (var userNode in userDetails)
                        if (userNode.OuterHtml.Contains("mailto:"))
                        {
                            detailedInfo.EmailId = Utils.RemoveHtmlTags(userNode.InnerHtml)?.Replace("Email", "")
                                ?.Replace("\n", "")?.Trim();
                        }
                        else if (userNode.OuterHtml.Contains("website"))
                        {
                            detailedInfo.PersonalWebsites = Utils.RemoveHtmlTags(userNode.InnerHtml)
                                ?.Replace("Website", "")?.Replace("\n", "")?.Trim();
                        }
                        else if (userNode.OuterHtml.Contains("Phone"))
                        {
                            detailedInfo.PersonalPhoneNumber =
                                Regex.Replace(Utils.RemoveHtmlTags(userNode.InnerHtml), "[^0-9]", "");
                        }
                        else if (userNode.OuterHtml.Contains("twitter"))
                        {
                            detailedInfo.TwitterUserName = Utils.RemoveHtmlTags(userNode.InnerHtml)
                                ?.Replace("Twitter", "")?.Replace("\n", "")?.Trim();
                        }
                        else if (userNode.OuterHtml.Contains("Birthday"))
                        {
                            detailedInfo.Birthdate = Utils.RemoveHtmlTags(userNode.InnerHtml)?.Replace("Birthday", "")
                                ?.Replace("\n", "")?.Trim();
                        }
                        else if (userNode.OuterHtml.Contains("Connected"))
                        {
                            detailedInfo.ConnectedTime = Utils.RemoveHtmlTags(userNode.InnerHtml)
                                ?.Replace("Connected", "")?.Replace("\n", "")?.Trim();

                            DateTime Date;
                            DateTime.TryParse(detailedInfo.ConnectedTime, out Date);
                            detailedInfo.ConnectedTime = Date.ConvertToEpoch().ToString();
                            detailedInfo.ConnectedTime = double.Parse(detailedInfo.ConnectedTime).EpochToDateTimeUtc()
                                .ToLocalTime().ToString(CultureInfo.InvariantCulture);
                        }


                    return;
                }

                var apiForContactInfo = _ldDataHelper.ApiForContactInfo(detailedInfo.ProfileId);
                var contactInfoResponse = ldFunctions.BrowserWindow == null
                    ? ldFunctions.GetHtmlFromUrlForMobileRequest(apiForContactInfo,
                        detailedInfo.Flagship3ProfileViewBase)
                    : profilePageSource;

                if (!string.IsNullOrEmpty(contactInfoResponse))
                {
                    var handler = JsonJArrayHandler.GetInstance;
                    var obj = handler.ParseJsonToJObject(contactInfoResponse);
                    var token = handler.GetJTokenOfJToken(obj, "data", "identityDashProfilesByMemberIdentity", "elements",0);
                    detailedInfo.TwitterUserName = handler.GetJTokenValue(token, "twitterHandles",0,"name");
                    detailedInfo.EmailId = handler.GetJTokenValue(token, "emailAddress", "emailAddress");
                    var phoneNumber = handler.GetJTokenValue(token, "phoneNumbers", 0, "number");
                    phoneNumber = string.IsNullOrEmpty(phoneNumber) ? handler.GetJTokenValue(token, "phoneNumbers", 0, "phoneNumber", "number") : phoneNumber;
                    detailedInfo.PersonalPhoneNumber = phoneNumber;
                    var arrayWebsites = handler.GetJArrayElement(handler.GetJTokenValue(token, "websites"));
                    if(arrayWebsites != null && arrayWebsites.HasValues)
                    {
                        foreach (var item in arrayWebsites)
                        {
                            var contactUrl = handler.GetJTokenValue(item, "url");
                            detailedInfo.PersonalWebsites += "[" + contactUrl + "] ";
                        }
                    }
                    detailedInfo.PersonalWebsites = detailedInfo.PersonalWebsites.Replace("[]", "");
                    var birthToken = handler.GetJTokenOfJToken(token, "birthDateOn");
                    var day = handler.GetJTokenValue(birthToken, "day");
                    day = day.Contains("}") ? day.Replace("}", "") : day;
                    string month;
                    if (!string.IsNullOrEmpty(month = handler.GetJTokenValue(birthToken,"month")))
                        month = Utils.GetMonth(month);
                    detailedInfo.Birthdate = month + " " + day;
                    long.TryParse(handler.GetJTokenValue(token, "memberRelationship", "memberRelationship", "connection", "createdAt"),out long Timespan);
                    if(Timespan > 0)
                    {
                        var origin = new DateTime(1970, 1, 1, 0, 0, 0, 0);
                        detailedInfo.ConnectedTime = origin.AddMilliseconds(Timespan).ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.Write(ex);
            }
        }


        /// <summary>
        ///     Get bio details if not get from pageSource
        /// </summary>
        /// <param name="profilePageSource"></param>
        /// <param name="objLdFunctions"></param>
        /// <param name="profileId"></param>
        /// <returns></returns>
        public string GetPersonalDescription(string profilePageSource, ILdFunctions objLdFunctions, string profileId, out string ProfilePicUrl,out string LinkedInProfileId)
        {
            ProfilePicUrl = string.Empty;
            LinkedInProfileId = string.Empty;
            try
            {
                //var flagship3ProfileViewBase =_ldDataHelper.Flagship3ProfileViewBase(profilePageSource); // Utils.GetBetween(profilePageSource, "urn:li:page:d_flagship3_profile_view_base;", "\n");
                var apiForContactInfo = $"https://www.linkedin.com/voyager/api/identity/profiles/{profileId}";
                var failedCount = 0;
                var personalDescription =objLdFunctions.GetInnerLdHttpHelper().GetRequest(apiForContactInfo).Response;
                while (failedCount++ < 2 && string.IsNullOrEmpty(personalDescription))
                {
                    _delayService.ThreadSleep(RandomUtilties.GetRandomNumber(2500, 1500));
                    personalDescription = objLdFunctions.GetInnerLdHttpHelper().GetRequest(apiForContactInfo).Response;
                }
                var summary = _ldDataHelper.PersonalDescription(personalDescription);
                var jsonHandler = JsonJArrayHandler.GetInstance;
                var jsonObject = jsonHandler.ParseJsonToJObject(personalDescription);
                var Details = jsonHandler.GetJTokenOfJToken(jsonObject, "miniProfile", "picture", "com.linkedin.common.VectorImage");
                ProfilePicUrl = jsonHandler.GetJTokenValue(Details, "rootUrl")+jsonHandler.GetJTokenValue(jsonHandler.GetJArrayElement(jsonHandler.GetJTokenValue(Details, "artifacts"))?.LastOrDefault(x=>x.ToString().Contains("\"width\": 800")|| x.ToString().Contains("\"width\": 400")|| x.ToString().Contains("\"width\": 200")|| x.ToString().Contains("\"width\": 100")), "fileIdentifyingUrlPathSegment");
                LinkedInProfileId = jsonHandler.GetJTokenValue(jsonObject, "miniProfile", "publicIdentifier");
                return summary;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
                return string.Empty;
            }
        }


        public void GetPastCompanyWithOtherDetails(string profilePageSource, ILdFunctions objLdFunctions,
            int unixTimestamp, UserScraperDetailedInfo detailedInfo)
        {
            try
            {
                var jArrayHandler = JsonJArrayHandler.GetInstance;
                var pastCompany = string.Empty;
                var companyCurrent = string.Empty;
                var thisResponseCompanyList = string.Empty;
                try
                {
                    detailedInfo.Flagship3ProfileViewBase = _ldDataHelper.Flagship3ProfileViewBase(profilePageSource);
                    var actionUrl =
                        $"https://www.linkedin.com/voyager/api/identity/profiles/{detailedInfo.ProfileId}/positions?count=100&start=0";
                    if (objLdFunctions.BrowserWindow != null)
                        thisResponseCompanyList =objLdFunctions.GetInnerHttpHelper().GetRequest(actionUrl).Response;
                    else
                        thisResponseCompanyList =objLdFunctions.GetHtmlFromUrlForMobileRequest(actionUrl,detailedInfo.Flagship3ProfileViewBase);
                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                }

                var objJObject = jArrayHandler.ParseJsonToJObject(thisResponseCompanyList);
                var elementsArray = jArrayHandler.GetJTokenOfJToken(objJObject, "elements") ?? jArrayHandler.GetJTokenOfJToken(objJObject, "included");
                var count = 0;
                foreach (var item in elementsArray)
                {
                    if (!item.ToString().Contains("companyName"))
                        continue;
                    ++count;

                    #region startDate and endDate

                    //var startYear = item["timePeriod"]["startDate"]["year"].ToString();
                    var startDate = jArrayHandler.GetJTokenValue(item, "timePeriod", "startDate", "year");
                    var startMonth = jArrayHandler.GetJTokenValue(item, "timePeriod", "startDate", "month");
                    //startDate = !string.IsNullOrEmpty(startDate) ? startDate + ":" + startMonth : startMonth;
                    startDate = !string.IsNullOrEmpty(startDate)
                        ? $"{Utils.GetMonth(startMonth)} {startDate}"
                        : startMonth;

                    // endYear = item["timePeriod"]["endDate"]["year"].ToString();
                    var endDate = jArrayHandler.GetJTokenValue(item, "timePeriod", "endDate", "year");
                    if (string.IsNullOrEmpty(endDate))
                    {
                        endDate = "Present";
                    }
                    else
                    {
                        var endMonth = jArrayHandler.GetJTokenValue(item, "timePeriod", "endDate", "month");
                        if (!string.IsNullOrEmpty(endMonth))
                            endDate = $"{Utils.GetMonth(endMonth)} {endDate}";
                    }

                    #endregion

                    if ((string.IsNullOrEmpty(endDate) || count == 1) && string.IsNullOrEmpty(companyCurrent))
                    {
                        companyCurrent = jArrayHandler.GetJTokenValue(item, "companyName");
                        detailedInfo.TitleCurrent = jArrayHandler.GetJTokenValue(item, "title");

                        #region CurrentCompanyId

                        try
                        {
                            var currentCompanyId = jArrayHandler.GetJTokenValue(item, "company","miniCompany","objectUrn")?.Split(':')?.LastOrDefault();
                            var username= jArrayHandler.GetJTokenValue(item, "company", "miniCompany", "universalName");
                            detailedInfo.CurrentCompanyUrl = "https://www.linkedin.com/company/" + (string.IsNullOrEmpty(username)?currentCompanyId:username);
                        }
                        catch (Exception ex)
                        {
                            ex.DebugLog();
                        }

                        if (string.IsNullOrEmpty(detailedInfo.CurrentCompanyUrl))
                            if (!string.IsNullOrEmpty(detailedInfo.CurrentCompanyUrl =jArrayHandler.GetJTokenValue(item,"companyUrn")))
                                detailedInfo.CurrentCompanyUrl =
                                    "https://www.linkedin.com/company/" +
                                    detailedInfo.CurrentCompanyUrl.Split(':').Last();

                        #endregion

                        detailedInfo.CompanyCurrent = $"{companyCurrent} ({startDate} - {endDate} )";
                    }
                    else
                    {
                        var company =jArrayHandler.GetJTokenValue(item,"companyName");
                        company = $"{company} ({startDate} - {endDate} )";
                        pastCompany = string.IsNullOrEmpty(pastCompany) ? company : pastCompany + " : " + company;
                    }
                    if (string.IsNullOrEmpty(detailedInfo.CurrentCompanyUrl))
                        detailedInfo.CurrentCompanyUrl = "https://www.linkedin.com/company/" + companyCurrent?.ToLower();
                }

                _delayService.ThreadSleep(5000);
                detailedInfo.PastCompany = pastCompany.Trim(':');
                // pastCompanyWithOtherDetails = pastCompany + "<:>" + companyCurrent + "<:>" + currentCompanyId + "<:>" + titleCurrent;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        public string GetEducationCollection(bool isUserFilter, string profilePageSource, ILdFunctions objLdFunctions,
            string profileId)
        {
            var educationCollection = string.Empty;

            var thisResponseEducationList = string.Empty;
            var strUniversityName = new string[] { };
            if (objLdFunctions.IsBrowser && !profilePageSource.Contains("Education"))
                return educationCollection;
            var educationList = new List<string>();
            try
            {
                var flagship3ProfileViewBase = _ldDataHelper.Flagship3ProfileViewBase(profilePageSource);
                //var actionUrl = $"https://www.linkedin.com/voyager/api/identity/profiles/{profileId}/educations?count=10&start=0";
                var actionUrl = $"https://www.linkedin.com/voyager/api/graphql?includeWebMetadata=true&variables=(vanityName:{profileId})&queryId=voyagerIdentityDashProfiles.a1a483e719b20537a256b6853cdca711";
                objLdFunctions.GetInnerHttpHelper().GetRequestParameter().Accept = LdConstants.AcceptApplicationOrJson;
                thisResponseEducationList =  objLdFunctions.GetInnerHttpHelper().GetRequest(actionUrl).Response;

                if (isUserFilter)
                {
                    var jsonHandler = JsonJArrayHandler.GetInstance;
                    var JObject = jsonHandler.ParseJsonToJObject(thisResponseEducationList);
                    var totalCountEducation = jsonHandler.GetJArrayElement(jsonHandler.GetJTokenValue(JObject, "data", "identityDashProfilesByMemberIdentity", "elements",0, "profileTopEducation", "elements")).Count.ToString();
                    return totalCountEducation;
                }
                //var actionUrlNext = "https://www.linkedin.com/voyager/api/identity/profiles/" + profileId +
                //                    "/educations?count=" + 10 + "&start=0";
                //thisResponseEducationList = objLdFunctions.IsBrowser
                //    ? objLdFunctions.GetInnerHttpHelper().GetRequest(actionUrlNext).Response
                //    : objLdFunctions.GetHtmlFromUrlForMobileRequest(actionUrlNext, flagship3ProfileViewBase);
                
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            try
            {
                strUniversityName = string.IsNullOrEmpty(thisResponseEducationList)
                    ? Regex.Split(profilePageSource, "entityUrn\":\"urn:li:fs_education:").Skip(1).ToArray()
                    : Regex.Split(thisResponseEducationList, "entityUrn\":\"urn:li:fs_education:").Skip(1).ToArray();
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            if (!strUniversityName.Any())
                try
                {
                    strUniversityName = string.IsNullOrEmpty(thisResponseEducationList)
                        ? Regex.Split(profilePageSource, "school\":\"urn:li:fs_miniSchool:").Skip(1).ToArray()
                        : Regex.Split(thisResponseEducationList, "school\":\"urn:li:fs_miniSchool:").Skip(1).ToArray();
                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                }
            if(strUniversityName is null || strUniversityName.Length == 0)
            {
                var handler = JsonJArrayHandler.GetInstance;
                var obj = handler.ParseJsonToJObject(thisResponseEducationList);
                var educations = handler.GetJArrayElement(handler.GetJTokenValue(obj, "data", "identityDashProfilesByMemberIdentity", "elements", 0, "profileTopEducation", "elements"));
                if(educations != null && educations.HasValues)
                {
                    foreach(var ed in educations)
                    {
                        var education = string.Empty;
                        var schoolName = handler.GetJTokenValue(ed, "school", "name");
                        schoolName = string.IsNullOrEmpty(schoolName) ? handler.GetJTokenValue(ed, "schoolName") : schoolName;
                        var url = handler.GetJTokenValue(ed, "school", "url");
                        if (!string.IsNullOrEmpty(schoolName))
                            education = $"[ {schoolName} ]";
                        if (!string.IsNullOrEmpty(url))
                            education += $"( {url} )";
                        if (!string.IsNullOrEmpty(education) && !educationList.Any(t => t == education))
                            educationList.Add(education);
                    }
                }
            }
            else
            {
                foreach (var item in strUniversityName)
                    try
                    {
                        var sessionEnd = string.Empty;
                        var sessionStart = string.Empty;
                        var education = string.Empty;
                        if (!item.Contains("\"schoolName\":\""))
                            continue;
                        try
                        {
                            var school = Utils.GetBetween(item, "schoolName\":\"", "\"");
                            var degree = Utils.GetBetween(item, "\"degreeName\":\"", "\"");
                            var fieldOfStudy = Utils.GetBetween(item, "fieldOfStudy\":\"", "\"");

                            if (sessionStart == string.Empty && sessionEnd == string.Empty)
                                education = " [" + school + "] Degree: " + degree + ";" + fieldOfStudy;
                            if (fieldOfStudy == string.Empty)
                                education = " [" + school + "] Degree: " + degree;
                            if (degree == string.Empty)
                                education = " [" + school + "] Degree: " + fieldOfStudy;
                            if (degree == string.Empty && fieldOfStudy == string.Empty)
                                education = " [" + school + "]";


                            var startDate = Utils.GetBetween(Utils.GetBetween(item, "\"startDate\":{", "}}") + "*", "\"year\":", "*");
                            var endDate = Utils.GetBetween(Utils.GetBetween(item, "\"endDate\":{", "},\"startDate\"") + "*", "\"year\":", "*");
                            education = $"{education} ({startDate} - {endDate})";
                        }
                        catch (Exception ex)
                        {
                            ex.DebugLog();
                        }

                        educationList.Add(education);
                    }
                    catch (Exception ex)
                    {
                        ex.DebugLog();
                    }

            }

            educationList = educationList.Distinct().ToList();

            foreach (var item in educationList)
                educationCollection = string.IsNullOrEmpty(educationCollection)
                    ? item.Replace("}", "").Replace("&amp;", "&")
                    : educationCollection + "  -  " + item.Replace("}", "").Replace("&amp;", "&");
            return educationCollection;
        }

        public string GetRecommendation(string profilePageSource, ILdFunctions objLdFunctions,
            string flagship3ProfileViewBase, string profileId, string PublicIdentifier)
        {
            var recommendation = string.Empty;
            var list = new List<string>();
            var RecommendationUrl = "";
            var RecommendationResponse = "";
            var Flagship3ProfileViewBase = string.IsNullOrEmpty(flagship3ProfileViewBase) ? _ldDataHelper.Flagship3ProfileViewBase(profilePageSource) : flagship3ProfileViewBase;
            try
            {
                var locale = Utils.GetBetween(profilePageSource, "\"locale\":\"", "\",");
                locale = string.IsNullOrEmpty(locale) ? Utils.GetBetween(profilePageSource, "<meta name=\"i18nLocale\" content=\"", "\">") : locale;
                if (objLdFunctions.IsBrowser)
                {
                    for (int index = 0; index < 2; index++)
                    {
                        if (objLdFunctions.IsBrowser)
                        {
                            RecommendationUrl = $"https://www.linkedin.com/in/{PublicIdentifier}/details/recommendations/?detailScreenTabIndex={index}";
                            RecommendationResponse = objLdFunctions.GetHtmlFromUrlForMobileRequest(RecommendationUrl, Flagship3ProfileViewBase);
                            var RecommendationType = index == 0 ? "RECEIVED" : "GIVEN";
                            var RecommendationCount = HtmlAgilityHelper.GetListNodesFromAttibute(RecommendationResponse, "li", AttributeIdentifierType.Id, null, $"RECOMMENDATIONS-VIEW-DETAILS-profileTabSection-{RecommendationType}-RECOMMENDATIONS-NONE-en-US").Count;
                            if ((RecommendationType == "RECEIVED" && !RecommendationResponse.Contains("You haven't received a recommendation yet")) || (RecommendationType == "GIVEN" && !RecommendationResponse.Contains("You haven't written any recommendations yet")))
                            {
                                list.Add(RecommendationCount.ToString());
                            }
                            else
                            {
                                list.Add("0");
                            }
                        }
                    }
                }
                else
                {
                    //RecommendationUrl = $"https://www.linkedin.com/voyager/api/graphql?includeWebMetadata=true&variables=(start:0,count:100,profileUrn:urn%3Ali%3Afsd_profile%3A{profileId},sectionType:recommendations,tabIndex:{index.ToString()},locale:{locale})&&queryId=voyagerIdentityDashProfileComponents.0a93f28770862d677f97ce83835f48e9";
                    RecommendationUrl = $"https://www.linkedin.com/voyager/api/graphql?variables=(profileUrn:urn%3Ali%3Afsd_profile%3A{profileId},sectionType:recommendations,tabIndex:0,locale:en_US)&queryId=voyagerIdentityDashProfileComponents.c5d4db426a0f8247b8ab7bc1d660775a";
                    objLdFunctions.GetInnerLdHttpHelper().GetRequestParameter().Accept = LdConstants.AcceptApplicationOrJson;
                    RecommendationResponse = objLdFunctions.GetInnerLdHttpHelper().GetRequest(RecommendationUrl).Response;
                    if (RecommendationResponse.Contains("\"sections\":[]"))
                    {
                        list.Add("0");
                    }
                    else
                    {
                        var jObject = JObject.Parse(RecommendationResponse);
                        var jArrayHandler = JsonJArrayHandler.GetInstance;
                        for(int index = 0; index < 2; index++)
                        {
                            var RecommendationTokenValue = jArrayHandler.GetJTokenValue(jObject, "data", "identityDashProfileComponentsBySectionType", "elements", 0, "components", "tabComponent", "sections", index, "subComponent", "components", "pagedListComponent", "components", "elements");
                            list.Add(jArrayHandler.GetJArrayElement(string.IsNullOrEmpty(RecommendationTokenValue) ? "[]" : RecommendationTokenValue)?.Count.ToString());
                        }
                    }
                }
                objLdFunctions.GetInnerLdHttpHelper().GetRequestParameter().Accept = LdConstants.AcceptApplicationOrVndLinkedInMobileDedupedJson;
                recommendation = "Recommendation Received = " + (string.IsNullOrEmpty(list[0])? "0":list[0]) + "  Recommendation Given = " +
                                     (string.IsNullOrEmpty(list[1]) ? "0" : list[1]) + " ";
                recommendation = recommendation.Trim();
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            if (recommendation.Contains("Recommendation Received = 0  Recommendation Given = 0"))
                recommendation = "N/A";

            return recommendation;
        }

        public string GetSkills(bool isUserFilter, string profilePageSource, ILdFunctions objLdFunctions,
            string PublicIdentifier)
        {
            try
            {
                var skill = string.Empty;
                var flagship3ProfileViewBase = _ldDataHelper.Flagship3ProfileViewBase(profilePageSource);
                var thisResponse = "";
                var skillsCount = "";
                var jsonHandler = JsonJArrayHandler.GetInstance;
                #region Old Browser Code For Skills Count.
                //if (objLdFunctions.IsBrowser)
                //{
                //    thisResponse = objLdFunctions.GetHtmlFromUrlForMobileRequest($"https://www.linkedin.com/in/{PublicIdentifier}/details/skills/", flagship3ProfileViewBase);
                //    var totalSkillsArray = HtmlAgilityHelper.GetListNodesFromAttibute(thisResponse, "li", AttributeIdentifierType.Id, null, "SKILLS-VIEW-DETAILS-profileTabSection-ALL-SKILLS-NONE");
                //    skillsCount = totalSkillsArray.Count.ToString();
                //    if (isUserFilter) return skillsCount;
                //    foreach (var item in totalSkillsArray)
                //    {
                //        var skillString = HtmlAgilityHelper.GetStringInnerTextFromClassName(item.InnerHtml, "visually-hidden");
                //        skillString = Utils.RemoveHtmlTags(skillString);
                //        skill += skillString + ":";
                //    } 
                //}
                //else
                //{
                #endregion
                //var SkillAPI = $"https://www.linkedin.com/voyager/api/identity/profiles/{PublicIdentifier}/endorsedSkills?includeHiddenEndorsers=true&count=100";
                var SkillAPI = $"https://www.linkedin.com/voyager/api/graphql?variables=(profileUrn:urn%3Ali%3Afsd_profile%3A{PublicIdentifier},sectionType:skills,locale:en_US)&queryId=voyagerIdentityDashProfileComponents.c5d4db426a0f8247b8ab7bc1d660775a";
                thisResponse = objLdFunctions.IsBrowser?
                        objLdFunctions.GetInnerHttpHelper().GetRequest(SkillAPI).Response
                        :objLdFunctions.GetHtmlFromUrlForMobileRequest(SkillAPI, flagship3ProfileViewBase);
                var totalSkillsArray = jsonHandler.GetJArrayElement(jsonHandler.GetJTokenValue(jsonHandler.ParseJsonToJObject(thisResponse), "data", "identityDashProfileComponentsBySectionType"
                    , "elements",0, "components", "tabComponent", "sections", 0, "subComponent", "components", "pagedListComponent", "components", "elements"));
                skillsCount = totalSkillsArray.Count.ToString();
                if (isUserFilter) return skillsCount;
                foreach (var skillsItem in totalSkillsArray)
                  {
                    var Skill = jsonHandler.GetJTokenValue(skillsItem, "components", "entityComponent", "titleV2","text","text");
                    if (skillsItem != totalSkillsArray.LastOrDefault())
                        skill += Skill + ":";
                    else
                        skill += Skill;
                  }
                skill = skill.Trim(':');
                return skill;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
                return "N/A";
            }
        }

        public string GetExperience(bool isUserFilter, string thisResponseCompanylist, string companycurrent,
            string titlecurrent)
        {
            var ldsExperience = string.Empty;

            try
            {
                var arrExperience = Regex.Split(thisResponseCompanylist, "\"entityUrn\":\"urn:li:fs_position:").Skip(1)
                    .ToArray();

                var requiredExperienceTimeData = Utils.GetBetween(thisResponseCompanylist, "[{\"$deletedFields\":",
                    "\"$type\":\"com.linkedin.voyager.common.MediaProcessorImage\"");

                var arrLen = arrExperience.Length;
                if (isUserFilter)
                    return arrLen.ToString();

                if (arrLen > 0)
                    foreach (var item in arrExperience)
                        try
                        {
                            var companyName = Utils.GetBetween(item, "\"companyName\":\"", "\"");
                            if (string.IsNullOrEmpty(companyName))
                                companyName = companycurrent;

                            var jobTitle = Utils.GetBetween(item, "title\":\"", "\"");
                            if (string.IsNullOrEmpty(jobTitle)) jobTitle = titlecurrent;
                            ldsExperience += "[Company Name -> " + companyName + " <> Job Title -> " + jobTitle + " ";
                            var conditionUse1 = Utils.GetBetween(item, "(", ")");
                            var thisArr = Regex.Split(requiredExperienceTimeData, "deletedFields\":");

                            foreach (var thisDate in thisArr)
                                try
                                {
                                    var conditionUse2 = Utils.GetBetween(thisDate, "(", ")");
                                    if (conditionUse1 == conditionUse2)
                                        if (thisDate.Contains("\"$id\":\"urn:li:fs_position:"))
                                        {
                                            var local = " <> " + "" + Utils.GetBetween(thisDate, ",timePeriod,", "\"") +
                                                        " -> " + Utils.GetBetween(thisDate, "[", ",\"$type\":")
                                                            .Replace("[", "").Replace("]", "") + "".Trim().Trim('-');
                                            if (!local.Contains("urn:li:fs_position:"))
                                                ldsExperience += local;
                                        }
                                }
                                catch (Exception ex)
                                {
                                    ex.DebugLog();
                                }

                            ldsExperience += "]";
                        }
                        catch (Exception ex)
                        {
                            ex.DebugLog();
                        }

                ldsExperience = ldsExperience.Replace(",", "-").Replace("-]", "]").Replace("\"day\"", " Day ")
                    .Replace("\"month\"", " Month ").Replace("\"year\"", " Year ").Replace(":", "").Trim();
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            return ldsExperience;
        }

        public CompanyInfo GetPastTitles(bool isUserFilter, string profilePageSource, ILdFunctions objLdFunctions,
            string profileId, string titlecurrent)
        {
            var ldsPastTitles = string.Empty;
            var thisResponseCompanyList = string.Empty;
            var experience = string.Empty;
            var CurrentAndPastCompany = new CurrentAndPastCompanyInfo();
            try
            {
                var flagship3ProfileViewBase = _ldDataHelper.Flagship3ProfileViewBase(profilePageSource);
                //var actionUrl = "https://www.linkedin.com/voyager/api/identity/profiles/" + profileId +
                //                "/positions?count=60&start=0";
                objLdFunctions.GetInnerHttpHelper().GetRequestParameter().Accept = LdConstants.AcceptApplicationOrJson;
                var actionUrl = $"https://www.linkedin.com/voyager/api/graphql?includeWebMetadata=true&variables=(profileUrn:urn%3Ali%3Afsd_profile%3A{profileId})&queryId=voyagerIdentityDashProfileCards.55af784c21dc8640b500ab5b45937064";
                thisResponseCompanyList = objLdFunctions.GetInnerHttpHelper().GetRequest(actionUrl).Response;
                
                if (isUserFilter)
                    return new CompanyInfo() { CompanyResponse = thisResponseCompanyList};

                if (!thisResponseCompanyList.Contains("total\":"))
                {
                    var arrExperiencePasTitle =
                        Regex.Split(profilePageSource, "\"entityUrn\":\"urn:li:fs_position:").Skip(1).ToArray();

                    foreach (var item in arrExperiencePasTitle)
                    {
                        var pastTitle = Utils.GetBetween(item, "\"title\":\"", "\"");
                        if (pastTitle == titlecurrent) continue;
                        pastTitle = pastTitle + ":";
                        ldsPastTitles = ldsPastTitles + pastTitle;
                    }
                }
                else
                {
                    var handler = JsonJArrayHandler.GetInstance;
                    var obj = handler.ParseJsonToJObject(thisResponseCompanyList);
                    var companies = new List<string>();
                    var experinces = new List<string>();
                    var count = 0;
                    var elements = handler.GetJArrayElement(handler.GetJTokenValue(obj, "data", "identityDashProfileCardsByInitialCards","elements"));
                    if(elements != null && elements.HasValues)
                    {
                        foreach(var outer in elements)
                        {
                            var topComponents = handler.GetJArrayElement(handler.GetJTokenValue(outer, "topComponents"));
                            if(topComponents != null && topComponents.HasValues)
                            {
                                foreach(var components in topComponents)
                                {
                                    var fixedList = handler.GetJTokenOfJToken(components, "components", "fixedListComponent", "components");
                                    if(fixedList != null && fixedList.HasValues)
                                    {
                                        foreach(var lst in fixedList)
                                        {
                                            var companyData = handler.GetJTokenOfJToken(lst, "components", "entityComponent");
                                            var controlName = handler.GetJTokenValue(companyData, "controlName");
                                            
                                            if (string.IsNullOrEmpty(controlName) || controlName != "experience_company_logo")
                                                continue;
                                            var subcomponents = handler.GetJArrayElement(handler.GetJTokenValue(companyData, "subComponents", "components"));
                                            var Found = false;
                                            if(subcomponents != null && subcomponents.HasValues)
                                            {
                                                foreach(var com in subcomponents)
                                                {
                                                    var titleToken = handler.GetJTokenOfJToken(com, "components", "entityComponent", "titleV2");
                                                    if (titleToken is null || !titleToken.HasValues)
                                                        continue;
                                                    Found = true;
                                                    break;
                                                }
                                            }
                                            var title = handler.GetJTokenValue(companyData, "titleV2", "text", "text");
                                            var subtitle = handler.GetJTokenValue(companyData, "subtitle", "text");
                                            var actionTarget = handler.GetJTokenValue(companyData, "textActionTarget");
                                            if (!string.IsNullOrEmpty(subtitle))
                                                title += $" : {subtitle}";
                                            if (!string.IsNullOrEmpty(title) && !companies.Any(t=>t==title))
                                            {
                                                companies.Add(title);
                                                count++;
                                                if (string.IsNullOrEmpty(CurrentAndPastCompany.TitleCurrent))
                                                {
                                                    CurrentAndPastCompany.TitleCurrent = CurrentAndPastCompany.CurrentCompany = Found ? title : subtitle;
                                                    if(!string.IsNullOrEmpty(actionTarget))
                                                        CurrentAndPastCompany.CurrentCompanyUrl = actionTarget;
                                                }
                                                if(string.IsNullOrEmpty(CurrentAndPastCompany.PastCompany) && count > 1 && !string.IsNullOrEmpty(actionTarget))
                                                {
                                                    CurrentAndPastCompany.PastCompany = Found ? title : subtitle;
                                                    CurrentAndPastCompany.PastCompanyUrl = actionTarget;
                                                    count = 0;
                                                }
                                            }
                                            if (Found ? !string.IsNullOrEmpty(title) && !experinces.Any(t => t == title) : !string.IsNullOrEmpty(subtitle) && !experinces.Any(t => t == subtitle))
                                                experinces.Add(Found ? title:subtitle);
                                        }
                                    }
                                }
                            }
                        }
                        if (companies.Count > 0)
                            ldsPastTitles = string.Join(":", companies);
                        if (experinces.Count > 0)
                            experience = string.Join(":", experinces);
                    }
                }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
            return new CompanyInfo
            {
                PastTitles = ldsPastTitles,
                CompanyResponse = thisResponseCompanyList,
                Experience = experience,
                CurrentAndPastCompanyInfo = CurrentAndPastCompany
            };
        }


        public bool IsFirstConnection(DominatorAccountModel dominatorAccountModel, IDbAccountService dbAccountService,
            LinkedinUser objLinkedinUser,
            ActivityType activityType, ILdFunctions ldFunctions)
        {
            var isFirstConnection = true;
            try
            {
                // checking user is a 1st connection
                // user may removed from connection from outside
                var profileName = Utils.GetBetween(objLinkedinUser.ProfileUrl + "$$", "in/", "$$").Replace("/", "");
                var apiUrl = $"https://www.linkedin.com/voyager/api/identity/profiles/{profileName}/profileActions";
                var responseParameter = ldFunctions.GetInnerHttpHelper().GetRequest(apiUrl);
                if (!responseParameter.Response.Contains(
                    "\"action\":{\"com.linkedin.voyager.identity.profile.actions.Message\""))
                {
                    isFirstConnection = false;
                    _delayService.ThreadSleep(5000);
                    dbAccountService.RemoveMatch<Connections>(x =>
                        x.ProfileUrl.Equals(objLinkedinUser.ProfileUrl));
                    dominatorAccountModel.CancellationSource.Token.ThrowIfCancellationRequested();
                    GlobusLogHelper.log.Info(Log.CustomMessage, dominatorAccountModel.AccountBaseModel.AccountNetwork,
                        dominatorAccountModel.AccountBaseModel.UserName,
                        activityType, $"Filtered {objLinkedinUser.FullName} is not a 1st connection.");
                }
            }
            catch (Exception exception)
            {
                exception.DebugLog();
            }

            return isFirstConnection;
        }


        public UserScraperDetailedInfo GetFullNameDetailedInfo(string profilePageSource, ILdFunctions ldFunctions, string user="")
        {
            var detailedInfo = new UserScraperDetailedInfo();
            try
            {
                var jArr = JsonJArrayHandler.GetInstance;
                var username = _ldDataHelper.GetPublicIdentifierFromPageSource(profilePageSource);
                if (string.IsNullOrEmpty(username))
                    username =Utilities.GetBetween(Utilities.GetBetween(profilePageSource, "{\"data\":{\"customPronoun", "included\":[]}"), "publicIdentifier\":\"", "\",\"");
                if (string.IsNullOrEmpty(username))
                    username = user;
                    detailedInfo.PublicIdentifier = username;
                // here we are not getting user details from page source therefore we are using http 
                //var resp = ldFunctions.IsBrowser? ldFunctions.GetInnerHttpHelper().GetRequest($"https://www.linkedin.com/voyager/api/identity/profiles/{username}").Response
                //    :ldFunctions.GetHtmlFromUrlForMobileRequest($"https://www.linkedin.com/voyager/api/identity/profiles/{username}", "");
                //resp = string.IsNullOrEmpty(resp) ? ldFunctions.IsBrowser? ldFunctions.GetInnerHttpHelper()
                //            .GetRequest($"https://www.linkedin.com/voyager/api/identity/profiles/{username}").Response
                //            :ldFunctions.GetHtmlFromUrlForMobileRequest($"https://www.linkedin.com/voyager/api/identity/profiles/{username}", "") : resp;
                //resp = string.IsNullOrEmpty(resp) ? ldFunctions.IsBrowser
                //        ?ldFunctions.GetInnerHttpHelper().GetRequest($"https://www.linkedin.com/voyager/api/identity/profiles/{username}").Response:
                //        ldFunctions.GetHtmlFromUrlForMobileRequest($"https://www.linkedin.com/voyager/api/identity/profiles/{username}", ""): resp;
                //resp = string.IsNullOrEmpty(resp) ? ldFunctions.IsBrowser ? ldFunctions.GetInnerHttpHelper().GetRequest($"https://www.linkedin.com/voyager/api/identity/profiles/{username}").Response
                //        : ldFunctions.GetHtmlFromUrlForMobileRequest($"https://www.linkedin.com/voyager/api/identity/profiles/{username}", "") : resp;
                var resp = ldFunctions.GetInnerHttpHelper().GetRequest($"https://www.linkedin.com/voyager/api/identity/dash/profiles?q=memberIdentity&memberIdentity={username}&decorationId=com.linkedin.voyager.dash.deco.identity.profile.FullProfileWithEntities-35").Response;
                var jobj = jArr.ParseJsonToJObject(resp);
                var firstName = jArr.GetJTokenValue(jobj, "firstName");
                firstName = string.IsNullOrEmpty(firstName) ? jArr.GetJTokenValue(jobj, "elements", 0, "firstName") : firstName;
                detailedInfo.Firstname = firstName;
                var locationData = string.Empty;
                var location1 = jArr.GetJTokenValue(jobj, "geoLocationName");                
                var location2 = jArr.GetJTokenValue(jobj, "locationName");
                if (string.IsNullOrEmpty(location2))
                    location2 = jArr.GetJTokenValue(jobj, "geoCountryName");
                if (!string.IsNullOrEmpty(location1) || !string.IsNullOrEmpty(location2))
                    locationData = location1 +" "+ location2;
                locationData = string.IsNullOrEmpty(locationData) ? jArr.GetJTokenValue(jobj, "elements", 0, "geoLocation", "geo", "defaultLocalizedName") : locationData;
                detailedInfo.Location = locationData;
                var lastName = jArr.GetJTokenValue(jobj, "lastName");
                lastName = string.IsNullOrEmpty(lastName) ? jArr.GetJTokenValue(jobj, "elements", 0, "lastName") : lastName;
                detailedInfo.Lastname = lastName;
                var profileID = jArr.GetJTokenValue(jobj, "entityUrn");
                profileID = string.IsNullOrEmpty(profileID) ? jArr.GetJTokenValue(jobj, "elements",0, "entityUrn") : profileID;
                detailedInfo.ProfileId = profileID?.Replace("urn:li:fs_miniProfile:", "")
                    ?.Replace("urn:li:fs_profile:", "")?.Replace("urn:li:fsd_profile:", "");
                var headline = jArr.GetJTokenValue(jobj, "headline");
                headline = string.IsNullOrEmpty(headline) ? jArr.GetJTokenValue(jobj, "elements", 0, "headline") : headline;
                detailedInfo.HeadlineTitle = headline;
                var trackingID = jArr.GetJTokenValue(jobj, "miniProfile", "trackingId");
                trackingID = string.IsNullOrEmpty(trackingID) ? jArr.GetJTokenValue(jobj, "elements",0, "trackingId") : trackingID;
                detailedInfo.TrackingId = trackingID;
                var memberId = jArr.GetJTokenValue(jobj, "miniProfile", "objectUrn");
                memberId = string.IsNullOrEmpty(memberId) ? jArr.GetJTokenValue(jobj, "elements",0, "objectUrn") : memberId;
                detailedInfo.MemberId = memberId?.Replace("urn:li:member:", "");
            }
            catch (Exception exception)
            {
                exception.DebugLog();
            }

            return detailedInfo;
        }

        public bool IsValidLinkJobProcessResult(string url, JobProcessResult jobProcessResult, ILdFunctions ldFunctions,
            DominatorAccountModel dominatorAccountModel, out string feedPageResponse)
        {
            //System.Net.ServicePointManager.Expect100Continue = false;
            //System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls;
            feedPageResponse = ldFunctions.GetRequestUpdatedUserAgent(url, true);

            if (string.IsNullOrEmpty(feedPageResponse) || string.IsNullOrEmpty(url))
            {
                GlobusLogHelper.log.Info(Log.ActivityFailed, dominatorAccountModel.AccountBaseModel.AccountNetwork,
                    dominatorAccountModel.AccountBaseModel.UserName, "post not exist : ",
                    $"[ {url} ]", "");
                jobProcessResult.IsProcessSuceessfull = false;
                return true;
            }

            return false;
        }

        public bool ScrapeCompanyDetails(ILdFunctions _ldFunctions, string companyUrl,
            ref CompanyScraperDetailedInfo company, DominatorAccountModel accountModel,string universalName="")
        {
            var isSuccess = false;
            var jsonResponse = "";
            var jsonJArrayHandler = JsonJArrayHandler.GetInstance;
            try
            {
                if(long.TryParse(universalName, out long IsID))
                {
                    var api = $"https://www.linkedin.com/voyager/api/graphql?includeWebMetadata=true&variables=(organizationalPageUrn:urn%3Ali%3Afsd_organizationalPage%3A{IsID},context:ORGANIZATIONAL_PAGE_MEMBER_HOME)&queryId=voyagerOrganizationDashViewWrapper.0b8be97f4f1386a8114db0ef77298c51";
                    var universalNameResponse = _ldFunctions.GetInnerLdHttpHelper().GetRequest(api).Response;
                    var Obj1 = jsonJArrayHandler.ParseJsonToJObject(universalNameResponse);
                    var action = jsonJArrayHandler.GetJArrayElement(jsonJArrayHandler.GetJTokenValue(Obj1, "data", "organizationDashViewWrapperByOrganizationalPageAndContext", "elements", 0, "actions"));
                    if(action != null && action.HasValues)
                    {
                        var target = action.FirstOrDefault(x => !string.IsNullOrEmpty(jsonJArrayHandler.GetJTokenValue(x, "action", "navigationAction", "urlV2", "absoluteUrl")));
                        if(target is null)
                        {
                            action = jsonJArrayHandler.GetJArrayElement(jsonJArrayHandler.GetJTokenValue(Obj1, "data", "organizationDashViewWrapperByOrganizationalPageAndContext", "elements", 0, "nestedComponents"));
                            target = action.FirstOrDefault(x => !string.IsNullOrEmpty(jsonJArrayHandler.GetJTokenValue(x, "externalComponent", "updatesCarousel", "updates", "elements",0, "actor", "navigationContext", "actionTarget")));
                        }
                        if(target != null)
                        {
                            var url = jsonJArrayHandler.GetJTokenValue(target, "action", "navigationAction", "urlV2", "absoluteUrl");
                            url = string.IsNullOrEmpty(url) ? jsonJArrayHandler.GetJTokenValue(target, "externalComponent", "updatesCarousel", "updates", "elements", 0, "actor", "navigationContext", "actionTarget") : url;
                            if(!string.IsNullOrEmpty(url))
                            {
                                string pattern = @"linkedin\.com/company/([^/?]+)";
                                var match = Regex.Match(url, pattern, RegexOptions.IgnoreCase);
                                if (match.Success)
                                    universalName = match.Groups[1].Value;
                            }
                        }
                    }
                }
                var CompanyDetailsAPI = _ldDataHelper.GetCompanyDetailsApi(universalName);
                int failedCount = 0;
                _ldFunctions.GetInnerLdHttpHelper().GetRequestParameter().Accept = LdConstants.AcceptApplicationOrJson;
                jsonResponse = _ldFunctions.GetInnerLdHttpHelper().GetRequest(CompanyDetailsAPI).Response;
                while (failedCount++ <= 2 && string.IsNullOrEmpty(jsonResponse))
                    jsonResponse = _ldFunctions.GetInnerLdHttpHelper().GetRequest(CompanyDetailsAPI).Response;
                _ldFunctions.GetInnerLdHttpHelper().GetRequestParameter().Accept = LdConstants.AcceptApplicationOrVndLinkedInMobileDedupedJson;
                var jsonHandler = new JsonHandler(jsonResponse);
                var included = jsonHandler.GetJToken("data", "organizationDashCompaniesByUniversalName", "elements");
                foreach (var include in included)
                {
                    if (!include.ToString().Contains("specialities"))
                        continue;
                    company = new CompanyScraperDetailedInfo();
                    GetUserDetails(company, accountModel);
                    company.CompanyName = Utils.AssignNa(Utils.RemoveSpecialCharacters(jsonJArrayHandler.GetJTokenValue(include, "name")));
                    company.CompanyDescription =
                        Utils.AssignNa(Utils.RemoveSpecialCharacters(jsonJArrayHandler.GetJTokenValue(include, "description")));
                    company.Specialties = Utils.AssignNa(Utils.RemoveSpecialCharacters(jsonJArrayHandler.GetJTokenValue(include, "specialities")));
                    company.CompanySize =
                        Utils.AssignNa(Utils.RemoveSpecialCharacters(jsonJArrayHandler.GetJTokenValue(include, "employeeCount")));
                    company.Website = Utils.AssignNa(Utils.RemoveSpecialCharacters(jsonJArrayHandler.GetJTokenValue(include, "websiteUrl")));
                    company.CompanyUrl = companyUrl;
                    company.FoundationDate =
                        Utils.AssignNa(Utils.RemoveSpecialCharacters(jsonJArrayHandler.GetJTokenValue(include, "foundedOn","year")));
                    var headquarter = jsonJArrayHandler.GetJTokenOfJToken(include, "headquarter");
                    company.Headquarter +=
                        (company.Country = Utils.AssignNa(Utils.RemoveSpecialCharacters(jsonJArrayHandler.GetJTokenValue(headquarter,"address", "country")))) ==
                        "N/A"
                            ? ""
                            : company.Country+" ";
                    company.Headquarter +=
                        (company.City = Utils.AssignNa(Utils.RemoveSpecialCharacters(jsonJArrayHandler.GetJTokenValue(headquarter,"address", "city")))) == "N/A"
                            ? ""
                            : company.State+" ";
                    company.Headquarter +=
                        (company.Zipcode =
                            Utils.AssignNa(Utils.RemoveSpecialCharacters(jsonJArrayHandler.GetJTokenValue(headquarter,"address", "postalCode")))) == "N/A"
                            ? ""
                            :company.Zipcode+" ";
                    company.Industry = Utils.AssignNa(Utils.RemoveSpecialCharacters(Utils.GetBetween(jsonResponse, "{\"localizedName\":\"", "\""))
                        .Replace("&amp;", ""));
                    if (company.Industry == "N/A")
                        company.Industry = Utils.RemoveSpecialCharacters(jsonJArrayHandler.GetJTokenValue(include, "industry", 0,"name"));
                    company.State = Utils.AssignNa(Utils.RemoveSpecialCharacters(jsonJArrayHandler.GetJTokenValue(headquarter, "address", "geographicArea")));
                    company.City = Utils.AssignNa(Utils.RemoveSpecialCharacters(jsonJArrayHandler.GetJTokenValue(headquarter, "address", "city")));
                    company.Zipcode = Utils.AssignNa(Utils.RemoveSpecialCharacters(jsonJArrayHandler.GetJTokenValue(headquarter, "address", "postalCode")));
                    company.Country = Utils.AssignNa(Utils.RemoveSpecialCharacters(jsonJArrayHandler.GetJTokenValue(headquarter, "address", "country")));
                    company.AddressLine1 = Utils.AssignNa(Utils.RemoveSpecialCharacters(jsonJArrayHandler.GetJTokenValue(headquarter, "address", "line1")));
                    company.AddressLine2 = Utils.AssignNa(Utils.RemoveSpecialCharacters(jsonJArrayHandler.GetJTokenValue(headquarter, "address", "line2")));
                    GetCompanyLocations(include, company);
                    company.Headquarter += company.AddressLine1 == "N/A" ? "" : " "+company.AddressLine1;
                    company.Headquarter += company.AddressLine2 == "N/A" ? "" : " "+company.AddressLine2;
                    company.CompanyId = Utils.AssignNa(jsonJArrayHandler.GetJTokenValue(include, "entityUrn"))?.Split(':').Last(x=>x!=string.Empty);
                    var logoPath = jsonJArrayHandler.GetJTokenOfJToken(include, "logoResolutionResult", "vectorImage");
                    company.CompanyLogoUrl = Utils.AssignNa(jsonJArrayHandler.GetJTokenValue(logoPath, "rootUrl") + jsonJArrayHandler.GetJTokenValue(jsonJArrayHandler.ParseJsonToJObject(jsonJArrayHandler.GetJArrayElement(jsonJArrayHandler.GetJTokenValue(logoPath, "artifacts"))?.LastOrDefault(x=>(x.ToString().Contains("\"width\": 400") || x.ToString().Contains("\"width\": 200") || x.ToString().Contains("\"width\": 100"))).ToString()), "fileIdentifyingUrlPathSegment"));
                    company.IsFollowing = jsonHandler.GetJTokenValue(include, "followingState", "following");
                    company.TotalEmployees = Utils.AssignNa(Utils.RemoveSpecialCharacters(jsonHandler.GetJTokenValue(include, "employeeCount")));
                    break;
                }

                isSuccess = true;
            }
            catch (Exception exception)
            {
                exception.DebugLog();
            }

            return isSuccess;
        }

        private void GetUserDetails(CompanyScraperDetailedInfo objDetailedInfo, DominatorAccountModel accountModel)
        {
            try
            {
                objDetailedInfo.AccountEmail = accountModel.AccountBaseModel.UserName;
                objDetailedInfo.AccountUserFullName = accountModel.AccountBaseModel.UserFullName;
                objDetailedInfo.AccountUserProfileUrl = accountModel.AccountBaseModel.ProfilePictureUrl;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }


        private void GetCompanyLocations(JToken jArray, CompanyScraperDetailedInfo companyScraperDetailedInfo)
        {
            try
            {
                var jArrayHandler = JsonJArrayHandler.GetInstance;
                var confirmedLocations = jArrayHandler.GetTokenElement(jArray, "confirmedLocations");
                var stringBuilder = new StringBuilder();
                var count = 0;
                if(confirmedLocations != null)
                {
                    foreach (var jData in confirmedLocations)
                        _ldDataHelper.GetCompanyAddress(stringBuilder, ++count, jArrayHandler, jData);
                    companyScraperDetailedInfo.OtherLocations = stringBuilder.ToString().Trim();
                }
            }
            catch (Exception exception)
            {
                exception.DebugLog();
            }
        }

        public CompanyScraperDetailedInfo ScrapeCompanyDetails(string companyUrl, ILdFunctions objLdFunctions)
        {
            var detailedInfo = new CompanyScraperDetailedInfo();
            try
            {
                if (string.IsNullOrEmpty(companyUrl))
                    return detailedInfo;
                var UniversalCompanyName = companyUrl.Split('/')?.LastOrDefault(x => x != string.Empty);
                var failedCount = 0;
                var CompanyDetailsAPI = LdConstants.GetCompanyDetailsAPI(UniversalCompanyName);
            TryAgain:
                var CompanyDetails = objLdFunctions.TryAndGetResponse(CompanyDetailsAPI);
                while (failedCount++ <= 2 && string.IsNullOrEmpty(CompanyDetails))
                    goto TryAgain;
                var jsonHandler = JsonJArrayHandler.GetInstance;
                var jsonObject = jsonHandler.ParseJsonToJObject(CompanyDetails);
                var details = jsonHandler.GetJTokenOfJToken(jsonObject, "data", "organizationDashCompaniesByUniversalName", "elements",0);
                detailedInfo.CompanyName = jsonHandler.GetJTokenValue(details, "name");
                detailedInfo.CompanyUrl = jsonHandler.GetJTokenValue(details, "url");
                detailedInfo.CompanyId = jsonHandler.GetJTokenValue(details, "entityUrn")?.Replace("urn:li:fsd_company:", "");
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
            return detailedInfo;
            #region OldCodeTOGetCompanyDetails.
            //try
            //{
            //    var ldDataHelper = LdDataHelper.GetInstance;
            //    var companyId =
            //        ldDataHelper
            //            .GetCompanyIdFromUserProfilePageSource(
            //                companyUrl); // Utils.GetBetween(companyUrl + "**", "company/", "**");
            //    var regex = new Regex("^[0-9]+$");
            //    if (!regex.IsMatch(companyId))
            //    {
            //        var pageResponse = objLdFunctions.GetRequestUpdatedUserAgent(companyUrl, true);
            //        //companyResolutionResult":"urn:li:fs_normalized_company:
            //        if (string.IsNullOrWhiteSpace(detailedInfo.CompanyId = Utils.GetBetween(pageResponse,
            //            "fs_organizationTargetedContent:(urn:li:fs_normalized_company:", ",")))
            //            detailedInfo.CompanyId = Utils.GetBetween(pageResponse,
            //                "companyResolutionResult\":\"urn:li:fs_normalized_company:", "\"");
            //        if (string.IsNullOrEmpty(detailedInfo.CompanyId))
            //            detailedInfo.CompanyId = Utils.GetBetween(pageResponse,
            //                "\"*elements\":[\"urn:li:fs_normalized_company:", "\"");
            //        if (string.IsNullOrEmpty(detailedInfo.CompanyId))
            //            detailedInfo.CompanyId = Utils.GetBetween(pageResponse,
            //                "\"*elements\":[\"urn:li:fsd_company:", "\"]");
            //        if (string.IsNullOrEmpty(detailedInfo.CompanyId))
            //            detailedInfo.CompanyId = Utils.GetBetween(pageResponse, "\"urn:li:fs_miniCompany:", "\">");
            //        if (string.IsNullOrWhiteSpace(detailedInfo.CompanyName =
            //            Utils.GetBetween(pageResponse, ".com/organization/", "/")))
            //        {
            //            detailedInfo.CompanyName = Utils.GetBetween(pageResponse, "\"View company:", "\"").Trim();
            //            if(string.IsNullOrEmpty(detailedInfo.CompanyName))
            //                detailedInfo.CompanyName = Utils.GetBetween(pageResponse, "zation.Location\"},\"universalName\":\"", "\"").Trim();
            //            if (string.IsNullOrWhiteSpace(detailedInfo.CompanyName))
            //                detailedInfo.CompanyName = companyId.Replace("/", "");
            //        }
            //        if (string.IsNullOrEmpty(detailedInfo.CompanyId))
            //        {
            //            var CompanyDetailsResponse = objLdFunctions.GetHtmlFromUrlNormalMobileRequest(LdConstants.GetCompanyDetailsAPI(detailedInfo.CompanyName));
            //            detailedInfo.CompanyId = Utils.GetBetween(CompanyDetailsResponse, "urn:li:fsd_company:", "\"");
            //        }
            //        detailedInfo.CompanyUrl = companyUrl;
            //    }

            //    return detailedInfo;
            //}
            //catch (OperationCanceledException)
            //{
            //    throw new OperationCanceledException("Operation Cancelled!");
            //}
            //catch (Exception ex)
            //{
            //    ex.DebugLog();
            //    return new CompanyScraperDetailedInfo();
            //}
            #endregion
        }
    }
}