using CommonServiceLocator;
using DominatorHouseCore;
using DominatorHouseCore.DatabaseHandler.FdTables.Accounts;
using DominatorHouseCore.Enums;
using DominatorHouseCore.FileManagers;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using DominatorHouseCore.Process;
using DominatorHouseCore.Request;
using DominatorHouseCore.Utility;
using FaceDominatorCore.FDEnums;
using FaceDominatorCore.FDLibrary.DAL;
using FaceDominatorCore.FDLibrary.FdClassLibrary;
using FaceDominatorCore.FDLibrary.FdFunctions;
using FaceDominatorCore.FDLibrary.FdFunctions.FdBrowserManager;
using FaceDominatorCore.FDLibrary.FdProcesses;
using FaceDominatorCore.FDModel.MessageModel;
using FaceDominatorCore.FDResponse.CommonResponse;
using FaceDominatorCore.Interface;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using ThreadUtils;
using Unity;

namespace FaceDominatorCore.FDLibrary.FdProcessors.UserProecessor
{
    public class BaseFbNonQueryUserProcessor : BaseFbProcessor
    {
        protected readonly IDbCampaignService CampaignService;

        private IResponseHandler _userInfoResponseHandler;

        private readonly IFdJobProcess _objFdJobProcess;

        protected readonly IFdUpdateAccountProcess FdUpdateAccountProcess;
        private readonly IDelayService _delayService;

        private readonly IAccountScopeFactory _accountScopeFactory;
        private readonly ITemplatesFileManager templatesFileManager;
        protected BaseFbNonQueryUserProcessor(IFdJobProcess jobProcess, IDbAccountServiceScoped dbAccountService,
         IDbCampaignServiceScoped campaignService, IFdRequestLibrary objFdRequestLibrary,
         IFdBrowserManager browserManager, IProcessScopeModel processScopeModel)
            : base(jobProcess, dbAccountService, campaignService, objFdRequestLibrary, browserManager, processScopeModel)
        {
            _accountScopeFactory = InstanceProvider.GetInstance<IAccountScopeFactory>();
            FdUpdateAccountProcess = InstanceProvider.GetInstance<IFdUpdateAccountProcess>();
            _objFdJobProcess = (FdJobProcess)jobProcess;
            CampaignService = campaignService;
            _userInfoResponseHandler = null;
            _delayService = InstanceProvider.GetInstance<IDelayService>();
            templatesFileManager = InstanceProvider.GetInstance<ITemplatesFileManager>();
        }

        protected override void Process(QueryInfo queryInfo, ref JobProcessResult jobProcessResult)
        {

        }

