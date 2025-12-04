using CommonServiceLocator;
using DominatorHouseCore;
using DominatorHouseCore.Enums;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using DominatorHouseCore.Process;
using DominatorHouseCore.Utility;
using FaceDominatorCore.FDFactories;
using FaceDominatorCore.FDLibrary.DAL;
using FaceDominatorCore.FDLibrary.FdClassLibrary;
using FaceDominatorCore.FDModel.ScraperModel;
using FaceDominatorCore.FDRequest;
using Newtonsoft.Json;
using System;
using AccountInteractedPages = DominatorHouseCore.DatabaseHandler.FdTables.Accounts.InteractedPages;
using CampaignInteractedPages = DominatorHouseCore.DatabaseHandler.FdTables.Campaigns.InteractedPages;

namespace FaceDominatorCore.FDLibrary.FdProcesses
{


    public class FdFanpageScraperProcess : FdJobProcessInteracted<AccountInteractedPages>
    {
        public FanpageScraperModel FanpageScraperModel { get; set; }

        public DominatorAccountModel Account { get; set; }

        public FdFanpageScraperProcess(IProcessScopeModel processScopeModel,
            IDbAccountServiceScoped accountServiceScoped,
            IFdQueryScraperFactory queryScraperFactory,
            IFdHttpHelper fdHttpHelper, IFdLoginProcess fdLogInProcess, IDbCampaignServiceScoped dbCampaignServiceScoped)
            : base(processScopeModel, accountServiceScoped, queryScraperFactory, fdHttpHelper, fdLogInProcess, dbCampaignServiceScoped)
        {
            FanpageScraperModel = processScopeModel.GetActivitySettingsAs<FanpageScraperModel>();
            AccountModel = DominatorAccountModel;
            CheckJobProcessLimitsReached();


        }

        public override JobProcessResult PostScrapeProcess(ScrapeResultNew scrapeResult)
        {
            JobProcessResult jobProcessResult = new JobProcessResult();

            FanpageDetails objFanpageDetails = (FanpageDetails)scrapeResult.ResultPage;


            try
            {
                //var _accountScopeFactory = InstanceProvider.GetInstance<IAccountScopeFactory>();

                //IFdBrowserManager pageSpecificWindow = _accountScopeFactory[$"{AccountModel.AccountId}{objFanpageDetails.FanPageID}"]
                //                .Resolve<IFdBrowserManager>();

                //pageSpecificWindow.CloseBrowser(DominatorAccountModel);

                JobCancellationTokenSource.Token.ThrowIfCancellationRequested();

                GlobusLogHelper.log.Info(Log.ActivitySuccessful, DominatorAccountModel.AccountBaseModel.AccountNetwork,
                    DominatorAccountModel.AccountBaseModel.UserName, ActivityType, objFanpageDetails.FanPageID);
                IncrementCounters();
                AddProfileScraperDataToDatabase(scrapeResult);
                jobProcessResult.IsProcessSuceessfull = true;
                DelayBeforeNextActivity();
            }
            catch (OperationCanceledException)
            {
                throw new OperationCanceledException();
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            return jobProcessResult;
        }

        public override void StartOtherConfiguration(ScrapeResultNew scrapeResult)
        {

        }


        private void AddProfileScraperDataToDatabase(ScrapeResultNew scrapeResult)
        {
            try
            {

                FanpageDetails page = (FanpageDetails)scrapeResult.ResultPage;


                var jobActivityConfigurationManager = InstanceProvider.GetInstance<IJobActivityConfigurationManager>();

                var modulesetting = jobActivityConfigurationManager[DominatorAccountModel.AccountId, ActivityType];

                if (modulesetting == null)
                    return;

                if (modulesetting.IsTemplateMadeByCampaignMode)
                {
                    DbCampaignService.Add(new CampaignInteractedPages
                    {
                        ActivityType = ActivityType.ToString(),
                        QueryType = scrapeResult.QueryInfo.QueryType,
                        QueryValue = scrapeResult.QueryInfo.QueryValue,
                        AccountEmail = DominatorAccountModel.AccountBaseModel.UserName,
                        PageId = page.FanPageID,
                        PageUrl = page.FanPageUrl,
                        PageName = page.FanPageName,
                        PageType = page.FanPageCategory,
                        TotalLikers = page.FanPageLikerCount,
                        PageFullDetails = JsonConvert.SerializeObject(page),
                        InteractionTimeStamp = DateTimeUtilities.GetEpochTime(),
                        InteractionDateTime = DateTime.Now


                    });
                }

                DbAccountService.Add(new AccountInteractedPages
                {
                    ActivityType = ActivityType.ToString(),
                    QueryType = scrapeResult.QueryInfo.QueryType,
                    QueryValue = scrapeResult.QueryInfo.QueryValue,
                    PageId = page.FanPageID,
                    PageUrl = page.FanPageUrl,
                    PageName = page.FanPageName,
                    PageType = page.FanPageCategory,
                    TotalLikers = page.FanPageLikerCount,
                    PageFullDetails = JsonConvert.SerializeObject(page),
                    InteractionTimeStamp = DateTimeUtilities.GetEpochTime(),
                    InteractionDateTime = DateTime.Now
                });
            }
            catch (Exception ex)
            {
                ex.ErrorLog(ex.Message);
            }
        }


    }
}
