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
using FaceDominatorCore.FDModel.MessageModel;
using FaceDominatorCore.FDRequest;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using AccountInteractedPages = DominatorHouseCore.DatabaseHandler.FdTables.Accounts.InteractedPages;
using CampaignInteractedPages = DominatorHouseCore.DatabaseHandler.FdTables.Campaigns.InteractedPages;

namespace FaceDominatorCore.FDLibrary.FdProcesses
{

    public class PlaceScraperProcess : FdJobProcessInteracted<AccountInteractedPages>
    {
        public MessageToPlacesModel MessageToPlacesModel { get; set; }

        public DominatorAccountModel Account { get; set; }

        public Dictionary<string, string> DictPageUrl = new Dictionary<string, string>();

        public IFdRequestLibrary FdRequestLibrary { get; set; }

        public Dictionary<string, string> AccountMessagePair { get; set; } = new Dictionary<string, string>();

        public PlaceScraperProcess(IProcessScopeModel processScopeModel,
             IDbAccountServiceScoped accountServiceScoped, IFdQueryScraperFactory queryScraperFactory,
             IFdHttpHelper fdHttpHelper, IFdLoginProcess fdLogInProcess, IFdRequestLibrary fdRequestLibrary,
             IDbCampaignServiceScoped dbCampaignServiceScoped)
            : base(processScopeModel, accountServiceScoped, queryScraperFactory, fdHttpHelper, fdLogInProcess, dbCampaignServiceScoped)
        {
            FdRequestLibrary = fdRequestLibrary;
            MessageToPlacesModel = processScopeModel.GetActivitySettingsAs<MessageToPlacesModel>();
            AccountModel = DominatorAccountModel;
            CheckJobProcessLimitsReached();
        }

        public override JobProcessResult PostScrapeProcess(ScrapeResultNew scrapeResult)
        {
            JobProcessResult jobProcessResult = new JobProcessResult();

            FanpageDetails objFanpageDetails = (FanpageDetails)scrapeResult.ResultPage;

            try
            {
                JobCancellationTokenSource.Token.ThrowIfCancellationRequested();

                bool isSuccess = true;

                if (isSuccess)
                {
                    GlobusLogHelper.log.Info(Log.ActivitySuccessful, DominatorAccountModel.AccountBaseModel.AccountNetwork, DominatorAccountModel.AccountBaseModel.UserName, ActivityType, objFanpageDetails.FanPageID);
                    IncrementCounters();
                    AddProfileScraperDataToDatabase(scrapeResult);
                    jobProcessResult.IsProcessSuceessfull = true;
                }
                else
                {
                    GlobusLogHelper.log.Info(Log.ActivityFailed, DominatorAccountModel.AccountBaseModel.AccountNetwork, DominatorAccountModel.AccountBaseModel.UserName, ActivityType, objFanpageDetails.FanPageID);
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

        public string ReplaceTagWithValue(FanpageDetails objobjFanpageDetails, string message)
        {
            try
            {
                message = Regex.Replace(message, "<PageName>", objobjFanpageDetails.FanPageName);
                if ((message.Contains("<MYNAME>") || message.Contains("<MYFIRSTNAME>")) && !string.IsNullOrEmpty(AccountModel.AccountBaseModel.UserFullName))
                {
                    var myFirstName = Regex.Split(AccountModel.AccountBaseModel.UserFullName, " ")[0];
                    message = Regex.Replace(message, "<MYNAME>", AccountModel.AccountBaseModel.UserFullName);
                    message = Regex.Replace(message, "<MYFIRSTNAME>", myFirstName);
                }


                return message;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            return string.Empty;
        }


        private void AddProfileScraperDataToDatabase(ScrapeResultNew scrapeResult)
        {
            try
            {

                FanpageDetails page = (FanpageDetails)scrapeResult.ResultPage;
                var jobActivityConfigurationManager = InstanceProvider.GetInstance<IJobActivityConfigurationManager>();

                var modulesetting = jobActivityConfigurationManager[DominatorAccountModel.AccountId, ActivityType];

                if (modulesetting == null)
                    return;

                if (modulesetting.IsTemplateMadeByCampaignMode)
                {
                    ObjDbCampaignService.Add(new CampaignInteractedPages
                    {
                        ActivityType = ActivityType.ToString(),
                        QueryType = scrapeResult.QueryInfo.QueryType,
                        QueryValue = scrapeResult.QueryInfo.QueryValue,
                        AccountEmail = DominatorAccountModel.AccountBaseModel.UserName,
                        PageId = page.FanPageID,
                        PageUrl = $"{FdConstants.FbHomeUrl}{page.FanPageID}",
                        PageFullDetails = JsonConvert.SerializeObject(page),
                        PageName = page.FanPageName,
                        PageType = page.FanPageCategory,
                        TotalLikers = page.FanPageLikerCount,
                        InteractionTimeStamp = DateTimeUtilities.GetEpochTime(),
                        InteractionDateTime = DateTime.Now


                    });
                }

                ObjDbAccountService.Add(new AccountInteractedPages
                {
                    ActivityType = ActivityType.ToString(),
                    QueryType = scrapeResult.QueryInfo.QueryType,
                    QueryValue = scrapeResult.QueryInfo.QueryValue,
                    PageId = page.FanPageID,
                    PageUrl = $"{FdConstants.FbHomeUrl}{page.FanPageID}",
                    PageName = page.FanPageName,
                    PageType = page.FanPageCategory,
                    PageFullDetails = JsonConvert.SerializeObject(page),
                    TotalLikers = page.FanPageLikerCount,
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
