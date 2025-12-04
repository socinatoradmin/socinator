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
using System.Linq;
using System.Text.RegularExpressions;
using AccountInteractedPages = DominatorHouseCore.DatabaseHandler.FdTables.Accounts.InteractedPages;
using CampaignInteractedPages = DominatorHouseCore.DatabaseHandler.FdTables.Campaigns.InteractedPages;

namespace FaceDominatorCore.FDLibrary.FdProcesses
{

    public class MessageToPlacesProcess : FdJobProcessInteracted<AccountInteractedPages>
    {
        public MessageToPlacesModel MessageToPlacesModel { get; set; }

        public DominatorAccountModel Account { get; set; }

        public Dictionary<string, string> DictPageUrl = new Dictionary<string, string>();

        public IFdRequestLibrary FdRequestLibrary { get; set; }

        public Dictionary<string, string> AccountMessagePair { get; set; } = new Dictionary<string, string>();

        public MessageToPlacesProcess(IProcessScopeModel processScopeModel,
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

                KeyValuePair<string, string> message;

                message = GetMessageDetails(scrapeResult.QueryInfo, objFanpageDetails);

                //if (!string.IsNullOrEmpty(message.Value))
                //    FdRequestLibrary.SendImageWithText(AccountModel, objFanpageDetails.FanPageID, new List<string>() { message.Value });


                var isSuccess = AccountModel.IsRunProcessThroughBrowser ?
                            FdLogInProcess._browserManager.SendMessage(AccountModel, objFanpageDetails.FanPageID, message.Key, MessageToPlacesModel.IsMessageAsPreview,
                            scrapeResult.QueryInfo.QueryTypeEnum, message.Value, openWindow: true) : (MessageToPlacesModel.IsMessageAsPreview ? FdRequestLibrary.SendTextMessageWithLinkPreview(AccountModel, objFanpageDetails.FanPageID, message.Key)
                            : FdRequestLibrary.SendTextMessage(AccountModel, objFanpageDetails.FanPageID, message.Key));

                if (isSuccess.Status)
                {
                    GlobusLogHelper.log.Info(Log.ActivitySuccessful, DominatorAccountModel.AccountBaseModel.AccountNetwork, DominatorAccountModel.AccountBaseModel.UserName, ActivityType, objFanpageDetails.FanPageID);
                    IncrementCounters();
                    AddProfileScraperDataToDatabase(scrapeResult, message);
                    jobProcessResult.IsProcessSuceessfull = true;
                }
                else
                {
                    GlobusLogHelper.log.Info(Log.ActivityFailed, DominatorAccountModel.AccountBaseModel.AccountNetwork, DominatorAccountModel.AccountBaseModel.UserName, ActivityType, objFanpageDetails.FanPageID, isSuccess.FbErrorDetails);
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



        private KeyValuePair<string, string> GetMessageDetails(QueryInfo queryInfo, FanpageDetails objFanpageDetails)
        {
            KeyValuePair<string, string> messageDetail = new KeyValuePair<string, string>();

            List<KeyValuePair<string, string>> lstMessage = new List<KeyValuePair<string, string>>();

            bool isMessageAdded = false;

            try
            {
                MessageToPlacesModel.LstDisplayManageMessageModel.ForEach(x =>
                {
                    x.SelectedQuery.ForEach(y =>
                    {
                        if (y.Content.QueryTypeEnum == queryInfo.QueryTypeEnum && y.Content.QueryValue == queryInfo.QueryValue)
                        {
                            if (!lstMessage.Any(z => z.Key == x.MessagesText && z.Value == x.MediaPath))
                            {
                                KeyValuePair<string, string> newMessage = new KeyValuePair<string, string>(x.MessagesText, x.MediaPath);
                                lstMessage.Add(newMessage);
                            }
                        }
                    });
                });

                foreach (var currentMessage in lstMessage)
                {
                    messageDetail = currentMessage;

                    var textMessage = currentMessage.Key;

                    var messageCollection = new List<string>();

                    var messages = new List<string>();

                    var messageText = string.Empty;

                    if (MessageToPlacesModel.IsTagChecked)
                    {
                        textMessage = ReplaceTagWithValue(objFanpageDetails, textMessage);
                    }

                    if (MessageToPlacesModel.IsSpintaxChecked)
                    {
                        messages = SpinTexHelper.GetSpinMessageCollection(textMessage);

                        foreach (var message in messages)
                        {
                            messageText = message.Trim();
                            messageCollection.Add(messageText);
                        }

                        if (AccountMessagePair.Count >= messageCollection.Count)
                            AccountMessagePair.Clear();
                    }
                    else
                    {
                        messageCollection.Add(textMessage);
                        if (AccountMessagePair.Count >= lstMessage.Count)
                            AccountMessagePair.Clear();
                    }

                    messageCollection.Shuffle();
                    messageCollection.Shuffle();

                    foreach (var message in messageCollection)
                    {
                        try
                        {
                            if (AccountMessagePair.Any(z =>
                                z.Key == message && z.Key.Contains(currentMessage.Value)))
                            {
                            }
                            else
                            {
                                messageDetail = new KeyValuePair<string, string>(message, currentMessage.Value);

                                AccountMessagePair.Add($"{message}:{currentMessage.Value}", AccountModel.AccountId);

                                isMessageAdded = true;

                                break;
                            }

                        }
                        catch (Exception ex)
                        {
                            ex.DebugLog();
                        }
                    }

                    if (isMessageAdded)
                    {
                        break;
                    }
                }

                return messageDetail;

            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            return messageDetail;
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


        private void AddProfileScraperDataToDatabase(ScrapeResultNew scrapeResult, KeyValuePair<string, string> message)
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
                        PageName = page.FanPageName,
                        PageType = page.FanPageCategory,
                        PageFullDetails = JsonConvert.SerializeObject(page),
                        TotalLikers = page.FanPageLikerCount,
                        InteractionTimeStamp = DateTimeUtilities.GetEpochTime(),
                        MessageText = message.Key,
                        UploadedMediaPath = message.Value,
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
                    TotalLikers = page.FanPageLikerCount,
                    PageFullDetails = JsonConvert.SerializeObject(page),
                    MessageText = message.Key,
                    UploadedMediaPath = message.Value,
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
