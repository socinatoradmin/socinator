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
using AccountInteractedUsres = DominatorHouseCore.DatabaseHandler.FdTables.Accounts.InteractedUsers;
using CampaignInteractedUsres = DominatorHouseCore.DatabaseHandler.FdTables.Campaigns.InteractedUsers;

namespace FaceDominatorCore.FDLibrary.FdProcesses
{

    public class SendMessageToNewFriendsProcess : FdJobProcessInteracted<AccountInteractedUsres>
    {
        public MessageRecentFriendsModel MessageRecentFriendsModel { get; set; }

        public DominatorAccountModel Account { get; set; }

        public Dictionary<string, string> AccountMessagePair { get; set; } = new Dictionary<string, string>();

        private static Dictionary<string, string> MessagePostPair { get; } = new Dictionary<string, string>();

        private static Dictionary<string, string> UniqueAccountMessagePair { get; } = new Dictionary<string, string>();

        private static readonly object MessageLock = new object();

        public IFdRequestLibrary FdRequestLibrary { get; set; }

        public SendMessageToNewFriendsProcess(IProcessScopeModel processScopeModel,
            IDbAccountServiceScoped accountServiceScoped, IFdQueryScraperFactory queryScraperFactory,
            IFdHttpHelper fdHttpHelper, IFdLoginProcess fdLogInProcess, IFdRequestLibrary fdRequestLibrary,
            IDbCampaignServiceScoped dbCampaignServiceScoped)
            : base(processScopeModel, accountServiceScoped, queryScraperFactory, fdHttpHelper, fdLogInProcess, dbCampaignServiceScoped)
        {
            FdRequestLibrary = fdRequestLibrary;
            MessageRecentFriendsModel = processScopeModel.GetActivitySettingsAs<MessageRecentFriendsModel>();
            AccountModel = DominatorAccountModel;
            CheckJobProcessLimitsReached();
        }

