using System;
using System.Collections.Generic;
using System.Linq;
using CommonServiceLocator;
using DominatorHouseCore;
using DominatorHouseCore.DatabaseHandler.PdTables.Accounts;
using DominatorHouseCore.FileManagers;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using DominatorHouseCore.Process;
using DominatorHouseCore.Process.JobLimits;
using DominatorHouseCore.Utility;
using Newtonsoft.Json;
using PinDominatorCore.PDFactories;
using PinDominatorCore.PDLibrary.DAL;
using PinDominatorCore.PDModel;
using PinDominatorCore.Request;
using PinDominatorCore.Response;

namespace PinDominatorCore.PDLibrary.Process
{
    public class AutoReplyToNewMessageProcess : PdJobProcessInteracted<InteractedUsers>
    {
        private readonly AutoReplyToNewMessageModel _autoReplyToNewMessageModel;
        private readonly IDbCampaignService _dbCampaignService;
        private int _activityFailedCount = 1;

        public AutoReplyToNewMessageProcess(IProcessScopeModel processScopeModel,
            IDbAccountServiceScoped accountServiceScoped, IDbGlobalService globalService,
            IDbCampaignService dbCampaignService,
            IExecutionLimitsManager executionLimitsManager, 
            IPdQueryScraperFactory queryScraperFactory, IPdHttpHelper pdhttpHelper, IPdLogInProcess pdLogInProcess)
            : base(processScopeModel, accountServiceScoped, globalService, executionLimitsManager, queryScraperFactory,
                pdhttpHelper, pdLogInProcess)
        {
            var templatesFileManager = InstanceProvider.GetInstance<ITemplatesFileManager>();
            _autoReplyToNewMessageModel =
                JsonConvert.DeserializeObject<AutoReplyToNewMessageModel>(templatesFileManager.Get()
                    .FirstOrDefault(x => x.Id == processScopeModel.TemplateId)?.ActivitySettings);
            _dbCampaignService = dbCampaignService;
        }

