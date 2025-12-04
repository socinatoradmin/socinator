using DominatorHouseCore;
using DominatorHouseCore.DatabaseHandler.FdTables.Accounts;
using DominatorHouseCore.DatabaseHandler.Utility;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Enums.DHEnum;
using DominatorHouseCore.FileManagers;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using DominatorHouseCore.ViewModel;
using FaceDominatorCore.FDLibrary.FdFunctions;
using FaceDominatorCore.FDLibrary.FdProcesses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using Unity;

namespace FaceDominatorCore.FDFactories
{
    public interface IFdAccountUpdateFactory : IAccountUpdateFactoryAsync { }

    public class FdAccountUpdateFactory : IFdAccountUpdateFactory
    {


        private static FdAccountUpdateFactory _instance;
        public static object LockAccountUpdate = new object();
        private DbOperations _dbGrowthOperations;
        private readonly IAccountScopeFactory _accountScopeFactory;
        private readonly IAccountsFileManager _accountsFileManager;
        private readonly SocialNetworks _networks;
        private IFdLoginProcess _fdLoginProcess;
        public IFdRequestLibrary _fdRequestLibrary;

        public static FdAccountUpdateFactory Instance
            => _instance ?? (_instance = new FdAccountUpdateFactory());

        FdAccountUpdateFactory() { }
        public static ActionBlock<FdUpdateAccountProcess> updaterBlock;

        public static ActionBlock<FdUpdateAccountProcess> UpdateInstance =>
            //Create object of FdUpdateAccountProcess
            updaterBlock ?? (updaterBlock = new ActionBlock<FdUpdateAccountProcess>(
                //Calling Method asyn UpdateAllUserDetails
                async job => await job.UpdateAllUserDetails(updaterBlock),
                new ExecutionDataflowBlockOptions { MaxDegreeOfParallelism = 7 }));

        public ActionBlock<FdUpdateAccountProcess> updaterFriendsBlock;

        public ActionBlock<FdUpdateAccountProcess> UpdateFriendsInstance =>
            //Create object of FdUpdateAccountProcess
            updaterFriendsBlock ?? (updaterFriendsBlock = new ActionBlock<FdUpdateAccountProcess>(
                //Calling Method asyn UpdateAllUserDetails
                async job => await job.UpdateFriendsNewAsync(updaterFriendsBlock),
                //number of parallel threads
                new ExecutionDataflowBlockOptions { MaxDegreeOfParallelism = 3 }));

        public FdAccountUpdateFactory(IAccountScopeFactory accountScopeFactory,
            IAccountsFileManager accountsFileManager)
        {
            _accountScopeFactory = accountScopeFactory;
            _accountsFileManager = accountsFileManager;
            _networks = SocialNetworks.Facebook;
        }


        public bool CheckStatus(DominatorAccountModel accountModel)
        => CheckStatusAsync(accountModel, accountModel.Token).Result;


        public bool SolveCaptchaManually(DominatorAccountModel accountModel) { return false; }

        public void UpdateDetails(DominatorAccountModel accountModel)
        {
            try
            {
                UpdateDetailsAsync(accountModel, accountModel.Token).Wait();
            }

            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

        }

        public async Task<bool> CheckStatusAsync(DominatorAccountModel accountModel, CancellationToken token)
        {
            var fdLoginProcess = _accountScopeFactory[accountModel.AccountId].Resolve<IFdLoginProcess>();

            return await fdLoginProcess.CheckLoginAsync(accountModel, token, displayLoginMsg: true);
        }

        public async Task UpdateDetailsAsync(DominatorAccountModel accountModel, CancellationToken token)
        {

            var fdLoginProcess = _accountScopeFactory[$"{accountModel.AccountId}"].Resolve<IFdLoginProcess>();
            if (accountModel.IsUserLoggedIn == false)
                await fdLoginProcess.CheckLoginAsync(accountModel, accountModel.Token,loginType:LoginType.UpdateDetails);
            // fdLoginProcess.CheckLogin(accountModel, token);

            if (accountModel.IsUserLoggedIn)
            {
                try
                {
                    await UpdateInstance.SendAsync(new FdUpdateAccountProcess(accountModel, _accountScopeFactory));

                    //        await UpdateFriendsInstance.SendAsync(new FdUpdateAccountProcess(accountModel, _accountScopeFactory));
                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                }

            }

        }

