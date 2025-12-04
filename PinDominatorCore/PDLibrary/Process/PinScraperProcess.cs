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
using PinDominatorCore.PDUtility;
using PinDominatorCore.Request;
using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;

namespace PinDominatorCore.PDLibrary.Process
{
    public class PinScraperProcess : PdJobProcessInteracted<InteractedPosts>
    {
        public PinScraperProcess(IProcessScopeModel processScopeModel,
            IDbAccountServiceScoped accountServiceScoped, IDbGlobalService globalService,
            IExecutionLimitsManager executionLimitsManager, 
            IPdQueryScraperFactory queryScraperFactory, IPdHttpHelper pdhttpHelper, IPdLogInProcess pdLogInProcess)
            : base(processScopeModel, accountServiceScoped, globalService, executionLimitsManager, queryScraperFactory,
                pdhttpHelper, pdLogInProcess)
        {
            var templatesFileManager = InstanceProvider.GetInstance<ITemplatesFileManager>();
            PinScraperModel = JsonConvert.DeserializeObject<PinScraperModel>(templatesFileManager
                .GetTemplateById(processScopeModel.TemplateId)?.ActivitySettings);
            var campaignFileManager = InstanceProvider.GetInstance<ICampaignsFileManager>();
            Campaign = campaignFileManager.GetCampaignById(CampaignId);
        }

        public PinScraperModel PinScraperModel { get; set; }
        public CampaignDetails Campaign { get; set; }

        public override JobProcessResult PostScrapeProcess(ScrapeResultNew scrapeResult)
        {
            var jobProcessResult = new JobProcessResult();

            try
            {
                var pin = (PinterestPin) scrapeResult.ResultPost;
                JobCancellationTokenSource.Token.ThrowIfCancellationRequested();

                if (PinScraperModel.IsDownLoadAfterScrap)
                {
                    pin = PinFunct.GetPinDetails(pin.PinId, DominatorAccountModel);
                    DownloadPin(pin.MediaString);
                }

                GlobusLogHelper.log.Info(Log.ActivitySuccessful, DominatorAccountModel.AccountBaseModel.AccountNetwork,
                    DominatorAccountModel.AccountBaseModel.UserName, ActivityType, pin?.PinId);

                IncrementCounters();

                AddPinScrapedDataToDataBase(scrapeResult);
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

        public void DownloadPin(string imageUrl)
        {
            try
            {
                if (string.IsNullOrEmpty(PinScraperModel.DownloadedFolderPath))
                    PinScraperModel.DownloadedFolderPath = PdConstants.DownloadImagePath;
                var downloadPinPath = PinScraperModel.DownloadedFolderPath +
                                      $"\\{ConstantVariable.ApplicationName}\\{DateTime.Now.Date.ToString("dd-MMM-yyyy", CultureInfo.InvariantCulture)}";

                if (!Directory.Exists(downloadPinPath))
                    DirectoryUtilities.CreateDirectory(downloadPinPath);

                var objWebClient = new WebClient();
                var imageBytes = objWebClient.DownloadData(imageUrl);
                downloadPinPath += "\\" + imageUrl.Split('/').Last();
                objWebClient.UploadData(downloadPinPath, imageBytes);
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        private void AddPinScrapedDataToDataBase(ScrapeResultNew scrapeResult)
        {
            try
            {
                IDbAccountService dbAccountService = new DbAccountService(DominatorAccountModel);
                IDbCampaignService dbCampaignService = new DbCampaignService(CampaignId);
                var pin = (PinterestPin) scrapeResult.ResultPost;
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
                        CommentCount = pin.CommentCount,
                        MediaType = pin.MediaType,
                        PinDescription = pin.Description,
                        PinId = pin.PinId,
                        PinWebUrl = pin.PinWebUrl,
                        UserId = pin.User.UserId,
                        MediaString = pin.MediaString,
                        //TryCount = pin.NoOfTried,
                        PinTitle = pin.PinName,
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
                    CommentCount = pin.CommentCount,
                    MediaType = pin.MediaType,
                    PinDescription = pin.Description,
                    PinId = pin.PinId,
                    PinWebUrl = pin.PinWebUrl,
                    UserId = pin.User.UserId,
                    MediaString = pin.MediaString,
                    //TryCount = pin.NoOfTried,
                    PinTitle = pin.PinName
                });
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }
    }
}