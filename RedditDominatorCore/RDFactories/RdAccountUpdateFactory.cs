using CommonServiceLocator;
using DominatorHouseCore;
using DominatorHouseCore.DatabaseHandler.RdTables.Accounts;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Enums.DHEnum;
using DominatorHouseCore.Extensions;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using DominatorHouseCore.ViewModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RedditDominatorCore.Interface;
using RedditDominatorCore.RDLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Unity;

namespace RedditDominatorCore.RDFactories
{
    public class RdAccountUpdateFactory : IRdAccountUpdateFactory
    {
        #region .ctor

        public RdAccountUpdateFactory(IAccountScopeFactory accountScopeFactory)
        {
            _accountScopeFactory = accountScopeFactory;
            _networks = SocialNetworks.Reddit;
        }

        #endregion

        #region private Methods

        private bool AddToDailyGrowth(string accountId, int score, int communities, int postKarma, int commentKarma)
        {
            var success = true;
            var today = DateTime.Today;
            var tomorrow = today.AddDays(1);

            try
            {
                var dbGrowthOperations = InstanceProvider.ResolveAccountDbOperations(accountId, _networks);
                var existingDataForToday =
                    dbGrowthOperations.GetSingle<DailyStatitics>(x => x.Date >= today && x.Date < tomorrow);

                if (existingDataForToday != null)
                {
                    existingDataForToday.Date = DateTime.Now;
                    existingDataForToday.Score = score;
                    existingDataForToday.Communities = communities;
                    existingDataForToday.PostKarma = postKarma;
                    existingDataForToday.CommentKarma = commentKarma;
                    dbGrowthOperations.Update(existingDataForToday);
                }
                else
                {
                    if (dbGrowthOperations.Get<DailyStatitics>().Count == 0)
                    {
                        dbGrowthOperations.Add(new DailyStatitics
                        {
                            Date = DateTime.Now.AddDays(-1),
                            Score = score,
                            Communities = communities,
                            PostKarma = postKarma,
                            CommentKarma = commentKarma
                        });
                        dbGrowthOperations.Add(new DailyStatitics
                        {
                            Date = DateTime.Now,
                            Score = score,
                            Communities = communities,
                            PostKarma = postKarma,
                            CommentKarma = commentKarma
                        });
                    }
                    else
                    {
                        dbGrowthOperations.Add(new DailyStatitics
                        {
                            Date = DateTime.Now,
                            Score = score,
                            Communities = communities,
                            PostKarma = postKarma,
                            CommentKarma = commentKarma
                        });
                    }
                }
            }
            catch (AssertFailedException)
            {
                throw;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
                success = false;
            }

            return success;
        }

        #endregion

        #region Private fields

        private readonly IAccountScopeFactory _accountScopeFactory;
        private readonly SocialNetworks _networks;

        #endregion


        #region Public fields 

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
            return await _accountScopeFactory[accountModel.AccountId].Resolve<IRedditLogInProcess>()
                .CheckLoginAsync(accountModel, token, true);
        }

