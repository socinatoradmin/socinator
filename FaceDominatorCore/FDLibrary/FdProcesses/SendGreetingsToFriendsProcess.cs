using CommonServiceLocator;
using DominatorHouseCore;
using DominatorHouseCore.DatabaseHandler.FdTables.Campaigns;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using DominatorHouseCore.Models.Publisher.CampaignsAdvanceSetting;
using DominatorHouseCore.Models.SocioPublisher;
using DominatorHouseCore.Process;
using DominatorHouseCore.Request;
using DominatorHouseCore.Utility;
using FaceDominatorCore.FDFactories;
using FaceDominatorCore.FDLibrary.DAL;
using FaceDominatorCore.FDLibrary.FdClassLibrary;
using FaceDominatorCore.FDLibrary.FdFunctions;
using FaceDominatorCore.FDModel.MessageModel;
using FaceDominatorCore.FDRequest;
using FaceDominatorCore.FDResponse.MessagesResponse;
using FaceDominatorCore.FDResponse.Publisher;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.RegularExpressions;
using AccountInteractedUsres = DominatorHouseCore.DatabaseHandler.FdTables.Accounts.InteractedUsers;
using CampaignInteractedUsres = DominatorHouseCore.DatabaseHandler.FdTables.Campaigns.InteractedUsers;

namespace FaceDominatorCore.FDLibrary.FdProcesses
{

    public class SendGreetingsToFriendsProcess : FdJobProcessInteracted<AccountInteractedUsres>
    {
        public SendGreetingsToFriendsModel SendGreetingsToFriendsModel { get; set; }

        public Dictionary<string, string> AccountMessagePair { get; set; } = new Dictionary<string, string>();

        private static Dictionary<string, string> MessagePostPair { get; } = new Dictionary<string, string>();

        //private static Dictionary<string, string> UniqueAccountMessagePair => _uniqueAccountMessagePair;

        //private static Dictionary<string, bool> UniqueMessageAvailability => _uniqueMessageAvailability;

        public DominatorAccountModel Account { get; set; }

        private readonly IFdRequestLibrary _fdRequestLibrary;

        private static readonly object MessageLock = new object();
        private static readonly Dictionary<string, string> UniqueAccountMessagePair = new Dictionary<string, string>();
        private static readonly Dictionary<string, bool> UniqueMessageAvailability = new Dictionary<string, bool>();


        public SendGreetingsToFriendsProcess(IProcessScopeModel processScopeModel,
            IDbAccountServiceScoped accountServiceScoped, IFdQueryScraperFactory queryScraperFactory,
            IFdHttpHelper fdHttpHelper, IFdLoginProcess fdLogInProcess, IFdRequestLibrary fdRequestLibrary,
            IDbCampaignServiceScoped dbCampaignServiceScoped)
            : base(processScopeModel, accountServiceScoped, queryScraperFactory, fdHttpHelper, fdLogInProcess, dbCampaignServiceScoped)
        {
            _fdRequestLibrary = fdRequestLibrary;
            //TemplateId = template;
            SendGreetingsToFriendsModel = processScopeModel.GetActivitySettingsAs<SendGreetingsToFriendsModel>();
            if (!UniqueMessageAvailability.ContainsKey(AccountModel.AccountId))
                UniqueMessageAvailability.Add(AccountModel.AccountId, false);
            AccountModel = DominatorAccountModel;
        }

