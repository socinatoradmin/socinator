using CommonServiceLocator;
using DominatorHouseCore;
using DominatorHouseCore.DatabaseHandler.FdTables.Accounts;
using DominatorHouseCore.Enums;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using DominatorHouseCore.Process;
using DominatorHouseCore.Utility;
using FaceDominatorCore.FDEnums;
using FaceDominatorCore.FDLibrary.DAL;
using FaceDominatorCore.FDLibrary.FdClassLibrary;
using FaceDominatorCore.FDLibrary.FdFunctions;
using FaceDominatorCore.FDLibrary.FdFunctions.FdBrowserManager;
using FaceDominatorCore.FDLibrary.FdProcesses;
using FaceDominatorCore.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Unity;

namespace FaceDominatorCore.FDLibrary.FdProcessors.UserProecessor
{
    public class BaseFbUserProcessor : BaseFbProcessor
    {
        /*
                private readonly object _lockReachedMaxTweetActionPerUser = new object();
        */
        private ActivityType ActivityType;
        private IResponseHandler _userInfoResponseHandler;

        private readonly FdJobProcess _objFdJobProcess;

        protected readonly IAccountScopeFactory _accountScopeFactory;

        protected BaseFbUserProcessor(IFdJobProcess jobProcess, IDbAccountServiceScoped dbAccountService,
            IDbCampaignServiceScoped campaignService, IFdRequestLibrary objFdRequestLibrary,
            IFdBrowserManager browserManager, IProcessScopeModel processScopeModel)
            : base(jobProcess, dbAccountService, campaignService, objFdRequestLibrary, browserManager, processScopeModel)
        {
            ActivityType = jobProcess.ActivityType;
            _accountScopeFactory = InstanceProvider.GetInstance<IAccountScopeFactory>();
            _objFdJobProcess = (FdJobProcess)jobProcess;
            _userInfoResponseHandler = null;
        }

        protected override void Process(QueryInfo queryInfo, ref JobProcessResult jobProcessResult)
        {

        }