        public override JobProcessResult PostScrapeProcess(ScrapeResultNew scrapeResult)
        {
            JobProcessResult jobProcessResult = new JobProcessResult();

            FacebookUser objFacebookUser = (FacebookUser)scrapeResult.ResultUser;

            try
            {
                if ((MessagePostPair.Count == 0 || UniqueAccountMessagePair.Count == 0) &&
                    (MessageRecentFriendsModel.IschkUniqueMessageChecked || MessageRecentFriendsModel.IschkUniqueMessageForUserChecked))
                {
                    LoadSentMessageFromDb();
                }

                JobCancellationTokenSource.Token.ThrowIfCancellationRequested();


                KeyValuePair<string, string> message = MessageRecentFriendsModel.IschkUniqueMessageChecked || MessageRecentFriendsModel.IschkUniqueMessageForUserChecked
                    ? GetUniqueMessageDetails(scrapeResult.QueryInfo, objFacebookUser)
                    : GetMessageDetails(objFacebookUser);

                if (!string.IsNullOrEmpty(message.Value) && !AccountModel.IsRunProcessThroughBrowser)
                    FdRequestLibrary.SendImageWithText(AccountModel, objFacebookUser.UserId, new List<string>() { message.Value });

                GlobusLogHelper.log.Info(Log.StartedActivity, DominatorAccountModel.AccountBaseModel.AccountNetwork, DominatorAccountModel.AccountBaseModel.UserName, ActivityType, objFacebookUser.UserId);

                //var isSuccess = false;
                var isSuccess = AccountModel.IsRunProcessThroughBrowser ?
                           FdLogInProcess._browserManager.SendMessage(AccountModel, objFacebookUser.UserId, message.Key, MessageRecentFriendsModel.IsMessageAsPreview,
                           scrapeResult.QueryInfo.QueryType, message.Value, openWindow: true)
                           : (MessageRecentFriendsModel.IsMessageAsPreview
                           ? FdRequestLibrary.SendTextMessageWithLinkPreview(AccountModel, objFacebookUser.UserId, message.Key)
                           : FdRequestLibrary.SendTextMessage(AccountModel, objFacebookUser.UserId, message.Key));

                if (isSuccess.IsMessageSent)
                {
                    GlobusLogHelper.log.Info(Log.ActivitySuccessful,
                        DominatorAccountModel.AccountBaseModel.AccountNetwork,
                        DominatorAccountModel.AccountBaseModel.UserName, ActivityType, objFacebookUser.UserId);
                    IncrementCounters();
                    AddSentMessageDataToDatabase(scrapeResult, message);

                    //SatartAfterAction(scrapeResult);
                    jobProcessResult.IsProcessSuceessfull = true;
                }
                else
                {
                    GlobusLogHelper.log.Info(Log.ActivityFailed, DominatorAccountModel.AccountBaseModel.AccountNetwork,
                        DominatorAccountModel.AccountBaseModel.UserName, ActivityType,
                        isSuccess.FbErrorDetails?.Description);

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



        private void AddSentMessageDataToDatabase(ScrapeResultNew scrapeResult, KeyValuePair<string, string> message)
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

                    DbCampaignService.Add(new CampaignInteractedUsres
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
                        ContactNo = user.ContactNo,
                        ProfilePic = user.ProfilePicUrl
                    });
                }

                DbAccountService.Add(new AccountInteractedUsres
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
                message = Regex.Replace(message, "<MYNAME>", AccountModel.AccountBaseModel.UserFullName);
                message = Regex.Replace(message, "<MYFIRSTNAME>", myFirstName);

                return message;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            return string.Empty;
        }

        private KeyValuePair<string, string> GetMessageDetails(FacebookUser objFacebookUser)
        {
            KeyValuePair<string, string> messageDetail = new KeyValuePair<string, string>();

            List<KeyValuePair<string, string>> lstMessage = new List<KeyValuePair<string, string>>();

            bool isMessageAdded = false;

            try
            {
                MessageRecentFriendsModel.LstDisplayManageMessageModel.ForEach(x =>
                {
                    if (!lstMessage.Any(z => z.Key == x.MessagesText && z.Value == x.MediaPath))
                    {
                        KeyValuePair<string, string> newMessage = new KeyValuePair<string, string>(x.MessagesText, x.MediaPath);
                        lstMessage.Add(newMessage);
                    }

                });


                foreach (var currentMessage in lstMessage)
                {
                    messageDetail = currentMessage;

                    var textMessage = currentMessage.Key;

                    var messages = new List<string>();

                    var messageText = string.Empty;

                    var messageCollection = new List<string>();

                    if (MessageRecentFriendsModel.IsTagChecked)
                        textMessage = ReplaceTagWithValue(objFacebookUser, textMessage);

                    if (MessageRecentFriendsModel.IsSpintaxChecked)
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
                            var fullMessage = string.IsNullOrEmpty(currentMessage.Value) ? message
                                : $"{message}:{currentMessage.Value}";

                            if (AccountMessagePair.Any(z =>
                                z.Key.Contains(message) && z.Key.Contains(currentMessage.Value)))
                            {
                            }
                            else
                            {
                                messageDetail = new KeyValuePair<string, string>(message, currentMessage.Value);

                                AccountMessagePair.Add($"{fullMessage}", AccountModel.AccountId);

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

        private KeyValuePair<string, string> GetUniqueMessageDetails(QueryInfo queryInfo, FacebookUser objFacebookUser)
        {
            KeyValuePair<string, string> messageDetail = new KeyValuePair<string, string>();

            List<KeyValuePair<string, string>> lstMessage = new List<KeyValuePair<string, string>>();

            try
            {
                MessageRecentFriendsModel.LstDisplayManageMessageModel.ForEach(x =>
                {
                    x.SelectedQuery.ForEach(y =>
                    {
                        if (y.Content.QueryType == queryInfo.QueryType && y.Content.QueryValue == queryInfo.QueryValue)
                        {
                            if (!lstMessage.Any(z => z.Key == x.MessagesText && z.Value == x.MediaPath))
                            {
                                KeyValuePair<string, string> newMessage = new KeyValuePair<string, string>(x.MessagesText, x.MediaPath);
                                lstMessage.Add(newMessage);
                            }
                        }
                    });
                });

                messageDetail = GetUniqueMessage(lstMessage, objFacebookUser);

                return messageDetail;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            return messageDetail;
        }


        public KeyValuePair<string, string> GetUniqueMessage(List<KeyValuePair<string, string>> lstMessage, FacebookUser objFacebookUser)
        {
            KeyValuePair<string, string> messageDetail = new KeyValuePair<string, string>();


            var jobActivityConfigurationManager = InstanceProvider.GetInstance<IJobActivityConfigurationManager>();

            var modulesetting = jobActivityConfigurationManager[DominatorAccountModel.AccountId, ActivityType];


            foreach (var currentMessage in lstMessage)
            {
                messageDetail = currentMessage;

                var textMessage = currentMessage.Key;

                var messageCollection = new List<string>();

                var message = string.Empty;

                if (MessageRecentFriendsModel.IsTagChecked)
                {
                    textMessage = ReplaceTagWithValue(objFacebookUser, textMessage);
                }

                if (MessageRecentFriendsModel.IsSpintaxChecked)
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
                        if (modulesetting.IsTemplateMadeByCampaignMode && MessageRecentFriendsModel.IschkUniqueMessageChecked)
                        {
                            lock (MessageLock)
                            {
                                if (!UniqueAccountMessagePair.Keys.Contains($"{message}"))
                                {
                                    UniqueAccountMessagePair.Add($"{message}:{currentMessage.Value}", $"{message}:{currentMessage.Value}");
                                }
                                else
                                {
                                    continue;
                                }
                            }
                        }

                        if (MessageRecentFriendsModel.IschkUniqueMessageForUserChecked)
                        {
                            lock (MessageLock)
                            {
                                if (!MessagePostPair.Keys.Contains($"{message}_{objFacebookUser.UserId}"))
                                {
                                    MessagePostPair.Add($"{message}:{currentMessage.Value}_{objFacebookUser.UserId}", $"{message}:{currentMessage.Value}_{objFacebookUser.UserId}");
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
            }

            return messageDetail;
        }

        private void LoadSentMessageFromDb()
        {
            List<DominatorHouseCore.DatabaseHandler.FdTables.Accounts.InteractedUsers> data = ObjDbCampaignService.GetAllInteractedData<DominatorHouseCore.DatabaseHandler.FdTables.Accounts.InteractedUsers>();

            data.ForEach(x =>
            {
                try
                {
                    var message = JsonConvert.DeserializeObject<KeyValuePair<string, string>>(x.DetailedUserInfo);

                    if (!UniqueAccountMessagePair.ContainsKey($"{message.Key}:{message.Value}"))
                    {
                        UniqueAccountMessagePair.Add($"{message.Key}:{message.Value}", $"{message.Key}:{message.Value}");
                    }
                    if (!MessagePostPair.ContainsKey($"{message.Key}:{message.Value}_{x.UserId}"))
                    {
                        MessagePostPair.Add($"{message.Key}:{message.Value}_{x.UserId}", $"{message.Key}:{message.Value}_{x.UserId}");
                    }

                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            });
        }

    }



}
