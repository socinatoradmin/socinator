using CommonServiceLocator;
using DominatorHouseCore;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using DominatorHouseCore.Process;
using DominatorHouseCore.Utility;
using FaceDominatorCore.FDFactories;
using FaceDominatorCore.FDLibrary.DAL;
using FaceDominatorCore.FDLibrary.FdClassLibrary;
using FaceDominatorCore.FDLibrary.FdFunctions;
using FaceDominatorCore.FDModel.ScraperModel;
using FaceDominatorCore.FDRequest;
using System;
using ThreadUtils;
using AccountInteractedComments = DominatorHouseCore.DatabaseHandler.FdTables.Accounts.InteractedComments;
using CampaignInteractedComments = DominatorHouseCore.DatabaseHandler.FdTables.Campaigns.InteractedComments;


namespace FaceDominatorCore.FDLibrary.FdProcesses
{

    public class FdCommentScraperProcess : FdJobProcessInteracted<AccountInteractedComments>
    {
        private readonly IDelayService _delayService;
        public CommentScraperModel CommentScraperModel { get; set; }

        public DominatorAccountModel Account { get; set; }

        public IFdRequestLibrary FdRequestLibrary { get; set; }
        public FdCommentScraperProcess(IProcessScopeModel processScopeModel,
            IDbAccountServiceScoped accountServiceScoped, IFdQueryScraperFactory queryScraperFactory,
            IFdHttpHelper fdHttpHelper, IFdLoginProcess fdLogInProcess, IFdRequestLibrary fdRequestLibrary,
            IDbCampaignServiceScoped dbCampaignServiceScoped, IDelayService delayService)
            : base(processScopeModel, accountServiceScoped, queryScraperFactory, fdHttpHelper, fdLogInProcess, dbCampaignServiceScoped)
        {
            FdRequestLibrary = fdRequestLibrary;
            _delayService = delayService;
            CommentScraperModel = processScopeModel.GetActivitySettingsAs<CommentScraperModel>();
            AccountModel = DominatorAccountModel;
            base.CheckJobProcessLimitsReached();
        }

        public override JobProcessResult PostScrapeProcess(ScrapeResultNew scrapeResult)
        {
            JobProcessResult jobProcessResult = new JobProcessResult();

            FdPostCommentDetails objFdPostCommentDetails = (FdPostCommentDetails)scrapeResult.ResultComment;


            try
            {
                JobCancellationTokenSource.Token.ThrowIfCancellationRequested();

                GlobusLogHelper.log.Info(Log.ActivitySuccessful, DominatorAccountModel.AccountBaseModel.AccountNetwork,
                    DominatorAccountModel.AccountBaseModel.UserName, ActivityType, objFdPostCommentDetails.CommentId,
                    "");
                IncrementCounters();
                jobProcessResult.IsProcessSuceessfull = true;
                AddCommentScraperDataToDatabase(scrapeResult);
                _delayService.ThreadSleep(100);
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

        private void AddCommentScraperDataToDatabase(ScrapeResultNew scrapeResult)
        {
            try
            {

                FdPostCommentDetails commenDetail = (FdPostCommentDetails)scrapeResult.ResultComment;


                var jobActivityConfigurationManager = InstanceProvider.GetInstance<IJobActivityConfigurationManager>();

                var modulesetting = jobActivityConfigurationManager[DominatorAccountModel.AccountId, ActivityType];

                if (modulesetting == null)
                    return;

                if (modulesetting.IsTemplateMadeByCampaignMode)
                {
                    DbCampaignService.Add(new CampaignInteractedComments
                    {
                        ActivityType = ActivityType.ToString(),
                        QueryType = scrapeResult.QueryInfo.QueryType,
                        QueryValue = scrapeResult.QueryInfo.QueryValue,
                        AccountEmail = DominatorAccountModel.AccountBaseModel.UserName,
                        CommentUrl = commenDetail.CommentUrl,
                        CommentText = commenDetail.CommentText,
                        CommenterId = commenDetail.CommenterID,
                        CommentPostId = commenDetail.PostId,
                        CommentTimeWithDate = commenDetail.CommentTimeWithDate,
                        CommetLikeCount = commenDetail.ReactionCountOnComment,
                        HasLikedByUser = commenDetail.HasLikedByUser,
                        InteractionTimeStamp = DateTimeUtilities.GetEpochTime(),
                        InteractionDateTime = DateTime.Now


                    });
                }

                DbAccountService.Add(new AccountInteractedComments
                {
                    ActivityType = ActivityType.ToString(),
                    QueryType = scrapeResult.QueryInfo.QueryType,
                    QueryValue = scrapeResult.QueryInfo.QueryValue,
                    CommentUrl = commenDetail.CommentUrl,
                    CommentText = commenDetail.CommentText,
                    CommenterId = commenDetail.CommenterID,
                    CommentPostId = commenDetail.PostId,
                    CommentTimeWithDate = commenDetail.CommentTimeWithDate,
                    CommetLikeCount = commenDetail.ReactionCountOnComment,
                    HasLikedByUser = commenDetail.HasLikedByUser,
                    InteractionTimeStamp = DateTimeUtilities.GetEpochTime(),
                    InteractionDateTime = DateTime.Now
                });
            }
            catch (Exception ex)
            {
                ex.ErrorLog(ex.Message);
            }
        }

        public override void StartOtherConfiguration(ScrapeResultNew scrapeResult)
        {

        }
    }
}