        public override JobProcessResult PostScrapeProcess(ScrapeResultNew scrapeResult)
        {
            var jobProcessResult = new JobProcessResult();

            GlobusLogHelper.log.Info(Log.StartedActivity, DominatorAccountModel.AccountBaseModel.AccountNetwork,
                DominatorAccountModel.AccountBaseModel.UserName, ActivityType, scrapeResult.ResultUser.Username);

            try
            {
                JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                var msgList = new List<string>();
                foreach (var item in _autoReplyToNewMessageModel.LstDisplayManageMessagesModel)
                    if (item.SelectedQuery.Any(x =>
                        x.Content.QueryValue.ToString() == scrapeResult.QueryInfo.QueryValue ||
                        x.Content.QueryValue.ToString() == "LangKeyReplyToMessagesThatContainsSpecificWord".FromResourceDictionary() ||
                       x.Content.QueryValue.ToString() == "LangKeyReplyToAllMessages".FromResourceDictionary()))
                        msgList.Add(item.MessagesText);
                var msg = msgList.GetRandomItem();
                if (_autoReplyToNewMessageModel.IsMakeMessageAsSpinText)
                    msg = SpinTexHelper.GetSpinText(msg);

                var pinterestUser = (PinterestUser)scrapeResult.ResultUser;
                msg = GetMessageWithUserName(msg, pinterestUser.Username, pinterestUser.FullName);

                MessageResponseHandler response = null;
                if (DominatorAccountModel.IsRunProcessThroughBrowser)
                    response = PdLogInProcess.BrowserManager.Message(DominatorAccountModel, pinterestUser.Username, msg,
                        JobCancellationTokenSource);
                else
                    response = PinFunct.Message(pinterestUser.Username, msg,
                    DominatorAccountModel, _autoReplyToNewMessageModel.IsSendPinAsAMessage);
                if (response != null && response.Success)
                {
                    scrapeResult.ResultUser = pinterestUser;

                    GlobusLogHelper.log.Info(Log.ActivitySuccessful,
                        DominatorAccountModel.AccountBaseModel.AccountNetwork,
                        DominatorAccountModel.AccountBaseModel.UserName, ActivityType,
                        scrapeResult.ResultUser.Username);

                    IncrementCounters();

                    AddFollowedDataToDataBase(scrapeResult, msg);
                    jobProcessResult.IsProcessSuceessfull = true;
                }
                else
                {
                    GlobusLogHelper.log.Info(Log.ActivityFailed, DominatorAccountModel.AccountBaseModel.AccountNetwork,
                        DominatorAccountModel.AccountBaseModel.UserName, ActivityType, response?.Issue?.Message);

                    //Reschedule if action block
                    if (_autoReplyToNewMessageModel.IsChkStopActivityAfterXXFailed && _activityFailedCount++ == _autoReplyToNewMessageModel.ActivityFailedCount)
                    {
                        GlobusLogHelper.log.Info(Log.CustomMessage, DominatorAccountModel.AccountBaseModel.AccountNetwork,
                            DominatorAccountModel.AccountBaseModel.UserName, ActivityType,
                            $"{"LangKeyStopActivityAfter".FromResourceDictionary()} {_activityFailedCount - 1} " +
                            $"{"LangKeyFailedAttemptFor".FromResourceDictionary()} {_autoReplyToNewMessageModel.FailedActivityReschedule} " +
                            $"{"LangKeyHour".FromResourceDictionary()}");

                        StopAndRescheduleJob(_autoReplyToNewMessageModel.FailedActivityReschedule);
                    }

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

        private void AddFollowedDataToDataBase(ScrapeResultNew scrapeResult, string msg)
        {
            try
            {
                var user = (PinterestUser) scrapeResult.ResultUser;
                var jobActivityConfigurationManager =
                    InstanceProvider.GetInstance<IJobActivityConfigurationManager>();
                var moduleSetting = jobActivityConfigurationManager[DominatorAccountModel.AccountId, ActivityType];
                if (moduleSetting == null)
                    return;
                if (moduleSetting.IsTemplateMadeByCampaignMode)
                    _dbCampaignService.Add(new DominatorHouseCore.DatabaseHandler.PdTables.Campaigns.InteractedUsers
                    {
                        ActivityType = ActivityType.ToString(),
                        QueryType = scrapeResult.QueryInfo.QueryType,
                        Query = scrapeResult.QueryInfo.QueryValue,
                        Bio = user.UserBio,
                        FollowersCount = user.FollowersCount,
                        FollowingsCount = user.FollowingsCount,
                        FullName = user.FullName,
                        HasAnonymousProfilePicture = user.HasProfilePic,
                        PinsCount = user.PinsCount,
                        ProfilePicUrl = user.ProfilePicUrl,
                        InteractedUsername = scrapeResult.ResultUser.Username,
                        InteractedUserId = user.UserId,
                        InteractionTime = DateTimeUtilities.GetEpochTime(),
                        Website = user.WebsiteUrl,
                        FollowedBack = user.FollowedBack,
                        IsVerified = user.IsVerified,
                        TriesCount = user.TriesCount,
                        SinAccId = DominatorAccountModel.AccountBaseModel.UserId,
                        SinAccUsername = DominatorAccountModel.AccountBaseModel.UserName,
                        DirectMessage = msg
                    });

                DbAccountService.Add(new InteractedUsers
                {
                    ActivityType = ActivityType.ToString(),
                    Date = DateTimeUtilities.GetEpochTime(),
                    QueryType = scrapeResult.QueryInfo.QueryType,
                    Query = scrapeResult.QueryInfo.QueryValue,
                    Bio = user.UserBio,
                    FollowersCount = user.FollowersCount,
                    FollowingsCount = user.FollowingsCount,
                    FullName = user.FullName,
                    HasAnonymousProfilePicture = user.HasProfilePic,
                    PinsCount = user.PinsCount,
                    ProfilePicUrl = user.ProfilePicUrl,
                    InteractedUsername = user.Username,
                    InteractedUserId = user.UserId,
                    InteractionTime = DateTimeUtilities.GetEpochTime(),
                    Website = user.WebsiteUrl,
                    FollowedBack = user.FollowedBack,
                    IsVerified = user.IsVerified,
                    TriesCount = user.TriesCount,
                    DirectMessage = msg
                });
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }
    }
}