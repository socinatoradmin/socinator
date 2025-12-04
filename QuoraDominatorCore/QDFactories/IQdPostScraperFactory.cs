using System;
using System.Threading;
using System.Threading.Tasks;
using CommonServiceLocator;
using DominatorHouseCore;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;

namespace QuoraDominatorCore.QDFactories
{
    public interface IQdPostScraperFactory : IAdScraperFactory
    {
    }

    public class QdPostScraperFactory : IQdPostScraperFactory
    {
        private readonly IAccountScopeFactory _accountScopeFactory;

        public QdPostScraperFactory()
        {
            _accountScopeFactory = InstanceProvider.GetInstance<IAccountScopeFactory>();
        }


        public async Task<bool> CheckStatusAsync(DominatorAccountModel accountModel, CancellationToken token)
        {
            try
            {
                //var login = _accountScopeFactory[accountModel.AccountId].Resolve<IQdLogInProcess>();
                //await login.LoginForAdsScrapingAsync(accountModel, token);

                //if (accountModel.AccountBaseModel.Status == AccountStatus.Success)
                    return true;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            return false;
        }

        public async Task ScrapeAdsAsync(DominatorAccountModel accountModel, CancellationToken token, string jobid = "")
        {
            //await ThreadFactory.Instance.Start(async () =>
            //{
            //    var adScraperProcess = _accountScopeFactory[accountModel.AccountId].Resolve<IQdAdScraperProcess>();
            //    await adScraperProcess.ScrapeAdsAsync(accountModel, token);
            //});
        }
    }
}