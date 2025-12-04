using CommonServiceLocator;
using DominatorHouseCore;
using DominatorHouseCore.Enums;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using DominatorHouseCore.Process;
using DominatorHouseCore.Utility;
using FaceDominatorCore.FDFactories;
using FaceDominatorCore.FDLibrary.DAL;
using FaceDominatorCore.FDLibrary.FdClassLibrary;
using FaceDominatorCore.FDLibrary.FdFunctions;
using FaceDominatorCore.FDLibrary.FdFunctions.FdBrowserManager;
using FaceDominatorCore.FDModel.MessageModel;
using FaceDominatorCore.FDRequest;
using FaceDominatorCore.FDResponse.MessagesResponse;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Unity;
using AccountInteractedUsres = DominatorHouseCore.DatabaseHandler.FdTables.Accounts.InteractedUsers;
using CampaignInteractedUsres = DominatorHouseCore.DatabaseHandler.FdTables.Campaigns.InteractedUsers;

namespace FaceDominatorCore.FDLibrary.FdProcesses
{

    public class FdBroadCastMessageProcess : FdJobProcessInteracted<AccountInteractedUsres>
    {
        public BrodcastMessageModel BrodcastMessageModel { get; set; }

        public Dictionary<string, string> AccountMessagePair { get; set; } = new Dictionary<string, string>();

        private static Dictionary<string, string> MessagePostPair { get; } = new Dictionary<string, string>();

        public DominatorAccountModel Account { get; set; }

        private static readonly object MessageLock = new object();
        private static readonly Dictionary<string, string> UniqueAccountMessagePair = new Dictionary<string, string>();
        private static readonly Dictionary<string, bool> UniqueMessageAvailability = new Dictionary<string, bool>();

        private int FailedCount { get; set; }

        public IFdRequestLibrary FdRequestLibrary { get; set; }

        IAccountScopeFactory _accountScopeFactory = InstanceProvider.GetInstance<IAccountScopeFactory>();
        public FdBroadCastMessageProcess(IProcessScopeModel processScopeModel,
            IDbAccountServiceScoped accountServiceScoped, IFdQueryScraperFactory queryScraperFactory,
            IFdHttpHelper fdHttpHelper, IFdLoginProcess fdLogInProcess, IFdRequestLibrary fdRequestLibrary,
            IDbCampaignServiceScoped dbCampaignServiceScoped)
            : base(processScopeModel, accountServiceScoped, queryScraperFactory, fdHttpHelper, fdLogInProcess, dbCampaignServiceScoped)
        {

            FdRequestLibrary = fdRequestLibrary;
            AccountModel = DominatorAccountModel;
            BrodcastMessageModel = processScopeModel.GetActivitySettingsAs<BrodcastMessageModel>();

            if (!UniqueMessageAvailability.ContainsKey(AccountModel.AccountId))
                UniqueMessageAvailability.Add(AccountModel.AccountId, true);
            else
                UniqueMessageAvailability[AccountModel.AccountId] = true;
        }