        protected void ProcessDataOfUsers(ref JobProcessResult jobProcessResult, List<FacebookUser> lstFilteredUserIds
            , string query, string queryValue)
        {
            _objFdJobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();

            FdFunctions.FdFunctions objFdFunctions = new FdFunctions.FdFunctions();

            lstFilteredUserIds = CheckBlacklistUser(lstFilteredUserIds);

            foreach (var user in lstFilteredUserIds)
            {
                _objFdJobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();

                try
                {
                    if (string.IsNullOrEmpty(user.UserId))
                        user.UserId = ObjFdRequestLibrary.GetFriendUserId(AccountModel, string.IsNullOrEmpty(user.ScrapedProfileUrl) ? user.ProfileUrl : user.ScrapedProfileUrl).UserId;

                    if (user.UserId == AccountModel.AccountBaseModel.UserId)
                        continue;

                    if (query == "Custom" || _ActivityType == ActivityType.SendGreetingsToFriends)
                        if (AlreadyInteractedUserCustom(user))
                            continue;

                        else if (AlreadyInteractedUser(user))
                            continue;

                    if (query == "UnfriendMutualFriend" && !user.HasMutualFriends)
                        continue;

                    if (query == "UnfollowMutualFriend" && !user.HasMutualFriends)
                        continue;

                    if (!CheckUserUniqueNess(jobProcessResult, user, _ActivityType))
                        continue;

                    if (_ActivityType == ActivityType.SendMessageToNewFriends)
                    {
                        if (AccountModel.IsRunProcessThroughBrowser
                            && Browsermanager.OpenFriendLinkAndSendMessage(AccountModel, user, true, true))
                        {
                            continue;
                        }
                        else if (ObjFdRequestLibrary.HasAlreadySentMessage(AccountModel, user.UserId))
                        {
                            Browsermanager.CloseAllMessageDialogues();
                            GlobusLogHelper.log.Info(Log.CustomMessage, AccountModel.AccountBaseModel.AccountNetwork,
                                AccountModel.AccountBaseModel.UserName, _ActivityType,
                                string.Format("LangKeyAlreadySentMessage".FromResourceDictionary(), $"{FdConstants.FbHomeUrl}{user.UserId}"));
                            continue;
                        }

                    }

                    if (AccountModel.IsRunProcessThroughBrowser && _ActivityType != ActivityType.SendGreetingsToFriends || AccountModel.IsRunProcessThroughBrowser && _ActivityType == ActivityType.SendGreetingsToFriends && string.IsNullOrEmpty(user.DateOfBirth))
                    {
                        var userSpecificWindow = _accountScopeFactory[$"{AccountModel.AccountId}{user.UserId}"]
                              .Resolve<IFdBrowserManager>();
                        _userInfoResponseHandler = userSpecificWindow.GetFullUserDetails(AccountModel, user);

                        userSpecificWindow.CloseBrowser(AccountModel);
                    }
                    else if (!AccountModel.IsRunProcessThroughBrowser)
                        _userInfoResponseHandler =
                        ObjFdRequestLibrary.GetDetailedInfoUserMobileScraper(user, AccountModel, false, true);
                    else
                    {
                        _userInfoResponseHandler = new FdUserInfoResponseHandlerMobile(new ResponseParameter() { Response = "" }, new List<string>(), user);
                    }

                    if (_userInfoResponseHandler == null && _ActivityType == ActivityType.SendFriendRequest)
                    {
                        GlobusLogHelper.log.Info(Log.CustomMessage, AccountModel.AccountBaseModel.AccountNetwork,
                            AccountModel.AccountBaseModel.UserName, _ActivityType,
                           string.Format("LangKeyCannotSendRequest".FromResourceDictionary(), $"{FdConstants.FbHomeUrl}{user.UserId}"));
                        continue;
                    }

                    _objFdJobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                    _userInfoResponseHandler.ObjFdScraperResponseParameters.FacebookUser.ScrapedProfileUrl =
                        string.IsNullOrEmpty(user.ScrapedProfileUrl) ? string.Empty : user.ScrapedProfileUrl;

                    _objFdJobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();

                    if (!string.IsNullOrEmpty(user.InteractionDate))
                        _userInfoResponseHandler.ObjFdScraperResponseParameters.FacebookUser.InteractionDate = user.InteractionDate;

                    if (_ActivityType == ActivityType.SendGreetingsToFriends)
                    {
                        var sendGreetingstoFrineds = JsonConvert.DeserializeObject<SendGreetingsToFriendsModel>(templatesFileManager.Get().FirstOrDefault(x => x.Id == JobProcess.TemplateId)?.ActivitySettings) ?? new SendGreetingsToFriendsModel();
                        var interactionDate = new DateTime();
                        //for beta verion Ui interacted data will be as b'day 
                        if (string.IsNullOrEmpty(user.DateOfBirth))
                            user.DateOfBirth = user.InteractionDate;
                        if (!string.IsNullOrEmpty(user.DateOfBirth)
                            || !string.IsNullOrEmpty(user.InteractionDate))
                        {
                            if (string.IsNullOrEmpty(_userInfoResponseHandler.ObjFdScraperResponseParameters.FacebookUser.DateOfBirth))
                                DateTime.TryParse(user.DateOfBirth, out interactionDate);
                            else
                                DateTime.TryParse(_userInfoResponseHandler.ObjFdScraperResponseParameters.FacebookUser.DateOfBirth, out interactionDate);
                        }
                        if (sendGreetingstoFrineds.IsFilterByAge &&
                            !sendGreetingstoFrineds.UserAge.InRange(DateTime.Now.Year - interactionDate.Year))
                        {
                            GlobusLogHelper.log.Info(Log.CustomMessage, AccountModel.AccountBaseModel.AccountNetwork,
                                AccountModel.AccountBaseModel.UserName, _ActivityType, string.Format("LangKeyNotMeetAgeFilter".FromResourceDictionary(), $"{FdConstants.FbHomeUrl}{_userInfoResponseHandler.ObjFdScraperResponseParameters.FacebookUser.UserId}"));
                            continue;
                        }
                        interactionDate = interactionDate.AddYears(DateTime.Now.Year - interactionDate.Year);
                        //Which is taking value that - or + for filtering by day
                        var addedDays = Math.Abs((interactionDate - DateTime.Now.Date).Days);
                        if (sendGreetingstoFrineds.IsFilterByDays && !sendGreetingstoFrineds.DaysBefore.InRange(addedDays))
                        {

                            if (sendGreetingstoFrineds.DaysBefore.EndValue < addedDays)
                            {
                                GlobusLogHelper.log.Info(Log.CustomMessage, AccountModel.AccountBaseModel.AccountNetwork,
                                    AccountModel.AccountBaseModel.UserName, _ActivityType, string.Format("LangKeyNoMoreBDayBetween".FromResourceDictionary(), $"{sendGreetingstoFrineds.DaysBefore.StartValue}", $"{sendGreetingstoFrineds.DaysBefore.EndValue}"));

                            }
                            if (sendGreetingstoFrineds.DaysBefore.EndValue < addedDays && lstFilteredUserIds.LastOrDefault().UserId == user.UserId)
                            {
                                jobProcessResult.HasNoResult = true;
                                jobProcessResult.IsProcessCompleted = true;
                                break;
                            }
                            else
                                continue;
                        }
                        query = Application.Current.FindResource("LangKeyGreetingOptions")?.ToString();
                        //It Will Make assign value that date and month matched then todays otherwise Upcoming
                        queryValue = interactionDate.Date == DateTime.Now.Date && interactionDate.Month == DateTime.Now.Month
                            ? Application.Current.FindResource("LangKeyTodaysBirthdays")?.ToString() :
                        Application.Current.FindResource("LangKeyUpcomingBirthdays")?.ToString();
                        var upcomingResult = DateTime.Compare(interactionDate, DateTime.Today);
                        //It will check previoius b'days if previous then goes for next
                        if (upcomingResult >= 0)
                        {
                            //it Will check whether messages selected for given query type value[Today's or Upcoming b'days]
                            bool upcoming = false;
                            sendGreetingstoFrineds.LstDisplayManageMessageModel.ForEach(x =>
                                    x.SelectedQuery.ForEach(y =>
                                    {
                                        if (y.Content.QueryValue == queryValue)
                                        {
                                            upcoming = true;
                                        }
                                    })
                                );
                            if (!upcoming)
                            {
                                //if upcoming b'day message not there then it will complete process
                                jobProcessResult.IsProcessCompleted = true;
                                GlobusLogHelper.log.Info(Log.CustomMessage, AccountModel.AccountBaseModel.AccountNetwork,
                                    AccountModel.AccountBaseModel.UserName, _ActivityType, "LangKeySelectedForTodayBDay".FromResourceDictionary());

                                break;
                            }
                        }
                        else
                            continue;
                    }
                    //else if(_ActivityType == ActivityType.SendMessageToNewFriends)
                    //{
                    //    var messaMessageRecentFriendsModel = JsonConvert.DeserializeObject<MessageRecentFriendsModel>(templatesFileManager.Get().FirstOrDefault(x => x.Id == JobProcess.TemplateId)?.ActivitySettings) ?? new MessageRecentFriendsModel();
                    //    var interactionDate = new DateTime();
                    //    if (string.IsNullOrEmpty(user.DateOfBirth))
                    //        user.DateOfBirth = user.InteractionDate;
                    //    if (!string.IsNullOrEmpty(user.DateOfBirth)
                    //        || !string.IsNullOrEmpty(user.InteractionDate))
                    //    {
                    //        if (string.IsNullOrEmpty(_userInfoResponseHandler.ObjFdScraperResponseParameters.FacebookUser.DateOfBirth))
                    //            DateTime.TryParse(user.InteractionDate, out interactionDate);
                    //        else
                    //            DateTime.TryParse(user.InteractionDate, out interactionDate);
                    //    }
                    //    var addedDays = (DateTime.Now - interactionDate).Days;
                    //    if (!messaMessageRecentFriendsModel.DaysBefore.InRange(addedDays))
                    //    {
                    //        GlobusLogHelper.log.Info(Log.CustomMessage, AccountModel.AccountBaseModel.AccountNetwork,
                    //           AccountModel.AccountBaseModel.UserName, _ActivityType, string.Format("LangKeyUserNotMeetDateFilter".FromResourceDictionary(), $"{FdConstants.FbHomeUrl}{_userInfoResponseHandler.ObjFdScraperResponseParameters.FacebookUser.UserId}"));
                    //        Browsermanager.CloseAllMessageDialogues();
                    //        continue;
                    //    }
                    //}
                    _objFdJobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                    FilterData(query, queryValue, ref jobProcessResult, _userInfoResponseHandler.ObjFdScraperResponseParameters.FacebookUser);
                    if (JobProcess.IsStopped())
                        break;

                }
                catch (OperationCanceledException)
                {
                    throw;
                }
                catch (Exception ex)
                {
                    jobProcessResult.IsProcessCompleted = true;
                    ex.DebugLog();
                }

                _objFdJobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
            }

            jobProcessResult.HasNoResult = _ActivityType != ActivityType.WithdrawSentRequest;
        }

