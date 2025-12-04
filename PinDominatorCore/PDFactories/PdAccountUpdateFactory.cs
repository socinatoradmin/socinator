using CommonServiceLocator;
using ThreadUtils;
using DominatorHouseCore;
using DominatorHouseCore.DatabaseHandler.PdTables.Accounts;
using DominatorHouseCore.Diagnostics;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Enums.DHEnum;
using DominatorHouseCore.Extensions;
using DominatorHouseCore.FileManagers;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using DominatorHouseCore.ViewModel;
using Newtonsoft.Json;
using PinDominatorCore.PDLibrary;
using PinDominatorCore.PDLibrary.DAL;
using PinDominatorCore.PDModel;
using PinDominatorCore.PDUtility;
using PinDominatorCore.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Unity;
using PinDominatorCore.Interface;

namespace PinDominatorCore.PDFactories
{
    public interface IPdAccountUpdateFactory : IAccountUpdateFactoryAsync
    {
    }

    public class PdAccountUpdateFactory : IPdAccountUpdateFactory, IAccountUpdateAccountTypeFactoryAsync
    {
        private static PdAccountUpdateFactory _instance;
        private readonly IDelayService _delayService;
        private readonly IAccountScopeFactory _accountScopeFactory;
        private IDbAccountService _dbGrowthOperations;
        private IPDAccountSessionManager SessionManager { get; }
        public static PdAccountUpdateFactory Instance =>
            _instance ?? (_instance = InstanceProvider.GetInstance<PdAccountUpdateFactory>());


        public PdAccountUpdateFactory(IAccountScopeFactory accountScopeFactory, IDelayService delayService,
            IPDAccountSessionManager pDAccountSession)
        {
            _accountScopeFactory = accountScopeFactory;
            _delayService = delayService;
            SessionManager = pDAccountSession;
        }

        public bool CheckStatus(DominatorAccountModel accountModel)
        {
            return CheckStatusAsync(accountModel, accountModel.Token).Result;
        }

        public void UpdateDetails(DominatorAccountModel accountModel)
        {
            UpdateDetailsAsync(accountModel, accountModel.Token).Wait();
        }

        public async Task<bool> CheckStatusAsync(DominatorAccountModel accountModel, CancellationToken token)
        {
            var logInProcess = _accountScopeFactory[accountModel.AccountId].Resolve<IPdLogInProcess>();
            try
            {
                if (!await logInProcess.CheckLoginAsync(accountModel, token, true))
                    return false;
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
                logInProcess?.BrowserManager?.CloseLast();
            }
            return true;
        }