        public async Task UpdateDetailsAsync(DominatorAccountModel accountModel, CancellationToken token)
        {
            try
            {
                if (!accountModel.IsUserLoggedIn)
                    _accountScopeFactory[accountModel.AccountId].Resolve<IRedditLogInProcess>()
                        .CheckLogin(accountModel, token);

                if (accountModel.IsUserLoggedIn)
                {
                    var rdUpdateAccountProcess =
                        InstanceProvider.GetInstance<IRdUpdateAccountProcess>(_networks.ToString());

                    await rdUpdateAccountProcess.UpdateAllUserDetails(accountModel, token);
                    if (accountModel.DisplayColumnValue1 != null)
                        if (accountModel.DisplayColumnValue2 != null)
                            if (accountModel.DisplayColumnValue3 != null)
                                if (accountModel.DisplayColumnValue4 != null)
                                    AddToDailyGrowth(accountModel.AccountBaseModel.AccountId,
                                        accountModel.DisplayColumnValue1.Value, accountModel.DisplayColumnValue2.Value,
                                        accountModel.DisplayColumnValue3.Value, accountModel.DisplayColumnValue4.Value);
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
        }

        DailyStatisticsViewModel IAccountUpdateFactory.GetDailyGrowth(string accountId, string username,
            GrowthPeriod period)
        {
            var response = new DailyStatisticsViewModel();
            try
            {
                var dbGrowthOperations = InstanceProvider.ResolveAccountDbOperations(accountId, _networks);

                List<DailyStatitics> growthList;
                // ReSharper disable once SwitchStatementMissingSomeCases
                switch (period)
                {
                    case GrowthPeriod.Daily:
                        growthList = dbGrowthOperations.Get<DailyStatitics>()
                            .Where(x => x.Date >= DateTime.Today.AddDays(-1)).OrderBy(x => x.Date).ToList();
                        break;
                    case GrowthPeriod.Weekly:
                        growthList = dbGrowthOperations.Get<DailyStatitics>()
                            .Where(x => x.Date >= DateTime.Today.AddDays(-7)).OrderBy(x => x.Date).ToList();
                        break;
                    case GrowthPeriod.Monthly:
                        growthList = dbGrowthOperations.Get<DailyStatitics>()
                            .Where(x => x.Date >= DateTime.Today.AddMonths(-1)).OrderBy(x => x.Date).ToList();
                        break;
                    default:
                        growthList = dbGrowthOperations.Get<DailyStatitics>().OrderBy(x => x.Date).ToList();
                        break;
                }

                if (growthList.Count > 0)
                {
                    var oldRecord = growthList.FirstOrDefault();
                    var latestRecord = growthList.LastOrDefault();
                    if (latestRecord != null)
                        if (oldRecord != null)
                        {
                            response.GrowthColumnValue1 = latestRecord.Score - oldRecord.Score;
                            response.GrowthColumnValue3 = latestRecord.Communities - oldRecord.Communities;
                            response.GrowthColumnValue4 = latestRecord.PostKarma - oldRecord.PostKarma;
                            response.GrowthColumnValue5 = latestRecord.CommentKarma - oldRecord.CommentKarma;
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
                var dbGrowthOperations = InstanceProvider.ResolveAccountDbOperations(accountId, _networks);

                int counter;
                List<DailyStatitics> growthList;
                // ReSharper disable once SwitchStatementMissingSomeCases
                switch (period)
                {
                    case GrowthChartPeriod.PastDay:
                        growthList = dbGrowthOperations.Get<DailyStatitics>()
                            .Where(x => x.Date >= DateTime.Today.AddDays(-1)).OrderBy(x => x.Date).ToList();
                        counter = 2;
                        break;
                    case GrowthChartPeriod.Past3Months:
                        {
                            counter = 3;
                            isMonthly = true;
                            growthList = dbGrowthOperations.Get<DailyStatitics>()
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
                            growthList = dbGrowthOperations.Get<DailyStatitics>()
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
                            growthList = dbGrowthOperations.Get<DailyStatitics>()
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

                            growthList = dbGrowthOperations.Get<DailyStatitics>()
                                .Where(x => x.Date >= DateTime.Today.AddDays(-7)).OrderBy(x => x.Date).ToList();
                            var q = from gt in growthList
                                    let dt = gt.Date
                                    group gt by new { y = dt.Year, m = dt.Month, d = dt.Day }
                                into g
                                    select g.OrderByDescending(t => t.Date).LastOrDefault();
                            growthList = q.ToList();
                            break;
                        }

                    default:
                        {
                            growthList = dbGrowthOperations.Get<DailyStatitics>().OrderBy(x => x.Date).ToList();
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

                        growthToAdd.GrowthColumnValue1 = setToZero || record == null ? 0 : record.Score;
                        growthToAdd.GrowthColumnValue2 = setToZero || record == null ? 0 : record.Communities;
                        growthToAdd.GrowthColumnValue3 = setToZero || record == null ? 0 : record.PostKarma;
                        growthToAdd.GrowthColumnValue4 = setToZero || record == null ? 0 : record.CommentKarma;
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
            return true;
        }

        #endregion
    }
}