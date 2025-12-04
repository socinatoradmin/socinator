using DominatorHouseCore;
using DominatorHouseCore.DatabaseHandler.TumblrTables.Account;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using DominatorHouseCore.Process;
using DominatorHouseCore.Process.JobLimits;
using DominatorHouseCore.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using TumblrDominatorCore.Models;
using TumblrDominatorCore.TumblrFactory;
using TumblrDominatorCore.TumblrLibrary.DAL;
using TumblrDominatorCore.TumblrLibrary.TumblrFunction;
using TumblrDominatorCore.TumblrRequest;
using TumblrDominatorCore.TumblrResponseHandler;

namespace TumblrDominatorCore.TumblrLibrary.TumblrProcesses
{
    public sealed class BroadCastMessegeProcess : TumblrJobProcessInteracted<InteractedUser>
    {
        private readonly ITumblrFunct TumblrFunct;

        public BroadCastMessegeProcess(IProcessScopeModel processScopeModel, IDbAccountServiceScoped _accountService,
            IDbGlobalService _dbGlobalService, IExecutionLimitsManager executionLimitsManager,
            ITumblrFunct _tumblrFunct, ITumblrQueryScraperFactory queryScraperFactory, ITumblrHttpHelper _httpHelper,
            ITumblrLoginProcess _tumblrLoginProcess) : base(processScopeModel, _accountService, _dbGlobalService,
            executionLimitsManager, queryScraperFactory, _httpHelper, _tumblrLoginProcess)
        {
            TumblrFunct = _tumblrFunct;
            BroadcastMessagesModel = processScopeModel.GetActivitySettingsAs<BroadcastMessagesModel>();
        }

        public BroadcastMessagesModel BroadcastMessagesModel { get; set; }

        /// <summary>
        ///     BroadCastMessages : message to Tumblr Blogs
        /// </summary>
        /// <param name="scrapeResultNew"></param>
        /// <returns></returns>
        public override JobProcessResult PostScrapeProcess(ScrapeResultNew scrapeResultNew)
        {
            var scrapeResult = (TumblrScrapeResult)scrapeResultNew;

            var jobProcessResult = new JobProcessResult();
            var isBroadCasted = false;
            var mediaid = string.Empty;
            JobCancellationTokenSource.Token.ThrowIfCancellationRequested();

            var tumblrUser = (TumblrUser)scrapeResult.ResultUser;
            MessageUserResponse response = null;
            var lstMessages = BroadcastMessagesModel.LstDisplayManageMessageModel.ToList();
            lstMessages.Shuffle();
            var messageList = new List<ManageMessagesModel>();
            lstMessages.ForEach(y =>
            {
                var temp = y.SelectedQuery.Count;
                for (var i = 0; i < temp; i++)
                    if (y.SelectedQuery[i].Content.QueryValue == scrapeResult.QueryInfo.QueryValue)
                        messageList.Add(y);
            });
            var item = new ManageMessagesModel();
            if (messageList.Count != 0)
                //item = messageList.GetRandomItem();
                item = messageList.FirstOrDefault(x => x.ToString() != string.Empty);
            if (!DominatorAccountModel.IsRunProcessThroughBrowser)
            {
                if (string.IsNullOrEmpty(tumblrUser.Uuid))
                {
                    var userDetails = TumblrFunct.GetUserDetails(DominatorAccountModel, tumblrUser);
                    tumblrUser.Uuid = userDetails.JsonHand.GetElementValue("uuid");
                }
                if (!string.IsNullOrEmpty(item.MediaPath))
                    mediaid = TumblrFunct.UploadImageinMessage(DominatorAccountModel, item.MediaPath,
                        scrapeResult.TumblrFormKey, tumblrUser);
                if (!string.IsNullOrEmpty(item.MessagesText))
                {
                    tumblrUser.Message = item.MessagesText;
                    response = TumblrFunct.Messageuser(DominatorAccountModel, tumblrUser, scrapeResult.TumblrFormKey);
                }
            }
            else
            {
                if (!string.IsNullOrEmpty(item.MessagesText))
                {
                    tumblrUser.Message = item.MessagesText;
                    isBroadCasted = _browserManager.BroadCastMessage(DominatorAccountModel, item.MessagesText,
                        item.MediaPath, ref tumblrUser);
                }
            }

            if (response != null && (response.Success || !string.IsNullOrEmpty(mediaid)) || DominatorAccountModel.IsRunProcessThroughBrowser && isBroadCasted)
            {
                JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                scrapeResult.ResultUser = tumblrUser;
                GlobusLogHelper.log.Info(Log.ActivitySuccessful, DominatorAccountModel.AccountBaseModel.AccountNetwork,
                    DominatorAccountModel.AccountBaseModel.UserName, ActivityType, scrapeResult.ResultUser.Username);
                IncrementCounters();
                AddMessagedUsersToDataBase(scrapeResult);
                var AccountModel = new AccountModel(DominatorAccountModel);
                AccountModel.LstFollowings = new List<TumblrUser> { tumblrUser };
                jobProcessResult.IsProcessSuceessfull = true;
            }
            else if (((response != null && !string.IsNullOrEmpty(response.ErrorMessage)) ||
               (DominatorAccountModel.IsRunProcessThroughBrowser && !isBroadCasted))
                && !tumblrUser.CanMessage)
            {

                GlobusLogHelper.log.Info(Log.ActivityFailed, DominatorAccountModel.AccountBaseModel.AccountNetwork,
                                            DominatorAccountModel.AccountBaseModel.UserName, ActivityType, $"Not permitted for send messages to >> {scrapeResult.ResultUser.Username}");
                jobProcessResult.IsProcessSuceessfull = false;
            }
            else if (response.messageResponse.Response.Contains("\"meta\":{\"status\":428,\"msg\":\"Precondition required\"") && !string.IsNullOrEmpty(response.ErrorMessage) && tumblrUser.CanMessage)
            {
                GlobusLogHelper.log.Info(Log.ActivityFailed, DominatorAccountModel.AccountBaseModel.AccountNetwork,
                                            DominatorAccountModel.AccountBaseModel.UserName, ActivityType, $"Condition Required for send messages to >> {scrapeResult.ResultUser.Username}");
                jobProcessResult.IsProcessSuceessfull = false;
            }
            else
            {
                GlobusLogHelper.log.Info(Log.CustomMessage, DominatorAccountModel.AccountBaseModel.AccountNetwork,
                    DominatorAccountModel.AccountBaseModel.UserName, ActivityType,
                    " Message sending failed for User => " + scrapeResult.ResultUser.Username);

                jobProcessResult.IsProcessSuceessfull = false;
            }

            DelayBeforeNextActivity();
            return jobProcessResult;
        }

