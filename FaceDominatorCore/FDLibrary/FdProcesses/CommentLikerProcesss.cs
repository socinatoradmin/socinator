using CommonServiceLocator;
using DominatorHouseCore;
using DominatorHouseCore.Enums.FdQuery;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using DominatorHouseCore.Process;
using DominatorHouseCore.Utility;
using FaceDominatorCore.FDFactories;
using FaceDominatorCore.FDLibrary.DAL;
using FaceDominatorCore.FDLibrary.FdClassLibrary;
using FaceDominatorCore.FDLibrary.FdFunctions;
using FaceDominatorCore.FDModel.LikerCommentorModel;
using FaceDominatorCore.FDRequest;
using System;
using AccountInteractedComments = DominatorHouseCore.DatabaseHandler.FdTables.Accounts.InteractedComments;
using CampaignInteractedComments = DominatorHouseCore.DatabaseHandler.FdTables.Campaigns.InteractedComments;

namespace FaceDominatorCore.FDLibrary.FdProcesses
{
    public class CommentLikerProcesss : FdJobProcessInteracted<AccountInteractedComments>
    {
        public CommentLikerModule CommentLikerModel { get; set; }

        public DominatorAccountModel Account { get; set; }

        public IFdRequestLibrary FdRequestLibrary { get; set; }


        public CommentLikerProcesss(IProcessScopeModel processScopeModel,
            IDbAccountServiceScoped accountServiceScoped, IFdQueryScraperFactory queryScraperFactory,
            IFdHttpHelper fdHttpHelper, IFdLoginProcess fdLogInProcess, IFdRequestLibrary fdRequestLibrary,
            IDbCampaignServiceScoped dbCampaignServiceScoped)
            : base(processScopeModel, accountServiceScoped, queryScraperFactory, fdHttpHelper, fdLogInProcess, dbCampaignServiceScoped)
        {
            FdRequestLibrary = fdRequestLibrary;
            CommentLikerModel = processScopeModel.GetActivitySettingsAs<CommentLikerModule>();
            AccountModel = DominatorAccountModel;
            base.CheckJobProcessLimitsReached();
        }

        public override JobProcessResult PostScrapeProcess(ScrapeResultNew scrapeResult)
        {
            JobProcessResult jobProcessResult = new JobProcessResult();

            FdPostCommentDetails objFdPostCommentDetails = (FdPostCommentDetails)scrapeResult.ResultComment;

            FanpageDetails objFanpageDetails = (FanpageDetails)scrapeResult.ResultPage;



            ReactionType objReactionType = ReactionType.Like;

            try
            {
                GetReactionDetails(ref objReactionType);

                var isChangedActor = objFanpageDetails != null
                        && !AccountModel.IsRunProcessThroughBrowser
                        && FdRequestLibrary.ChangeActor(AccountModel, objFdPostCommentDetails.PostId, objFanpageDetails.FanPageID);

                var responseHandler = AccountModel.IsRunProcessThroughBrowser
                     ? FdLogInProcess._browserManager.LikeComments(AccountModel, objFdPostCommentDetails, objReactionType, objFanpageDetails)
                     : FdRequestLibrary.LikeComments(AccountModel, objFdPostCommentDetails, objReactionType, objFanpageDetails);

                JobCancellationTokenSource.Token.ThrowIfCancellationRequested();

                if (responseHandler.Status)
                {
                    GlobusLogHelper.log.Info(Log.ActivitySuccessful, DominatorAccountModel.AccountBaseModel.AccountNetwork, DominatorAccountModel.AccountBaseModel.UserName, ActivityType, objFdPostCommentDetails.CommentId, "");
                    IncrementCounters();
                    jobProcessResult.IsProcessSuceessfull = true;
                    AddCommentScraperDataToDatabase(scrapeResult, objReactionType);
                }
                else
                {
                    GlobusLogHelper.log.Info(Log.ActivityFailed, DominatorAccountModel.AccountBaseModel.AccountNetwork, DominatorAccountModel.AccountBaseModel.UserName, ActivityType, $"{responseHandler.ObjFdScraperResponseParameters.FailedReason}");
                    jobProcessResult.IsProcessSuceessfull = false;
                }


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

        private void GetReactionDetails(ref ReactionType objReactionType)
        {

            try
            {
                if (CommentLikerModel.LikerCommentorConfigModel.ListReactionType.Count > 0)
                {
                    var random = new Random();

                    int index = random.Next(CommentLikerModel.LikerCommentorConfigModel.ListReactionType.Count);

                    objReactionType = CommentLikerModel.LikerCommentorConfigModel.ListReactionType[index];
                }
                else
                    objReactionType = CommentLikerModel.LikerCommentorConfigModel.ListReactionType[0];

            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }



        private void AddCommentScraperDataToDatabase(ScrapeResultNew scrapeResult, ReactionType objReactionType)
        {
            try
            {

                FdPostCommentDetails commenDetail = (FdPostCommentDetails)scrapeResult.ResultComment;

                FanpageDetails objFanpageDetails = (FanpageDetails)scrapeResult.ResultPage;

                var likeType = objReactionType.ToString();

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
                        HasLikedByUser = likeType,
                        InteractionTimeStamp = DateTimeUtilities.GetEpochTime(),
                        InteractionDateTime = DateTime.Now,
                        LikeAsPageId = objFanpageDetails == null ? string.Empty : objFanpageDetails.FanPageID,
                        CommentId = commenDetail.CommentId
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
                    HasLikedByUser = likeType,
                    InteractionTimeStamp = DateTimeUtilities.GetEpochTime(),
                    InteractionDateTime = DateTime.Now,
                    LikeAsPageId = objFanpageDetails == null ? string.Empty : objFanpageDetails.FanPageID,
                    CommentId = commenDetail.CommentId
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