        public bool AddToDailyGrowth(string accountId, int friends, int joinedGroups, int ownPages)
        {
            bool success = true;
            try
            {
                _dbGrowthOperations = new DbOperations(accountId, SocialNetworks.Facebook, ConstantVariable.GetAccountDb);
                var existingDataForToday = _dbGrowthOperations.Get<DailyStatitics>().FirstOrDefault(
                                            x => x.Date.Year == DateTime.Today.Year && x.Date.Month == DateTime.Today.Month && x.Date.Day == DateTime.Today.Day);
                if (existingDataForToday != null)
                {
                    existingDataForToday.Date = DateTime.Now;
                    existingDataForToday.Friends = friends;
                    existingDataForToday.JoinedGroups = joinedGroups;
                    existingDataForToday.OwnPages = ownPages;
                    _dbGrowthOperations.Update(existingDataForToday);
                }
                else
                {
                    if (_dbGrowthOperations.Count<DailyStatitics>() == 0)
                    {
                        _dbGrowthOperations.Add(new DailyStatitics()
                        {
                            Date = DateTime.Now.AddDays(-1),
                            Friends = friends,
                            JoinedGroups = joinedGroups,
                            OwnPages = ownPages
                        });
                        _dbGrowthOperations.Add(new DailyStatitics()
                        {
                            Date = DateTime.Now,
                            Friends = friends,
                            JoinedGroups = joinedGroups,
                            OwnPages = ownPages
                        });
                    }
                    else
                    {
                        _dbGrowthOperations.Add(new DailyStatitics()
                        {
                            Date = DateTime.Now,
                            Friends = friends,
                            JoinedGroups = joinedGroups,
                            OwnPages = ownPages
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

        public DailyStatisticsViewModel GetDailyGrowth(string accountId, string username, GrowthPeriod period)
        {
            var response = new DailyStatisticsViewModel();
            try
            {

                _dbGrowthOperations = new DbOperations(accountId, SocialNetworks.Facebook, ConstantVariable.GetAccountDb);

                List<DailyStatitics> growthList;
                if (period == GrowthPeriod.Daily)
                {
                    growthList = _dbGrowthOperations.Get<DailyStatitics>().Where(x => x.Date >= DateTime.Today.AddDays(-1)).OrderBy(x => x.Date).ToList();
                }
                else if (period == GrowthPeriod.Weekly)
                {
                    growthList = _dbGrowthOperations.Get<DailyStatitics>().Where(x => x.Date >= DateTime.Today.AddDays(-7)).OrderBy(x => x.Date).ToList();
                }
                else if (period == GrowthPeriod.Monthly)
                {
                    growthList = _dbGrowthOperations.Get<DailyStatitics>().Where(x => x.Date >= DateTime.Today.AddMonths(-1)).OrderBy(x => x.Date).ToList();
                }
                else
                {
                    growthList = _dbGrowthOperations.Get<DailyStatitics>().OrderBy(x => x.Date).ToList();
                }
                if (growthList.Count > 0)
                {
                    var oldRecord = growthList.FirstOrDefault();
                    var latestRecord = growthList.LastOrDefault();
                    if (latestRecord != null)
                    {
                        if (oldRecord != null)
                        {
                            response.GrowthColumnValue1 = latestRecord.Friends - oldRecord.Friends;
                            response.GrowthColumnValue3 = latestRecord.JoinedGroups - oldRecord.JoinedGroups;
                            response.GrowthColumnValue5 = latestRecord.OwnPages - oldRecord.OwnPages;
                        }
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
            try
            {
                _dbGrowthOperations = new DbOperations(accountId, SocialNetworks.Facebook, ConstantVariable.GetAccountDb);

                int counter;
                List<DailyStatitics> growthList;
                if (period == GrowthChartPeriod.PastDay)
                {
                    growthList = _dbGrowthOperations.Get<DailyStatitics>().Where(x => x.Date >= DateTime.Today.AddDays(-1)).OrderBy(x => x.Date).ToList();
                    counter = 2;

                }
                else if (period == GrowthChartPeriod.Past3Months)
                {
                    counter = 3;
                    isMonthly = true;
                    growthList = _dbGrowthOperations.Get<DailyStatitics>().Where(x => x.Date >= DateTime.Today.AddMonths(-3)).OrderBy(x => x.Date).ToList();

                    var q = from gt in growthList
                            let dt = gt.Date
                            group gt by new { m = dt.Month } into g
                            select g.OrderByDescending(t => t.Date).FirstOrDefault();
                    growthList = q.ToList();
                }
                else if (period == GrowthChartPeriod.Past30Days)
                {
                    counter = 30;
                    growthList = _dbGrowthOperations.Get<DailyStatitics>().Where(x => x.Date >= DateTime.Today.AddDays(-30)).OrderBy(x => x.Date).ToList();
                    var q = from gt in growthList
                            let dt = gt.Date
                            group gt by new { y = dt.Year, m = dt.Month, d = dt.Day } into g
                            select g.OrderByDescending(t => t.Date).LastOrDefault();
                    growthList = q.ToList();



                }
                else if (period == GrowthChartPeriod.Past6Months)
                {
                    counter = 6;
                    isMonthly = true;
                    growthList = _dbGrowthOperations.Get<DailyStatitics>().Where(x => x.Date >= DateTime.Today.AddMonths(-6)).OrderBy(x => x.Date).ToList();

                    var q = from gt in growthList
                            let dt = gt.Date
                            group gt by new { m = dt.Month } into g
                            select g.OrderByDescending(t => t.Date).LastOrDefault();
                    growthList = q.ToList();
                }
                else if (period == GrowthChartPeriod.PastWeek)
                {
                    counter = 7;

                    growthList = _dbGrowthOperations.Get<DailyStatitics>().Where(x => x.Date >= DateTime.Today.AddDays(-7)).OrderBy(x => x.Date).ToList();
                    var q = from gt in growthList
                            let dt = gt.Date
                            group gt by new { y = dt.Year, m = dt.Month, d = dt.Day } into g
                            select g.OrderByDescending(t => t.Date).LastOrDefault();
                    growthList = q.ToList();
                }
                else
                {
                    growthList = _dbGrowthOperations.Get<DailyStatitics>().OrderBy(x => x.Date).ToList();
                    if (growthList.FirstOrDefault()?.Date < growthList.LastOrDefault()?.Date.AddDays(-30))
                    {
                        var q = from gt in growthList
                                let dt = gt.Date
                                group gt by new { m = dt.Month } into g
                                select g.OrderByDescending(t => t.Date).LastOrDefault();
                        growthList = q.ToList();
                        isMonthly = true;
                    }
                    else
                    {
                        var q = from gt in growthList
                                let dt = gt.Date
                                group gt by new { y = dt.Year, m = dt.Month, d = dt.Day } into g
                                select g.OrderByDescending(t => t.Date).LastOrDefault();
                        growthList = q.ToList();


                    }
                    counter = growthList.Count;
                }
                if (growthList.Count > 0)
                {

                    //foreach (var growth in growthList) {
                    //    var growthToAdd = new DailyStatisticsViewModel();
                    //    growthToAdd.GrowthColumnValue1 = growth.Followers;
                    //    growthToAdd.GrowthColumnValue2 = growth.Followings;
                    //    growthToAdd.GrowthColumnValue3 = growth.Tweets;
                    //    responseList.Add(growthToAdd);
                    //}

                    var lastAvailableRecord = new DailyStatitics();
                    var currentDate = DateTime.Now.Date;
                    for (int i = counter - 1; i >= 0; i--)
                    {
                        var setToZero = false;
                        var growthToAdd = new DailyStatisticsViewModel();

                        var record = isMonthly ? growthList.FirstOrDefault(x => x.Date.Month == currentDate.AddMonths(-i).Month) : growthList.FirstOrDefault(x => x.Date.Date == currentDate.AddDays(-i).Date);
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

                        growthToAdd.GrowthColumnValue1 = setToZero || record == null ? 0 : record.Friends;
                        growthToAdd.GrowthColumnValue2 = setToZero || record == null ? 0 : record.JoinedGroups;
                        growthToAdd.GrowthColumnValue3 = setToZero || record == null ? 0 : record.OwnPages;
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
    }
}
