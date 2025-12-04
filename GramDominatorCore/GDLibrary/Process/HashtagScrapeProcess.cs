using ThreadUtils;
using DominatorHouseCore;
using DominatorHouseCore.DatabaseHandler.GdTables.Accounts;
using DominatorHouseCore.Enums;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using DominatorHouseCore.Process;
using DominatorHouseCore.Utility;
using GramDominatorCore.Factories;
using GramDominatorCore.GDLibrary.DAL;
using GramDominatorCore.GDLibrary.InstagramBrowser;
using GramDominatorCore.GDModel;
using GramDominatorCore.Request;
using System;
using GramDominatorCore.Utility;

namespace GramDominatorCore.GDLibrary
{
    public class HashtagScrapeProcess : GdJobProcessInteracted<HashtagScrape>
    {
        public HashtagScrapeProcess(IProcessScopeModel processScopeModel,
            IDbAccountServiceScoped accountServiceScoped, IGdQueryScraperFactory queryScraperFactory, IGdHttpHelper httpHelper, IGdBrowserManager gdBrowser, IDelayService _delayService)
            : base(processScopeModel, accountServiceScoped, queryScraperFactory, httpHelper, gdBrowser,_delayService)
        {
        }

        public override JobProcessResult PostScrapeProcess(ScrapeResultNew scrapeResult)
        {
            JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
            GlobusLogHelper.log.Info(Log.StartedActivity, DominatorAccountModel.AccountBaseModel.AccountNetwork,
                DominatorAccountModel.AccountBaseModel.UserName, ActivityType, scrapeResult.QueryInfo.QueryValue);
            JobProcessResult jobProcessResult = new JobProcessResult();
            try
            {
                ScrapeResultNewTag scrapResultWIthTag = (ScrapeResultNewTag)scrapeResult;
                if (scrapResultWIthTag.TagDetails != null)
                {
                    AddScrapedHashtagsIntoDataBase(scrapeResult);

                    GlobusLogHelper.log.Info(Log.ActivitySuccessful, DominatorAccountModel.AccountBaseModel.AccountNetwork,
                        DominatorAccountModel.AccountBaseModel.UserName, ActivityType,string.Format("{0} With HastagPage ==> {1}", scrapeResult.QueryInfo.QueryValue,GramStatic.GetHastagUrl(scrapResultWIthTag?.TagDetails?.Name)));

                    IncrementCounters();

                    jobProcessResult.IsProcessSuceessfull = true;
                }              
                DelayBeforeNextActivity();
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            return jobProcessResult;
        }


        private void AddScrapedHashtagsIntoDataBase(ScrapeResultNew scrapeResult)
        {
            ScrapeResultNewTag scrapResultWIthTag = (ScrapeResultNewTag)scrapeResult;
            JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
            // Add data to respected Campaign's HashtagScrape table
            if (!string.IsNullOrEmpty(CampaignId))
            {
                CampaignDbOperation?.Add(new DominatorHouseCore.DatabaseHandler.GdTables.Campaigns.HashtagScrape()
                {
                    AccountUsername = DominatorAccountModel.UserName,
                    Keyword = scrapeResult.QueryInfo.QueryValue,
                    HashtagName = scrapResultWIthTag.TagDetails.Name,
                    HashtagId = scrapResultWIthTag.TagDetails.Id,
                    MediaCount = scrapResultWIthTag.TagDetails.Count.ToString(),
                    ActivityType = ActivityType.HashtagsScraper,
                    Date = DateTimeUtilities.GetEpochTime()
                });
            }

            AccountDbOperation.Add(
                new DominatorHouseCore.DatabaseHandler.GdTables.Accounts.HashtagScrape()
                {
                    AccountUsername = DominatorAccountModel.UserName,
                    Keyword = scrapeResult.QueryInfo.QueryValue,
                    HashtagName = scrapResultWIthTag.TagDetails.Name,
                    HashtagId = scrapResultWIthTag.TagDetails.Id,
                    MediaCount = scrapResultWIthTag.TagDetails.Count.ToString(),
                    ActivityType = ActivityType.HashtagsScraper,
                    Date = DateTimeUtilities.GetEpochTime()
                });
        }

        public override void StartOtherConfiguration(ScrapeResultNew scrapeResult)
        {

        }

    }
}
