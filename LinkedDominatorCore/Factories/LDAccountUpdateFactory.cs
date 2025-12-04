#region MyRegion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using CommonServiceLocator;
using DominatorHouseCore;
using DominatorHouseCore.DatabaseHandler.LdTables.Account;
using DominatorHouseCore.Diagnostics;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Enums.DHEnum;
using DominatorHouseCore.Extensions;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using DominatorHouseCore.Process.ExecutionCounters;
using DominatorHouseCore.Utility;
using DominatorHouseCore.ViewModel;
using LinkedDominatorCore.LDLibrary;
using LinkedDominatorCore.LDLibrary.DAL;
using LinkedDominatorCore.LDModel;
using LinkedDominatorCore.LDUtility;
using LinkedDominatorCore.Utility;
using Unity;

#endregion

namespace LinkedDominatorCore.Factories
{
    public interface ILdAccountUpdateFactory : IAccountUpdateFactoryAsync
    {
        Task UpdateConnections(DominatorAccountModel dominatorAccountModel, ILdFunctions ldFunctions,
            IDbAccountService dbAccountService);

        void SaveCount(DominatorAccountModel dominatorAccountModel);

        void DbInsertionHelper(DominatorAccountModel dominatorAccountModel, List<Connections> connectionsList);

        Task UpdateInvitationsSent(DominatorAccountModel dominatorAccountModel, ILdFunctions ldFunctions,
            IDbAccountService dbAccountService);

        Task UpdateGroups(DominatorAccountModel dominatorAccountModel, ILdFunctions ldFunctions,
            IDbAccountService dbAccountService);
    }

    public class LdAccountUpdateFactory : ILdAccountUpdateFactory
    {
        private readonly IClassMapper _classMapper;

        private readonly LdDataHelper _ldDataHelper = LdDataHelper.GetInstance;

        // ReSharper disable once InconsistentNaming
        public IEntityCountersManager entityCountersManager;

        public LdAccountUpdateFactory(IAccountScopeFactory accountScopeFactory)
        {
            _accountScopeFactory = accountScopeFactory;
            _classMapper = InstanceProvider.GetInstance<IClassMapper>();
        }


        public static ILdAccountUpdateFactory Instance
            => _instance ?? (_instance = InstanceProvider.GetInstance<ILdAccountUpdateFactory>());

        public bool CheckStatus(DominatorAccountModel dominatorAccountModel)
        {
            return CheckStatusAsync(dominatorAccountModel, dominatorAccountModel.Token).Result;
        }

        public bool SolveCaptchaManually(DominatorAccountModel accountModel)
        {
            return false;
        }