        private void FilterData(string query, string queryValue, ref JobProcessResult jobProcessResult, FacebookUser objFacebookUser)
        {
            //Assighn dictionary Values to List
            AddLocationFilterValues();

            _objFdJobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();

            if (_ActivityType == ActivityType.Unfriend)
            {
                if (JobProcess.ModuleSetting.UnfriendOptionModel.IsFilterApplied)
                {
                    DateTime interactionDate;

                    if (DateTime.TryParse(objFacebookUser.InteractionDate, out interactionDate)
                        && _ActivityType == ActivityType.Unfriend)
                    {
                        var hours = (DateTime.Now - interactionDate).Hours;
                        var days = (DateTime.Now - interactionDate).Days;

                        if (days >= JobProcess.ModuleSetting.UnfriendOptionModel.DaysBefore)
                        {
                            _delayService.ThreadSleep(2000);
                            return;
                        }
                        else if (days != JobProcess.ModuleSetting.UnfriendOptionModel.DaysBefore && hours < JobProcess.ModuleSetting.UnfriendOptionModel.HoursBefore)
                        {
                            _delayService.ThreadSleep(2000);
                            return;
                        }

                    }

                }
                if (JobProcess.ModuleSetting.UnfriendOptionModel.IsNotPostOnWall)
                {
                    //JobProcess.ModuleSetting.UnfriendOptionModel.DaysNotPostedOnWall
                    Browsermanager.SearchByFriendUrl(AccountModel, FbEntityType.Post, objFacebookUser.ProfileUrl);

                    IResponseHandler responseHandler = Browsermanager.ScrollWindowAndGetDataForPost(AccountModel, FbEntityType.Post, 1, 0);

                    var data = responseHandler.ObjFdScraperResponseParameters.ListPostDetails.FirstOrDefault();
                    var day = (DateTime.Now - data.PostedDateTime).Days;
                    if (JobProcess.ModuleSetting.UnfriendOptionModel.DaysNotPostedOnWall > day)
                    {
                        GlobusLogHelper.log.Info(Log.CustomMessage, AccountModel.AccountBaseModel.AccountNetwork,
                            AccountModel.AccountBaseModel.UserName, _ActivityType,
                           string.Format("LangKeyUserNotMeetDateFilter".FromResourceDictionary(), $"{objFacebookUser.ProfileUrl}"));
                        return;
                    }
                    //var days = (DateTime.Now- responseHandler.ObjFdScraperResponseParameters.ListPostDetails.FirstOrDefault()).Days;
                }
            }
            if (_ActivityType == ActivityType.WithdrawSentRequest)
            {
                if (JobProcess.ModuleSetting.UnfriendOptionModel.IsFilterApplied)
                {
                    DateTime interactionDate;

                    if (DateTime.TryParse(objFacebookUser.InteractionDate, out interactionDate)
                        && _ActivityType == ActivityType.WithdrawSentRequest)
                    {
                        var hours = (DateTime.Now - interactionDate).Hours;
                        var days = (DateTime.Now - interactionDate).Days;

                        if (days <= JobProcess.ModuleSetting.UnfriendOptionModel.DaysBefore)
                        {

                            _delayService.ThreadSleep(2000);
                            GlobusLogHelper.log.Info(Log.CustomMessage, AccountModel.AccountBaseModel.AccountNetwork,
                                AccountModel.AccountBaseModel.UserName, _ActivityType, string.Format("LangKeyUserNotMeetDateFilter".FromResourceDictionary(), $"{FdConstants.FbHomeUrl}{objFacebookUser.UserId}"));
                            return;
                        }
                        else if (days != JobProcess.ModuleSetting.UnfriendOptionModel.DaysBefore && hours < JobProcess.ModuleSetting.UnfriendOptionModel.HoursBefore)
                        {
                            _delayService.ThreadSleep(2000);
                            return;
                        }

                    }
                }
            }

            if (JobProcess.ModuleSetting.GenderAndLocationFilter.IsFilterByGender)
            {
                if (JobProcess.ModuleSetting.GenderAndLocationFilter.SelectMaleUser
                     && !string.IsNullOrEmpty(objFacebookUser.Gender)
                    && objFacebookUser.Gender != Gender.Male.ToString())
                {
                    Thread.Sleep(2000);
                    GlobusLogHelper.log.Info(Log.CustomMessage, AccountModel.AccountBaseModel.AccountNetwork,
                        AccountModel.AccountBaseModel.UserName, _ActivityType, string.Format("LangKeyNotMeetGenderFilter".FromResourceDictionary(), $"{FdConstants.FbHomeUrl}{objFacebookUser.UserId}"));
                    return;
                }
                else if (JobProcess.ModuleSetting.GenderAndLocationFilter.SelectFemaleUser
                    && !string.IsNullOrEmpty(objFacebookUser.Gender)
                    && objFacebookUser.Gender != Gender.Female.ToString())
                {
                    Thread.Sleep(2000);
                    GlobusLogHelper.log.Info(Log.CustomMessage, AccountModel.AccountBaseModel.AccountNetwork,
                        AccountModel.AccountBaseModel.UserName, _ActivityType, string.Format("LangKeyNotMeetGenderFilter".FromResourceDictionary(), $"{FdConstants.FbHomeUrl}{objFacebookUser.UserId}"));
                    return;
                }

            }

            if (JobProcess.ModuleSetting.GenderAndLocationFilter.IsLocationFilterChecked)
            {
                JobProcess.ModuleSetting.GenderAndLocationFilter.ListLocationUrl.RemoveAll(x => string.IsNullOrEmpty(x));

                if (string.IsNullOrEmpty(objFacebookUser.Currentcity) && string.IsNullOrEmpty(objFacebookUser.Hometown))
                {
                    _delayService.ThreadSleep(2000);
                    _objFdJobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                    GlobusLogHelper.log.Info(Log.CustomMessage, AccountModel.AccountBaseModel.AccountNetwork,
                        AccountModel.AccountBaseModel.UserName, _ActivityType, string.Format("LangKeyNotMeetLocationFilter".FromResourceDictionary(), $"{FdConstants.FbHomeUrl}{objFacebookUser.UserId}"));
                    return;
                }

                if (!JobProcess.ModuleSetting.GenderAndLocationFilter.ListLocationUrl.Any(x =>
                        Regex.IsMatch(x, $@"\b{objFacebookUser.Currentcity.Replace(",", " ").Split(' ').FirstOrDefault()}\b", RegexOptions.IgnoreCase) ||
                        Regex.IsMatch(x, $@"\b{objFacebookUser.Hometown.Replace(",", " ").Split(' ').FirstOrDefault()}\b", RegexOptions.IgnoreCase)))
                {
                    _delayService.ThreadSleep(2000);
                    _objFdJobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                    GlobusLogHelper.log.Info(Log.CustomMessage, AccountModel.AccountBaseModel.AccountNetwork,
                        AccountModel.AccountBaseModel.UserName, _ActivityType, string.Format("LangKeyNotMeetLocationFilter".FromResourceDictionary(), $"{FdConstants.FbHomeUrl}{objFacebookUser.UserId}"));
                    return;
                }

                if (JobProcess.ModuleSetting.GenderAndLocationFilter.ListLocationUrl.FirstOrDefault
                (z => objFacebookUser.Currentcity.ToLower().Replace(",", string.Empty).Replace(" ", string.Empty)
                .Contains(z.ToLower().Replace(",", string.Empty).Replace(" ", string.Empty))
                || objFacebookUser.Hometown.ToLower().Replace(",", string.Empty).Replace(" ", string.Empty)
                .Contains(z.ToLower().Replace(",", string.Empty).Replace(" ", string.Empty))) != null)
                {
                    _objFdJobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                    GlobusLogHelper.log.Info(Log.CustomMessage, AccountModel.AccountBaseModel.AccountNetwork,
                        AccountModel.AccountBaseModel.UserName, _ActivityType, string.Format("LangKeyNotMeetLocationFilter".FromResourceDictionary(), $"{FdConstants.FbHomeUrl}{objFacebookUser.UserId}"));
                    return;
                }

            }

            if (JobProcess.ModuleSetting.GenderAndLocationFilter.IsMutualFriendFilterChecked &&
                _ActivityType == ActivityType.IncommingFriendRequest)
            {
                var mutualFriendList = ObjFdRequestLibrary.GetAllMutualFriends(AccountModel, objFacebookUser.UserId);

                if (JobProcess.ModuleSetting.GenderAndLocationFilter.IsNoOfMutualFriend)
                {
                    if (mutualFriendList.Count > JobProcess.ModuleSetting.GenderAndLocationFilter.TotalNoOfMutualFriend)
                        return;
                }
                if (JobProcess.ModuleSetting.GenderAndLocationFilter.IsFriendOfFriend)
                {
                    if (mutualFriendList.FirstOrDefault
                        (x => JobProcess.ModuleSetting.GenderAndLocationFilter.ListFriends.Any(y => x.UserId == y)) == null)
                        return;
                }
            }

            SendToPerformActivity(ref jobProcessResult, objFacebookUser,
                    query, queryValue);
        }