        public void ProcessDataOfUsers(QueryInfo queryInfo, ref JobProcessResult jobProcessResult,
            List<FacebookUser> objLstFacebookUser)
        {
            IFdBrowserManager pageSpecificWindow = null;

            try
            {
                if (queryInfo.QueryTypeEnum == "CustomProfileUrl" && !AccountModel.IsRunProcessThroughBrowser)
                {
                    objLstFacebookUser.ForEach(x =>
                    {
                        x.UserId = ObjFdRequestLibrary.GetFriendUserId(AccountModel, x.ProfileUrl).UserId;
                    });
                }

                objLstFacebookUser = CheckBlacklistUser(objLstFacebookUser);

                _objFdJobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();

                var pageDetails = new FanpageDetails()
                {
                    FanPageUrl = !queryInfo.QueryValue.Contains(FdConstants.FbHomeUrl)
                        ? FdConstants.FbHomeUrl + queryInfo?.QueryValue
                        : queryInfo?.QueryValue
                };

                if (objLstFacebookUser.Count == 0 && queryInfo.QueryTypeEnum == "CustomProfileUrl"
                  && _objFdJobProcess.ModuleSetting.SkipBlacklist.IsSkipBlackListUsers)
                {
                    GlobusLogHelper.log.Info(Log.CustomMessage,
                            AccountModel.AccountBaseModel.AccountNetwork,
                            AccountModel.AccountBaseModel.UserName, ActivityType,
                            "Skipped User , Present in Blacklist ");
                    //GlobusLogHelper.log.Info(Log.CustomMessage, AccountModel.AccountBaseModel.AccountNetwork,
                    //    AccountModel.AccountBaseModel.UserName, ActivityType, $"{"LangKeyAlreadyFriend".FromResourceDictionary()}");
                }

                if (queryInfo.QueryTypeEnum == "FanpageLikers")
                {
                    pageSpecificWindow = _accountScopeFactory[$"{AccountModel.AccountId}{pageDetails.FanPageID}"].Resolve<IFdBrowserManager>();

                    pageDetails = AccountModel.IsRunProcessThroughBrowser
                        ? pageSpecificWindow.GetFullPageDetails(AccountModel, pageDetails, isCloseBrowser: false).ObjFdScraperResponseParameters.FanpageDetails
                        : ObjFdRequestLibrary.GetPageDetailsFromUrl(AccountModel, queryInfo.QueryValue);
                }

                foreach (var fbuser in objLstFacebookUser)
                {
                    _objFdJobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                    FacebookUser user = fbuser;
                    try
                    {
                        if (string.IsNullOrEmpty(user.UserId) && string.IsNullOrEmpty(user.ProfileId) || user.UserId == AccountModel.AccountBaseModel.UserId
                            || !string.IsNullOrEmpty(user.ProfileId) && user.ProfileId == AccountModel.AccountBaseModel.ProfileId)
                            continue;

                        if (AlreadyInteractedUser(user))
                        {
                            GlobusLogHelper.log.Info(
                                Log.CustomMessage,
                                AccountModel.AccountBaseModel.AccountNetwork,
                                AccountModel.AccountBaseModel.UserName,
                                ActivityType,
                                string.Format("LangKeySkipedAlreadyInteractedUsers".FromResourceDictionary()
                                                    , $"{FdConstants.FbHomeUrl}{user.UserId}")
                                );
                            continue;
                        }
                        if (bool.TryParse(user.IsAlreadyFriend, out bool isAlreadyFriend) && isAlreadyFriend && _ActivityType == ActivityType.SendFriendRequest)
                        {
                            GlobusLogHelper.log.Info(Log.CustomMessage, AccountModel.AccountBaseModel.AccountNetwork,
                                        AccountModel.AccountBaseModel.UserName, _ActivityType,
                                        $"You are already friend with {FdConstants.FbHomeUrl}{user.UserId}");

                            continue;
                        }
                        var canSendParse = bool.TryParse(user.CanSendFriendRequest, out bool isCanSendFriend);
                        var canFollowParse = bool.TryParse(user.CanFollow, out bool isCanFollow);
                        if (canSendParse && canFollowParse && !isCanSendFriend && !isCanFollow && _ActivityType == ActivityType.SendFriendRequest)
                        {
                            GlobusLogHelper.log.Info(Log.CustomMessage, AccountModel.AccountBaseModel.AccountNetwork,
                                        AccountModel.AccountBaseModel.UserName, _ActivityType,
                                        $"Cannot send friend request to user with {FdConstants.FbHomeUrl}{user.UserId}");

                            continue;
                        }
                        if (!CheckUserUniqueNess(jobProcessResult, user, _ActivityType))
                            continue;

                        if (queryInfo.QueryTypeEnum == "FanpageLikers")
                        {

                            GlobusLogHelper.log.Info
                            (Log.CustomMessage, AccountModel.AccountBaseModel.AccountNetwork,
                                AccountModel.AccountBaseModel.UserName, ActivityType,
                                string.Format("LangKeyVerifyingUser".FromResourceDictionary(), $"{FdConstants.FbHomeUrl}{user.UserId}"));

                            IResponseHandler mobileUserFanPageScraper = null;/*new MobileUserFanPageScraperResponseHandler(new ResponseParameter() { Response = string.Empty });*/
                            List<FanpageDetails> fanpageDetailsList = new List<FanpageDetails>();

                            if (AccountModel.IsRunProcessThroughBrowser)
                            {
                                string userProfileURL = string.Empty;

                                if (!string.IsNullOrEmpty(user.UserId))
                                {
                                    userProfileURL = FdFunctions.FdFunctions.IsIntegerOnly(user.UserId) ? $"https://www.facebook.com/profile.php?id={user.UserId}&sk=likes_all"
                                                                : $"https://www.facebook.com/{user.UserId}/likes_all";
                                }
                                else
                                {
                                    userProfileURL = FdFunctions.FdFunctions.IsIntegerOnly(user.ProfileId) ? $"https://www.facebook.com/profile.php?id={user.ProfileId}&sk=likes_all"
                                        : $"https://www.facebook.com/{user.ProfileId}/likes_all";
                                }

                                pageSpecificWindow.LoadPageSource(AccountModel, userProfileURL, timeSec: 20);

                                if (pageSpecificWindow.ScrollWindowAndGetCurrentPage
                                    (AccountModel, FbEntityType.Fanpage, 100, pageDetails))
                                    fanpageDetailsList.Add(pageDetails);
                            }
                            else
                            {
                                if (ObjFdRequestLibrary.GetUserLikedPages(AccountModel, mobileUserFanPageScraper, user, pageDetails.FanPageUrl))
                                    fanpageDetailsList.Add(pageDetails);
                            }

                            if (fanpageDetailsList.FirstOrDefault(fanPage => fanPage.FanPageUrl == pageDetails.FanPageUrl) == null)
                            {
                                GlobusLogHelper.log.Info
                                (Log.CustomMessage, AccountModel.AccountBaseModel.AccountNetwork,
                                    AccountModel.AccountBaseModel.UserName, ActivityType,
                                    $"User have not Liked page {pageDetails.FanPageName}");
                                continue;
                            }
                            else
                                GlobusLogHelper.log.Info
                                    (Log.CustomMessage, AccountModel.AccountBaseModel.AccountNetwork,
                                        AccountModel.AccountBaseModel.UserName, ActivityType,
                                        $"User Have Liked page {pageDetails.FanPageName}");

                        }

                        if (AccountModel.IsRunProcessThroughBrowser && !user.IsAllDetailsScrapped)
                        {
                            var userSpecificWindow = _accountScopeFactory[$"{AccountModel.AccountId}{user.UserId}"]
                                  .Resolve<IFdBrowserManager>();
                            _userInfoResponseHandler = userSpecificWindow.GetFullUserDetails(AccountModel, user);
                            userSpecificWindow.CloseBrowser(AccountModel);
                        }
                        else if (!AccountModel.IsRunProcessThroughBrowser && !user.IsAllDetailsScrapped)
                        {
                            _userInfoResponseHandler = _ActivityType == ActivityType.SendFriendRequest
                                ? ObjFdRequestLibrary.GetDetailedInfoUserMobile(user, AccountModel, false, true)
                                : ObjFdRequestLibrary.GetDetailedInfoUserMobileScraper(user, AccountModel, false, true);
                        }
                        if (_userInfoResponseHandler != null)
                            user = _userInfoResponseHandler.ObjFdScraperResponseParameters?.FacebookUser;
                        canSendParse = bool.TryParse(user.CanSendFriendRequest, out isCanSendFriend);
                        canFollowParse = bool.TryParse(user.CanFollow, out isCanFollow);
                        if ((canSendParse && canFollowParse && !isCanSendFriend && !isCanFollow && _ActivityType == ActivityType.SendFriendRequest)
                            || (!user.CanSendMessage && _ActivityType == ActivityType.BroadcastMessages))
                        {
                            var activitylang = ActivityType == ActivityType.BroadcastMessages
                                ? "Cannot Send Message To {0}"
                                : "LangKeyCannotSendRequest".FromResourceDictionary();

                            GlobusLogHelper.log.Info
                            (Log.CustomMessage, AccountModel.AccountBaseModel.AccountNetwork,
                                AccountModel.AccountBaseModel.UserName, ActivityType,
                                string.Format($"{activitylang}", $"{FdConstants.FbHomeUrl}{user.UserId}"));
                            JobProcess.DelayBeforeNextActivity();
                            AddSkippedDataToDb(user.UserId);
                            continue;
                        }

                        if (!string.IsNullOrEmpty(user.Familyname)
                            && user.Familyname.Contains("\n ·")
                            && queryInfo.QueryTypeEnum != "ConnectedPeopleInMessenger")
                            continue;

                        _objFdJobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();

                        FilterData(queryInfo, ref jobProcessResult, user);
                    }
                    catch (OperationCanceledException)
                    {
                        throw;
                    }
                    catch (Exception ex)
                    {
                        if (ex.Message != "One or more errors occurred.")
                            ex.DebugLog();

                        jobProcessResult.IsProcessCompleted = true;
                    }


                    if (jobProcessResult.IsProcessCompleted)
                        break;

                    _objFdJobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                }

                if (AccountModel.IsRunProcessThroughBrowser && pageSpecificWindow != null)
                    pageSpecificWindow.CloseBrowser(AccountModel);
            }
            catch (OperationCanceledException)
            {
                if (AccountModel.IsRunProcessThroughBrowser)
                    pageSpecificWindow.CloseBrowser(AccountModel);
                throw;
            }

        }