        public async Task<bool> CheckStatusAsync(DominatorAccountModel dominatorAccountModel,
            CancellationToken cancellationToken)
        {
            // Thread.Sleep(17000);
            try
            {
                GlobusLogHelper.log.Info(Log.AccountLogin, dominatorAccountModel.AccountBaseModel.AccountNetwork,
                    dominatorAccountModel.AccountBaseModel.UserName);
                // first initialize login process for that account using account id
                // automatically call other classes constructor required inside this
                var _logInProcess = _accountScopeFactory[dominatorAccountModel.AccountId].Resolve<ILdLogInProcess>();
                _logInProcess.AssignFunctions(dominatorAccountModel, _accountScopeFactory);
                _logInProcess.IsCheckAccountStatus = true;
                if (dominatorAccountModel.IsRunProcessThroughBrowser)
                    _logInProcess.LoginWithBrowserMethod(dominatorAccountModel, cancellationToken);
                else
                    await _logInProcess.LoginWithDataBaseCookiesAsync(dominatorAccountModel, true,
                        dominatorAccountModel.Token);

                if (dominatorAccountModel.IsUserLoggedIn)
                    GlobusLogHelper.log.Info(Log.SuccessfulLogin, dominatorAccountModel.AccountBaseModel.AccountNetwork,
                        dominatorAccountModel.AccountBaseModel.UserName);
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
            finally
            {
                LDAccountsBrowserDetails.CheckAndCloseBrowser(dominatorAccountModel,
                    !dominatorAccountModel.IsRunProcessThroughBrowser, BrowserInstanceType.CheckAccountStatus);
            }

            return dominatorAccountModel.IsUserLoggedIn;
        }

        public void UpdateDetails(DominatorAccountModel dominatorAccountModel)
        {
            UpdateDetailsAsync(dominatorAccountModel, dominatorAccountModel.Token).Wait();
        }

        public async Task UpdateDetailsAsync(DominatorAccountModel dominatorAccountModel,
            CancellationToken cancellationToken)
        {
            try
            {
                // if  not already loggedIn we send it for logging in
                var _logInProcess = _accountScopeFactory[dominatorAccountModel.AccountId].Resolve<ILdLogInProcess>();
                _logInProcess.IsCheckAccountStatus = true;

                if (dominatorAccountModel.IsRunProcessThroughBrowser)
                    _logInProcess.LoginWithBrowserMethod(dominatorAccountModel, cancellationToken);
                else
                    await _logInProcess.LoginWithDataBaseCookiesAsync(dominatorAccountModel, true,
                        dominatorAccountModel.Token);

                // if  successfully logged in we send it for further account connection update process
                if (dominatorAccountModel.IsUserLoggedIn)
                {
                    var ldFunctions = _accountScopeFactory[dominatorAccountModel.AccountId]
                        .Resolve<ILdFunctionFactory>().LdFunctions;

                    try
                    {
                        //use this(dbAccountService) local variable  instead of using instance variable (_dbAccountService)
                        // otherwise it raise issue when multiple account friendship updating
                        // it saves the all connections to db of last account
                        // therefore we are using local account service inside
                        // note: don't remove this comment it might help further.
                        var dbAccountService = new DbAccountService(dominatorAccountModel);

                        await UpdateConnections(dominatorAccountModel, ldFunctions, dbAccountService);

                        await UpdateGroups(dominatorAccountModel, ldFunctions, dbAccountService);

                        await UpdateInvitationsSent(dominatorAccountModel, ldFunctions, dbAccountService);

                        await UpdatePages(dominatorAccountModel, ldFunctions, dbAccountService);

                        GlobusLogHelper.log.Info(Log.DetailsUpdated,
                            dominatorAccountModel.AccountBaseModel.AccountNetwork,
                            dominatorAccountModel.AccountBaseModel.UserName, " Account's all details");
                    }
                    catch (Exception ex)
                    {
                        ex.DebugLog();
                    }

                    dominatorAccountModel.Token.ThrowIfCancellationRequested();
                    if (dominatorAccountModel.DisplayColumnValue1 != null &&
                        dominatorAccountModel.DisplayColumnValue2 != null)
                    {
                        AddToDailyGrowth(dominatorAccountModel.AccountId,
                            dominatorAccountModel.DisplayColumnValue1.Value,
                            dominatorAccountModel.DisplayColumnValue2.Value,
                            dominatorAccountModel.DisplayColumnValue3.Value);

                        SocinatorAccountBuilder.Instance(dominatorAccountModel.AccountBaseModel.AccountId)
                            .AddOrUpdateDominatorAccountBase(dominatorAccountModel.AccountBaseModel)
                            .AddOrUpdateCookies(dominatorAccountModel.Cookies)
                            .AddOrUpdateBrowserCookies(dominatorAccountModel.BrowserCookies)
                            .AddOrUpdateDisplayColumn1(dominatorAccountModel.DisplayColumnValue1)
                            .AddOrUpdateDisplayColumn2(dominatorAccountModel.DisplayColumnValue2)
                            .AddOrUpdateDisplayColumn3(dominatorAccountModel.DisplayColumnValue3)
                            .AddOrUpdateDisplayColumn4(dominatorAccountModel.DisplayColumnValue4)
                            .SaveToBinFile();
                    }
                }
            }
            catch (OperationCanceledException)
            {
                throw new OperationCanceledException();
            }
            catch (AggregateException ae)
            {
                ae.HandleOperationCancellation();
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
            finally
            {
                LDAccountsBrowserDetails.CheckAndCloseBrowser(dominatorAccountModel,
                    !dominatorAccountModel.IsRunProcessThroughBrowser, BrowserInstanceType.CheckAccountStatus);
            }
        }


        public async Task UpdateConnections(DominatorAccountModel dominatorAccountModel, ILdFunctions ldFunctions,
            IDbAccountService dbAccountService)
        {
            try
            {
                var allConnections = dbAccountService.GetConnections().ToList();
                //here we are updating connection if any connection profile url is empty
                foreach (var lst in allConnections.Where(x => x.ProfileUrl == null))
                {
                    lst.ProfileUrl = "https://www.linkedin.com/in/" + lst.PublicIdentifier;
                }
                GlobusLogHelper.log.Info(Log.UpdatingDetails, dominatorAccountModel.AccountBaseModel.AccountNetwork,
                    dominatorAccountModel.AccountBaseModel.UserName, "connections");
                // SetConnectionCount
                _ldDataHelper.GetAndSetConnectionCount(dominatorAccountModel, ldFunctions);

                if (CheckAndRemoveConnection(dominatorAccountModel, allConnections.Count,
                    (DbAccountService) dbAccountService))
                    allConnections = dbAccountService.GetConnections().ToList();

                SaveCount(dominatorAccountModel);
                var maxAlreadyPresentCount = 250;
                var currentAlreadyPresentCount = 0;


                #region Get All Connections and Store in DB

                try
                {
                    var connectionApi = dominatorAccountModel.IsRunProcessThroughBrowser
                        ? "https://www.linkedin.com/mynetwork/invite-connect/connections/"
                        : "https://www.linkedin.com/voyager/api/relationships/connections?sortType=RECENTLY_ADDED";
                    var connectionApiWithFirstPage =
                        connectionApi + (dominatorAccountModel.IsRunProcessThroughBrowser ? "" : "&count=100&start=0");
                    var searchConnectionResponseHandler =
                        await ldFunctions.SearchForLinkedinConnectionsAsync(connectionApiWithFirstPage,
                            dominatorAccountModel.Token);
                    var loopCount = 0;
                    var start = 0;

                    var imapper = _classMapper.GetIMapper<LinkedinUser, Connections>();
                    while (searchConnectionResponseHandler.Success &&
                           currentAlreadyPresentCount < maxAlreadyPresentCount)
                    {
                        var actionUrl = dominatorAccountModel.IsRunProcessThroughBrowser
                            ? ""
                            : $"{connectionApi}&count=100&start={start}";

                        if (loopCount > 0)
                            searchConnectionResponseHandler =
                                await ldFunctions.SearchForLinkedinConnectionsAsync(actionUrl,
                                    dominatorAccountModel.Token);
                        if (searchConnectionResponseHandler.ConnectionsList.Count > 0)
                        {
                            var connectionsList = new List<Connections>();
                            foreach (var linkedinUser in searchConnectionResponseHandler.ConnectionsList)
                                try
                                {
                                    var connections = new Connections();
                                    _classMapper.MapModelClass(linkedinUser, ref connections, imapper);
                                    connections.IsDetailedUserInfoVisible = true;
                                    connections.HasAnonymousProfilePicture = connections.HasAnonymousProfilePicture;
                                    connections.ConnectionType = ConnectionType.FirstDegree;
                                    connections.InteractionTimeStamp = DateTimeUtilities.GetEpochTime();
                                    connections.PublicIdentifier = linkedinUser.PublicIdentifier;
                                    connections.ProfilePicUrl = linkedinUser.ProfilePicUrl;                                    
                                    if (allConnections.Any(y => y.ProfileUrl.Equals(connections.ProfileUrl)))
                                    {
                                        ++currentAlreadyPresentCount;
                                    }
                                    else
                                    {
                                        allConnections.Add(connections);
                                        connectionsList.Add(connections);
                                    }
                                }
                                catch (Exception e)
                                {
                                    e.DebugLog();
                                }
                            if (connectionsList.Count != 0)
                                dbAccountService.AddRange(connectionsList);
                        }
                        else
                        {
                            Utils.RandomDelay(8000, 15000);
                            break;
                        }

                        #region Paginate ConnectionApi

                        start = start + 100;
                        loopCount += 1;
                        Utils.RandomDelay(3000, 7000);

                        #endregion

                        var reached = dominatorAccountModel.DisplayColumnValue1 * 10 / 100;
                        if (dominatorAccountModel.IsRunProcessThroughBrowser && allConnections.Count >=
                            dominatorAccountModel.DisplayColumnValue1 - reached)
                            break;
                    }

                    GlobusLogHelper.log.Info(Log.DetailsUpdated, dominatorAccountModel.AccountBaseModel.AccountNetwork,
                        dominatorAccountModel.AccountBaseModel.UserName, "connections");
                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                }

                #endregion
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        public void SaveCount(DominatorAccountModel dominatorAccountModel)
        {
            SocinatorAccountBuilder.Instance(dominatorAccountModel.AccountBaseModel.AccountId)
                .AddOrUpdateDominatorAccountBase(dominatorAccountModel.AccountBaseModel)
                .AddOrUpdateCookies(dominatorAccountModel.Cookies)
                .AddOrUpdateDisplayColumn1(dominatorAccountModel.DisplayColumnValue1)
                .AddOrUpdateDisplayColumn2(dominatorAccountModel.DisplayColumnValue2)
                .AddOrUpdateDisplayColumn3(dominatorAccountModel.DisplayColumnValue3)
                .AddOrUpdateDisplayColumn4(dominatorAccountModel.DisplayColumnValue4)
                .SaveToBinFile();
        }

        //Method to fetch daily Growth
        public DailyStatisticsViewModel GetDailyGrowth(string accountId, string username, GrowthPeriod period)
        {
            var response = new DailyStatisticsViewModel();
            try
            {
                var dbAccountService = new DbAccountService(new DominatorAccountModel {AccountId = accountId});
                List<DailyStatitics> growthList;
                if (period == GrowthPeriod.Daily)
                {
                    var startDate = DateTime.Today.AddDays(-1);
                    growthList = dbAccountService.Get<DailyStatitics>(x => x.Date >= startDate).OrderBy(x => x.Date)
                        .ToList();
                }
                else if (period == GrowthPeriod.Weekly)
                {
                    growthList = dbAccountService.Get<DailyStatitics>().Where(x => x.Date >= DateTime.Today.AddDays(-7))
                        .OrderBy(x => x.Date).ToList();
                }
                else if (period == GrowthPeriod.Monthly)
                {
                    growthList = dbAccountService.Get<DailyStatitics>()
                        .Where(x => x.Date >= DateTime.Today.AddMonths(-1)).OrderBy(x => x.Date).ToList();
                }
                else
                {
                    growthList = dbAccountService.Get<DailyStatitics>().OrderBy(x => x.Date).ToList();
                }

                if (growthList.Count > 0)
                {
                    var oldRecord = growthList.FirstOrDefault();
                    var latestRecord = growthList.LastOrDefault();
                    if (latestRecord != null && oldRecord != null)
                    {
                        response.GrowthColumnValue1 = latestRecord.Connections - oldRecord.Connections;
                        response.GrowthColumnValue2 = latestRecord.Posts - oldRecord.Posts;
                        response.GrowthColumnValue3 = latestRecord.LinkedinGroups - oldRecord.LinkedinGroups;
                    }
                }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
                response = null;
            }

            return response;
        }

        public List<DailyStatisticsViewModel> GetDailyGrowthForAccount(string accountId, GrowthChartPeriod period)
        {
            var isMonthly = false;
            var responseList = new List<DailyStatisticsViewModel>();
            var dbAccountService = new DbAccountService(new DominatorAccountModel {AccountId = accountId});
            try
            {
                List<DailyStatitics> growthList;
                int counter;
                switch (period)
                {
                    case GrowthChartPeriod.PastDay:
                        growthList = dbAccountService.Get<DailyStatitics>()
                            .Where(x => x.Date >= DateTime.Today.AddDays(-1)).OrderBy(x => x.Date).ToList();
                        counter = 2;
                        break;
                    case GrowthChartPeriod.Past3Months:
                    {
                        counter = 3;
                        isMonthly = true;
                        growthList = dbAccountService.Get<DailyStatitics>()
                            .Where(x => x.Date >= DateTime.Today.AddMonths(-3)).OrderBy(x => x.Date).ToList();

                        var q = from gt in growthList
                            let dt = gt.Date
                            group gt by new {m = dt.Month}
                            into g
                            select g.OrderByDescending(t => t.Date).FirstOrDefault();
                        growthList = q.ToList();
                        break;
                    }

                    case GrowthChartPeriod.Past30Days:
                    {
                        counter = 30;
                        growthList = dbAccountService.Get<DailyStatitics>()
                            .Where(x => x.Date >= DateTime.Today.AddDays(-30)).OrderBy(x => x.Date).ToList();
                        var q = from gt in growthList
                            let dt = gt.Date
                            group gt by new {y = dt.Year, m = dt.Month, d = dt.Day}
                            into g
                            select g.OrderByDescending(t => t.Date).LastOrDefault();
                        growthList = q.ToList();
                        break;
                    }

                    case GrowthChartPeriod.Past6Months:
                    {
                        counter = 6;
                        isMonthly = true;
                        growthList = dbAccountService.Get<DailyStatitics>()
                            .Where(x => x.Date >= DateTime.Today.AddMonths(-6)).OrderBy(x => x.Date).ToList();

                        var q = from gt in growthList
                            let dt = gt.Date
                            group gt by new {m = dt.Month}
                            into g
                            select g.OrderByDescending(t => t.Date).LastOrDefault();
                        growthList = q.ToList();
                        break;
                    }

                    case GrowthChartPeriod.PastWeek:
                    {
                        counter = 7;

                        growthList = dbAccountService.Get<DailyStatitics>()
                            .Where(x => x.Date >= DateTime.Today.AddDays(-7)).OrderBy(x => x.Date).ToList();
                        var q = from gt in growthList
                            let dt = gt.Date
                            group gt by new {y = dt.Year, m = dt.Month, d = dt.Day}
                            into g
                            select g.OrderByDescending(t => t.Date).LastOrDefault();
                        growthList = q.ToList();
                        break;
                    }

                    default:
                    {
                        growthList = dbAccountService.Get<DailyStatitics>().OrderBy(x => x.Date).ToList();
                        if (growthList.FirstOrDefault()?.Date < growthList.LastOrDefault()?.Date.AddDays(-30))
                        {
                            var q = from gt in growthList
                                let dt = gt.Date
                                group gt by new {m = dt.Month}
                                into g
                                select g.OrderByDescending(t => t.Date).LastOrDefault();
                            growthList = q.ToList();
                            isMonthly = true;
                        }
                        else
                        {
                            var q = from gt in growthList
                                let dt = gt.Date
                                group gt by new {y = dt.Year, m = dt.Month, d = dt.Day}
                                into g
                                select g.OrderByDescending(t => t.Date).LastOrDefault();
                            growthList = q.ToList();
                        }

                        counter = growthList.Count;
                        break;
                    }
                }

                if (growthList.Count > 0)
                {
                    var lastAvailableRecord = new DailyStatitics();
                    var currentDate = DateTime.Now.Date;
                    for (var i = counter - 1; i >= 0; i--)
                    {
                        var setToZero = false;
                        var growthToAdd = new DailyStatisticsViewModel();

                        var record = isMonthly
                            ? growthList.FirstOrDefault(x => x.Date.Month == currentDate.AddMonths(-i).Month)
                            : growthList.FirstOrDefault(x => x.Date.Date == currentDate.AddDays(-i).Date);
                        if (record != null)
                        {
                            growthToAdd.Date = record.Date;
                            lastAvailableRecord = record;
                        }
                        else
                        {
                            if (lastAvailableRecord.Id != 0)
                            {
                                record = growthList.FirstOrDefault(x => x.Date.Date > lastAvailableRecord.Date.Date);
                            }
                            else
                            {
                                setToZero = true;
                                record = new DailyStatitics();
                            }

                            growthToAdd.Date = currentDate.AddDays(-i);
                        }

                        growthToAdd.GrowthColumnValue1 = setToZero || record == null ? 0 : record.Connections;
                        growthToAdd.GrowthColumnValue2 = setToZero || record == null ? 0 : record.LinkedinGroups;
                        growthToAdd.GrowthColumnValue3 = setToZero || record == null ? 0 : record.Posts;
                        responseList.Add(growthToAdd);
                    }
                }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
                return null;
            }

            return responseList;
        }

        public void DbInsertionHelper(DominatorAccountModel dominatorAccountModel, List<Connections> connectionsList)
        {
            try
            {
                var accountService =
                    InstanceProvider.ResolveAccountDbOperations(dominatorAccountModel.AccountId,
                        SocialNetworks.LinkedIn);
                accountService.AddRange(connectionsList);
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        public async Task UpdateGroups(DominatorAccountModel dominatorAccountModel, ILdFunctions ldFunctions,
            IDbAccountService dbAccountService)
        {
            try
            {
                var accountService = new DbAccountService(dominatorAccountModel);
                GlobusLogHelper.log.Info(Log.UpdatingDetails, dominatorAccountModel.AccountBaseModel.AccountNetwork,
                    dominatorAccountModel.AccountBaseModel.UserName, "Groups");

                #region Get All Joined Groups Display Count and  Insert In DB

                try
                {
                    var apiUrl = dominatorAccountModel.IsRunProcessThroughBrowser
                        ? "https://www.linkedin.com/groups/"
                        : GetGroupApiUrl(ldFunctions);

                    var myGroupsResponseHandler =
                        await ldFunctions.SearchForMyGroups(apiUrl, true, dominatorAccountModel.Token);

                    if (myGroupsResponseHandler.Success)
                    {
                        if (myGroupsResponseHandler.GroupsList.Count > 0)
                        {
                            dominatorAccountModel.DisplayColumnValue2 = myGroupsResponseHandler.GroupsList.Count;
                            dbAccountService.RemoveAll<Groups>();
                            if (myGroupsResponseHandler.GroupsList.Count != 0)
                            {
                                var groupsList = new List<Groups>();
                                myGroupsResponseHandler.GroupsList.ForEach(x =>
                                {
                                    try
                                    {
                                        var group = new Groups();
                                        _classMapper.MapModelClass(x, ref group);
                                        group.InteractionTimeStamp = DateTimeUtilities.GetEpochTime();

                                        if (!groupsList.Contains(group))
                                            groupsList.Add(group);
                                    }
                                    catch (Exception e)
                                    {
                                        e.DebugLog();
                                    }
                                });
                                accountService.AddRange(groupsList);
                            }
                        }
                        else
                        {
                            dominatorAccountModel.DisplayColumnValue2 = 0;
                        }
                    }
                    else
                    {
                        dominatorAccountModel.DisplayColumnValue2 = 0;
                    }

                    SaveCount(dominatorAccountModel);
                    GlobusLogHelper.log.Info(Log.DetailsUpdated, dominatorAccountModel.AccountBaseModel.AccountNetwork,
                        dominatorAccountModel.AccountBaseModel.UserName, "Groups");
                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                }

                #endregion
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        private static string GetGroupApiUrl(ILdFunctions ldFunctions)
        {
            var apiUrl = "";
            try
            {
                var groupPageResponse =
                    HttpUtility.HtmlDecode(
                        ldFunctions.GetRequestUpdatedUserAgent("https://www.linkedin.com/groups/my-groups"));
                var pageText = groupPageResponse;
                var data = Utils.GetBetween(pageText, " {\"request\":\"/voyager/api/groups/groups?", "\",\"");
                if(string.IsNullOrEmpty(data))
                {
                    var groupApi = "count=10&membershipStatuses=List(MANAGER,MEMBER,OWNER)&q=member&start=0";
                    data = groupApi;
                }
                var decodedUrl = Regex.Unescape(Regex.Replace(data, "\\\\([^u])", "\\\\$1"))
                    .Replace("start=0&count=10", "start=0&count=100").Replace("count=10", "count=100");
                if (!string.IsNullOrEmpty(decodedUrl))
                    apiUrl = $"https://www.linkedin.com/voyager/api/groups/groups?{decodedUrl}";
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            return apiUrl;
        }

        public async Task UpdateInvitationsSent(DominatorAccountModel dominatorAccountModel, ILdFunctions ldFunctions,
            IDbAccountService dbAccountService)
        {
            try
            {
                _ldDataHelper.GetAndSetSentConnectionCount(dominatorAccountModel, ldFunctions);

                var listInvitationsSents = dbAccountService.Get<InvitationsSent>().ToList();
                var maxAlreadyPresentCount = 150;
                var currentAlreadyPresentCount = 0;

                if (CheckAndRemoveSentConnection(dominatorAccountModel, listInvitationsSents.Count, dbAccountService))
                    listInvitationsSents = dbAccountService.Get<InvitationsSent>().ToList();

                // update and save count in db
                SaveCount(dominatorAccountModel);

                GlobusLogHelper.log.Info(Log.UpdatingDetails, dominatorAccountModel.AccountBaseModel.AccountNetwork,
                    dominatorAccountModel.AccountBaseModel.UserName, "sent invitations");
                var sentInvitationsApiFirstPage = dominatorAccountModel.IsRunProcessThroughBrowser
                    ? "https://www.linkedin.com/mynetwork/invitation-manager/sent/"
                    : "https://www.linkedin.com/voyager/api/relationships/sentInvitationView?count=100&start=0&type=SINGLES_ALL&q=sent";
                var objSentInvitationsResponseHandler =
                    await ldFunctions.AllPendingConnectionRequest(sentInvitationsApiFirstPage,
                        dominatorAccountModel.Token);
                int start = 0, loopCount = 0, Page = 2;

                while (objSentInvitationsResponseHandler.Success && currentAlreadyPresentCount < maxAlreadyPresentCount)
                {
                    var constructedApi = dominatorAccountModel.IsRunProcessThroughBrowser
                        ? $"https://www.linkedin.com/mynetwork/invitation-manager/sent/?invitationType=&page={start}"
                        : $"https://www.linkedin.com/voyager/api/relationships/sentInvitationView?count=100&type=SINGLES_ALL&q=sent&start={start}";
                    if (loopCount > 0)
                        objSentInvitationsResponseHandler =
                            await ldFunctions.AllPendingConnectionRequest(constructedApi, dominatorAccountModel.Token);

                    if (objSentInvitationsResponseHandler.Success &&
                        objSentInvitationsResponseHandler.LstAllPendingConnectionRequest.Count > 0)
                    {
                        var lstAllPendingConnectionRequest = new List<InvitationsSent>();
                        objSentInvitationsResponseHandler.LstAllPendingConnectionRequest.ForEach(x =>
                        {
                            try
                            {
                                var invitationsSent = new InvitationsSent();
                                _classMapper.MapModelClass(x, ref invitationsSent);
                                invitationsSent.HasAnonymousProfilePicture =
                                    x.HasAnonymousProfilePicture != null && x.HasAnonymousProfilePicture != false;
                                invitationsSent.ConnectionType = ConnectionType.SeondDegree;
                                invitationsSent.InteractionTimeStamp = DateTimeUtilities.GetEpochTime();

                                if (listInvitationsSents.Any(y => y.ProfileUrl == invitationsSent.ProfileUrl))
                                {
                                    ++currentAlreadyPresentCount;
                                }
                                else
                                {
                                    listInvitationsSents.Add(invitationsSent);
                                    lstAllPendingConnectionRequest.Add(invitationsSent);
                                }
                            }
                            catch (Exception)
                            {
                            }
                        });
                        if (lstAllPendingConnectionRequest.Count > 0)
                            dbAccountService.AddRange(lstAllPendingConnectionRequest);
                    }
                    else
                    {
                        Utils.RandomDelay(8000, 15000);
                        break;
                    }

                    #region Pagination

                    start += dominatorAccountModel.IsRunProcessThroughBrowser ? start == 0 ? Page : 1 : 100;
                    loopCount += 1;

                    Utils.RandomDelay(8000, 15000);

                    #endregion

                    var reached = dominatorAccountModel.DisplayColumnValue1 * 10 / 100;
                    if (dominatorAccountModel.IsRunProcessThroughBrowser && listInvitationsSents.Count >=
                        dominatorAccountModel.DisplayColumnValue3 - reached)
                        break;
                }

                dominatorAccountModel.DisplayColumnValue3 = dbAccountService.Get<InvitationsSent>().Count;
                SaveCount(dominatorAccountModel);

                GlobusLogHelper.log.Info(Log.DetailsUpdated, dominatorAccountModel.AccountBaseModel.AccountNetwork,
                    dominatorAccountModel.AccountBaseModel.UserName, "invitations sent");
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        public async Task UpdatePages(DominatorAccountModel dominatorAccountModel, ILdFunctions ldFunctions,
            IDbAccountService dbAccountService)
        {
            try
            {
                var accountService = new DbAccountService(dominatorAccountModel);
                GlobusLogHelper.log.Info(Log.UpdatingDetails, dominatorAccountModel.AccountBaseModel.AccountNetwork,
                    dominatorAccountModel.AccountBaseModel.UserName, "Pages");

                try
                {
                    var apiUrl= "https://www.linkedin.com/voyager/api/graphql?includeWebMetadata=true&variables=(count:3,viewerPermissions:(canReadOrganizationUpdateAnalytics:true))&&queryId=voyagerOrganizationDashCompanies.aff7800037a46988b56daae19ecf4915";
                    var myPagesResponseHandler =
                        await ldFunctions.SearchForMyPages(apiUrl, true, dominatorAccountModel.Token);

                    if (myPagesResponseHandler.Success)
                    {
                        if (myPagesResponseHandler.listOfPages.Count > 0)
                        {
                            dbAccountService.RemoveAll<Pages>();
                            dominatorAccountModel.DisplayColumnValue4 = myPagesResponseHandler.listOfPages.Count;
                            if (myPagesResponseHandler.listOfPages.Count != 0)
                            {
                                var pagesList = new List<Pages>();
                                myPagesResponseHandler.listOfPages.ForEach(x =>
                                {
                                    try
                                    {
                                        var page = new Pages();
                                        _classMapper.MapModelClass(x, ref page);
                                        page.InteractionTimeStamp = DateTimeUtilities.GetEpochTime();

                                        if (!pagesList.Contains(page))
                                            pagesList.Add(page);
                                    }
                                    catch (Exception e)
                                    {
                                        e.DebugLog();
                                    }
                                });
                                accountService.AddRange(pagesList);
                            }
                            else
                            {
                                dominatorAccountModel.DisplayColumnValue4 = 0;
                            }
                        }
                        else
                        {
                            dominatorAccountModel.DisplayColumnValue4 = 0;
                        }

                        SaveCount(dominatorAccountModel);
                        GlobusLogHelper.log.Info(Log.DetailsUpdated,
                            dominatorAccountModel.AccountBaseModel.AccountNetwork,
                            dominatorAccountModel.AccountBaseModel.UserName, "Pages");
                    }
                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        public bool AddToDailyGrowth(string accountId, int connectionsCount, int groupsCount, int invitationsSentCount)
        {
            var success = true;
            try
            {
                var date = DateTime.Today;
                var dbAccountService =
                    InstanceProvider.ResolveAccountDbOperations(accountId, SocialNetworks.LinkedIn);

                var existingDataForToday = dbAccountService.GetSingle<DailyStatitics>(x => x.Date == date);

                if (existingDataForToday != null)
                {
                    existingDataForToday.Date = DateTime.Now;
                    existingDataForToday.Connections = connectionsCount;
                    existingDataForToday.LinkedinGroups = groupsCount;
                    existingDataForToday.Posts = invitationsSentCount;
                    dbAccountService.Update(existingDataForToday);
                }
                else
                {
                    if (dbAccountService.Get<DailyStatitics>().Count == 0)
                    {
                        dbAccountService.Add(new DailyStatitics
                        {
                            Date = DateTime.Now.AddDays(-1),
                            Connections = connectionsCount,
                            LinkedinGroups = groupsCount,
                            Posts = invitationsSentCount
                        });
                        dbAccountService.Add(new DailyStatitics
                        {
                            Date = DateTime.Now,
                            Connections = connectionsCount,
                            LinkedinGroups = groupsCount,
                            Posts = invitationsSentCount
                        });
                    }
                    else
                    {
                        dbAccountService.Add(new DailyStatitics
                        {
                            Date = DateTime.Now,
                            Connections = connectionsCount,
                            LinkedinGroups = groupsCount,
                            Posts = invitationsSentCount
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
                success = false;
            }

            return success;
        }

        public bool CheckAndRemoveConnection(DominatorAccountModel dominatorAccountModel, int lastConnectionCount,
            DbAccountService accountService)
        {
            var isLessConnections = false;
            try
            {
                // remove all connection and start updating from start if account have less than 70% connections 
                // or total connections count are less than 800
                if (dominatorAccountModel.DisplayColumnValue1 != null)
                {
                    var countPercent = lastConnectionCount * 100 / dominatorAccountModel.DisplayColumnValue1.Value;
                    if (countPercent < 70 || countPercent > 100 ||
                        dominatorAccountModel.DisplayColumnValue1.Value < 800)
                    {
                        isLessConnections = true;
                        accountService.RemoveAll<Connections>();
                    }
                }
            }
            catch (Exception exception)
            {
                exception.DebugLog();
            }

            return isLessConnections;
        }

        public bool CheckAndRemoveSentConnection(DominatorAccountModel dominatorAccountModel, int lastConnectionCount,
            IDbAccountService accountService)
        {
            var isLessConnections = false;
            try
            {
                // remove all sent connection and start updating from start if account have less than 70% connections 
                // or total sent connections count are less than 500
                if (dominatorAccountModel.DisplayColumnValue3 != null)
                {
                    var countPercent = lastConnectionCount * 100 / dominatorAccountModel.DisplayColumnValue3.Value;
                    if (countPercent < 70 || dominatorAccountModel.DisplayColumnValue3.Value < 500)
                        isLessConnections = accountService.RemoveAll<InvitationsSent>();
                }
            }
            catch (Exception exception)
            {
                exception.DebugLog();
            }

            return isLessConnections;
        }

        #region Private fields

        private static ILdAccountUpdateFactory _instance;

        private readonly IAccountScopeFactory _accountScopeFactory;

        #endregion
    }
}