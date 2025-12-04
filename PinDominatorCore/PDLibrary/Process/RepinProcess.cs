using System;
using System.Linq;
using System.Text.RegularExpressions;
using CommonServiceLocator;
using ThreadUtils;
using DominatorHouseCore;
using DominatorHouseCore.DatabaseHandler.PdTables.Accounts;
using DominatorHouseCore.Enums;
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
using System.Collections.Generic;
using DominatorHouseCore.Models.SocioPublisher;
using System.Threading;

namespace PinDominatorCore.PDLibrary.Process
{
    public class RepinProcess : PdJobProcessInteracted<InteractedPosts>
    {
        private readonly IDelayService _delayService;
        private readonly IDbCampaignService _dbCampaignService;
        private int _activityFailedCount = 1;

        public RepinProcess(IProcessScopeModel processScopeModel,
            IDbAccountServiceScoped accountServiceScoped, IDbGlobalService globalService, IDbCampaignService dbCampaignService,
            IExecutionLimitsManager executionLimitsManager,
            IPdQueryScraperFactory queryScraperFactory, IPdHttpHelper pdhttpHelper, IPdLogInProcess pdLogInProcess,
            IDelayService delayService)
            : base(processScopeModel, accountServiceScoped, globalService, executionLimitsManager, queryScraperFactory,
                pdhttpHelper, pdLogInProcess)
        {
            _delayService = delayService;
            var templatesFileManager = InstanceProvider.GetInstance<ITemplatesFileManager>();

            RePinModel = JsonConvert.DeserializeObject<RePinModel>(templatesFileManager.Get()
                .FirstOrDefault(x => x.Id == processScopeModel.TemplateId)?.ActivitySettings);
            _dbCampaignService = dbCampaignService;
        }


        public RePinModel RePinModel { get; set; }


