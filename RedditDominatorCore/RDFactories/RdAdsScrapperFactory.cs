using CommonServiceLocator;
using DominatorHouseCore;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using RedditDominatorCore.Interface;
using RedditDominatorCore.RDLibrary;
using System;
using System.Threading;
using System.Threading.Tasks;
using Unity;

namespace RedditDominatorCore.RDFactories
{
    public class RdAdsScrapperFactory : IRdAdsScrapperFactory
    {
        private readonly IAccountScopeFactory _accountScopeFactory;
        public RdAdsScrapperFactory()
        {
            _accountScopeFactory = InstanceProvider.GetInstance<IAccountScopeFactory>();
        }
        public async Task<bool> CheckStatusAsync(DominatorAccountModel accountModel, CancellationToken token)
        {
            try
            {
                var login = _accountScopeFactory[accountModel.AccountId].Resolve<IRedditLogInProcess>();
                await login.LoginForAdsScrapingAsync(accountModel, token);
                if (accountModel.AccountBaseModel.Status == AccountStatus.Success)
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
            //await ThreadFactory.Instance.Start(async()=> {
            //    var adScraperProcess = _accountScopeFactory[accountModel.AccountId].Resolve<IRdAdScrapperProcess>();
            //    await adScraperProcess.ScrapeAdsAsync(accountModel, token);
            //});
        }
    }
}
