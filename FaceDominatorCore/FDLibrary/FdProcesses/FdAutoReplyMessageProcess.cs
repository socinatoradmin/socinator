using CommonServiceLocator;
using DominatorHouseCore;
using DominatorHouseCore.Interfaces;
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
    public class FdAutoReplyMessageProcess : FdJobProcessInteracted<AccountInteractedUsres>
    {
        public AutoReplyMessageModel AutoReplyMessageModel { get; set; }

        private Dictionary<string, string> AccountMessagePair { get; } = new Dictionary<string, string>();

        private static Dictionary<string, string> MessagePostPair { get; } = new Dictionary<string, string>();

        public DominatorAccountModel Account { get; set; }

        private static readonly object MessageLock = new object();

        public IFdRequestLibrary FdRequestLibrary { get; set; }

        public FdAutoReplyMessageProcess(IProcessScopeModel processScopeModel,
            IDbAccountServiceScoped accountServiceScoped, IFdQueryScraperFactory queryScraperFactory,
            IFdHttpHelper fdHttpHelper, IFdLoginProcess fdLogInProcess, IFdRequestLibrary fdRequestLibrary,
            IDbCampaignServiceScoped dbCampaignServiceScoped)
            : base(processScopeModel, accountServiceScoped, queryScraperFactory, fdHttpHelper, fdLogInProcess, dbCampaignServiceScoped)
        {
            FdRequestLibrary = fdRequestLibrary;
            AutoReplyMessageModel = processScopeModel.GetActivitySettingsAs<AutoReplyMessageModel>();
            AccountModel = DominatorAccountModel;
            // TemplateId = template;

        }

        public override JobProcessResult PostScrapeProcess(ScrapeResultNew scrapeResult)
        {
            JobProcessResult jobProcessResult = new JobProcessResult();

            FacebookUser objFacebookUser = (FacebookUser)scrapeResult.ResultUser;


            try
            {
                JobCancellationTokenSource.Token.ThrowIfCancellationRequested();

                var jobActivityConfigurationManager = InstanceProvider.GetInstance<IJobActivityConfigurationManager>();

                var modulesetting = jobActivityConfigurationManager[DominatorAccountModel.AccountId, ActivityType];

                if (modulesetting == null)
                    return jobProcessResult;

                KeyValuePair<string, string> message;

                if (AutoReplyMessageModel.IschkUniqueMessageChecked || AutoReplyMessageModel.IschkUniqueMessageForUserChecked)
                    message = GetUniqueMessageDetails(scrapeResult.QueryInfo, objFacebookUser);
                else
                    message = GetMessageDetails(scrapeResult.QueryInfo, objFacebookUser);

                if (!string.IsNullOrEmpty(message.Value) && !AccountModel.IsRunProcessThroughBrowser)
                    FdRequestLibrary.SendImageWithText(AccountModel, objFacebookUser.UserId, new List<string>() { message.Value });


                //var isSuccess = false;
                var isSuccess = AccountModel.IsRunProcessThroughBrowser ?
                           FdLogInProcess._browserManager.SendMessage(AccountModel, objFacebookUser.UserId, message.Key, AutoReplyMessageModel.IsMessageAsPreview,
                           scrapeResult.QueryInfo.QueryType, message.Value, openWindow: true) : (AutoReplyMessageModel.IsMessageAsPreview ? FdRequestLibrary.SendTextMessageWithLinkPreview(AccountModel, objFacebookUser.UserId, message.Key)
                           : FdRequestLibrary.SendTextMessage(AccountModel, objFacebookUser.UserId, message.Key));

                if (isSuccess.IsMessageSent)
                {
                    GlobusLogHelper.log.Info(Log.ActivitySuccessful, DominatorAccountModel.AccountBaseModel.AccountNetwork, DominatorAccountModel.AccountBaseModel.UserName, ActivityType, objFacebookUser.UserId);
                    IncrementCounters();
                    AddSendRequestDataToDatabase(scrapeResult, message);
                    jobProcessResult.IsProcessSuceessfull = true;
                }
                else
                {
                    GlobusLogHelper.log.Info(Log.ActivityFailed, DominatorAccountModel.AccountBaseModel.AccountNetwork, DominatorAccountModel.AccountBaseModel.UserName, ActivityType, objFacebookUser.UserId, "");
                    jobProcessResult.IsProcessSuceessfull = false;
                }
                DelayBeforeNextActivity();
            }

            catch (OperationCanceledException)
            {
                throw new OperationCanceledException();
            }
            catch (Exception ex) { ex.DebugLog(); }

            return jobProcessResult;
        }

        private KeyValuePair<string, string> GetMessageDetails(QueryInfo queryInfo, FacebookUser objFacebookUser)
        {
            KeyValuePair<string, string> messageDetail = new KeyValuePair<string, string>();

            List<KeyValuePair<string, string>> lstMessage = new List<KeyValuePair<string, string>>();

            bool isMessageAdded = false;

            try
            {
                AutoReplyMessageModel.LstDisplayManageMessageModel.ForEach(x =>
                {
                      x.SelectedQuery.ForEach(y =>
                      {
                          if (y.Content.QueryType == queryInfo.QueryType)
                          {
                              if (!lstMessage.Any(z => z.Key == x.MessagesText && z.Value == x.MediaPath))
                              {
                                  KeyValuePair<string, string> newMessage = new KeyValuePair<string, string>(x.MessagesText, x.MediaPath);
                                  lstMessage.Add(newMessage);
                              }
                          }
                      });
                });



                if (AccountMessagePair.Count >= lstMessage.Count)
                    AccountMessagePair.Clear();

                foreach (var currentMessage in lstMessage)
                {
                    messageDetail = currentMessage;

                    var textMessage = currentMessage.Key;

                    var messageCollection = new List<string>();

                    var messages = new List<string>();

                    var messageText = string.Empty;

                    if (AutoReplyMessageModel.IsTagChecked)
                        textMessage = ReplaceTagWithValue(objFacebookUser, textMessage);

                    if (AutoReplyMessageModel.IsSpintaxChecked)
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
                                z.Key.Contains(currentMessage.Key) && z.Key.Contains(currentMessage.Value)))
                            {
                            }
                            else
                            {
                                messageDetail = new KeyValuePair<string, string>(message, currentMessage.Value);

                                AccountMessagePair.Add($"{currentMessage.Key}:{currentMessage.Value}", AccountModel.AccountId);

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
                        break;
                }



                return messageDetail;

            }
            catch (Exception ex) { ex.DebugLog(); }

            return messageDetail;
        }


        private KeyValuePair<string, string> GetUniqueMessageDetails(QueryInfo queryInfo, FacebookUser objFacebookUser)
        {
            KeyValuePair<string, string> messageDetail = new KeyValuePair<string, string>();

            List<KeyValuePair<string, string>> lstMessage = new List<KeyValuePair<string, string>>();

            try
            {
                AutoReplyMessageModel.LstDisplayManageMessageModel.ForEach(x =>
                {
                    x.SelectedQuery.ForEach(y =>
                    {
                        if (y.Content.QueryType == queryInfo.QueryType)
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

            var fdCampaignInteractionDetails = InstanceProvider.GetInstance<ICampaignInteractionDetails>();

            var jobActivityConfigurationManager = InstanceProvider.GetInstance<IJobActivityConfigurationManager>();

            var modulesetting = jobActivityConfigurationManager[DominatorAccountModel.AccountId, ActivityType];


            foreach (var currentMessage in lstMessage)
            {
                messageDetail = currentMessage;

                var textMessage = currentMessage.Key;

                var messageCollection = new List<string>();

                var message = string.Empty;

                if (AutoReplyMessageModel.IsTagChecked)
                    textMessage = ReplaceTagWithValue(objFacebookUser, textMessage);

                if (AutoReplyMessageModel.IsSpintaxChecked)
                    messageCollection = SpinTexHelper.GetSpinMessageCollection(textMessage);
                else
                    messageCollection.Add(currentMessage.Key);


                foreach (var messageText in messageCollection)
                {
                    try
                    {
                        message = messageText.Trim();

                        if (modulesetting.IsTemplateMadeByCampaignMode && AutoReplyMessageModel.IschkUniqueMessageChecked)
                        {
                            try
                            {
                                fdCampaignInteractionDetails.AddInteractedData(SocialNetworks, CampaignId, message);
                            }
                            catch (Exception)
                            {
                                continue;

                            }
                        }

                        if (AutoReplyMessageModel.IschkUniqueMessageForUserChecked)
                        {
                            lock (MessageLock)
                            {
                                if (!MessagePostPair.Keys.Contains($"{message}_{objFacebookUser.UserId}"))
                                {
                                    MessagePostPair.Add($"{message}_{objFacebookUser.UserId}", $"{message}_{objFacebookUser.UserId}");
                                }
                                else
                                {
                                    continue;
                                }
                            }
                        }

                        messageDetail = new KeyValuePair<string, string>(message, currentMessage.Value);

                        AccountMessagePair.Add($"{message}:{currentMessage.Value}", AccountModel.AccountId);

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

        public override void StartOtherConfiguration(ScrapeResultNew scrapeResult)
        {
            try
            {

                base.StartOtherConfiguration(scrapeResult);

                var fdRequestLibrary = InstanceProvider.GetInstance<IFdRequestLibrary>();

                //FdRequestLibrary fdRequestLibrary = new FdRequestLibrary();

                fdRequestLibrary.ChangeLanguage(AccountModel, FdConstants.AccountLanguage[AccountModel.AccountBaseModel.UserId]);

            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }


        private void AddSendRequestDataToDatabase(ScrapeResultNew scrapeResult, KeyValuePair<string, string> message)
        {
            try
            {

                FacebookUser user = (FacebookUser)scrapeResult.ResultUser;
                var jobActivityConfigurationManager = InstanceProvider.GetInstance<IJobActivityConfigurationManager>();

                var moduleConfiguration = jobActivityConfigurationManager[DominatorAccountModel.AccountId, ActivityType];
                if (moduleConfiguration != null && moduleConfiguration.IsTemplateMadeByCampaignMode)
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
                        Gender = user.Gender,
                        University = user.University,
                        Workplace = user.WorkPlace,
                        CurrentCity = user.Currentcity,
                        HomeTown = user.Hometown,
                        BirthDate = user.DateOfBirth,
                        ContactNo = user.ContactNo,
                        ProfilePic = user.ProfilePicUrl,
                        InteractionTimeStamp = DateTimeUtilities.GetEpochTime(),
                        InteractionDateTime = DateTime.Now,
                        DetailedUserInfo = JsonConvert.SerializeObject(message)

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


    }
}
