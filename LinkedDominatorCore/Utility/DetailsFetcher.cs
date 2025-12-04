using System;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using ThreadUtils;
using DominatorHouseCore;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using DominatorHouseCore.Process;
using DominatorHouseCore.Utility;
using LinkedDominatorCore.DetailedInfo;
using LinkedDominatorCore.LDLibrary;
using LinkedDominatorCore.LDModel;
using LinkedDominatorCore.LDUtility;
using Newtonsoft.Json.Linq;

namespace LinkedDominatorCore.Utility
{
    public interface IDetailsFetcher
    {
        UserScraperDetailedInfo GetUserScraperDetailedInfo(DominatorAccountModel dominatorAccountModel);
        FileIdentifyingUrlPath GetFileIdentifyPath(string feedPageResponse);

        FileIdentifyingUrlPath GetFileIdentifyPath(string feedPageResponse, string memberId);

        bool UserInformation(string profileInput, bool isCheckedFilterProfileImageCheckbox,
            string profileUrl, string profilePageSource, LinkedinUser objLinkedinUser, ILdFunctions ldFunctions,
            DominatorAccountModel dominatorAccountModel, ActivityType activityType);

        void DebugLogInfo(string response, string dataHeader = "");
        void DebugLogTrack(IResponseParameter responseParameter, string dataHeader = "", int trimLength = 500);
    }

    public class DetailsFetcher : IDetailsFetcher
    {
        private readonly IDelayService _delayService;

        public DetailsFetcher(IDelayService delayService)
        {
            _delayService = delayService;
        }