        // ReSharper disable once RedundantAssignment
        private void SendToPerformActivity(ref JobProcessResult jobProcessResult, FacebookUser objFacebookUser, string query, string queryValue)
        {
            _objFdJobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();

            QueryInfo objQuery = new QueryInfo
            {
                QueryTypeEnum = _ActivityType == ActivityType.SendGreetingsToFriends ? "GreetingOptions" :
              query,
                QueryType = query,
                QueryValue = queryValue
            };

            jobProcessResult = JobProcess.FinalProcess(new ScrapeResultNew()
            {
                ResultUser = objFacebookUser,
                QueryInfo = objQuery
            });
        }

        public List<FacebookUser> FilterBySourceType(QueryInfo queryInfo, List<FacebookUser> lstUserIds)
        {
            var lstUser = _dbAccountService.GetInteractedUsers(_ActivityType);

            var lstFilteredUserIds = new List<FacebookUser>();

            if (queryInfo.QueryType.Contains("Both"))
                return lstUserIds;

            else if (queryInfo.QueryType.Contains("Through Software"))
            {
                lstUser = _dbAccountService.GetInteractedUsers(ActivityType.SendFriendRequest);

                lstUserIds.ForEach(x =>
                {
                    if (lstUser.FirstOrDefault(y => y.UserId == x.UserId) != null)
                        lstFilteredUserIds.Add(x);
                });
            }
            else if (queryInfo.QueryType.Contains("Outside Software"))
            {
                lstUserIds.ForEach(x =>
                {
                    if (lstUser.FirstOrDefault(y => y.UserId == x.UserId) == null)
                        lstFilteredUserIds.Add(x);
                });
            }

            return lstFilteredUserIds;
        }


