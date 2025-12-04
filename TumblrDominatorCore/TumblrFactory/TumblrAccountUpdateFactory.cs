using DominatorHouseCore;
using DominatorHouseCore.DatabaseHandler.TumblrTables.Account;
using DominatorHouseCore.DatabaseHandler.Utility;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Enums.DHEnum;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using DominatorHouseCore.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TumblrDominatorCore.Interface;
using TumblrDominatorCore.TumblrLibrary.TumblrProcesses;
using Unity;

namespace TumblrDominatorCore.TumblrFactory
{
    public class TumblrAccountUpdateFactory : ITumblrAccountUpdateFactory
    {
        #region private Field
        private readonly IAccountScopeFactory _accountScopeFactory;
        private DbOperations _dbGrowthOperations;
        private readonly SocialNetworks currentNetwork;
        #endregion
        public TumblrAccountUpdateFactory(IAccountScopeFactory accountScopeFactory)
        {
            _accountScopeFactory = accountScopeFactory;
            currentNetwork = SocialNetworks.Tumblr;
        }


        public bool CheckStatus(DominatorAccountModel accountModel)
        {
            return CheckStatusAsync(accountModel, accountModel.CancellationSource.Token).Result;
        }

        public void UpdateDetails(DominatorAccountModel accountModel)
        {
            UpdateDetailsAsync(accountModel, accountModel.CancellationSource.Token).Wait();
        }
        public async Task<bool> CheckStatusAsync(DominatorAccountModel accountModel, CancellationToken token)
        {
            return await _accountScopeFactory[accountModel.AccountId].Resolve<ITumblrLoginProcess>().CheckLoginAsync(accountModel, token, true);
        }


        public async Task UpdateDetailsAsync(DominatorAccountModel accountModel, CancellationToken token)
        {
            await _accountScopeFactory[accountModel.AccountId].Resolve<ITumblrLoginProcess>().UpdateAccountAsync(accountModel, accountModel.CancellationSource.Token);
            if (accountModel.IsUserLoggedIn)
                GlobusLogHelper.log.Info(Log.DetailsUpdated, accountModel.AccountBaseModel.AccountNetwork,
                   accountModel.AccountBaseModel.UserName, "Update Details");
            else
                GlobusLogHelper.log.Info(Log.CustomMessage, accountModel.AccountBaseModel.AccountNetwork,
                    accountModel.AccountBaseModel.UserName, "Failed to Update Account details");
        }

