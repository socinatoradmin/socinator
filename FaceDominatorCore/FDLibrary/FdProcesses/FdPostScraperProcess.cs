using CommonServiceLocator;
using DominatorHouseCore;
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
using AccountInteractedPosts = DominatorHouseCore.DatabaseHandler.FdTables.Accounts.InteractedPosts;
using CampaignInteractedPosts = DominatorHouseCore.DatabaseHandler.FdTables.Campaigns.InteractedPosts;

namespace FaceDominatorCore.FDLibrary.FdProcesses
{

    public class FdPostScraperProcess : FdJobProcessInteracted<AccountInteractedPosts>
    {
        public PostScraperModel PostScraperModel { get; set; }

        public DominatorAccountModel Account { get; set; }


        public FdPostScraperProcess(IProcessScopeModel processScopeModel,
            IDbAccountServiceScoped accountServiceScoped,
            IFdQueryScraperFactory queryScraperFactory,
            IFdHttpHelper fdHttpHelper, IFdLoginProcess fdLogInProcess, IDbCampaignServiceScoped dbCampaignServiceScoped)
            : base(processScopeModel, accountServiceScoped, queryScraperFactory, fdHttpHelper, fdLogInProcess, dbCampaignServiceScoped)
        {

            PostScraperModel = processScopeModel.GetActivitySettingsAs<PostScraperModel>();
            AccountModel = DominatorAccountModel;
            CheckJobProcessLimitsReached();
        }

        public override JobProcessResult PostScrapeProcess(ScrapeResultNew scrapeResult)
        {
            JobProcessResult jobProcessResult = new JobProcessResult();

            FacebookPostDetails objFacebookPostDetails = (FacebookPostDetails)scrapeResult.ResultPost;

            try
            {
                JobCancellationTokenSource.Token.ThrowIfCancellationRequested();

                GlobusLogHelper.log.Info(Log.ActivitySuccessful, DominatorAccountModel.AccountBaseModel.AccountNetwork,
                    DominatorAccountModel.AccountBaseModel.UserName, ActivityType, objFacebookPostDetails.Id);
                IncrementCounters();
                jobProcessResult.IsProcessSuceessfull = true;
                AddProfileScraperDataToDatabase(scrapeResult);
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



        void AddProfileScraperDataToDatabase(ScrapeResultNew scrapeResult)
        {
            try
            {

                FacebookPostDetails group = (FacebookPostDetails)scrapeResult.ResultPost;

                var jobActivityConfigurationManager = InstanceProvider.GetInstance<IJobActivityConfigurationManager>();

                var modulesetting = jobActivityConfigurationManager[DominatorAccountModel.AccountId, ActivityType];

                if (modulesetting == null)
                    return;

                if (modulesetting.IsTemplateMadeByCampaignMode)
                {

                    DbCampaignService.Add(new CampaignInteractedPosts
                    {
                        ActivityType = ActivityType.ToString(),
                        QueryType = group.QueryType,
                        QueryValue = group.QueryValue,
                        AccountEmail = DominatorAccountModel.AccountBaseModel.UserName,
                        PostDescription = JsonConvert.SerializeObject(group),
                        PostId = group.Id,
                        InteractionTimeStamp = DateTimeUtilities.GetEpochTime(),
                        InteractionDateTime = DateTime.Now,
                        PostUrl = group.PostUrl,
                        Likes = group.LikersCount,
                        Comments = group.CommentorCount,
                        Shares = group.SharerCount

                    });
                }

                DbAccountService.Add(new AccountInteractedPosts
                {
                    ActivityType = ActivityType.ToString(),
                    QueryType = group.QueryType,
                    QueryValue = group.QueryValue,
                    PostId = group.Id,
                    PostDescription = JsonConvert.SerializeObject(group),
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