        public override JobProcessResult PostScrapeProcess(ScrapeResultNew scrapeResult)
        {
            JobProcessResult jobProcessResult = new JobProcessResult();

            try
            {
                var binFileHelper = InstanceProvider.GetInstance<IBinFileHelper>();

                var campaignDetails = binFileHelper.GetCampaignDetail().FirstOrDefault(x => x.CampaignId == CampaignId);
                //If Process from Account Activity
                var noOfAccount = 1;

                if (campaignDetails != null)
                    noOfAccount = campaignDetails.SelectedAccountList.Count;

                var objFacebookUser = (FacebookUser)scrapeResult.ResultUser;



                IFdBrowserManager browserManager = null;
                bool isCloseWindow = false;
                string url = objFacebookUser.ScrapedProfileUrl == $"{FdConstants.FbHomeUrl}" || string.IsNullOrEmpty(objFacebookUser.ScrapedProfileUrl)
                    ? objFacebookUser.ProfileUrl
                    : objFacebookUser.ScrapedProfileUrl;

                if (AccountModel.IsRunProcessThroughBrowser && scrapeResult.QueryInfo.QueryTypeEnum != "CustomProfileUrl")
                {
                    browserManager = _accountScopeFactory[$"{objFacebookUser.UserId}{AccountModel.AccountId}"]
                                 .Resolve<IFdBrowserManager>();
                    browserManager.LoadPageSource(AccountModel, url);
                    isCloseWindow = true;
                }
                else
                    browserManager = FdLogInProcess._browserManager;

                if ((MessagePostPair.Count == 0 || UniqueAccountMessagePair.Count == 0) &&
                    (BrodcastMessageModel.IschkUniqueMessageOptionChecked))
                {
                    LoadSentMessageFromDb();
                }

                try
                {
                    JobCancellationTokenSource.Token.ThrowIfCancellationRequested();

                    var message = BrodcastMessageModel.IschkUniqueMessageOptionChecked
                        ? GetUniqueMessageDetails(scrapeResult.QueryInfo, objFacebookUser)
                        : GetMessageDetails(scrapeResult.QueryInfo, objFacebookUser);

                    if (!string.IsNullOrEmpty(message.Item1.Value) && !AccountModel.IsRunProcessThroughBrowser)
                        FdRequestLibrary.SendImageWithText(AccountModel, objFacebookUser.UserId, new List<string>() { message.Item1.Value });

                    FdSendTextMessageResponseHandler isSuccess = null;

                    if (!string.IsNullOrEmpty(message.Item1.Key) || message.Item2.Count != 0)
                    {
                        //For own friends we are sending with singe window so opening dialog
                        var cansend = scrapeResult.QueryInfo.QueryTypeEnum == "OwnFriends" && AccountModel.IsRunProcessThroughBrowser
                            ? browserManager.OpenFriendLinkAndSendMessage(AccountModel, objFacebookUser, false)
                            : true;

                        isSuccess = AccountModel.IsRunProcessThroughBrowser && cansend ?
                            browserManager.SendMessage(AccountModel, objFacebookUser.UserId, message.Item1.Key, BrodcastMessageModel.IsMessageAsPreview,
                            scrapeResult.QueryInfo.QueryTypeEnum, message.Item1.Value, medias: message.Item2, openWindow: true) : (BrodcastMessageModel.IsMessageAsPreview ? FdRequestLibrary.SendTextMessageWithLinkPreview(AccountModel, objFacebookUser.UserId, message.Item1.Key)
                            : FdRequestLibrary.SendTextMessage(AccountModel, objFacebookUser.UserId, message.Item1.Key));

                        if ((BrodcastMessageModel.IschkUniqueMessageOptionChecked) && (isSuccess == null || !isSuccess.IsMessageSent))
                        {
                            try
                            {
                                UniqueAccountMessagePair.Remove(UniqueAccountMessagePair.FirstOrDefault(x =>
                                                           x.Key.Contains(message.Item1.Key) && x.Key.Contains(CampaignId) &&
                                                           x.Key.Contains(message.Item1.Value)).Key);
                            }
                            catch (Exception ex)
                            {
                                ex.DebugLog();
                            }
                        }
                    }

                    if (string.IsNullOrEmpty(message.Item1.Key) && string.IsNullOrEmpty(message.Item1.Value)
                              && (BrodcastMessageModel.IschkUniqueMessageOptionChecked))
                    {
                        if (UniqueMessageAvailability.ContainsKey(AccountModel.AccountId))
                        {
                            UniqueMessageAvailability[AccountModel.AccountId] = false;
                            FailedCount++;
                        }

                        if (FailedCount > 10 || (UniqueMessageAvailability.Count == noOfAccount) &&
                            UniqueMessageAvailability.All(y => !y.Value))
                        {
                            GlobusLogHelper.log.Info(Log.CustomMessage,
                                DominatorAccountModel.AccountBaseModel.AccountNetwork, DominatorAccountModel.UserName,
                                ActivityType, "No more unique message available for this campaign!");
                            Stop();
                            jobProcessResult.IsProcessSuceessfull = false;
                            //unique messages got over then complete the process
                            jobProcessResult.IsProcessCompleted = true;
                            jobProcessResult.HasNoResult = true;
                        }
                        else
                        {
                            GlobusLogHelper.log.Info(Log.CustomMessage,
                                DominatorAccountModel.AccountBaseModel.AccountNetwork, DominatorAccountModel.UserName,
                                ActivityType, "No more unique message available!");
                            jobProcessResult.IsProcessSuceessfull = false;
                            jobProcessResult.IsProcessCompleted = true;
                            jobProcessResult.HasNoResult = true;

                        }

                    }
                    if (isSuccess != null && isSuccess.IsMessageSent)
                    {
                        GlobusLogHelper.log.Info(Log.ActivitySuccessful,
                            DominatorAccountModel.AccountBaseModel.AccountNetwork,
                            DominatorAccountModel.AccountBaseModel.UserName, ActivityType, objFacebookUser.UserId);
                        IncrementCounters();
                        AddSendRequestDataToDatabase(scrapeResult, message.Item1);
                        jobProcessResult.IsProcessSuceessfull = true;
                    }
                    else
                    {
                        GlobusLogHelper.log.Info(Log.ActivityFailed,
                            DominatorAccountModel.AccountBaseModel.AccountNetwork,
                            DominatorAccountModel.AccountBaseModel.UserName, ActivityType,
                            isSuccess != null && isSuccess.FbErrorDetails != null ? isSuccess.FbErrorDetails.Description : "Unknown Error");
                        jobProcessResult.IsProcessSuceessfull = false;
                    }

                    if (isCloseWindow)
                        browserManager.CloseBrowser(DominatorAccountModel);

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

            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            return jobProcessResult;
        }

        private void LoadSentMessageFromDb()
        {
            List<CampaignInteractedUsres> data = ObjDbCampaignService.GetAllInteractedData<CampaignInteractedUsres>();

            data.ForEach(x =>
            {
                try
                {
                    var message = JsonConvert.DeserializeObject<KeyValuePair<string, string>>(x.DetailedUserInfo);

                    var fullMessageForPost = string.IsNullOrEmpty(message.Value) ? $"{message.Key}_{x.UserId}"
                        : $"{message.Key}:{message.Value}_{x.UserId}";

                    var fullMessageForAccount = string.IsNullOrEmpty(message.Value) ? message.Key
                        : $"{message.Key}:{message.Value}";


                    if (!UniqueAccountMessagePair.ContainsKey($"{fullMessageForAccount}"))
                        UniqueAccountMessagePair.Add($"{fullMessageForAccount}", $"{fullMessageForAccount}");
                    if (!MessagePostPair.ContainsKey($"{fullMessageForPost}"))
                        MessagePostPair.Add($"{fullMessageForPost}", $"{fullMessageForPost}");

                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            });
        }

        private void AddSendRequestDataToDatabase(ScrapeResultNew scrapeResult, KeyValuePair<string, string> message)
        {
            try
            {

                FacebookUser user = (FacebookUser)scrapeResult.ResultUser;

                var jobActivityConfigurationManager = InstanceProvider.GetInstance<IJobActivityConfigurationManager>();

                var modulesetting = jobActivityConfigurationManager[DominatorAccountModel.AccountId, ActivityType];

                if (modulesetting == null)
                    return;

                if (modulesetting.IsTemplateMadeByCampaignMode)
                {
                    ObjDbCampaignService.Add(new CampaignInteractedUsres
                    {
                        ActivityType = ActivityType.ToString(),
                        QueryType = scrapeResult.QueryInfo.QueryType,
                        QueryValue = scrapeResult.QueryInfo.QueryValue,
                        AccountEmail = DominatorAccountModel.AccountBaseModel.UserName,
                        UserId = user.UserId,
                        Username = user.Familyname,
                        UserProfileUrl = user.ProfileUrl,
                        InteractionTimeStamp = DateTimeUtilities.GetEpochTime(),
                        InteractionDateTime = DateTime.Now,
                        DetailedUserInfo = JsonConvert.SerializeObject(message),
                        Gender = user.Gender,
                        University = user.University,
                        Workplace = user.WorkPlace,
                        CurrentCity = user.Currentcity,
                        HomeTown = user.Hometown,
                        BirthDate = user.DateOfBirth,
                        ContactNo = user.ContactNo
                    });
                }

                ObjDbAccountService.Add(new AccountInteractedUsres
                {
                    ActivityType = ActivityType.ToString(),
                    QueryType = scrapeResult.QueryInfo.QueryType,
                    QueryValue = scrapeResult.QueryInfo.QueryValue,
                    UserId = user.UserId,
                    Username = user.Familyname,
                    UserProfileUrl = user.ProfileUrl,
                    InteractionTimeStamp = DateTimeUtilities.GetEpochTime(),
                    InteractionDateTime = DateTime.Now,
                    DetailedUserInfo = JsonConvert.SerializeObject(message)
                });
            }
            catch (Exception ex)
            {
                ex.ErrorLog(ex.Message);
            }
        }



        public string ReplaceTagWithValue(FacebookUser objFacebookUser, string message)
        {
            try
            {
                var splitUserName = Regex.Split(objFacebookUser.FullName, " ");
                var myFirstName = Regex.Split(AccountModel.AccountBaseModel.UserFullName, " ")[0];
                var firstName = splitUserName[0];
                var lastName = splitUserName.Length > 2 ? splitUserName[2] : (splitUserName.Length > 1 ? splitUserName[1] : string.Empty);
                message = Regex.Replace(message, "<FULLNAME>", objFacebookUser.FullName);
                message = Regex.Replace(message, "<FIRSTNAME>", firstName);
                message = Regex.Replace(message, "<LASTNAME>", lastName);
                if ((message.Contains("<MYNAME>") || message.Contains("<MYFIRSTNAME>")) && !string.IsNullOrEmpty(AccountModel.AccountBaseModel.UserFullName))
                {
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

        private (KeyValuePair<string, string>, List<string>) GetMessageDetails(QueryInfo queryInfo, FacebookUser objFacebookUser)
        {
            KeyValuePair<string, string> messageDetail = new KeyValuePair<string, string>();

            List<KeyValuePair<string, string>> lstMessage = new List<KeyValuePair<string, string>>();
            List<string> mediasList = new List<string>();
            bool isMessageAdded = false;

            try
            {
                BrodcastMessageModel.LstDisplayManageMessageModel.ForEach(x =>
                {
                    x.SelectedQuery.ForEach(y =>
                    {
                        if (y.Content.QueryTypeEnum == queryInfo.QueryTypeEnum && y.Content.QueryValue.Trim() == queryInfo.QueryValue.Trim())
                        {
                            if (!lstMessage.Any(z => z.Key == x.MessagesText && z.Value == x.MediaPath))
                            {
                                KeyValuePair<string, string> newMessage = new KeyValuePair<string, string>(x.MessagesText ?? string.Empty, x.MediaPath);
                                lstMessage.Add(newMessage);
                            }
                            x.Medias.ForEach(m =>
                            {
                                if (!mediasList.Any(z => z.Contains(m.MediaPath)))
                                    mediasList.Add(m.MediaPath);
                            });
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

                    if (BrodcastMessageModel.IsTagChecked)
                    {
                        textMessage = ReplaceTagWithValue(objFacebookUser, textMessage);
                    }

                    if (BrodcastMessageModel.IsSpintaxChecked)
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

                    foreach (var message in messageCollection)
                    {
                        try
                        {
                            if (AccountMessagePair.Any(z =>
                                z.Key.Contains(message) && z.Key.Contains(currentMessage.Value)))
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

                return (messageDetail, mediasList);

            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            return (messageDetail, mediasList);
        }

        private (KeyValuePair<string, string>, List<string>) GetUniqueMessageDetails(QueryInfo queryInfo, FacebookUser objFacebookUser)
        {
            KeyValuePair<string, string> messageDetail = new KeyValuePair<string, string>();

            List<KeyValuePair<string, string>> lstMessage = new List<KeyValuePair<string, string>>();
            List<string> mediasList = new List<string>();
            try
            {
                BrodcastMessageModel.LstDisplayManageMessageModel.ForEach(x =>
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
                        x.Medias.ForEach(m =>
                        {
                            if (!mediasList.Any(z => z.Contains(m.MediaPath)))
                                mediasList.Add(m.MediaPath);
                        });
                    });
                });

                messageDetail = GetUniqueMessage(lstMessage, objFacebookUser);

                return (messageDetail, mediasList);
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            return (messageDetail, mediasList);
        }


        public KeyValuePair<string, string> GetUniqueMessage(List<KeyValuePair<string, string>> lstMessage, FacebookUser objFacebookUser)
        {
            KeyValuePair<string, string> messageDetail = new KeyValuePair<string, string>();


            var jobActivityConfigurationManager = InstanceProvider.GetInstance<IJobActivityConfigurationManager>();

            var modulesetting = jobActivityConfigurationManager[DominatorAccountModel.AccountId, ActivityType];


            foreach (var currentMessage in lstMessage)
            {
                var textMessage = currentMessage.Key;

                var messageCollection = new List<string>();

                var message = string.Empty;

                if (BrodcastMessageModel.IsTagChecked)
                {
                    textMessage = ReplaceTagWithValue(objFacebookUser, textMessage);
                }

                if (BrodcastMessageModel.IsSpintaxChecked)
                {
                    messageCollection = SpinTexHelper.GetSpinMessageCollection(textMessage);
                }
                else
                {
                    messageCollection.Add(textMessage);
                }

                if (AccountMessagePair.Count >= messageCollection.Count)
                {
                    AccountMessagePair.Clear();
                }

                foreach (var messageText in messageCollection)
                {
                    try
                    {
                        message = messageText.Trim();
                        //UniqueAccountMessagePair.Clear();

                        if (modulesetting.IsTemplateMadeByCampaignMode && BrodcastMessageModel.IschkUniqueMessageChecked)
                        {
                            lock (MessageLock)
                            {
                                var fullMessage = string.IsNullOrEmpty(currentMessage.Value) ? $"{message}_{CampaignId}"
                                    : $"{message}_{CampaignId}:{currentMessage.Value}";

                                if (!UniqueAccountMessagePair.Any(x => x.Key == fullMessage))
                                {
                                    UniqueAccountMessagePair.Add(fullMessage, $"{AccountModel.AccountBaseModel.UserName}");
                                }
                                else
                                {
                                    continue;
                                }
                            }
                        }

                        if (modulesetting.IsTemplateMadeByCampaignMode && BrodcastMessageModel.IschkUniqueMessageForUserChecked)
                        {
                            lock (MessageLock)
                            {
                                var fullMessageForUser = string.IsNullOrEmpty(currentMessage.Value) ? $"{message}_{objFacebookUser.UserId}_{CampaignId}"
                                    : $"{message}:{currentMessage.Value}_{objFacebookUser.UserId}_{CampaignId}";

                                if (!MessagePostPair.Keys.Any(x => x == fullMessageForUser))
                                {
                                    MessagePostPair.Add($"{fullMessageForUser}", $"{fullMessageForUser}");
                                }
                                else
                                {
                                    continue;
                                }
                            }
                        }

                        AccountMessagePair.Add($"{message}:{currentMessage.Value}", AccountModel.AccountId);

                        messageDetail = new KeyValuePair<string, string>(message, currentMessage.Value);

                        break;

                    }
                    catch (Exception ex)
                    {
                        ex.DebugLog();
                    }
                }

                if (!string.IsNullOrEmpty(messageDetail.Key))
                    break;

            }

            return messageDetail;
        }



        public override void StartOtherConfiguration(ScrapeResultNew scrapeResult)
        {
            try
            {

                base.StartOtherConfiguration(scrapeResult);

                //FdRequestLibrary fdRequestLibrary = new FdRequestLibrary();
                var fdRequestLibrary = InstanceProvider.GetInstance<IFdRequestLibrary>();

                fdRequestLibrary.ChangeLanguage(AccountModel, FdConstants.AccountLanguage[AccountModel.AccountBaseModel.UserId]);

            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }


    }
}