        /// <summary>
        ///     Get Details required for make request in engage module
        /// </summary>
        /// <param name="dominatorAccountModel"></param>
        /// <returns></returns>
        public UserScraperDetailedInfo GetUserScraperDetailedInfo(DominatorAccountModel dominatorAccountModel)
        {
            var userScraperDetailedInfo = new UserScraperDetailedInfo();
            try
            {
                var dict = dominatorAccountModel.ExtraParameters;
                userScraperDetailedInfo.ProfileId = dominatorAccountModel.AccountBaseModel.UserId;
                userScraperDetailedInfo.Firstname = dict.Keys.Contains("FirstName") ? dict["FirstName"] : "";

                userScraperDetailedInfo.Lastname = dict.Keys.Contains("LastName") ? dict["LastName"] : "";
                userScraperDetailedInfo.PublicIdentifier =
                    dict.Keys.Contains("PublicIdentifier") ? dict["PublicIdentifier"] : "";
                userScraperDetailedInfo.MemberId = dict.Keys.Contains("MemberId") ? dict["MemberId"] : "";
                userScraperDetailedInfo.TrackingId = dict.Keys.Contains("TrackingId") ? dict["TrackingId"] : "";
                userScraperDetailedInfo.Occupation = dict.Keys.Contains("Occupation") ? dict["Occupation"] : "";
                if (string.IsNullOrEmpty(dominatorAccountModel.AccountBaseModel.UserFullName))
                    dominatorAccountModel.AccountBaseModel.UserFullName =
                        $"{userScraperDetailedInfo.Firstname} {userScraperDetailedInfo.Lastname}";

                var comapany_name = Regex.Split(userScraperDetailedInfo.Occupation, " at ");
                if (comapany_name.Count() > 1)
                    userScraperDetailedInfo.CompanyCurrent = comapany_name.LastOrDefault();
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            return userScraperDetailedInfo;
        }

        public FileIdentifyingUrlPath GetFileIdentifyPath(string feedPageResponse)
        {
            var fileIdentifyingUrlPath = new FileIdentifyingUrlPath();
            try
            {
                fileIdentifyingUrlPath.GroupPostId =
                    Utils.GetBetween(feedPageResponse, "urn:li:groupPost:", "\"").Replace(")", "");

                var requiredData = Utils.GetBetween(feedPageResponse,
                    "{\"data\":{\"$deletedFields\":[],\"publicContactInfo\":\"", "</code>");
                requiredData = "{\"data\":{\"$deletedFields\":[],\"publicContactInfo\":\"" + requiredData;
                var jArrayHandler = JsonJArrayHandler.GetInstance;
                var obj = jArrayHandler.ParseJsonToJObject(requiredData);
                if (obj == null)
                {
                    requiredData = "{\"data\":{\"publicContactInfo\":{" + Utils.GetBetween(feedPageResponse,
                                       "{\"data\":{\"publicContactInfo\":{", "</code>");
                    obj = jArrayHandler.ParseJsonToJObject(requiredData);
                }

                var arr = obj["included"];
                if (string.IsNullOrEmpty(jArrayHandler.GetJTokenValue(arr.First, "width")))
                    arr = jArrayHandler.GetTokenElement(arr, 0, "picture", "artifacts");

                foreach (var item in arr)
                    try
                    {
                        var width = item["width"].ToString();
                        switch (width)
                        {
                            case "100":
                                fileIdentifyingUrlPath.FileIdentifyingUrlPathSegment100 =
                                    item["fileIdentifyingUrlPathSegment"].ToString();
                                break;
                            case "200":
                                fileIdentifyingUrlPath.FileIdentifyingUrlPathSegment200 =
                                    item["fileIdentifyingUrlPathSegment"].ToString();
                                break;
                            case "400":
                                fileIdentifyingUrlPath.FileIdentifyingUrlPathSegment400 =
                                    item["fileIdentifyingUrlPathSegment"].ToString();
                                break;
                            case "800":
                                fileIdentifyingUrlPath.FileIdentifyingUrlPathSegment800 =
                                    item["fileIdentifyingUrlPathSegment"].ToString();
                                break;
                        }

                        if (!string.IsNullOrEmpty(fileIdentifyingUrlPath.Artifacts)) continue;
                        try
                        {
                            fileIdentifyingUrlPath.Artifacts = item["$id"].ToString();
                            fileIdentifyingUrlPath.Artifacts = fileIdentifyingUrlPath.Artifacts.Split(',').Last();
                            fileIdentifyingUrlPath.Artifacts =
                                fileIdentifyingUrlPath.Artifacts.Remove(fileIdentifyingUrlPath.Artifacts.Length - 2, 2);
                        }
                        catch (Exception ex)
                        {
                            ex.DebugLog();
                        }

                        fileIdentifyingUrlPath.ClientPageInstanceId =
                            LdDataHelper.GetInstance.GetInstanceIdFromUserProfilePageSource(feedPageResponse);
                    }
                    catch
                    {
                        try
                        {
                            fileIdentifyingUrlPath.RootUrl = item["rootUrl"].ToString();
                        }
                        catch (Exception ex)
                        {
                            ex.DebugLog();
                        }
                    }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            return fileIdentifyingUrlPath;
        }

        public FileIdentifyingUrlPath GetFileIdentifyPath(string feedPageResponse, string memberId)
        {
            var fileIdentifyingUrlPath = new FileIdentifyingUrlPath();
            try
            {
                fileIdentifyingUrlPath.GroupPostId = Utils.GetBetween(feedPageResponse, "urn:li:groupPost:", "\"");

                var requiredData = Utils.GetBetween(feedPageResponse,
                    "{\"data\":{\"$deletedFields\":[],\"publicContactInfo\":\"", "</code>");
                requiredData = "{\"data\":{\"$deletedFields\":[],\"publicContactInfo\":\"" + requiredData;
                var jArrayHandler = JsonJArrayHandler.GetInstance;
                var obj = jArrayHandler.ParseJsonToJObject(requiredData);
                if (obj == null)
                {
                    requiredData = Utils.GetBetweenAndAddStart(feedPageResponse,
                        "{\"data\":{\"plainId\":" + memberId + ",\"publicContactInfo\":", "</code>");
                    requiredData = string.IsNullOrEmpty(requiredData) ? feedPageResponse : requiredData;
                    obj = jArrayHandler.ParseJsonToJObject(requiredData);
                }

                var arr = obj["included"];
                arr = arr == null ? obj:arr;
                fileIdentifyingUrlPath.RootUrl = jArrayHandler.GetJTokenValue(arr, "miniProfile", "picture", "com.linkedin.common.VectorImage", "rootUrl");
                arr = jArrayHandler.GetTokenElement(arr, "miniProfile", "picture", "com.linkedin.common.VectorImage", "artifacts");
                foreach (var item in arr)
                    try
                    {
                        var width = item["width"].ToString();
                        switch (width)
                        {
                            case "100":
                                fileIdentifyingUrlPath.FileIdentifyingUrlPathSegment100 =
                                    item["fileIdentifyingUrlPathSegment"].ToString();
                                break;
                            case "200":
                                fileIdentifyingUrlPath.FileIdentifyingUrlPathSegment200 =
                                    item["fileIdentifyingUrlPathSegment"].ToString();
                                break;
                            case "400":
                                fileIdentifyingUrlPath.FileIdentifyingUrlPathSegment400 =
                                    item["fileIdentifyingUrlPathSegment"].ToString();
                                break;
                            case "800":
                                fileIdentifyingUrlPath.FileIdentifyingUrlPathSegment800 =
                                    item["fileIdentifyingUrlPathSegment"].ToString();
                                break;
                        }

                        if (!string.IsNullOrEmpty(fileIdentifyingUrlPath.Artifacts)) continue;
                        try
                        {
                            fileIdentifyingUrlPath.Artifacts = item["$id"]?.ToString();
                            fileIdentifyingUrlPath.Artifacts = fileIdentifyingUrlPath?.Artifacts?.Split(',')?.Last();
                            if(fileIdentifyingUrlPath.Artifacts != null)
                                fileIdentifyingUrlPath.Artifacts =
                                    fileIdentifyingUrlPath.Artifacts.Remove(fileIdentifyingUrlPath.Artifacts.Length - 2, 2);
                        }
                        catch (Exception)
                        {
                            //ex.DebugLog();
                        }

                        fileIdentifyingUrlPath.ClientPageInstanceId =
                            LdDataHelper.GetInstance.GetInstanceIdFromUserProfilePageSource(feedPageResponse);
                    }
                    catch
                    {
                        try
                        {
                            fileIdentifyingUrlPath.RootUrl = item["rootUrl"].ToString();
                        }
                        catch (Exception ex)
                        {
                            ex.DebugLog();
                        }
                    }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            return fileIdentifyingUrlPath;
        }


        public bool UserInformation(string profileInput, bool isCheckedFilterProfileImageCheckbox,
            string profileUrl, string profilePageSource, LinkedinUser objLinkedinUser, ILdFunctions ldFunctions,
            DominatorAccountModel dominatorAccountModel, ActivityType activityType)
        {
            #region ProfileId and PublicIdentifier

            var profileId = string.Empty;
            var fullName = string.Empty;
            var forPublicidentify = string.Empty;
            var publicIdentifier = string.Empty;
            var objGetDetailedUserInfo = new GetDetailedUserInfo(_delayService);
            var ldDataHelper = LdDataHelper.GetInstance;

            try
            {
                if (profileInput.Contains("<:>"))
                {
                    #region Get ProfileId and PublicIdentifier When ProfileInput contain <:>

                    try
                    {
                        profileId = Regex.Split(profileInput, "<:>")[2];
                    }
                    catch (Exception ex)
                    {
                        ex.DebugLog();
                    }

                    var pubIdentifier = ldDataHelper.GetPublicInstanceFromProfileUrl(profileUrl);
                    if (!string.IsNullOrEmpty(pubIdentifier) && pubIdentifier.Length != 39)
                        publicIdentifier = pubIdentifier;

                    #endregion
                }

                else
                {
                    #region Get ProfileId and PublicIdentifier When ProfileInput doesn't contain <:>

                    profileId = ldDataHelper.GetProfileIdFromUserProfilePageSource(profilePageSource);
                    publicIdentifier = Utils.GetBetween(profileUrl + "**", "in/", "**").Replace("/", "");
                    if(string.IsNullOrEmpty(publicIdentifier) && profileUrl.Contains("sales/lead"))
                        publicIdentifier=profileUrl.Split(',')?.FirstOrDefault()?.Replace("https://www.linkedin.com/sales/lead/", "");
                    #endregion
                }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            #endregion

            if (isCheckedFilterProfileImageCheckbox)
            {
                #region MyRegion

                try
                {
                    if (ldFunctions.IsBrowser)
                    {
                        var response = WebUtility.HtmlDecode(ldFunctions.BrowserWindow.GetPageSource());
                        var divData = HtmlAgilityHelper.GetListNodesFromClassName(response,
                            "presence-entity presence-entity--size-9 pv-top-card__image");
                        if(divData.Count<=0)
                           divData= HtmlAgilityHelper.GetListNodesFromClassName(response,
                            "pv-top-card__non-self-photo-wrapper ml0");
                        if(divData.Count<=0)
                            divData = HtmlAgilityHelper.GetListNodesFromClassName(response,
                            "pv-top-card-profile-picture pv-top-card-profile-picture__container");
                        objLinkedinUser.ProfilePicUrl = Utilities.GetBetween(divData?.FirstOrDefault()?.OuterHtml, "src=\"", "\"");

                        if (objLinkedinUser.ProfilePicUrl.Contains("data:image/gif"))
                            objLinkedinUser.ProfilePicUrl = "";
                    }
                    else
                    {
                        var actionUrl = $"https://www.linkedin.com/voyager/api/identity/profiles/{publicIdentifier}";
                        var response = ldFunctions.GetInnerHttpHelper().GetRequest(actionUrl).Response;
                        var jsonHandler = JsonJArrayHandler.GetInstance;
                        var objJObject = jsonHandler.ParseJsonToJObject(response);
                        var rootUrl =jsonHandler.GetJTokenValue(objJObject,"miniProfile","picture","com.linkedin.common.VectorImage","rootUrl");
                        var artifactsArray =jsonHandler.GetJArrayElement(jsonHandler.GetJTokenValue(objJObject,"miniProfile","picture","com.linkedin.common.VectorImage","artifacts"));
                        var fileIdentifyingUrlPathSegment = jsonHandler.GetJTokenValue(artifactsArray?.LastOrDefault(), "fileIdentifyingUrlPathSegment");
                        objLinkedinUser.ProfilePicUrl = rootUrl + fileIdentifyingUrlPathSegment;
                        objLinkedinUser.HeadlineTitle = jsonHandler.GetJTokenValue(objJObject, "headline");
                    }
                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                    objLinkedinUser.ProfilePicUrl = "N/A";
                }


                if (!string.IsNullOrEmpty(objLinkedinUser.ProfilePicUrl) && objLinkedinUser.ProfilePicUrl != "N/A")
                {
                    objLinkedinUser.HasAnonymousProfilePicture = true;
                }
                else
                {
                    objLinkedinUser.HasAnonymousProfilePicture = false;
                    GlobusLogHelper.log.Info(Log.CustomMessage, dominatorAccountModel.AccountBaseModel.AccountNetwork,
                        dominatorAccountModel.AccountBaseModel.UserName, activityType,
                        "successfully filtered [" + profileUrl + "] having no profile picture");
                    return true;
                }

                #endregion
            }

            if (profileInput.Contains("<:>"))
            {
                #region Get FullName when ProfileInput contains <:>

                try
                {
                    fullName = Regex.Split(profileInput, "<:>")[1];
                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                }

                #endregion
            }
            else
            {
                var info = objGetDetailedUserInfo.GetFullNameDetailedInfo(profilePageSource, ldFunctions, publicIdentifier);
                forPublicidentify = info.PublicIdentifier;
                if (string.IsNullOrEmpty(info.Firstname))
                    info = objGetDetailedUserInfo.GetFullNameDetailedInfo(profilePageSource, ldFunctions, publicIdentifier);
                fullName = profileInput.Contains("company")
                    ? Utils.GetBetween(profilePageSource, "org-top-card-summary__title t-24 t-black truncate", ">")
                        .Replace("\"", "").Replace("title=", "")?.Trim()
                    : $"{info?.Firstname} {info?.Lastname}"?.Replace("string string", "")?.Replace(","," ");
                profileId = info.ProfileId;
            }

            objLinkedinUser.FullName = fullName;
            objLinkedinUser.FullName = Utils.InsertSpecialCharactersInCsv(objLinkedinUser.FullName);
            objLinkedinUser.ProfileId = profileId;
            objLinkedinUser.PublicIdentifier =
                string.IsNullOrEmpty(publicIdentifier) ? forPublicidentify : publicIdentifier;
            objLinkedinUser.ProfileUrl = profileUrl;
            return false;
        }


        public void DebugLogInfo(string response, string dataHeader = "")
        {
            try
            {
                var wholeData = $"{dataHeader} : ";
                if (string.IsNullOrEmpty(response?.Trim()) || !(response?.Trim().Length > 200))
                    return;
                wholeData += response?.Trim().Length > 200 ? response.Substring(0, 200) : response;
                GlobusLogHelper.log.Debug(wholeData);
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        public void DebugLogTrack(IResponseParameter responseParameter, string dataHeader = "", int trimLength = 500)
        {
            try
            {
                if (responseParameter == null)
                    return;
                ;

                var response = responseParameter.Response;
                var wholeData = $"{dataHeader} : ";
                if (responseParameter.Exception != null)
                {
                    wholeData += responseParameter.Exception.Message?.Trim().Length > trimLength
                        ? responseParameter.Exception.Message.Substring(0, trimLength)
                        : responseParameter.Exception.Message;
                    GlobusLogHelper.log.Debug(wholeData);
                }
                else if (!string.IsNullOrEmpty(response?.Trim()) && response?.Trim().Length > trimLength)
                {
                    wholeData += response?.Trim().Length > trimLength ? response.Substring(0, trimLength) : response;
                    GlobusLogHelper.log.Debug(wholeData);
                }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        public bool IsValidProfileUrlJobProcessResult(LinkedinPost objLinkedinPost, JobProcessResult jobProcessResult,
            ILdFunctions ldFunctions, DominatorAccountModel dominatorAccountModel, out string feedPageResponse)
        {
            feedPageResponse = ldFunctions.GetRequestUpdatedUserAgent(objLinkedinPost.PostLink, true);

            if (string.IsNullOrEmpty(feedPageResponse) || string.IsNullOrEmpty(objLinkedinPost.PostLink))
            {
                GlobusLogHelper.log.Info(Log.ActivityFailed, dominatorAccountModel.AccountBaseModel.AccountNetwork,
                    dominatorAccountModel.AccountBaseModel.UserName, "post not exist : ",
                    "[ " + objLinkedinPost.PostLink + " ]", "");
                jobProcessResult.IsProcessSuceessfull = false;
                return true;
            }

            return false;
        }
    }
}