        public bool AddToDailyGrowth(string accountId, int followers, int following, int postsCount, int channelCount)
        {
            var success = true;
            try
            {
                _dbGrowthOperations = new DbOperations(accountId, currentNetwork, ConstantVariable.GetAccountDb);

                var existingDataForToday = _dbGrowthOperations.GetSingle<DailyStatitics>(
                    x => x.Date.Year == DateTime.Today.Year && x.Date.Month == DateTime.Today.Month &&
                         x.Date.Day == DateTime.Today.Day);

                if (existingDataForToday != null)
                {
                    existingDataForToday.Date = DateTime.Now;
                    existingDataForToday.Followers = followers;
                    existingDataForToday.Followings = following;
                    existingDataForToday.PostsCount = postsCount;
                    existingDataForToday.ChannelsCount = channelCount;
                    _dbGrowthOperations.Update(existingDataForToday);
                }
                else
                {
                    if (_dbGrowthOperations.Get<DailyStatitics>().Count == 0)
                    {
                        _dbGrowthOperations.Add(new DailyStatitics
                        {
                            Date = DateTime.Now.AddDays(-1),
                            Followers = followers,
                            Followings = following,
                            PostsCount = postsCount,
                            ChannelsCount = channelCount
                        });
                        _dbGrowthOperations.Add(new DailyStatitics
                        {
                            Date = DateTime.Now,
                            Followers = followers,
                            Followings = following,
                            PostsCount = postsCount,
                            ChannelsCount = channelCount
                        });
                    }
                    else
                    {
                        _dbGrowthOperations.Add(new DailyStatitics
                        {
                            Date = DateTime.Now,
                            Followers = followers,
                            Followings = following,
                            PostsCount = postsCount,
                            ChannelsCount = channelCount
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
                _dbGrowthOperations = new DbOperations(accountId, currentNetwork, ConstantVariable.GetAccountDb);

                List<DailyStatitics> growthList;
                if (period != GrowthPeriod.Daily)
                {
                    if (period != GrowthPeriod.Weekly)
                    {
                        if (period == GrowthPeriod.Monthly)
                            growthList = _dbGrowthOperations.Get<DailyStatitics>()
                                .Where(x => x.Date >= DateTime.Today.AddMonths(-1)).OrderBy(x => x.Date).ToList();
                        else
                            growthList = _dbGrowthOperations.Get<DailyStatitics>().OrderBy(x => x.Date).ToList();
                    }
                    else
                    {
                        growthList = _dbGrowthOperations.Get<DailyStatitics>()
                            .Where(x => x.Date >= DateTime.Today.AddDays(-7)).OrderBy(x => x.Date).ToList();
                    }
                }
                else
                {
                    growthList = _dbGrowthOperations.Get<DailyStatitics>()
                        .Where(x => x.Date >= DateTime.Today.AddDays(-1)).OrderBy(x => x.Date).ToList();
                }

                if (growthList.Count > 0)
                {
                    var oldRecord = growthList.FirstOrDefault();

                    var latestRecord = growthList.LastOrDefault();
                    if (latestRecord != null)
                        if (oldRecord != null)
                        {
                            response.GrowthColumnValue1 = latestRecord.Followers - oldRecord.Followers;
                            response.GrowthColumnValue2 = latestRecord.Followings - oldRecord.Followings;
                            response.GrowthColumnValue4 = latestRecord.PostsCount - oldRecord.PostsCount;
                            response.GrowthColumnValue5 = latestRecord.ChannelsCount - oldRecord.ChannelsCount;
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
                _dbGrowthOperations = new DbOperations(accountId, currentNetwork, ConstantVariable.GetAccountDb);

                var counter = 0;
                var growthList = new List<DailyStatitics>();
                if (period != GrowthChartPeriod.PastDay)
                {
                    switch (period)
                    {
                        case GrowthChartPeriod.Past3Months:
                            {
                                counter = 3;
                                isMonthly = true;
                                growthList = _dbGrowthOperations.Get<DailyStatitics>()
                                    .Where(x => x.Date >= DateTime.Today.AddMonths(-3)).OrderBy(x => x.Date).ToList();

                                var q = from gt in growthList
                                        let dt = gt.Date
                                        group gt by new { m = dt.Month }
                                    into g
                                        select g.OrderByDescending(t => t.Date).FirstOrDefault();
                                growthList = q.ToList();
                                break;
                            }

                        case GrowthChartPeriod.Past30Days:
                            {
                                counter = 30;
                                growthList = _dbGrowthOperations.Get<DailyStatitics>()
                                    .Where(x => x.Date >= DateTime.Today.AddDays(-30)).OrderBy(x => x.Date).ToList();
                                var q = from gt in growthList
                                        let dt = gt.Date
                                        group gt by new { y = dt.Year, m = dt.Month, d = dt.Day }
                                    into g
                                        select g.OrderByDescending(t => t.Date).LastOrDefault();
                                growthList = q.ToList();
                                break;
                            }

                        case GrowthChartPeriod.Past6Months:
                            {
                                counter = 6;
                                isMonthly = true;
                                growthList = _dbGrowthOperations.Get<DailyStatitics>()
                                    .Where(x => x.Date >= DateTime.Today.AddMonths(-6)).OrderBy(x => x.Date).ToList();

                                var q = from gt in growthList
                                        let dt = gt.Date
                                        group gt by new { m = dt.Month }
                                    into g
                                        select g.OrderByDescending(t => t.Date).LastOrDefault();
                                growthList = q.ToList();
                                break;
                            }

                        case GrowthChartPeriod.PastWeek:
                            {
                                counter = 7;

                                growthList = _dbGrowthOperations.Get<DailyStatitics>()
                                    .Where(x => x.Date >= DateTime.Today.AddDays(-7)).OrderBy(x => x.Date).ToList();
                                var q = from gt in growthList
                                        let dt = gt.Date
                                        group gt by new { y = dt.Year, m = dt.Month, d = dt.Day }
                                    into g
                                        select g.OrderByDescending(t => t.Date).LastOrDefault();
                                growthList = q.ToList();
                                break;
                            }

                        case GrowthChartPeriod.PastDay:
                            break;
                        case GrowthChartPeriod.AllTime:
                            break;
                        default:
                            growthList = _dbGrowthOperations.Get<DailyStatitics>().OrderBy(x => x.Date).ToList();
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
                            break;
                    }
                }
                else
                {
                    growthList = _dbGrowthOperations.Get<DailyStatitics>()
                        .Where(x => x.Date >= DateTime.Today.AddDays(-1)).OrderBy(x => x.Date).ToList();
                    counter = 2;
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

                        growthToAdd.GrowthColumnValue1 = setToZero || record == null ? 0 : record.Followers;
                        growthToAdd.GrowthColumnValue2 = setToZero || record == null ? 0 : record.Followings;
                        growthToAdd.GrowthColumnValue3 = setToZero || record == null ? 0 : record.PostsCount;
                        growthToAdd.GrowthColumnValue3 = setToZero || record == null ? 0 : record.ChannelsCount;
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

        public bool SolveCaptchaManually(DominatorAccountModel accountModel)
        {
            return false;
        }
    }
}