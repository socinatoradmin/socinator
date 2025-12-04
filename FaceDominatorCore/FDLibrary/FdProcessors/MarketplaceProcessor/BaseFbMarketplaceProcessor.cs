using DominatorHouseCore.Models;
using DominatorHouseCore.Process;
using FaceDominatorCore.FDLibrary.DAL;
using FaceDominatorCore.FDLibrary.FdFunctions;
using FaceDominatorCore.FDLibrary.FdFunctions.FdBrowserManager;
using FaceDominatorCore.FDLibrary.FdProcesses;
using FaceDominatorCore.FDModel.FilterModel;
using FaceDominatorCore.FDResponse.AccountsResponse;
using FaceDominatorCore.FDResponse.ScrapersResponse;

namespace FaceDominatorCore.FDLibrary.FdProcessors.MarketplaceProcessor
{
    public class BaseFbMarketplaceProcessor : BaseFbProcessor
    {
        FanpageScraperResponseHandler _objFanpageScraperResponseHandler;

        private readonly FdJobProcess _objFdJobProcess;

        private readonly IDbAccountServiceScoped _dbAccountService;

        protected readonly MarketplaceFilterModel MarketplaceFilterModel;

        protected AccountMarketplaceDetailsHandler AccountMarketplaceDetailsHandler;


        protected BaseFbMarketplaceProcessor(IFdJobProcess jobProcess, IDbAccountServiceScoped dbAccountService,
         IDbCampaignServiceScoped campaignService, IFdRequestLibrary objFdRequestLibrary,
         IFdBrowserManager browserManager, IProcessScopeModel processScopeModel)
            : base(jobProcess, dbAccountService, campaignService, objFdRequestLibrary, browserManager, processScopeModel)
        {

            _dbAccountService = dbAccountService;
            _objFdJobProcess = (FdJobProcess)jobProcess;
            _objFanpageScraperResponseHandler = null;
            MarketplaceFilterModel = jobProcess.ModuleSetting.MarketplaceFilterModel;
        }

        protected void UpdateMarketplaceDetails()
        {
            FdFunctions.FdFunctions objFdFunctions = new FdFunctions.FdFunctions(AccountModel);
            AccountMarketplaceDetailsHandler = ObjFdRequestLibrary.GetAccountMarketPlaceDetails(AccountModel);
            objFdFunctions.SaveMarketPlaceDetails(AccountMarketplaceDetailsHandler);
        }


        protected void ChangeMarketplaceLocation(string location)
        {
            FdFunctions.FdFunctions objFdFunctions = new FdFunctions.FdFunctions(AccountModel);
            AccountMarketplaceDetailsHandler = ObjFdRequestLibrary.ChangeMarketplaceLocation(AccountModel, location, MarketplaceFilterModel);
            objFdFunctions.SaveMarketPlaceDetails(AccountMarketplaceDetailsHandler);
        }


        protected override void Process(QueryInfo queryInfo, ref JobProcessResult jobProcessResult)
        {

        }
    }
}