        public override JobProcessResult PostScrapeProcess(ScrapeResultNew scrapeResult)
        {
            var jobProcessResult = new JobProcessResult();
            var pinDetails = (PinterestPin)scrapeResult.ResultPost;

            try
            {
                GlobusLogHelper.log.Info(Log.StartedActivity, DominatorAccountModel.AccountBaseModel.AccountNetwork,
                    DominatorAccountModel.AccountBaseModel.UserName, ActivityType,
                   $"{pinDetails.PinId} " + string.Format("LangKeyWithBoard".FromResourceDictionary(), pinDetails.BoardUrlToRepin));
                JobCancellationTokenSource.Token.ThrowIfCancellationRequested();

                var label = RePinModel.AccountPagesBoardsPair.FirstOrDefault(x => x.LstofPinsToRepin.Key == pinDetails.BoardUrlToRepin)?.Label;
                if(pinDetails.LstSection?.Count > 0)
                {
                    pinDetails.LstSection.ForEach(section =>
                    {
                        GlobusLogHelper.log.Info(Log.CustomMessage, DominatorAccountModel.AccountBaseModel.AccountNetwork,DominatorAccountModel.AccountBaseModel.UserName, ActivityType,string.Format("LangKeyTryingToPostOnBoardSection".FromResourceDictionary()?.Replace("post","repin"),pinDetails.BoardUrlToRepin,section.SectionTitle));
                        RePinOnBoard(pinDetails,scrapeResult,label,jobProcessResult,section);
                        var delay = RandomUtilties.GetRandomNumber(30, 20);
                        if(section.SectionId!=pinDetails.LstSection.LastOrDefault().SectionId)
                        {
                            GlobusLogHelper.log.Info(Log.CustomMessage, DominatorAccountModel.AccountBaseModel.AccountNetwork, DominatorAccountModel.AccountBaseModel.UserName, ActivityType,$"Next Activity Of Repin Inside Section Will Start After {delay} Seconds.");
                            Thread.Sleep(delay * 1000);
                        }
                    });
                }else
                    RePinOnBoard(pinDetails, scrapeResult, label, jobProcessResult);
                JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                if (RePinModel != null && RePinModel.IsEnableAdvancedUserMode && RePinModel.EnableDelayBetweenPerformingActionOnSamePost)
                    DelayBeforeNextActivity(RePinModel.DelayBetweenPerformingActionOnSamePost.GetRandom());
                else
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

        private void RePinOnBoard(PinterestPin pinDetails,ScrapeResultNew scrapeResult,string label,JobProcessResult jobProcessResult,SectionDetails section=null)
        {
            RepostPinResponseHandler response;
            try
            {
                Thread.Sleep(TimeSpan.FromSeconds(RandomUtilties.GetRandomNumber(5, 2)));
                var SuccessMessage = $"{pinDetails.PinId} {string.Format("LangKeyWithBoard".FromResourceDictionary(), pinDetails.BoardUrlToRepin)}";
                if (section != null)
                    SuccessMessage += $"==>Inside Section ==>{section.SectionTitle}";
                if (DominatorAccountModel.IsRunProcessThroughBrowser)
                    response = PdLogInProcess.BrowserManager.Repin(DominatorAccountModel, pinDetails.PinId, pinDetails.BoardUrlToRepin, JobCancellationTokenSource, section);
                else
                    response = PinFunct.RepostPin(pinDetails.PinId, pinDetails.BoardUrlToRepin, DominatorAccountModel, pinDetails.BoardUrl, scrapeResult.QueryInfo.QueryType, JobCancellationTokenSource, section);

               
                if (response != null && response.Success)
                {
                    GlobusLogHelper.log.Info(Log.ActivitySuccessful,
                        DominatorAccountModel.AccountBaseModel.AccountNetwork,
                        DominatorAccountModel.AccountBaseModel.UserName, ActivityType, SuccessMessage);

                    IncrementCounters();

                    AfterRepinAddDataToDataBase(scrapeResult, label, response.PinId);

                    //After Repin Action
                    if (RePinModel.ChkTryOnPinAfterRepinChecked || RePinModel.ChkCommentOnPinAfterRepinChecked)
                        PostRepinProcess(scrapeResult.QueryInfo, response.PinId);
                    jobProcessResult.IsProcessSuceessfull = true;
                }
                else
                {
                    GlobusLogHelper.log.Info(Log.ActivityFailed, DominatorAccountModel.AccountBaseModel.AccountNetwork,
                        DominatorAccountModel.AccountBaseModel.UserName, ActivityType, response?.Issue?.Message);

                    //Reschedule if action block
                    if (RePinModel.IsChkStopActivityAfterXXFailed && _activityFailedCount++ == RePinModel.ActivityFailedCount)
                    {
                        GlobusLogHelper.log.Info(Log.CustomMessage, DominatorAccountModel.AccountBaseModel.AccountNetwork,
                            DominatorAccountModel.AccountBaseModel.UserName, ActivityType,
                            $"{"LangKeyStopActivityAfter".FromResourceDictionary()} {_activityFailedCount - 1} " +
                            $"{"LangKeyFailedAttemptFor".FromResourceDictionary()} {RePinModel.FailedActivityReschedule} " +
                            $"{"LangKeyHour".FromResourceDictionary()}");

                        StopAndRescheduleJob(RePinModel.FailedActivityReschedule);
                    }
                    jobProcessResult.IsProcessSuceessfull = false;
                }
            }catch(OperationCanceledException)
            {
                throw new OperationCanceledException();
            }
            catch(Exception ex) { ex.DebugLog(); }
        }

        public void PostRepinProcess(QueryInfo queryInfo, string repinId)
        {
            try
            {
                GlobusLogHelper.log.Info(Log.CustomMessage, DominatorAccountModel.AccountBaseModel.AccountNetwork,
                    DominatorAccountModel.UserName, ActivityType, "LangKeyStartedAfterRepin".FromResourceDictionary());

                #region Try Post process after Follow

                if (RePinModel.ChkTryOnPinAfterRepinChecked)
                    TryOnPinAfterRepin(repinId, queryInfo);

                #endregion

                #region Comment on post after Follow process

                if (RePinModel.ChkCommentOnPinAfterRepinChecked)
                    CommentOnPinAfterRepin(repinId, queryInfo);

                #endregion
            }
            catch (OperationCanceledException)
            {
                throw new OperationCanceledException();
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        private void TryOnPinAfterRepin(string repinId, QueryInfo queryInfo)
        {
            try
            {
                GlobusLogHelper.log.Info(Log.CustomMessage, DominatorAccountModel.AccountBaseModel.AccountNetwork,
                    DominatorAccountModel.UserName, ActivityType, string.Format("LangKeyStartedSomeActivityPostAfterSomeAction".FromResourceDictionary(), "LangKeyTry".FromResourceDictionary(), "LangKeyRepin".FromResourceDictionary()));
                GlobusLogHelper.log.Info(Log.CustomMessage, DominatorAccountModel.AccountBaseModel.AccountNetwork,
                    DominatorAccountModel.UserName, ActivityType,
                    string.Format("LangKeyDelayBeforeSomeProcessMessage".FromResourceDictionary(), 20, "LangKeyTryProcess".FromResourceDictionary()));
                JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                _delayService.ThreadSleep(20 * 1000);

                JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                string note = RePinModel.LstNotes.GetRandomItem<string>();

                TryResponse tryResponse = null;
                if (DominatorAccountModel.IsRunProcessThroughBrowser)
                    tryResponse = PdLogInProcess.BrowserManager.Try(DominatorAccountModel, repinId,
                       note, RePinModel.MediaPath, JobCancellationTokenSource);
                else
                    tryResponse = PinFunct.TryPin(RePinModel.MediaPath, note, repinId, DominatorAccountModel);

                var jobActivityConfigurationManager =
                    InstanceProvider.GetInstance<IJobActivityConfigurationManager>();
                var moduleSetting =
                    jobActivityConfigurationManager[DominatorAccountModel.AccountId, ActivityType];

                if (tryResponse.Success)
                {
                    GlobusLogHelper.log.Info(Log.CustomMessage,
                        DominatorAccountModel.AccountBaseModel.AccountNetwork,
                        DominatorAccountModel.UserName, ActivityType,
                        string.Format("LangKeySuccessfullyTriedWithSomeNameAndPin".FromResourceDictionary(), DominatorAccountModel.AccountBaseModel.UserName, repinId));
                    var pinInfo = PinFunct.GetPinDetails(repinId, DominatorAccountModel);
                    if (pinInfo.Success)
                    {
                        if (moduleSetting.IsTemplateMadeByCampaignMode)
                            _dbCampaignService.Add(
                                new DominatorHouseCore.DatabaseHandler.PdTables.Campaigns.InteractedPosts
                                {
                                    InteractionDate = DateTimeUtilities.GetEpochTime(),
                                    MediaType = MediaType.Image,
                                    OperationType = ActivityType.Try.ToString(),
                                    QueryType = queryInfo.QueryType,
                                    Query = queryInfo.QueryValue,
                                    PinId = pinInfo.PinId,
                                    UserId = pinInfo.UserId,
                                    Username = pinInfo.UserName,
                                    SourceBoard = pinInfo.BoardId,
                                    SourceBoardName = pinInfo.BoardName,
                                    CommentCount = pinInfo.CommentCount,
                                    PinDescription = pinInfo.Description,
                                    PinWebUrl = pinInfo.PinWebUrl,
                                    SinAccId = DominatorAccountModel.AccountBaseModel.UserId,
                                    SinAccUsername = DominatorAccountModel.AccountBaseModel.UserName
                                });

                        DbAccountService.Add(new InteractedPosts
                        {
                            InteractionDate = DateTimeUtilities.GetEpochTime(),
                            MediaType = MediaType.Image,
                            QueryType = queryInfo.QueryType,
                            Query = queryInfo.QueryValue,
                            OperationType = ActivityType.Try.ToString(),
                            PinId = pinInfo.PinId,
                            UserId = pinInfo.UserId,
                            Username = pinInfo.UserName,
                            SourceBoard = pinInfo.BoardId,
                            SourceBoardName = pinInfo.BoardName,
                            CommentCount = pinInfo.CommentCount,
                            PinDescription = pinInfo.Description,
                            PinWebUrl = pinInfo.PinWebUrl
                        });
                    }
                }
            }
            catch (OperationCanceledException)
            {
                throw new OperationCanceledException();
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        private void CommentOnPinAfterRepin(string repinId, QueryInfo queryInfo)
        {
            try
            {
                GlobusLogHelper.log.Info(Log.CustomMessage, DominatorAccountModel.AccountBaseModel.AccountNetwork,
                    DominatorAccountModel.UserName, ActivityType,
                    string.Format("LangKeyStartedSomeActivityPostAfterSomeAction".FromResourceDictionary(), "LangKeyComment".FromResourceDictionary(), "LangKeyRepin".FromResourceDictionary()));
                GlobusLogHelper.log.Info(Log.CustomMessage, DominatorAccountModel.AccountBaseModel.AccountNetwork,
                    DominatorAccountModel.UserName, ActivityType,
                    string.Format("LangKeyDelayBeforeSomeProcessMessage".FromResourceDictionary(), 20, "LangKeyCommentProcess".FromResourceDictionary()));
                JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                PinFunct.DelayBeforeOperation(20000);
                JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                var comment = RePinModel.LstComments.GetRandomItem();
                var commentResponse = PinFunct.CommentOnPin(repinId, comment, DominatorAccountModel);

                var jobActivityConfigurationManager =
                    InstanceProvider.GetInstance<IJobActivityConfigurationManager>();
                var moduleSetting =
                    jobActivityConfigurationManager[DominatorAccountModel.AccountId, ActivityType];

                if (commentResponse.Success)
                {
                    GlobusLogHelper.log.Info(Log.CustomMessage,
                        DominatorAccountModel.AccountBaseModel.AccountNetwork,
                        DominatorAccountModel.UserName, ActivityType,
                        string.Format("LangKeySuccessfullyCommentedWithSomeNameAndPin".FromResourceDictionary(), DominatorAccountModel.AccountBaseModel.UserName, repinId));
                    var pinInfo = PinFunct.GetPinDetails(repinId, DominatorAccountModel);
                    if (pinInfo.Success)
                    {
                        if (moduleSetting.IsTemplateMadeByCampaignMode)
                            _dbCampaignService.Add(
                                new DominatorHouseCore.DatabaseHandler.PdTables.Campaigns.InteractedPosts
                                {
                                    OperationType = ActivityType.Comment.ToString(),
                                    InteractionDate = DateTimeUtilities.GetEpochTime(),
                                    Username = pinInfo.UserName,
                                    QueryType = queryInfo.QueryType,
                                    Query = queryInfo.QueryValue,
                                    SourceBoard = pinInfo.BoardId,
                                    SourceBoardName = pinInfo.BoardName,
                                    CommentCount = pinInfo.CommentCount,
                                    MediaType = pinInfo.MediaType,
                                    PinDescription = pinInfo.Description,
                                    PinId = pinInfo.PinId,
                                    PinWebUrl = pinInfo.PinWebUrl,
                                    UserId = pinInfo.UserId,
                                    MediaString = pinInfo.MediaString,
                                    Comment = comment,
                                    SinAccId = DominatorAccountModel.AccountBaseModel.UserId,
                                    SinAccUsername = DominatorAccountModel.AccountBaseModel.UserName
                                });

                        DbAccountService.Add(new InteractedPosts
                        {
                            OperationType = ActivityType.Comment.ToString(),
                            InteractionDate = DateTimeUtilities.GetEpochTime(),
                            Username = pinInfo.UserName,
                            QueryType = queryInfo.QueryType,
                            Query = queryInfo.QueryValue,
                            SourceBoard = pinInfo.BoardId,
                            SourceBoardName = pinInfo.BoardName,
                            CommentCount = pinInfo.CommentCount,
                            MediaType = pinInfo.MediaType,
                            PinDescription = pinInfo.Description,
                            PinId = pinInfo.PinId,
                            PinWebUrl = pinInfo.PinWebUrl,
                            UserId = pinInfo.UserId,
                            MediaString = pinInfo.MediaString,
                            Comment = comment
                        });
                    }
                }
            }
            catch (OperationCanceledException)
            {
                throw new OperationCanceledException();
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }
        private void AfterRepinAddDataToDataBase(ScrapeResultNew scrapeResult, string label, string repinId)
        {
            try
            {
                IDbAccountService dbAccountService = new DbAccountService(DominatorAccountModel);
                IDbCampaignService dbCampaignService = new DbCampaignService(CampaignId);
                var pin = (PinterestPin)scrapeResult.ResultPost;
                var jobActivityConfigurationManager =
                    InstanceProvider.GetInstance<IJobActivityConfigurationManager>();
                var moduleSetting = jobActivityConfigurationManager[DominatorAccountModel.AccountId, ActivityType];

                if (moduleSetting == null)
                    return;
                if (moduleSetting.IsTemplateMadeByCampaignMode)
                    dbCampaignService.Add(new DominatorHouseCore.DatabaseHandler.PdTables.Campaigns.InteractedPosts
                    {
                        OperationType = ActivityType.ToString(),
                        InteractionDate = DateTimeUtilities.GetEpochTime(),
                        QueryType = scrapeResult.QueryInfo.QueryType,
                        Query = scrapeResult.QueryInfo.QueryValue,
                        Username = pin.User.Username,
                        SourceBoard = pin.BoardUrl,
                        SourceBoardName = pin.BoardName,
                        DestinationBoard = pin.BoardUrlToRepin,
                        CommentCount = pin.CommentCount,
                        MediaType = pin.MediaType,
                        PinDescription = pin.Description,
                        PinId = pin.PinId,
                        GeneratedPinId = repinId,
                        PinWebUrl = pin.PinWebUrl,
                        UserId = pin.User.UserId,
                        TryCount = pin.NoOfTried,
                        MediaString = pin.MediaString,
                        BoardLabel = label,
                        PublishedDate = pin.PublishDate,
                        SinAccId = DominatorAccountModel.AccountBaseModel.UserId,
                        SinAccUsername = DominatorAccountModel.AccountBaseModel.UserName
                    });
                dbAccountService.Add(new InteractedPosts
                {
                    OperationType = ActivityType.ToString(),
                    InteractionDate = DateTimeUtilities.GetEpochTime(),
                    QueryType = scrapeResult.QueryInfo.QueryType,
                    Query = scrapeResult.QueryInfo.QueryValue,
                    Username = pin.User.Username,
                    SourceBoard = pin.BoardUrl,
                    SourceBoardName = pin.BoardName,
                    DestinationBoard = pin.BoardUrlToRepin,
                    CommentCount = pin.CommentCount,
                    MediaType = pin.MediaType,
                    PinDescription = pin.Description,
                    PinId = pin.PinId,
                    GeneratedPinId = repinId,
                    PinWebUrl = pin.PinWebUrl,
                    UserId = pin.User.UserId,
                    TryCount = pin.NoOfTried,
                    MediaString = pin.MediaString,
                    PublishedDate = pin.PublishDate,
                    BoardLabel = label
                });
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }
    }
}