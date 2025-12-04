using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using ThreadUtils;
using DominatorHouseCore;
using DominatorHouseCore.DatabaseHandler.LdTables.Account;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Enums.DHEnum;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using DominatorHouseCore.Process;
using DominatorHouseCore.Utility;
using EmbeddedBrowser;
using LinkedDominatorCore.Factories;
using LinkedDominatorCore.LDLibrary.DAL;
using LinkedDominatorCore.LDModel;
using LinkedDominatorCore.LDModel.GrowConnection;
using LinkedDominatorCore.LDUtility;

namespace LinkedDominatorCore.LDLibrary.Processor.Users
{
    public class ProfileUrlProcessor : BaseLinkedinUserProcessor, IQueryProcessor
    {
        private readonly IProcessScopeModel _processScopeModel;
        private BrowserWindow _browserWindow;

        public ProfileUrlProcessor(ILdJobProcess jobProcess,
            IDbCampaignService campaignService, ILdFunctionFactory ldFunctionFactory,
            IProcessScopeModel processScopeModel, IDelayService delayService) :
            base(jobProcess, campaignService, ldFunctionFactory, delayService, processScopeModel)
        {
            _processScopeModel = processScopeModel;
        }

        protected override void Process(QueryInfo queryInfo, ref JobProcessResult jobProcessResult)
        {
            try
            {
                var isChkPrivateBlackList = false;
                var IsChkSkipBlackListedUser = false;
                var isChkGroupBlackList = false;
                var isChkViewProfile = false;
                var isCheckedFilterProfileImageCheckbox = false;
                var profileUrl = queryInfo.QueryValue;
                var publicIdentifier = string.Empty;
                var interactedUser = new List<InteractedUsers>();
                if (string.IsNullOrEmpty(profileUrl) && (jobProcessResult.HasNoResult = true))
                    return;

                #region PublicIdentifier

                try
                {
                    if (queryInfo.QueryValue.Contains("<:>"))
                    {
                        // this condition is for when we trying to send connection request to
                        // exported scraped users from sales navigator campaign 
                        try
                        {
                            profileUrl = Regex.Split(queryInfo.QueryValue, "<:>")[0];
                        }
                        catch (Exception ex)
                        {
                            ex.DebugLog();
                        }

                        publicIdentifier = Utils.GetBetween(profileUrl + "**", "in/", "**").Replace("/", "");
                        if (!string.IsNullOrEmpty(publicIdentifier) && publicIdentifier.Length >= 39)
                            publicIdentifier = "";

                        interactedUser.Add(new InteractedUsers {ProfileId = publicIdentifier});
                    }
                    else
                    {
                        publicIdentifier = Utils.GetBetween(queryInfo.QueryValue + "**", "in/", "**").Replace("/", "");
                        // this is for normal profileUrl

                        AddCustomUrlToList(queryInfo.QueryValue, interactedUser);
                        profileUrl = queryInfo.QueryValue;
                    }
                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                }

                #endregion

                // only skip for custom profile url
                if (interactedUser.Count > 0 && RemoveOrSkipAlreadyInteractedUsers(interactedUser))
                {
                    jobProcessResult.HasNoResult = true;
                    return;
                }

                if (SetModel(queryInfo, jobProcessResult, ref IsChkSkipBlackListedUser,
                    ref isCheckedFilterProfileImageCheckbox, ref isChkPrivateBlackList, ref isChkGroupBlackList,
                    ref profileUrl, ref isChkViewProfile))
                    return;

                //Skip Blacklisted User
                if (IsChkSkipBlackListedUser && (isChkPrivateBlackList || isChkGroupBlackList) &&
                    manageBlacklistWhitelist.FilterBlackListedUser(publicIdentifier, isChkPrivateBlackList,
                        isChkGroupBlackList))
                {
                    GlobusLogHelper.log.Info(Log.CustomMessage, SocialNetworks.LinkedIn, DominatorAccountModel.UserName
                        , UserType.BlackListedUser, $"Filtered user {publicIdentifier}.");
                    jobProcessResult.HasNoResult = true;
                    return;
                }
                #region MyRegion

                LdJobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                var objLinkedinUser = GetUserInformation(profileUrl, isCheckedFilterProfileImageCheckbox);
                if (isChkViewProfile)
                    if (_browserWindow == null)
                    {
                        var automationExtension = new BrowserAutomationExtension(_browserWindow);
                        _browserWindow = automationExtension.ViewProfileBrowserInitializing(DominatorAccountModel,
                            queryInfo.QueryValue, out _);
                    }

                if (objLinkedinUser != null)
                {
                    LdJobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                    jobProcessResult = LdJobProcess.FinalProcess(new ScrapeResultNew
                    {
                        ResultUser = objLinkedinUser,
                        QueryInfo = queryInfo
                    });
                }

                jobProcessResult.HasNoResult = true;

                #endregion
            }
            catch (OperationCanceledException)
            {
                throw new OperationCanceledException("Operation Cancelled!");
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        private bool SetModel(QueryInfo queryInfo, JobProcessResult jobProcessResult, ref bool IsChkSkipBlackListedUser,
            ref bool isCheckedFilterProfileImageCheckbox, ref bool isChkPrivateBlackList, ref bool isChkGroupBlackList,
            ref string profileUrl, ref bool isChkViewProfile)
        {
            switch (ActivityType)
            {
                case ActivityType.ConnectionRequest:
                {
                    var connectionRequestModel = _processScopeModel.GetActivitySettingsAs<ConnectionRequestModel>();
                    isCheckedFilterProfileImageCheckbox =
                        connectionRequestModel.LDUserFilterModel.IsCheckedFilterProfileImageCheckbox;
                    IsChkSkipBlackListedUser = connectionRequestModel.IsChkSkipBlackListedUser;
                    isChkPrivateBlackList = connectionRequestModel.IsChkPrivateBlackList;
                    isChkGroupBlackList = connectionRequestModel.IsChkGroupBlackList;
                    break;
                }

                case ActivityType.UserScraper:
                {
                    var userScraperModel = _processScopeModel.GetActivitySettingsAs<UserScraperModel>();
                    isCheckedFilterProfileImageCheckbox =
                        userScraperModel.LDUserFilterModel.IsCheckedFilterProfileImageCheckbox;
                    IsChkSkipBlackListedUser = userScraperModel.IsChkSkipBlackListedUser;
                    isChkPrivateBlackList = userScraperModel.IsChkPrivateBlackList;
                    isChkGroupBlackList = userScraperModel.IsChkGroupBlackList;
                    isChkViewProfile = userScraperModel.IsViewProfileUsingEmbeddedBrowser;
                    break;
                }

                // ActivityType.SalesNavigatorUserScraper
                default:
                {
                    // here we blackList filtering part done at last in process

                    #region MyRegion

                    var profileId = string.Empty;
                    var authToken = string.Empty;
                    var userScraperModel = _processScopeModel.GetActivitySettingsAs<LDModel.SalesNavigatorScraper.UserScraperModel>();
                    IsChkSkipBlackListedUser =userScraperModel.IsChkSkipBlackListedUser;
                    isCheckedFilterProfileImageCheckbox =
                        userScraperModel.LDUserFilterModel.IsCheckedFilterProfileImageCheckbox;
                    isChkPrivateBlackList = userScraperModel.IsChkPrivateBlackList;
                    isChkGroupBlackList = userScraperModel.IsChkGroupBlackList;
                    profileUrl = queryInfo.QueryValue;

                    try
                    {
                        var id= Utils.GetBetween(profileUrl, "people/", ",");
                        profileId = string.IsNullOrEmpty(id) ?Utils.GetBetween(profileUrl,"lead/",",NAME_SEARCH"): id;
                        authToken = Regex.Split(profileUrl, ",")[2];
                        if (authToken.Contains("?"))
                            authToken = Utils.GetBetween("$$" + authToken, "$$", "?");
                    }
                    catch (Exception ex)
                    {
                        ex.DebugLog();
                    }


                    var linkedinUser = new LinkedinUser
                    {
                        ProfileId = profileId,
                        AuthToken = authToken,
                        ProfileUrl = profileUrl
                    };

                    if (RemoveOrSkipAlreadyInteractedLinkedInUsers(new List<LinkedinUser> {linkedinUser}))
                        return jobProcessResult.HasNoResult = true;


                    LdJobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                    LdJobProcess.FinalProcess(new ScrapeResultNew
                    {
                        ResultUser = linkedinUser,
                        QueryInfo = queryInfo
                    });
                    return jobProcessResult.HasNoResult = true;

                    #endregion
                }
            }

            return false;
        }
    }
}