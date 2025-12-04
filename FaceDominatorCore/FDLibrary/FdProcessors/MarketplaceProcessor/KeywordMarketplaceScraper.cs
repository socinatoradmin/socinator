using DominatorHouseCore;
using DominatorHouseCore.Models;
using DominatorHouseCore.Process;
using FaceDominatorCore.FDLibrary.DAL;
using FaceDominatorCore.FDLibrary.FdFunctions;
using FaceDominatorCore.FDLibrary.FdFunctions.FdBrowserManager;
using FaceDominatorCore.FDLibrary.FdProcesses;
using FaceDominatorCore.FDResponse.ScrapersResponse;
using System;

namespace FaceDominatorCore.FDLibrary.FdProcessors.MarketplaceProcessor
{
    public class KeywordMarketplaceScraper : BaseFbMarketplaceProcessor
    {
        public KeywordMarketplaceScraper(IFdJobProcess jobProcess, IDbAccountServiceScoped dbAccountService,
            IDbCampaignServiceScoped campaignService, IFdRequestLibrary objFdRequestLibrary,
            IFdBrowserManager browserManager, IProcessScopeModel processScopeModel)
            : base(jobProcess, dbAccountService, campaignService, objFdRequestLibrary, browserManager, processScopeModel)
        {
        }

        protected override void Process(QueryInfo queryInfo, ref JobProcessResult jobProcessResult)
        {
            UpdateMarketplaceDetails();

            try
            {

                if (MarketplaceFilterModel.IsLocationFilterChecked)
                {
                    foreach (var location in MarketplaceFilterModel.LstLocation)
                    {
                        ChangeMarketplaceLocation(location);
                        ScrapeDataForEachLocation(queryInfo, ref jobProcessResult);
                    }
                }
                else
                    ScrapeDataForEachLocation(queryInfo, ref jobProcessResult);

            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            jobProcessResult.IsProcessCompleted = true;
        }

        private void ScrapeDataForEachLocation(QueryInfo queryInfo, ref JobProcessResult jobProcessResult)
        {
            string keyword = queryInfo.QueryValue;

            MarketplaceScraperResponseHandler marketplaceScraperResponseHandle = null;

            while (!jobProcessResult.IsProcessCompleted && !jobProcessResult.HasNoResult)
            {
                marketplaceScraperResponseHandle =
                    ObjFdRequestLibrary.GetProductListFromMarketplace(AccountModel, keyword, AccountMarketplaceDetailsHandler,
                    MarketplaceFilterModel, marketplaceScraperResponseHandle);
            }
        }

    }
}