        private void AddSkippedDataToDb(string userId)
        {
            JobProcess.DbAccountService.Add(new InteractedUsers
            {
                ActivityType = "SkippedUsers",
                UserId = userId,
            });
        }

        private void FilterData(QueryInfo queryInfo, ref JobProcessResult jobProcessResult, FacebookUser user)
        {
            AddLocationFilterValues();

            _objFdJobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();

            if (_ActivityType == ActivityType.Unfriend)
            {
                if (JobProcess.ModuleSetting.UnfriendOptionModel.IsFilterApplied)
                {
                    DateTime interactionDate;

                    if (DateTime.TryParse(user.InteractionDate, out interactionDate)
                        && _ActivityType == ActivityType.Unfriend)
                    {
                        var hours = (DateTime.Now - interactionDate).Hours;
                        var days = (DateTime.Now - interactionDate).Days;

                        if (days < JobProcess.ModuleSetting.UnfriendOptionModel.DaysBefore)
                        {
                            _objFdJobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();

                            GlobusLogHelper.log.Info(Log.CustomMessage, AccountModel.AccountBaseModel.AccountNetwork,
                                AccountModel.AccountBaseModel.UserName, ActivityType, string.Format("LangKeyUserNotMeetDateFilter".FromResourceDictionary(), $"{FdConstants.FbHomeUrl}{user.UserId}"));
                            return;
                        }
                        else if (days == JobProcess.ModuleSetting.UnfriendOptionModel.DaysBefore && hours < JobProcess.ModuleSetting.UnfriendOptionModel.HoursBefore)
                        {
                            _objFdJobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                            GlobusLogHelper.log.Info(Log.CustomMessage, AccountModel.AccountBaseModel.AccountNetwork,
                                AccountModel.AccountBaseModel.UserName, ActivityType, string.Format("LangKeyUserNotMeetDateFilter".FromResourceDictionary(), $"{FdConstants.FbHomeUrl}{user.UserId}"));
                            return;
                        }
                    }
                }
            }

            _objFdJobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();


            if (JobProcess.ModuleSetting.GenderAndLocationFilter.IsFilterByGender)
            {
                //selected all and User Should not be neither male nor female
                if (JobProcess.ModuleSetting.GenderAndLocationFilter.SelectMaleUser
                    && JobProcess.ModuleSetting.GenderAndLocationFilter.SelectFemaleUser
                    && (user.Gender.ToLower() != Gender.Male.ToString().ToLower() && user.Gender.ToLower() != Gender.Female.ToString().ToLower()))
                {
                    _objFdJobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                    GlobusLogHelper.log.Info(Log.CustomMessage, AccountModel.AccountBaseModel.AccountNetwork,
                        AccountModel.AccountBaseModel.UserName, ActivityType, string.Format("LangKeyNotMeetGenderFilter".FromResourceDictionary(), $"{FdConstants.FbHomeUrl}{user.UserId}"));
                    return;
                }
                else if (!(JobProcess.ModuleSetting.GenderAndLocationFilter.SelectMaleUser
                    && JobProcess.ModuleSetting.GenderAndLocationFilter.SelectFemaleUser))
                {
                    //selected Male And Came User To be Female
                    if (JobProcess.ModuleSetting.GenderAndLocationFilter.SelectMaleUser
                    && user.Gender.ToLower() != Gender.Male.ToString().ToLower())
                    {
                        _objFdJobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                        GlobusLogHelper.log.Info(Log.CustomMessage, AccountModel.AccountBaseModel.AccountNetwork,
                            AccountModel.AccountBaseModel.UserName, ActivityType, string.Format("LangKeyNotMeetGenderFilter".FromResourceDictionary(), $"{FdConstants.FbHomeUrl}{user.UserId}"));
                        return;
                    }
                    //selected Female and came user is not be Female
                    else if (JobProcess.ModuleSetting.GenderAndLocationFilter.SelectFemaleUser
                        && user.Gender.ToLower() != Gender.Female.ToString().ToLower())
                    {
                        _objFdJobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                        GlobusLogHelper.log.Info(Log.CustomMessage, AccountModel.AccountBaseModel.AccountNetwork,
                            AccountModel.AccountBaseModel.UserName, ActivityType, string.Format("LangKeyNotMeetGenderFilter".FromResourceDictionary(), $"{FdConstants.FbHomeUrl}{user.UserId}"));
                        return;
                    }
                }
            }

            _objFdJobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();

            if (JobProcess.ModuleSetting.GenderAndLocationFilter.IsLocationFilterChecked)
            {
                JobProcess.ModuleSetting.GenderAndLocationFilter.ListLocationUrl.RemoveAll(x => string.IsNullOrEmpty(x));
                var country = !string.IsNullOrEmpty(user.Currentcity) ? FdConstants.getCountryFromAddress(user.Currentcity) : "";
                country = !string.IsNullOrEmpty(country) ? country : string.IsNullOrEmpty(country) && !string.IsNullOrEmpty(user.Hometown) ? FdConstants.getCountryFromAddress(user.Hometown) : "";
                if (string.IsNullOrEmpty(user.Currentcity) && string.IsNullOrEmpty(user.Hometown)
                    || !string.IsNullOrEmpty(country) && !JobProcess.ModuleSetting.GenderAndLocationFilter.ListCountry.Any(x => Regex.IsMatch(x, $@"\b{country}\b", RegexOptions.IgnoreCase)))
                {
                    _objFdJobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                    GlobusLogHelper.log.Info(Log.CustomMessage, AccountModel.AccountBaseModel.AccountNetwork,
                        AccountModel.AccountBaseModel.UserName, ActivityType, string.Format("LangKeyNotMeetLocationFilter".FromResourceDictionary(), $"{FdConstants.FbHomeUrl}{user.UserId}"));
                    return;
                }
                if (!JobProcess.ModuleSetting.GenderAndLocationFilter.ListLocationUrl.Any(x =>
                       Regex.IsMatch(x, $@"\b{user.Currentcity.Replace(",", " ").Split(' ').FirstOrDefault()}\b"
                       , RegexOptions.IgnoreCase) || Regex.IsMatch(x, $@"\b{user.Hometown.Replace(",", " ").Split(' ').FirstOrDefault()}\b"
                       , RegexOptions.IgnoreCase)))
                {
                    _objFdJobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                    GlobusLogHelper.log.Info(Log.CustomMessage, AccountModel.AccountBaseModel.AccountNetwork,
                        AccountModel.AccountBaseModel.UserName, ActivityType, string.Format("LangKeyNotMeetLocationFilter".FromResourceDictionary(), $"{FdConstants.FbHomeUrl}{user.UserId}"));
                    return;
                }

                if (JobProcess.ModuleSetting.GenderAndLocationFilter.ListLocationUrl.FirstOrDefault
                (z => user.Currentcity.ToLower().Replace(",", string.Empty).Replace(" ", string.Empty)
                .Contains(z.ToLower().Replace(",", string.Empty).Replace(" ", string.Empty))
                || user.Hometown.ToLower().Replace(",", string.Empty).Replace(" ", string.Empty)
                .Contains(z.ToLower().Replace(",", string.Empty).Replace(" ", string.Empty))) == null)
                {
                    _objFdJobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                    GlobusLogHelper.log.Info(Log.CustomMessage, AccountModel.AccountBaseModel.AccountNetwork,
                        AccountModel.AccountBaseModel.UserName, ActivityType, string.Format("LangKeyNotMeetLocationFilter".FromResourceDictionary(), $"{FdConstants.FbHomeUrl}{user.UserId}"));
                    return;
                }

                var currentCity = user.Currentcity.Split(',');
                var homeTown = user.Hometown.Split(',');
                var checkCityAndTown = JobProcess.ModuleSetting.GenderAndLocationFilter.ListLocationUrl.Any(x =>
                {
                    if (x.Equals(currentCity[0]) || x.Equals(homeTown[0]))
                    {
                        return true;
                    }
                    return false;
                });

                if (!checkCityAndTown)
                {
                    _objFdJobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                    GlobusLogHelper.log.Info(Log.CustomMessage, AccountModel.AccountBaseModel.AccountNetwork,
                        AccountModel.AccountBaseModel.UserName, ActivityType, string.Format("LangKeyNotMeetLocationFilter".FromResourceDictionary(), $"{FdConstants.FbHomeUrl}{user.UserId}"));
                    return;
                }

            }

            _objFdJobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();


            if (JobProcess.ModuleSetting.GenderAndLocationFilter.IsMutualFriendFilterChecked &&
                _ActivityType == ActivityType.IncommingFriendRequest)
            {
                var mutualFriendList = ObjFdRequestLibrary.GetAllMutualFriends(AccountModel, user.UserId);

                if (JobProcess.ModuleSetting.GenderAndLocationFilter.IsNoOfMutualFriend)
                {
                    if (mutualFriendList.Count <= JobProcess.ModuleSetting.GenderAndLocationFilter.TotalNoOfMutualFriend)
                    {
                        _objFdJobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                        GlobusLogHelper.log.Info(Log.CustomMessage, AccountModel.AccountBaseModel.AccountNetwork,
                            AccountModel.AccountBaseModel.UserName, ActivityType, string.Format("LangKeyUserNotMeetFilter".FromResourceDictionary(), $"{FdConstants.FbHomeUrl}{user.UserId}"));
                        return;
                    }
                }
                if (JobProcess.ModuleSetting.GenderAndLocationFilter.IsFriendOfFriend)
                {
                    if (mutualFriendList.FirstOrDefault
                        (x => JobProcess.ModuleSetting.GenderAndLocationFilter.ListFriends.Any(y => x.UserId == y)) == null)
                    {
                        _objFdJobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                        GlobusLogHelper.log.Info(Log.CustomMessage, AccountModel.AccountBaseModel.AccountNetwork,
                            AccountModel.AccountBaseModel.UserName, ActivityType, string.Format("LangKeyUserNotMeetFilter".FromResourceDictionary(), $"{FdConstants.FbHomeUrl}{user.UserId}"));
                        return;
                    }
                }
            }

            _objFdJobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();

            SendToPerformActivity(ref jobProcessResult, user, queryInfo);

        }


        // ReSharper disable once RedundantAssignment
        public void SendToPerformActivity(ref JobProcessResult jobProcessResult, FacebookUser objFacebookUser,
                    QueryInfo queryInfo)
        {

            _objFdJobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();

            jobProcessResult = JobProcess.FinalProcess(new ScrapeResultNew()
            {
                ResultUser = objFacebookUser,
                QueryInfo = queryInfo
            });
        }

    }
}