        public async Task<List<FacebookUser>> FilterBySourceTypeWithQueryList(QueryInfo queryInfo)
        {
            if (AccountModel.LastUpdateTime < DateTime.Now.AddDays(-1).ConvertToEpoch())
                await FdUpdateAccountProcess.CustomUpdateFriends(AccountModel, CancellationToken.None, Browsermanager);

            var friendsList = _dbAccountService.Get<Friends>().ToList();
            var lstUser = _dbAccountService.GetInteractedUsers(_ActivityType);
            lstUser = _dbAccountService.GetInteractedUsers(ActivityType.SendFriendRequest);

            var lstFilteredUserIds = new List<FacebookUser>();

            if (queryInfo.QueryType.Contains("Both"))
            {
                friendsList.ForEach(x =>
                {
                    lstFilteredUserIds.Add(new FacebookUser()
                    {
                        UserId = x.FriendId,
                        ProfileUrl = string.IsNullOrEmpty(x.ProfileUrl) ? $"{FdConstants.FbHomeUrl}{x.FriendId}" : x.ProfileUrl,
                        Username = x.FullName,
                        QueryType = "Both"
                    });
                });
            }
            if (queryInfo.QueryType.Contains("Through Software"))
            {

                friendsList.ForEach(x =>
                {
                    if (lstUser.FirstOrDefault(y => y.UserId == x.FriendId) != null || lstUser.FirstOrDefault(y => y.DetailedUserInfo.Contains(x.FriendId)) != null || lstUser.FirstOrDefault(y => y.ScrapedProfileUrl == x.ProfileUrl) != null || lstUser.FirstOrDefault(y => y.UserProfileUrl == x.ProfileUrl) != null)
                        lstFilteredUserIds.Add(new FacebookUser()
                        {
                            UserId = x.FriendId,
                            Username = x.FullName,
                            ProfileUrl = string.IsNullOrEmpty(x.ProfileUrl) ? $"{FdConstants.FbHomeUrl}{x.FriendId}" : x.ProfileUrl,
                            QueryType = "Through Software"
                        });
                });

            }
            if (queryInfo.QueryType.Contains("Outside Software"))
            {
                friendsList.ForEach(x =>
                {
                    if (lstUser.FirstOrDefault(y => y.UserId == x.FriendId) == null)
                        lstFilteredUserIds.Add(new FacebookUser()
                        {
                            UserId = x.FriendId,
                            Username = x.FullName,
                            ProfileUrl = string.IsNullOrEmpty(x.ProfileUrl) ? $"{FdConstants.FbHomeUrl}{x.FriendId}" : x.ProfileUrl,
                            QueryType = "Outside Software"
                        });
                });
            }

            return lstFilteredUserIds;
        }
    }
}