        /// <summary>
        ///     Add Sent Messages data to DB
        /// </summary>
        /// <param name="scrapeResult"></param>
        private void AddMessagedUsersToDataBase(ScrapeResultNew scrapeResult)
        {
            try
            {
                var instaUser = (TumblrUser)scrapeResult.ResultUser;

                if (!string.IsNullOrEmpty(CampaignId))
                {
                    IDbCampaignService campaignService = new DbCampaignService(CampaignId);
                    campaignService.Add(new DominatorHouseCore.DatabaseHandler.TumblrTables.Campaign.InteractedUser
                    {
                        AccountEmail = DominatorAccountModel.AccountBaseModel.UserName,
                        ActivityType = ActivityType.ToString(),
                        InteractionTimeStamp = DateTimeUtilities.GetEpochTime(),
                        QueryType = scrapeResult.QueryInfo?.QueryType,
                        QueryValue = scrapeResult.QueryInfo?.QueryValue,
                        UserName = DominatorAccountModel.AccountBaseModel.UserId,
                        InteractedUsername = scrapeResult.ResultUser.Username,
                        TemplateId = TemplateId,
                        DirectMessage = instaUser.Message
                    });
                }

                DbAccountService.Add(new InteractedUser
                {
                    //FirstOrDefault
                    ActivityType = ActivityType.ToString(),
                    InteractionTimeStamp = DateTimeUtilities.GetEpochTime(),
                    QueryType = scrapeResult.QueryInfo?.QueryType,
                    QueryValue = scrapeResult.QueryInfo?.QueryValue,
                    UserName = DominatorAccountModel.AccountBaseModel.UserId,
                    InteractedUsername = scrapeResult.ResultUser.Username,
                    DirectMessage = instaUser.Message
                });
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }
    }
}