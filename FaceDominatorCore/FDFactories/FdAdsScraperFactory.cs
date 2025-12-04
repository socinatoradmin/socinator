using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Models;
using System.Threading;
using System.Threading.Tasks;

namespace FaceDominatorCore.FDFactories
{
    public interface IFdAdsScraperFactory : IAdScraperFactory { }

    public class FdAdsScraperFactory : IFdAdsScraperFactory
    {

        //private static FdAccountUpdateFactory _instance;
        //public static object LockAccountUpdate = new object();
        //private DbOperations _dbGrowthOperations;
        //private readonly IAccountScopeFactory _accountScopeFactory;
        //private readonly IAccountsFileManager _accountsFileManager;
        //private readonly SocialNetworks _networks;
        //private IFdLoginProcess _fdLoginProcess;
        //public IFdRequestLibrary _fdRequestLibrary;


        //public static ActionBlock<FdPostDetailsScraperProcess> updaterBlock;



        //public FdAdsScraperFactory(IAccountScopeFactory accountScopeFactory,
        //    IAccountsFileManager accountsFileManager)
        //{
        //    _accountScopeFactory = accountScopeFactory;
        //    _accountsFileManager = accountsFileManager;
        //    _networks = SocialNetworks.Facebook;
        //}





        public async Task<bool> CheckStatusAsync(DominatorAccountModel accountModel, CancellationToken token)
        {
            //    if (!accountModel.IsUserLoggedIn)
            //    {
            //        _fdLoginProcess = _accountScopeFactory[$"{accountModel.AccountId}_Scraper"].Resolve<IFdLoginProcess>();
            //        return await _fdLoginProcess.CheckLoginPostScrapperAsync(accountModel, token, true);

            //    }
            //    else
            //    {
            //        _fdLoginProcess = _accountScopeFactory[$"{accountModel.AccountId}_Scraper"].Resolve<IFdLoginProcess>();

            //        return await _fdLoginProcess.CheckLoginPostScrapperAsync(accountModel, token);
            //    }
            return false;
        }

        public async Task ScrapeAdsAsync(DominatorAccountModel accountModel, CancellationToken token, string jobId = "")
        {
            //    try
            //    {

            //        if (accountModel.IsRunProcessThroughBrowser)
            //        {

            //            if (accountModel.IsUserLoggedIn)
            //            {
            //                FdPostDetailsScraperThroughBrowserProcess fdPostDetailsScraperThroughBrowserProcess
            //                    = new FdPostDetailsScraperThroughBrowserProcess(accountModel);

            //                await fdPostDetailsScraperThroughBrowserProcess.StartScrapNewPostDetails();

            //            }

            //        }
            //        else
            //        {
            //            if (!accountModel.IsUserLoggedIn)
            //            {
            //                _fdLoginProcess = _accountScopeFactory[$"{accountModel.AccountId}_Scraper"].Resolve<IFdLoginProcess>();
            //                await _fdLoginProcess.CheckLoginPostScrapperAsync(accountModel, accountModel.Token);
            //                // fdLoginProcess.CheckLogin(accountModel, token);
            //            }

            //            if (accountModel.IsUserLoggedIn)
            //            {
            //                try
            //                {
            //                    FdPostDetailsScraperProcess postScraperProcess =
            //                        new FdPostDetailsScraperProcess(accountModel);

            //                    await postScraperProcess.StartScrapNewPostDetails();//AdsDetails
            //                }
            //                catch (Exception ex)
            //                {
            //                    Console.WriteLine(ex.ToString());
            //                }

            //            }
            //        }
            //    }
            //    catch (Exception ex)
            //    {
            //        Console.WriteLine(ex.ToString());
            //    }

        }


    }
}