        public async Task UpdateDetailsAsync(DominatorAccountModel objDominatorAccountModel, CancellationToken token)
        {
            var logInProcess = _accountScopeFactory[$"{objDominatorAccountModel.AccountId}_UpdateCheck"].Resolve<IPdLogInProcess>();
            try
            {
                SessionManager?.AddOrUpdateSession(ref objDominatorAccountModel);
                if (objDominatorAccountModel.AccountBaseModel.Status != AccountStatus.Success ||
                    objDominatorAccountModel.Cookies.Count == 0)
                {
                    if (objDominatorAccountModel.AccountBaseModel.Status == AccountStatus.Success)
                        objDominatorAccountModel.AccountBaseModel.Status = AccountStatus.Failed;
                    return;
                }


                var headers = logInProcess.PinFunct.SetHeaders(objDominatorAccountModel);
                headers.Cookies = objDominatorAccountModel.Cookies;
                logInProcess.PinFunct.HttpHelper.SetRequestParameter(headers);

                if (logInProcess.PinFunct.HttpHelper.GetRequestParameter().Cookies?.Count < 5 &&
                    !await logInProcess.CheckLoginAsync(objDominatorAccountModel, objDominatorAccountModel.Token,
                        true))
                    return;

                var url = string.Empty;
                if (objDominatorAccountModel.Cookies != null)
                {
                    logInProcess.PinFunct.Domain = objDominatorAccountModel.Cookies["csrftoken"]?.Domain;
                    logInProcess.PinFunct.Domain = logInProcess.PinFunct.Domain[0].Equals('.')
                        ? logInProcess.PinFunct.Domain.Remove(0, 1)
                        : logInProcess.PinFunct.Domain;
                    logInProcess.BrowserManager.Domain = logInProcess.PinFunct.Domain;
                }

                if (!url.Contains("https")) url = PdConstants.Https + logInProcess.PinFunct.Domain;
                _delayService.ThreadSleep(new Random().Next(2, 5) * 1000);
                IResponseParameter accloginResp;
                var Count = 0;
            TryAgain:
                if (objDominatorAccountModel.IsRunProcessThroughBrowser)
                {
                    logInProcess = logInProcess ??
                                    _accountScopeFactory[$"{objDominatorAccountModel.AccountId}_UpdateCheck"]
                                        .Resolve<IPdLogInProcess>();
                    if (logInProcess.BrowserManager.BrowserWindows.Count == 0)
                        logInProcess.BrowserManager.OpenBrowser(objDominatorAccountModel, url,
                            objDominatorAccountModel.CancellationSource);
                    logInProcess.BrowserManager.BrowserDispatcher(PDEnums.BrowserFuncts.GetPageSource,
                        objDominatorAccountModel.CancellationSource,
                        4);
                    accloginResp = new DominatorHouseCore.Request.ResponseParameter()
                        {Response = logInProcess.BrowserManager.CurrentData};
                }
                else
                    accloginResp =
                        await logInProcess.PinFunct.HttpHelper.GetRequestAsync(url, objDominatorAccountModel.Token);
                while (Count++ <= 3 && string.IsNullOrEmpty(accloginResp.Response))
                    goto TryAgain;
                var RequestHeaderDetails = PdRequestHeaderDetails.GetRequestHeader(accloginResp.Response,TokenDetailsType.Users);
                var userName = RequestHeaderDetails.Username;
                if (!string.IsNullOrEmpty(userName))
                    objDominatorAccountModel.AccountBaseModel.ProfileId = userName;
                token.ThrowIfCancellationRequested();
                SocinatorAccountBuilder.Instance(objDominatorAccountModel.AccountBaseModel.AccountId)
                    .AddOrUpdateDominatorAccountBase(objDominatorAccountModel.AccountBaseModel)
                    .AddOrUpdateCookies(objDominatorAccountModel.Cookies)
                    .SaveToBinFile();

                _delayService.ThreadSleep(new Random().Next(2, 5) * 1000);

                GlobusLogHelper.log.Info(Log.UpdatingDetails, objDominatorAccountModel.AccountBaseModel.AccountNetwork,
                    objDominatorAccountModel.AccountBaseModel.UserName, "AccountInfo");

                UserNameInfoPtResponseHandler objAccountDetails;
                //if (objDominatorAccountModel.IsRunProcessThroughBrowser)
                //    objAccountDetails = logInProcess.BrowserManager.SearchByCustomUser(objDominatorAccountModel,
                //        objDominatorAccountModel.AccountBaseModel.ProfileId,
                //        objDominatorAccountModel.CancellationSource);
                //else
                    objAccountDetails = await logInProcess.PinFunct.GetUserDetailsAsync(objDominatorAccountModel);
                int failedCount = 1;
                while ((objAccountDetails == null || string.IsNullOrEmpty(objAccountDetails.ProfilePicUrl)) && failedCount++ < 5)
                    objAccountDetails = await logInProcess.PinFunct.GetUserDetailsAsync(objDominatorAccountModel);
                List<PinterestBoard> boardsList;
                if (objDominatorAccountModel.IsRunProcessThroughBrowser)
                    boardsList = logInProcess.BrowserManager.SearchBoardsOfUser(objDominatorAccountModel,
                        objDominatorAccountModel.AccountBaseModel.ProfileId,
                        objDominatorAccountModel.CancellationSource);
                else
                    boardsList = await logInProcess.PinFunct.GetBoardsOfUserAsync(objDominatorAccountModel, token);
                if (logInProcess.PinFunct != null)
                    try { await logInProcess.PinFunct.UpdatePinterestBoardSections(objDominatorAccountModel, ref boardsList); } catch (Exception ex) { ex.DebugLog(ex.GetBaseException().Message); }
                failedCount = 1;
                while (objAccountDetails == null && failedCount++ < 5)
                {
                    if (objDominatorAccountModel.IsRunProcessThroughBrowser)
                        boardsList = logInProcess.BrowserManager.SearchBoardsOfUser(objDominatorAccountModel,
                            objDominatorAccountModel.AccountBaseModel.ProfileId,
                            objDominatorAccountModel.CancellationSource);
                    else
                        boardsList =
                            await logInProcess.PinFunct.GetBoardsOfUserAsync(objDominatorAccountModel, token);
                }

                if (objAccountDetails != null)
                {
                    objDominatorAccountModel.AccountBaseModel.UserId = objAccountDetails.UserId;
                    objDominatorAccountModel.AccountBaseModel.UserFullName = objAccountDetails.FullName;
                    objDominatorAccountModel.AccountBaseModel.ProfilePictureUrl = objAccountDetails.ProfilePicUrl;
                    objDominatorAccountModel.AccountBaseModel.ProfileId = objAccountDetails.Username;
                    objDominatorAccountModel.DisplayColumnValue1 = objAccountDetails.FollowerCount;
                    objDominatorAccountModel.DisplayColumnValue2 = objAccountDetails.FollowingCount;
                    objDominatorAccountModel.DisplayColumnValue3 = boardsList.Count;
                    objDominatorAccountModel.DisplayColumnValue4 = objAccountDetails.PinsCount;

                    AddToDailyGrowth(objDominatorAccountModel.AccountId, objAccountDetails.FollowerCount,
                        objAccountDetails.FollowingCount, objAccountDetails.PinsCount, objAccountDetails.BoardCount);

                    token.ThrowIfCancellationRequested();

                    IDbAccountService dbAccountService = null;
                    try
                    {
                        dbAccountService = new DbAccountService(objDominatorAccountModel);
                    }
                    catch (Exception ex)
                    {
                        ex.DebugLog();
                    }

                    if (objDominatorAccountModel.IsRunProcessThroughBrowser)
                        logInProcess.BrowserManager.CloseLast();

                    UpdateBoards(objDominatorAccountModel, boardsList, dbAccountService);

                    GlobusLogHelper.log.Info(Log.DetailsUpdated,
                        objDominatorAccountModel.AccountBaseModel.AccountNetwork,
                        objDominatorAccountModel.AccountBaseModel.UserName, "AccountInfo");

                    token.ThrowIfCancellationRequested();

                    SocinatorAccountBuilder.Instance(objDominatorAccountModel.AccountBaseModel.AccountId)
                        .AddOrUpdateDominatorAccountBase(objDominatorAccountModel.AccountBaseModel)
                        .AddOrUpdateCookies(objDominatorAccountModel.Cookies)
                        .AddOrUpdateDisplayColumn1(objDominatorAccountModel.DisplayColumnValue1)
                        .AddOrUpdateDisplayColumn2(objDominatorAccountModel.DisplayColumnValue2)
                        .AddOrUpdateDisplayColumn3(objDominatorAccountModel.DisplayColumnValue3)
                        .AddOrUpdateDisplayColumn4(objDominatorAccountModel.DisplayColumnValue4)
                        .AddOrUpdateDisplayColumn11(objDominatorAccountModel.DisplayColumnValue11)
                        .SaveToBinFile();
                }

            }
            catch (OperationCanceledException)
            {
                throw new OperationCanceledException();
            }
            catch (AggregateException ae)
            {
                foreach (var e in ae.InnerExceptions)
                {
                    if (e is TaskCanceledException || e is OperationCanceledException)
                        e.DebugLog("Cancellation requested before task completion!");
                    else
                        e.DebugLog(e.StackTrace + e.Message);
                }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
            finally
            {
                logInProcess?.BrowserManager?.CloseLast();
            }
        }

        private void UpdateBoards(DominatorAccountModel objDominatorAccountModel, List<PinterestBoard> boardsList, IDbAccountService dbAccountService)
        {
            try
            {
                GlobusLogHelper.log.Info(Log.UpdatingDetails,
                    objDominatorAccountModel.AccountBaseModel.AccountNetwork,
                    objDominatorAccountModel.AccountBaseModel.UserName, "LangKeyBoardDetails".FromResourceDictionary());
                var lstOwnBoards = dbAccountService.GetOwnBoards().ToList();
                _delayService.ThreadSleep(new Random().Next(2, 5) * 1000);
                var listOfBoards = FilterDuplicatesFromListOfBoards(lstOwnBoards, boardsList);
                //Add New Board To Data Base.
                listOfBoards.ForEach(x =>
                {
                    try
                    {
                        var boards = new OwnBoards
                        {
                            BoardDescription = x.BoardDescription,
                            BoardName = x.BoardName,
                            BoardUrl = x.BoardUrl,
                            BoardFollowers = x.FollowersCount,
                            PinsCount = x.PinsCount,
                            Username = x.UserName,
                            BoardSectionCount = x.BoardSections.Count,
                            BoardSections = JsonConvert.SerializeObject(x.BoardSections)
                        };
                        dbAccountService.Add(boards);
                    }
                    catch (Exception e)
                    {
                        e.DebugLog();
                    }
                });
                //Update The Existing Board With Updated Data.
                boardsList.ForEach(scrapBoard =>
                    {
                        try
                        {
                            var updateBoard = dbAccountService.GetSingle<OwnBoards>(x => scrapBoard.BoardUrl == x.BoardUrl);
                            if (updateBoard != null)
                            {
                                updateBoard.BoardName = scrapBoard.BoardName;
                                updateBoard.BoardDescription = scrapBoard.BoardDescription;
                                updateBoard.BoardUrl = scrapBoard.BoardUrl;
                                updateBoard.BoardFollowers = scrapBoard.FollowersCount;
                                updateBoard.PinsCount = scrapBoard.PinsCount;
                                updateBoard.Username = scrapBoard.UserName;
                                updateBoard.BoardSectionCount = scrapBoard.BoardSections.Count;
                                updateBoard.BoardSections = JsonConvert.SerializeObject(scrapBoard.BoardSections);
                                dbAccountService.Update(updateBoard);
                            }
                        }catch(Exception e) { e.DebugLog(); }
                    });
                lstOwnBoards = dbAccountService.GetOwnBoards().ToList();

                var boardsToRemove = new List<OwnBoards>();
                if (lstOwnBoards.Count > boardsList.Count)
                    boardsToRemove = RemoveDuplicatesFromListOfBoards(lstOwnBoards, boardsList);

                boardsToRemove.ForEach(x =>
                {
                    try
                    {
                        dbAccountService.RemoveMatch<OwnBoards>(board => board.BoardUrl == x.BoardUrl);
                    }
                    catch (Exception e)
                    {
                        e.DebugLog();
                    }
                });
                lstOwnBoards = dbAccountService.GetOwnBoards().ToList();
                GlobusLogHelper.log.Info(Log.DetailsUpdated,
                    objDominatorAccountModel.AccountBaseModel.AccountNetwork,
                    objDominatorAccountModel.AccountBaseModel.UserName, "LangKeyBoardDetails".FromResourceDictionary());
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        public void SaveToDatabase(List<PinterestUser> lstPinterestUser, IDbAccountService dbAccountService, FollowType followType)
        {
            lstPinterestUser.ForEach(x =>
            {
                try
                {
                    Friendships friendship = new Friendships()
                    {
                        Username = x.Username,
                        Followers = x.FollowersCount,
                        Followings = x.FollowingsCount,
                        BoardsCount = x.BoardsCount,
                        PinsCount = x.PinsCount,
                        FullName = x.FullName,
                        ProfilePicUrl = x.ProfilePicUrl,
                        HasAnonymousProfilePicture = x.HasProfilePic,
                        Bio = x.UserBio,
                        FollowType = followType,
                        Time = DateTimeUtilities.GetEpochTime()
                    };
                    dbAccountService.Add(friendship);
                }

                catch (Exception e)
                {
                    e.DebugLog();
                }
            });
        }

        public void UpdateDatabase(List<Friendships> lstFriendships, IDbAccountService dbAccountService, FollowType followType)
        {
            lstFriendships.ForEach(x =>
            {
                try
                {
                    Friendships friendship = new Friendships()
                    {
                        Id = x.Id,
                        Username = x.Username,
                        Followers = x.Followers,
                        Followings = x.Followings,
                        BoardsCount = x.BoardsCount,
                        PinsCount = x.PinsCount,
                        FullName = x.FullName,
                        ProfilePicUrl = x.ProfilePicUrl,
                        HasAnonymousProfilePicture = x.HasAnonymousProfilePicture,
                        FollowType = FollowType.Mutual
                    };

                    dbAccountService.Update(friendship);
                }
                catch (Exception e)
                {
                    e.DebugLog();
                }
            });
        }

        public DailyStatisticsViewModel GetDailyGrowth(string accountId, string username, GrowthPeriod period)
        {
            var response = new DailyStatisticsViewModel();
            var growthList = new List<DailyStatitics>();
            try
            {
                var accountsFileManager = InstanceProvider.GetInstance<IAccountsFileManager>();
                var account = accountsFileManager.GetAll().FirstOrDefault(x => x.AccountId == accountId);
                _dbGrowthOperations = InstanceProvider.ResolveWithDominatorAccount<IDbAccountService>(account);

                if (period == GrowthPeriod.Daily)
                    growthList = _dbGrowthOperations.GetDailyStatitics()
                        .Where(x => x.Date >= DateTime.Today.AddDays(-1)).OrderBy(x => x.Date).ToList();
                else if (period == GrowthPeriod.Weekly)
                    growthList = _dbGrowthOperations.GetDailyStatitics()
                        .Where(x => x.Date >= DateTime.Today.AddDays(-7)).OrderBy(x => x.Date).ToList();
                else if (period == GrowthPeriod.Monthly)
                    growthList = _dbGrowthOperations.GetDailyStatitics()
                        .Where(x => x.Date >= DateTime.Today.AddMonths(-1)).OrderBy(x => x.Date).ToList();
                else
                    growthList = _dbGrowthOperations.GetDailyStatitics().OrderBy(x => x.Date).ToList();
                if (growthList != null && growthList.Count > 0)
                {
                    var oldRecord = growthList.FirstOrDefault();
                    var latestRecord = growthList.LastOrDefault();
                    if (latestRecord != null && oldRecord != null)
                    {
                        response.GrowthColumnValue1 = latestRecord.Followers - oldRecord.Followers;
                        response.GrowthColumnValue2 = latestRecord.Followings - oldRecord.Followings;
                        response.GrowthColumnValue4 = latestRecord.Pins - oldRecord.Pins;
                        response.GrowthColumnValue5 = latestRecord.Boards - oldRecord.Boards;
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
            var growthList = new List<DailyStatitics>();
            var responseList = new List<DailyStatisticsViewModel>();
            try
            {
                var counter = 0;
                if (period == GrowthChartPeriod.PastDay)
                {
                    growthList = _dbGrowthOperations.GetDailyStatitics()
                        .Where(x => x.Date >= DateTime.Today.AddDays(-1)).OrderBy(x => x.Date).ToList();
                    counter = 2;
                }
                else if (period == GrowthChartPeriod.Past3Months)
                {
                    counter = 3;
                    isMonthly = true;
                    growthList = _dbGrowthOperations.GetDailyStatitics()
                        .Where(x => x.Date >= DateTime.Today.AddMonths(-3)).OrderBy(x => x.Date).ToList();

                    var q = from gt in growthList
                            let dt = gt.Date
                            group gt by new { m = dt.Month }
                        into g
                            select g.OrderByDescending(t => t.Date).FirstOrDefault();
                    growthList = q.ToList();
                }
                else if (period == GrowthChartPeriod.Past30Days)
                {
                    counter = 30;
                    growthList = _dbGrowthOperations.GetDailyStatitics()
                        .Where(x => x.Date >= DateTime.Today.AddDays(-30)).OrderBy(x => x.Date).ToList();
                    var q = from gt in growthList
                            let dt = gt.Date
                            group gt by new { y = dt.Year, m = dt.Month, d = dt.Day }
                        into g
                            select g.OrderByDescending(t => t.Date).LastOrDefault();
                    growthList = q.ToList();
                }
                else if (period == GrowthChartPeriod.Past6Months)
                {
                    counter = 6;
                    isMonthly = true;
                    growthList = _dbGrowthOperations.GetDailyStatitics()
                        .Where(x => x.Date >= DateTime.Today.AddMonths(-6)).OrderBy(x => x.Date).ToList();

                    var q = from gt in growthList
                            let dt = gt.Date
                            group gt by new { m = dt.Month }
                        into g
                            select g.OrderByDescending(t => t.Date).LastOrDefault();
                    growthList = q.ToList();
                }
                else if (period == GrowthChartPeriod.PastWeek)
                {
                    counter = 7;

                    growthList = _dbGrowthOperations.GetDailyStatitics()
                        .Where(x => x.Date >= DateTime.Today.AddDays(-7)).OrderBy(x => x.Date).ToList();
                    var q = from gt in growthList
                            let dt = gt.Date
                            group gt by new { y = dt.Year, m = dt.Month, d = dt.Day }
                        into g
                            select g.OrderByDescending(t => t.Date).LastOrDefault();
                    growthList = q.ToList();
                }
                else
                {
                    growthList = _dbGrowthOperations.GetDailyStatitics().OrderBy(x => x.Date).ToList();
                    if (growthList.FirstOrDefault()?.Date < growthList.LastOrDefault()?.Date.AddDays(-30))
                    {
                        var q = from gt in growthList
                                let dt = gt.Date
                                group gt by new { m = dt.Month }
                            into g
                                select g.OrderByDescending(t => t.Date).LastOrDefault();
                        growthList = q.ToList();
                        isMonthly = true;
                    }
                    else
                    {
                        var q = from gt in growthList
                                let dt = gt.Date
                                group gt by new { y = dt.Year, m = dt.Month, d = dt.Day }
                            into g
                                select g.OrderByDescending(t => t.Date).LastOrDefault();
                        growthList = q.ToList();
                    }

                    counter = growthList.Count;
                }

                if (growthList != null && growthList.Count > 0)
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

                        growthToAdd.GrowthColumnValue1 = setToZero || record == null ? 0 : record.Followers;
                        growthToAdd.GrowthColumnValue2 = setToZero || record == null ? 0 : record.Followings;
                        growthToAdd.GrowthColumnValue3 = setToZero || record == null ? 0 : record.Boards;
                        responseList.Add(growthToAdd);
                    }

                }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            return responseList;
        }

        public List<OwnBoards> RemoveDuplicatesFromListOfBoards(List<OwnBoards> listFromDb,
            List<PinterestBoard> listToBeFiltered)
        {
            var listOfFilteredData = new List<OwnBoards>(listFromDb);
            foreach (var dataFromDb in listToBeFiltered)
                foreach (var dataToFilter in listFromDb)
                    if (dataToFilter.BoardUrl == dataFromDb.BoardUrl)
                        listOfFilteredData.Remove(dataToFilter);
            return listOfFilteredData;
        }

        public List<PinterestBoard> FilterDuplicatesFromListOfBoards(List<OwnBoards> listFromDb,
            List<PinterestBoard> listToBeFiltered)
        {
            var listOfFilteredData = new List<PinterestBoard>(listToBeFiltered);
            foreach (var dataFromDb in listFromDb)
                foreach (var dataToFilter in listToBeFiltered)
                    if (dataToFilter.BoardUrl == dataFromDb.BoardUrl)
                        listOfFilteredData.Remove(dataToFilter);
            return listOfFilteredData;
        }

        public List<PinterestUser> FilterDuplicatesFromListOfUsers(List<Friendships> listFromDb,
            List<PinterestUser> listToBeFiltered)
        {
            var listOfFilteredData = new List<PinterestUser>(listToBeFiltered);
            foreach (var dataFromDb in listFromDb) listOfFilteredData.RemoveAll(x => x.Username == dataFromDb.Username);
            return listOfFilteredData;
        }

        public List<Friendships> FilterMutualFromListOfUsers(List<PinterestUser> listOfFollowings,
            List<Friendships> listOfFollowers)
        {
            var listOfMutuals = new List<Friendships>();
            foreach (var dataFromDb in listOfFollowings)
                foreach (var dataToFilter in listOfFollowers)
                    if (dataToFilter.Username == dataFromDb.Username)
                        listOfMutuals.Add(dataToFilter);
            return listOfMutuals;
        }

        public bool AddToDailyGrowth(string accountId, int followers, int following, int pins, int boards)
        {
            var success = true;
            try
            {
                var dbGrowthOperations = InstanceProvider.ResolveAccountDbOperations(accountId, SocialNetworks.Pinterest);
                var existingDataForToday = dbGrowthOperations.GetSingle<DailyStatitics>(x => x.Date == DateTime.Today);
                if (existingDataForToday != null)
                {
                    existingDataForToday.Date = DateTime.Now;
                    existingDataForToday.Followers = followers;
                    existingDataForToday.Followings = following;
                    existingDataForToday.Pins = pins;
                    existingDataForToday.Boards = boards;
                    _dbGrowthOperations.Update(existingDataForToday);
                }
                else
                {
                    if (_dbGrowthOperations.GetDailyStatitics().Count == 0)
                    {
                        _dbGrowthOperations.Add(new DailyStatitics
                        {
                            Date = DateTime.Now.AddDays(-1),
                            Followers = followers,
                            Followings = following,
                            Pins = pins,
                            Boards = boards
                        });
                        _dbGrowthOperations.Add(new DailyStatitics
                        {
                            Date = DateTime.Now,
                            Followers = followers,
                            Followings = following,
                            Pins = pins,
                            Boards = boards
                        });
                    }
                    else
                    {
                        _dbGrowthOperations.Add(new DailyStatitics
                        {
                            Date = DateTime.Now,
                            Followers = followers,
                            Followings = following,
                            Pins = pins,
                            Boards = boards
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

        public void SwitchToBusinessAccount(DominatorAccountModel account, CancellationToken token, bool isBusinessAccount = true)
        {
            SwitchToBusinessAccountAsync(account, token, isBusinessAccount).Wait();
        }

        public async Task SwitchToBusinessAccountAsync(DominatorAccountModel account, CancellationToken token, bool isBusinessAccount = true)
        {
            try
            {
                Thread.Sleep(10000);
                var logInProcess = _accountScopeFactory[$"{account.AccountId}_CheckLoginToSwitchProfile"].Resolve<IPdLogInProcess>();
                if (!await logInProcess.CheckLoginAsync(account, token, true))
                    return;

                if (account.IsRunProcessThroughBrowser)
                    logInProcess.BrowserManager?.CloseLast();

                if (account.Cookies != null)
                {
                    logInProcess.PinFunct.Domain = account.Cookies["csrftoken"].Domain;
                    logInProcess.PinFunct.Domain = logInProcess.PinFunct.Domain[0].Equals('.') ? logInProcess.PinFunct.Domain.Remove(0, 1)
                        : logInProcess.PinFunct.Domain;
                }

                //Switch to Business Mode
                if (PinterestAccountType.Inactive == account.AccountBaseModel.PinterestAccountType || isBusinessAccount)
                {
                    //var isBusiness = logInProcess.PinFunct.SwithcToBusiness(account);
                    GlobusLogHelper.log.Info(Log.CustomMessage, SocialNetworks.Pinterest, account.AccountBaseModel.UserName,"Switch To Business", "Please Wait,Trying To Switch Into Business Account");
                    var SwitchToBusinessAccountResponse = logInProcess.BrowserManager.SwithcToBusinessAccount(account,logInProcess.BrowserManager);
                    if (SwitchToBusinessAccountResponse.status)
                    {
                        GlobusLogHelper.log.Info(Log.CustomMessage, SocialNetworks.Pinterest, account.AccountBaseModel.UserName,
                        "Switch To Business", "Successfully Switched To Business Account");
                        account.AccountBaseModel.PinterestAccountType = PinterestAccountType.Active;
                        account.DisplayColumnValue11 = PinterestAccountType.Active.GetDescriptionAttr()
                                .FromResourceDictionary();
                        SocinatorAccountBuilder.Instance(account.AccountBaseModel.AccountId)
                            .AddOrUpdateDominatorAccountBase(account.AccountBaseModel)
                            .AddOrUpdateCookies(account.Cookies)
                            .AddOrUpdateUserAgentWeb(account.UserAgentWeb)
                            .AddOrUpdateDisplayColumn11(account.DisplayColumnValue11)
                            .SaveToBinFile();

                        Thread.Sleep(5000);

                        GlobusLogHelper.log.Info(Log.CustomMessage, SocialNetworks.Pinterest, account.AccountBaseModel.UserName,
                        "Update Details", $"Please Wait, Updating {account.AccountBaseModel.UserFullName} Details");
                        await UpdateDetailsAsync(account, token);
                    }else
                        GlobusLogHelper.log.Info(Log.CustomMessage, SocialNetworks.Pinterest, account.AccountBaseModel.UserName,
                        "Switch To Business","LangKeyFailedTo".FromResourceDictionary()+" Switch Account To Business "+ "LangKeyWithError".FromResourceDictionary()+$" ==> {SwitchToBusinessAccountResponse.Response}");
                }

                //Switch to Normal Mode
                else if (PinterestAccountType.Active == account.AccountBaseModel.PinterestAccountType || !isBusinessAccount)
                {
                    var isPrivate = logInProcess.PinFunct.SwitchToPrivate(account);

                    if (isPrivate)
                    {
                        account.AccountBaseModel.PinterestAccountType = PinterestAccountType.Inactive;
                        account.DisplayColumnValue11 = PinterestAccountType.Inactive.GetDescriptionAttr()
                                .FromResourceDictionary();
                        SocinatorAccountBuilder.Instance(account.AccountBaseModel.AccountId)
                            .AddOrUpdateDominatorAccountBase(account.AccountBaseModel)
                            .AddOrUpdateCookies(account.Cookies)
                            .AddOrUpdateDisplayColumn11(account.DisplayColumnValue11)
                            .AddOrUpdateUserAgentWeb(account.UserAgentWeb)
                            .SaveToBinFile();

                        Thread.Sleep(5000);

                        GlobusLogHelper.log.Info(Log.CustomMessage, SocialNetworks.Pinterest, account.AccountBaseModel.UserName,
                        "Update Board", "Please Wait, Updating Board Details");
                        await UpdateDetailsAsync(account, token);
                    }
                }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        public bool SolveCaptchaManually(DominatorAccountModel accountModel)
        {
            return false;
        }
    }
}