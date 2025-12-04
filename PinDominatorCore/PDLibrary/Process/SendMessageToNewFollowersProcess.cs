using System;
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
using PinDominatorCore.Utility;

namespace PinDominatorCore.PDLibrary.Process
{
    public class SendMessageToNewFollowersProcess : PdJobProcessInteracted<InteractedUsers>
    {
        public SendMessageToNewFollowersModel SendMessageToNewFollowersModel { get; set; }
        private int _activityFailedCount = 1;

        public SendMessageToNewFollowersProcess(IProcessScopeModel processScopeModel,
            IDbAccountServiceScoped accountServiceScoped, IDbGlobalService globalService,
            IExecutionLimitsManager executionLimitsManager, 
            IPdQueryScraperFactory queryScraperFactory, IPdHttpHelper pdhttpHelper, IPdLogInProcess pdLogInProcess)
            : base(processScopeModel, accountServiceScoped, globalService, executionLimitsManager, queryScraperFactory,
                pdhttpHelper, pdLogInProcess)
        {
            var templatesFileManager = InstanceProvider.GetInstance<ITemplatesFileManager>();
            SendMessageToNewFollowersModel =
                JsonConvert.DeserializeObject<SendMessageToNewFollowersModel>(templatesFileManager.Get()
                    .FirstOrDefault(x => x.Id == processScopeModel.TemplateId)?.ActivitySettings);
        }

        public override JobProcessResult PostScrapeProcess(ScrapeResultNew scrapeResult)
        {
            var jobProcessResult = new JobProcessResult();

            GlobusLogHelper.log.Info(Log.StartedActivity, DominatorAccountModel.AccountBaseModel.AccountNetwork,
                DominatorAccountModel.AccountBaseModel.UserName, ActivityType, scrapeResult.ResultUser.Username);

            try
            {
                JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                var msgList = SendMessageToNewFollowersModel.LstMessages;
                var msg = msgList.GetRandomItem();
                if (SendMessageToNewFollowersModel.IsMakeMessageAsSpinText)
                    msg = SpinTexHelper.GetSpinText(msg);

                PinterestUser pinterestUser = (PinterestUser)scrapeResult.ResultUser;
                msg = GetMessageWithUserName(msg, pinterestUser.Username, pinterestUser.FullName);

                MessageResponseHandler response = null;
                if (DominatorAccountModel.IsRunProcessThroughBrowser)
                    response = PdLogInProcess.BrowserManager.Message(DominatorAccountModel, 
                                                                      pinterestUser.Username, msg, JobCancellationTokenSource);
                else
                    response = PinFunct.Message(pinterestUser.Username,PdUtility.ReplaceSpecialCharacter(msg),
                                                DominatorAccountModel, SendMessageToNewFollowersModel.IsSendPinAsAMessage);

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
                    //Reschedule if action block
                    if (SendMessageToNewFollowersModel.IsChkStopActivityAfterXXFailed && _activityFailedCount++ == SendMessageToNewFollowersModel.ActivityFailedCount)
                    {
                        GlobusLogHelper.log.Info(Log.CustomMessage, DominatorAccountModel.AccountBaseModel.AccountNetwork,
                            DominatorAccountModel.AccountBaseModel.UserName, ActivityType,
                            $"{"LangKeyStopActivityAfter".FromResourceDictionary()} {_activityFailedCount - 1} " +
                            $"{"LangKeyFailedAttemptFor".FromResourceDictionary()} {SendMessageToNewFollowersModel.FailedActivityReschedule} " +
                            $"{"LangKeyHour".FromResourceDictionary()}");

                        StopAndRescheduleJob(SendMessageToNewFollowersModel.FailedActivityReschedule);
                    }
                    var message = string.IsNullOrEmpty(response?.Issue?.Message) ? "Message Option Is Not Available" : response?.Issue?.Message;
                    GlobusLogHelper.log.Info(Log.ActivityFailed, DominatorAccountModel.AccountBaseModel.AccountNetwork,
                        DominatorAccountModel.AccountBaseModel.UserName, ActivityType,message);
                    if (message.Contains("LangKeyUserNotExistMessage".FromResourceDictionary()))
                        return jobProcessResult;
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
                IDbAccountService dbAccountService = new DbAccountService(DominatorAccountModel);
                IDbCampaignService dbCampaignService = new DbCampaignService(CampaignId);
                var user = (PinterestUser) scrapeResult.ResultUser;
                var jobActivityConfigurationManager =
                    InstanceProvider.GetInstance<IJobActivityConfigurationManager>();
                var moduleSetting = jobActivityConfigurationManager[DominatorAccountModel.AccountId, ActivityType];

                if (moduleSetting == null)
                    return;
                if (moduleSetting.IsTemplateMadeByCampaignMode)
                    dbCampaignService.Add(new DominatorHouseCore.DatabaseHandler.PdTables.Campaigns.InteractedUsers
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
                        Username = user.Username,
                        InteractedUsername = scrapeResult.ResultUser.Username,
                        InteractedUserId = user.UserId,
                        InteractionTime = DateTimeUtilities.GetEpochTime(),
                        Website = user.WebsiteUrl,
                        FollowedBack = user.FollowedBack,
                        IsVerified = user.IsVerified,
                        TriesCount = user.TriesCount,
                        DirectMessage = msg,
                        SinAccId = DominatorAccountModel.AccountBaseModel.UserId,
                        SinAccUsername = DominatorAccountModel.AccountBaseModel.UserName
                    });

                dbAccountService.Add(new InteractedUsers
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
                    InteractedUsername = scrapeResult.ResultUser.Username,
                    InteractedUserId = user.UserId,
                    InteractionTime = DateTimeUtilities.GetEpochTime(),
                    Website = user.WebsiteUrl,
                    FollowedBack = user.FollowedBack,
                    DirectMessage = msg,
                    IsVerified = user.IsVerified,
                    TriesCount = user.TriesCount
                });
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }
    }
}