        public override JobProcessResult PostScrapeProcess(ScrapeResultNew scrapeResult)
        {
            string publishedMessage = string.Empty;

            JobProcessResult jobProcessResult = new JobProcessResult();

            var fdCampaignInteractionDetails = InstanceProvider.GetInstance<ICampaignInteractionDetails>();

            var jobActivityConfigurationManager = InstanceProvider.GetInstance<IJobActivityConfigurationManager>();

            var modulesetting = jobActivityConfigurationManager[DominatorAccountModel.AccountId, ActivityType];

            FacebookUser objFacebookUser = (FacebookUser)scrapeResult.ResultUser;

            if (AccountModel.IsRunProcessThroughBrowser)
                FdLogInProcess._browserManager.LoadPageSource(AccountModel, objFacebookUser.ProfileUrl);

            if (modulesetting.IsTemplateMadeByCampaignMode && SendGreetingsToFriendsModel.IschkUniqueRequestChecked)
            {
                try
                {
                    fdCampaignInteractionDetails.AddInteractedData(SocialNetworks, CampaignId, objFacebookUser.UserId);
                }
                catch (Exception)
                {
                    jobProcessResult.IsProcessSuceessfull = false;
                    return jobProcessResult;
                }
            }


            if (ActivityType.ToString() != "SendGreetingsToFriends" && (MessagePostPair.Count == 0 || UniqueAccountMessagePair.Count == 0) &&
                (SendGreetingsToFriendsModel.IschkUniqueMessageChecked || SendGreetingsToFriendsModel.IschkUniqueMessageForUserChecked))
            {
                LoadSentMessageFromDb();
            }

            try
            {

                JobCancellationTokenSource.Token.ThrowIfCancellationRequested();

                KeyValuePair<string, string> message;

                message = ActivityType.ToString() != "SendGreetingsToFriends" &&
                    (SendGreetingsToFriendsModel.IschkUniqueMessageChecked ||
                    SendGreetingsToFriendsModel.IschkUniqueMessageForUserChecked)
                        ? GetUniqueMessageDetails(scrapeResult.QueryInfo, objFacebookUser)
                        : GetMessageDetails(scrapeResult.QueryInfo, objFacebookUser);

                if (!string.IsNullOrEmpty(message.Value) && !AccountModel.IsRunProcessThroughBrowser)
                    _fdRequestLibrary.SendImageWithText(AccountModel, objFacebookUser.UserId, new List<string>() { message.Value });

                FdSendTextMessageResponseHandler isSuccess = null;

                if (!string.IsNullOrEmpty(message.Key))
                    isSuccess = AccountModel.IsRunProcessThroughBrowser
                             ? FdLogInProcess._browserManager.SendMessage(AccountModel, objFacebookUser.UserId, message.Key, SendGreetingsToFriendsModel.IsMessageAsPreview, scrapeResult.QueryInfo.QueryTypeEnum, message.Value, openWindow: true)
                             : (SendGreetingsToFriendsModel.IsMessageAsPreview
                             ? _fdRequestLibrary.SendTextMessageWithLinkPreview(AccountModel, objFacebookUser.UserId, message.Key)
                             : _fdRequestLibrary.SendTextMessage(AccountModel, objFacebookUser.UserId, message.Key));

                if (isSuccess != null && isSuccess.IsMessageSent)
                {
                    GlobusLogHelper.log.Info(Log.ActivitySuccessful,
                        DominatorAccountModel.AccountBaseModel.AccountNetwork,
                        DominatorAccountModel.AccountBaseModel.UserName, ActivityType, objFacebookUser.UserId);
                    IncrementCounters();

                    PublisherResponseHandler publisherDetails = null;
                    if (SendGreetingsToFriendsModel.IsPostToOwnWallChecked)
                        publisherDetails = StartAfterActionForGreetings(objFacebookUser, ref publishedMessage);
                    AddSendRequestDataToDatabase(scrapeResult, message, publishedMessage, publisherDetails);
                    jobProcessResult.IsProcessSuceessfull = true;
                }
                else if (string.IsNullOrEmpty(message.Key))
                {
                    GlobusLogHelper.log.Info(Log.ActivityFailed,
                                               DominatorAccountModel.AccountBaseModel.AccountNetwork,
                                               DominatorAccountModel.AccountBaseModel.UserName, ActivityType,
                                               isSuccess != null ? isSuccess.FbErrorDetails.Description : "LangKeyNoMessagesMatchwithCurrentQuery".FromResourceDictionary());
                    jobProcessResult.IsProcessSuceessfull = false;
                }
                else
                {
                    GlobusLogHelper.log.Info(Log.ActivityFailed,
                        DominatorAccountModel.AccountBaseModel.AccountNetwork,
                        DominatorAccountModel.AccountBaseModel.UserName, ActivityType,
                        isSuccess != null ? isSuccess.FbErrorDetails?.Description : "Unknown Error");
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

        private PublisherResponseHandler StartAfterActionForGreetings(FacebookUser objFacebookUser, ref
            string publishedMessage)
        {

            GlobusLogHelper.log.Info(Log.CustomMessage, DominatorAccountModel.AccountBaseModel.AccountNetwork,
                            DominatorAccountModel.AccountBaseModel.UserName, $"{ActivityType}_AfterAction", string.Format("LangKeyStartAfterAction".FromResourceDictionary(), $"{objFacebookUser.ProfileUrl}"));

            var lstPostDetails = SendGreetingsToFriendsModel.ListPostDetails;

            lstPostDetails.Shuffle();

            publishedMessage = SendGreetingsToFriendsModel.IsSpintaxForPostChechked ?
                SpinTexHelper.GetSpinMessageCollection(lstPostDetails.FirstOrDefault()).FirstOrDefault() :
                lstPostDetails.FirstOrDefault();

            ObservableCollection<string> collection = new ObservableCollection<string>();
            SendGreetingsToFriendsModel.FbMultiMediaModel.MediaPaths.ForEach(media => collection.Add(media.MediaPath));

            publishedMessage = SendGreetingsToFriendsModel.IsTagForPostChecked
                ? ReplaceTagWithValue(objFacebookUser, publishedMessage)
                : lstPostDetails.FirstOrDefault();

            string postUrl = !AccountModel.IsRunProcessThroughBrowser
                ? _fdRequestLibrary.PostToFriends(AccountModel, objFacebookUser.UserId,
                             new PublisherPostlistModel() { PostDescription = publishedMessage, MediaList = collection }, new System.Threading.CancellationTokenSource(),
                             new GeneralModel(), new FacebookModel()).ObjFdScraperResponseParameters.PostDetails.PostUrl
                : FdLogInProcess._browserManager.ShareToFriendProfiles(AccountModel, objFacebookUser.ProfileUrl, new PublisherPostlistModel() { PostDescription = publishedMessage, MediaList = collection }
                            , new System.Threading.CancellationTokenSource(), new GeneralModel(), new FacebookModel()).ObjFdScraperResponseParameters?.PostDetails?.PostUrl;

            GlobusLogHelper.log.Info(Log.CustomMessage, DominatorAccountModel.AccountBaseModel.AccountNetwork,
                       DominatorAccountModel.AccountBaseModel.UserName, $"{ActivityType}_AfterAction", !string.IsNullOrEmpty(postUrl)
                       ? $"{"LangKeySuccessfulTo".FromResourceDictionary()} {"LangKeyPost".FromResourceDictionary()} {objFacebookUser.ProfileUrl}"
                       : $"{"LangKeyFailedTo".FromResourceDictionary()} {"LangKeyPost".FromResourceDictionary()} {objFacebookUser.ProfileUrl}");

            return new PublisherResponseHandler(new ResponseParameter(), postUrl);
        }

        private void LoadSentMessageFromDb()
        {
            List<InteractedUsers> data = ObjDbCampaignService.GetAllInteractedData<InteractedUsers>();

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
                catch (OperationCanceledException)
                {
                    throw new OperationCanceledException();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            });
        }

        private void AddSendRequestDataToDatabase(ScrapeResultNew scrapeResult,
            KeyValuePair<string, string> message, string publishedMessage, PublisherResponseHandler responseHandler)
        {
            try
            {

                FacebookUser user = (FacebookUser)scrapeResult.ResultUser;

                var jobActivityConfigurationManager = InstanceProvider.GetInstance<IJobActivityConfigurationManager>();

                var demodulating = jobActivityConfigurationManager[DominatorAccountModel.AccountId, ActivityType];

                if (demodulating == null)
                    return;



                if (demodulating.IsTemplateMadeByCampaignMode)
                {
                    DbCampaignService.Add(new CampaignInteractedUsres
                    {
                        ActivityType = ActivityType.ToString(),
                        QueryType = scrapeResult.QueryInfo.QueryType,
                        QueryValue = scrapeResult.QueryInfo.QueryValue,
                        AccountEmail = DominatorAccountModel.AccountBaseModel.UserName,
                        UserId = user.UserId,
                        Username = user.Familyname,
                        UserProfileUrl = user.ScrapedProfileUrl,
                        InteractionTimeStamp = DateTimeUtilities.GetEpochTime(),
                        InteractionDateTime = DateTime.Now,
                        IsPublishedToWall = responseHandler != null && responseHandler.Status ? responseHandler.Status : false,
                        PublishedUrl = responseHandler != null && responseHandler.Status ? responseHandler.ObjFdScraperResponseParameters.PostDetails.PostUrl : string.Empty,
                        PostDescription = responseHandler != null && responseHandler.Status ? publishedMessage : string.Empty,
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
                    UserProfileUrl = user.ScrapedProfileUrl,
                    InteractionTimeStamp = DateTimeUtilities.GetEpochTime(),
                    InteractionDateTime = DateTime.Now,
                    IsPublishedToWall = responseHandler != null && responseHandler.Status ? responseHandler.Status : false,
                    PublishedUrl = responseHandler != null && responseHandler.Status ? responseHandler.ObjFdScraperResponseParameters.PostDetails.PostUrl : string.Empty,
                    PostDescription = responseHandler != null && responseHandler.Status ? publishedMessage : string.Empty,
                    DetailedUserInfo = JsonConvert.SerializeObject(message)
                });
            }
            catch (OperationCanceledException)
            {
                throw new OperationCanceledException();
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
            catch (OperationCanceledException)
            {
                throw new OperationCanceledException();
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            return string.Empty;
        }

        private KeyValuePair<string, string> GetMessageDetails(QueryInfo queryInfo, FacebookUser objFacebookUser)
        {
            KeyValuePair<string, string> messageDetail = new KeyValuePair<string, string>();

            List<KeyValuePair<string, string>> lstMessage = new List<KeyValuePair<string, string>>();

            bool isMessageAdded = false;

            try
            {
                SendGreetingsToFriendsModel.LstDisplayManageMessageModel.ForEach(x =>
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

                    if (SendGreetingsToFriendsModel.IsTagChecked)
                    {
                        textMessage = ReplaceTagWithValue(objFacebookUser, textMessage);
                    }

                    if (SendGreetingsToFriendsModel.IsSpintaxChecked)
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
            catch (OperationCanceledException)
            {
                throw new OperationCanceledException();
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
                SendGreetingsToFriendsModel.LstDisplayManageMessageModel.ForEach(x =>
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

                messageDetail = GetUniqueMessage(lstMessage, objFacebookUser);

                return messageDetail;
            }
            catch (OperationCanceledException)
            {
                throw new OperationCanceledException();
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
                var textMessage = currentMessage.Key;

                var messageCollection = new List<string>();

                var message = string.Empty;

                if (SendGreetingsToFriendsModel.IsTagChecked)
                {
                    textMessage = ReplaceTagWithValue(objFacebookUser, textMessage);
                }

                if (SendGreetingsToFriendsModel.IsSpintaxChecked)
                    messageCollection = SpinTexHelper.GetSpinMessageCollection(textMessage);
                else
                    messageCollection.Add(currentMessage.Key);

                if (AccountMessagePair.Count >= messageCollection.Count)
                    AccountMessagePair.Clear();

                foreach (var messageText in messageCollection)
                {
                    try
                    {
                        message = messageText.Trim();

                        if (modulesetting.IsTemplateMadeByCampaignMode && SendGreetingsToFriendsModel.IschkUniqueMessageChecked)
                        {
                            lock (MessageLock)
                            {
                                var fullMessage = string.IsNullOrEmpty(currentMessage.Value) ? message
                                    : $"{message}:{currentMessage.Value}";

                                if (!UniqueAccountMessagePair.Keys.Contains($"{message}"))
                                    UniqueAccountMessagePair.Add($"{fullMessage}", $"{AccountModel.AccountBaseModel.UserName}");
                                else
                                    continue;

                            }
                        }

                        if (modulesetting.IsTemplateMadeByCampaignMode && SendGreetingsToFriendsModel.IschkUniqueMessageForUserChecked)
                        {
                            lock (MessageLock)
                            {
                                var fullMessageForUser = string.IsNullOrEmpty(currentMessage.Value) ? $"{message}_{objFacebookUser.UserId}"
                                    : $"{message}:{currentMessage.Value}_{objFacebookUser.UserId}";

                                if (!MessagePostPair.Keys.Contains($"{fullMessageForUser}"))
                                    MessagePostPair.Add($"{fullMessageForUser}", $"{fullMessageForUser}");
                                else
                                    continue;

                            }
                        }

                        AccountMessagePair.Add($"{message}:{currentMessage.Value}", AccountModel.AccountId);

                        messageDetail = new KeyValuePair<string, string>(message, currentMessage.Value);

                        break;

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
            }

            return messageDetail;
        }



        public override void StartOtherConfiguration(ScrapeResultNew scrapeResult)
        {
            try
            {
                base.StartOtherConfiguration(scrapeResult);

                _fdRequestLibrary.ChangeLanguage(AccountModel, FdConstants.AccountLanguage[AccountModel.AccountBaseModel.UserId]);

            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }


    }
}
