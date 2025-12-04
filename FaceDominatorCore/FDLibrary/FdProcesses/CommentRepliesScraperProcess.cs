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
using AccountInteractedComments = DominatorHouseCore.DatabaseHandler.FdTables.Accounts.InteractedCommentReplies;
using CampaignInteractedComments = DominatorHouseCore.DatabaseHandler.FdTables.Campaigns.InteractedCommentReplies;

namespace FaceDominatorCore.FDLibrary.FdProcesses
{
    public class CommentRepliesScraperProcess : FdJobProcessInteracted<AccountInteractedComments>
    {
        private readonly IDelayService _delayService;
        public CommentRepliesScraperModel CommentRepliesScraperModel { get; set; }
        public DominatorAccountModel Account { get; set; }
        public IFdRequestLibrary FdRequestLibrary { get; set; }

        public CommentRepliesScraperProcess(IProcessScopeModel processScopeModel,
             IDbAccountServiceScoped accountServiceScoped, IFdQueryScraperFactory queryScraperFactory,
             IFdHttpHelper fdHttpHelper, IFdLoginProcess fdLogInProcess, IFdRequestLibrary fdRequestLibrary,
             IDbCampaignServiceScoped dbCampaignServiceScoped, IDelayService delayService)
            : base(processScopeModel, accountServiceScoped, queryScraperFactory, fdHttpHelper, fdLogInProcess, dbCampaignServiceScoped)
        {
            FdRequestLibrary = fdRequestLibrary;
            _delayService = delayService;
            CommentRepliesScraperModel = processScopeModel.GetActivitySettingsAs<CommentRepliesScraperModel>();
            AccountModel = DominatorAccountModel;
            base.CheckJobProcessLimitsReached();
        }

        public override JobProcessResult PostScrapeProcess(ScrapeResultNew scrapeResult)
        {
            JobProcessResult jobProcessResult = new JobProcessResult();

            FdPostCommentRepliesDetails objFdPostCommentDetails = (FdPostCommentRepliesDetails)scrapeResult.ResultComment;


            try
            {
                JobCancellationTokenSource.Token.ThrowIfCancellationRequested();

                GlobusLogHelper.log.Info(Log.ActivitySuccessful, DominatorAccountModel.AccountBaseModel.AccountNetwork,
                    DominatorAccountModel.AccountBaseModel.UserName, ActivityType, objFdPostCommentDetails.ReplyCommentId, "");
                IncrementCounters();
                jobProcessResult.IsProcessSuceessfull = true;
                AddCommentScraperDataToDatabase(scrapeResult);
                _delayService.ThreadSleep(100);
                DelayBeforeNextActivity(10);
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

                FdPostCommentRepliesDetails commenDetail = (FdPostCommentRepliesDetails)scrapeResult.ResultComment;


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
                        ReplyCommentId = commenDetail.ReplyCommentId,
                        ReplyCommentUrl = $"{FdConstants.FbHomeUrl}{commenDetail.ReplyCommentId}",
                        ReplyCommentText = commenDetail.ReplyCommentText,
                        ReplyCommenterId = commenDetail.ReplyCommenterID,
                        CommentPostUrl = commenDetail.PostId,
                        ReplyCommentTimeWithDate = commenDetail.CommentTimeWithDate,
                        InteractionTimeStamp = DateTimeUtilities.GetEpochTime(),
                        InteractionDateTime = DateTime.Now,
                        
                        

                    });
                }

                DbAccountService.Add(new AccountInteractedComments
                {
                    ActivityType = ActivityType.ToString(),
                    QueryType = scrapeResult.QueryInfo.QueryType,
                    QueryValue = scrapeResult.QueryInfo.QueryValue,
                    CommentUrl = commenDetail.CommentUrl,
                    ReplyCommentId = commenDetail.ReplyCommentId,
                    ReplyCommentUrl = $"{FdConstants.FbHomeUrl}{commenDetail.ReplyCommentId}",
                    ReplyCommentText = commenDetail.ReplyCommentText,
                    ReplyCommenterId = commenDetail.ReplyCommenterID,
                    CommentPostUrl = commenDetail.PostId,
                    ReplyCommentTimeWithDate = commenDetail.CommentTimeWithDate,
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
