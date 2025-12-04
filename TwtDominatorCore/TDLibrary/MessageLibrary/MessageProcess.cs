using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text.RegularExpressions;
using DominatorHouseCore;
using DominatorHouseCore.DatabaseHandler.TdTables.Accounts;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using DominatorHouseCore.Process;
using DominatorHouseCore.Process.JobLimits;
using DominatorHouseCore.Utility;
using TwtDominatorCore.Database;
using TwtDominatorCore.Factories;
using TwtDominatorCore.Requests;
using TwtDominatorCore.Response;
using TwtDominatorCore.TDFactories;
using TwtDominatorCore.TDLibrary.GeneralLibrary.DAL;
using TwtDominatorCore.TDModels;
using TwtDominatorCore.TDUtility;

namespace TwtDominatorCore.TDLibrary
{
    internal class MessageProcess : TdJobProcessInteracted<InteractedUsers>
    {
        #region Construction

        public MessageProcess(IProcessScopeModel processScopeModel,
            IDbAccountServiceScoped accountServiceScoped, IDbGlobalService globalService,
            IExecutionLimitsManager executionLimitsManager, ITwitterFunctionFactory twitterFunctionFactory,
            IJobActivityConfigurationManager jobActivityConfigurationManager,
            ITdQueryScraperFactory queryScraperFactory, ITdHttpHelper tdHttpHelper, ITwtLogInProcess twtLogInProcess,
            IDbInsertionHelper dbInsertionHelper, IDbCampaignService campaignService)
            : base(processScopeModel, accountServiceScoped, globalService, executionLimitsManager, queryScraperFactory,
                tdHttpHelper, twtLogInProcess)
        {
            _twtFuncFactory = twitterFunctionFactory;
            _jobActivityConfigurationManager = jobActivityConfigurationManager;
            _dbInsertionHelper = dbInsertionHelper;
            _campaignService = campaignService;
            MessageModel = processScopeModel.GetActivitySettingsAs<MessageModel>();
        }

        #endregion

        #region Public Fields

        public MessageModel MessageModel { get; set; }

        #endregion

        // changement in to assign functions factory beacuse of NewUi error to slove this we use this way.
        public ITwitterFunctions _twtFunc => _twtFuncFactory.TwitterFunctions;

        #region Public Methods

        public override JobProcessResult PostScrapeProcess(ScrapeResultNew scrapeResult)
        {
            var jobProcessResult = new JobProcessResult();
            try
            {
                var lstAllMessages = new NameValueCollection();

                JobCancellationTokenSource.Token.ThrowIfCancellationRequested();

                #region getting message wrt specific module

                if (ActivityType.ToString() == "AutoReplyToNewMessage" ||
                    ActivityType.ToString() == "SendMessageToFollower")
                {
                    lstAllMessages = GetSpecificMessage(scrapeResult, false);

                    // if we don't get any message as per query we go for default messages
                    if (lstAllMessages.Count == 0) lstAllMessages = GetSpecificMessage(scrapeResult, true);
                }
                else if (ActivityType.ToString() == "BroadcastMessages")
                {
                    var isCustomUser = false;
                    try
                    {
                        if (MessageModel.MessageSetting.CustomFollowersList != null)
                            isCustomUser = MessageModel.MessageSetting.CustomFollowersList.Any(x =>
                                x.ToString().Equals(scrapeResult.ResultUser.Username,
                                    StringComparison.CurrentCultureIgnoreCase)||(!string.IsNullOrEmpty(x)&&x.Contains(scrapeResult.ResultUser.Username)));
                        else if (MessageModel.MessageSetting.CustomFollowersList != null && !string.IsNullOrEmpty(MessageModel.MessageSetting.CustomFollowers.Trim()))
                            isCustomUser = MessageModel.MessageSetting.CustomFollowers.Trim().ToLower()
                                .Equals(scrapeResult.ResultUser.Username.ToLower());
                    }
                    catch (Exception ex)
                    {
                        ex.DebugLog();
                    }

                    lstAllMessages = GetMessageForRandom(isCustomUser);

                    if (lstAllMessages.Count == 0) lstAllMessages = GetSpecificMessage(scrapeResult, true);
                }
                else
                {
                    MessageModel.LstDisplayManageMessageModel.ForEach(x =>
                    {
                        lstAllMessages.Add(x.MessagesText, x.MediaPath);
                    });
                }

                #endregion

                #region setting message

                var index = new Random().Next(0, lstAllMessages.Count);
                var messageText = lstAllMessages.Count == 0 ? "" : lstAllMessages.GetKey(index);
                string currentMessage;

                if (string.IsNullOrEmpty(messageText) &&
                    (lstAllMessages.Count == 0 || string.IsNullOrEmpty(lstAllMessages[index])))
                {
                    GlobusLogHelper.log.Info(Log.CustomMessage, DominatorAccountModel.AccountBaseModel.AccountNetwork,
                        DominatorAccountModel.UserName, ActivityType,
                        $"Not Found suitable Message reply for User {scrapeResult.ResultUser.Username}");

                    jobProcessResult.IsProcessSuceessfull = false;

                    return jobProcessResult;
                }

                if (messageText.Contains("<End>"))
                {
                    var listMessage = Regex.Split(messageText, "<End>").ToList();

                    listMessage.Shuffle();

                    currentMessage = listMessage.FirstOrDefault()?.Trim();

                    if (string.IsNullOrEmpty(currentMessage)) currentMessage = listMessage[1].Trim();
                }
                else
                {
                    currentMessage = messageText.Trim();
                }

                #endregion

                var twitterUser = (TwitterUser) scrapeResult.ResultUser;
                JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                DirectMessageResponseHandler messageResponse;

                #region  sending message with or without media

                var imagePath = lstAllMessages[index]?.Split(',')?.ToList()?.OrderBy(x => Guid.NewGuid())?.ToList()?.FirstOrDefault();

                if (MessageModel.IsSpintax && !string.IsNullOrEmpty(currentMessage))
                    currentMessage = TdUtility.GetSpinTaxMessage(currentMessage);
                if (MessageModel.IsTagChecked && !string.IsNullOrEmpty(currentMessage))
                    currentMessage = ReplaceTagWithValue(twitterUser, currentMessage);

                if (string.IsNullOrEmpty(imagePath))
                    messageResponse
                        =
                        _twtFunc.SendDirectMessage(DominatorAccountModel, twitterUser.UserId, currentMessage,
                            twitterUser.Username);
                else
                    messageResponse = _twtFunc.SendDirectMessage(DominatorAccountModel, twitterUser.UserId,
                        currentMessage, twitterUser.Username, imagePath);

                #endregion

                if (messageResponse.Success)
                {
                    IncrementCounters();

                    #region  GetModuleSetting

                    var moduleModeSetting =
                        _jobActivityConfigurationManager[DominatorAccountModel.AccountId, ActivityType];

                    if (moduleModeSetting == null) return jobProcessResult;

                    #endregion

                    _dbInsertionHelper.AddInteractedUserDetailsInAccountDb(twitterUser, ActivityType.ToString(),
                        ActivityType.ToString(), scrapeResult, currentMessage, imagePath);

                    if (moduleModeSetting.IsTemplateMadeByCampaignMode)
                        _dbInsertionHelper.AddInteractedUserDetailsInCampaignDb(twitterUser, ActivityType.ToString(),
                            scrapeResult, currentMessage, imagePath);

                    _dbInsertionHelper.UpdateMessageStatusOfFriendship(twitterUser.UserId);
                    jobProcessResult.IsProcessSuceessfull = true;
                    JobCancellationTokenSource.Token.ThrowIfCancellationRequested();

                    GlobusLogHelper.log.Info(Log.ActivitySuccessful,
                        DominatorAccountModel.AccountBaseModel.AccountNetwork, DominatorAccountModel.UserName,
                        ActivityType, TdUtility.GetProfileUrl(twitterUser.Username));
                }
                else
                {
                    GlobusLogHelper.log.Info(Log.ActivityFailed, DominatorAccountModel.AccountBaseModel.AccountNetwork,
                        DominatorAccountModel.UserName, ActivityType.ToString(), TdUtility.GetProfileUrl(twitterUser.Username) + " ==> "+ messageResponse.Issue?.Message);
                    jobProcessResult.IsProcessSuceessfull = false;
                }

                JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                DelayBeforeNextActivity();
            }
            catch (OperationCanceledException)
            {
                throw new OperationCanceledException("Operation Cancelled!");
            }
            catch (Exception ex)
            {
                if (DominatorAccountModel?.AccountBaseModel?.UserName != null)
                    ex.DebugLog(
                        $"TwtDominator : [Account: {DominatorAccountModel.AccountBaseModel.UserName}] => Error : {ex.Message}. StackTrace => {ex.StackTrace}  (Module => {ActivityType.ToString()})");
            }

            return jobProcessResult;
        }

        #endregion

        #region Private Fields

        //private readonly ITwitterFunctions _twtFunc;
        private readonly ITwitterFunctionFactory _twtFuncFactory;
        private readonly IJobActivityConfigurationManager _jobActivityConfigurationManager;
        private readonly IDbInsertionHelper _dbInsertionHelper;
        private readonly IDbCampaignService _campaignService;

        #endregion

        public string ReplaceTagWithValue(TwitterUser _twitterUser, string message)
        {
            try
            {
                var splitUserName = Regex.Split(_twitterUser.FullName, " ");
                var myFirstName = Regex.Split(DominatorAccountModel.AccountBaseModel.UserFullName, " ")[0];
                var firstName = splitUserName[0];
                var lastName = splitUserName.Length > 2 ? splitUserName[2] : (splitUserName.Length > 1 ? splitUserName[1] : string.Empty);
                message = Regex.Replace(message, "<FULLNAME>", _twitterUser.FullName);
                message = Regex.Replace(message, "<FIRSTNAME>", firstName);
                message = Regex.Replace(message, "<LASTNAME>", lastName);
                //if (string.IsNullOrEmpty(DominatorAccountModel.AccountBaseModel.UserFullName))
                //{
                //    FdFunctions.FdFunctions objFdFunctions = new FdFunctions.FdFunctions();
                //    //FdRequestLibrary fdRequestLibrary = new FdRequestLibrary();
                //    var fdRequestLibrary = InstanceProvider.GetInstance<IFdRequestLibrary>();
                //    FacebookUser objOwnDetails = new FacebookUser()
                //    {
                //        UserId = AccountModel.AccountBaseModel.UserId
                //    };
                //    var userFullDetails =
                //        fdRequestLibrary.GetDetailedInfoUserMobileScraper(objOwnDetails, AccountModel, true, true);

                //    objFdFunctions.UpdateAccountInfoToModel(AccountModel, userFullDetails.ObjFdScraperResponseParameters.FacebookUser);
                //}
                message = Regex.Replace(message, "<MYNAME>", DominatorAccountModel.AccountBaseModel.UserFullName);
                message = Regex.Replace(message, "<MYFIRSTNAME>", myFirstName);

                return message;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            return string.Empty;
        }

        #region Private Methods

        private NameValueCollection GetSpecificMessage(ScrapeResultNew scrapeResult, bool isDefault)
        {
            var lstAllMessages = new NameValueCollection();

            var interactedUsersMessageList =
                new List<DominatorHouseCore.DatabaseHandler.TdTables.Campaign.InteractedUsers>();
            // var interactedUsersMessageList1 = new System.Collections.Generic.List<DominatorHouseCore.DatabaseHandler.TdTables.Accounts.InteractedUsers>();

            try
            {
                var twtUser = scrapeResult.ResultUser as TwitterUser;

                if (MessageModel.IsCampaignWiseUniqueChecked)
                    interactedUsersMessageList = _campaignService.GetAllInteractedUsers().Where(x =>
                            x.InteractedUsername.Equals(scrapeResult.ResultUser.Username,
                                StringComparison.OrdinalIgnoreCase))
                        .ToList();

                foreach (var message in MessageModel.LstDisplayManageMessageModel)
                {
                    #region unique region

                    try
                    {
                        if (interactedUsersMessageList.Any(temp =>
                            temp.DirectMessage == message?.MessagesText &&
                            temp.MediaPath.Equals(message?.MediaPath)))
                            continue;
                    }
                    catch (Exception exception)
                    {
                        exception.DebugLog();
                    }

                    #endregion

                    if (lstAllMessages.GetValues(message.MessagesText) == null || !lstAllMessages
                            .GetValues(message.MessagesText).Any(x => x == message.MediaPath))
                        message.SelectedQuery.Select(x => x.Content.QueryValue).ToList().ForEach(y =>
                        {
                            if (isDefault)
                            {
                                if (y.ToLower() == "Default".ToLower())
                                    lstAllMessages.Add(message.MessagesText == null ? "" : message.MessagesText,
                                        message.MediaPath);
                            }
                            else if (StringHandler.IsContainsIgnoreCase(twtUser.Message, y))
                            {
                                lstAllMessages.Add(message.MessagesText == null ? "" : message.MessagesText,
                                    message.MediaPath);
                            }
                        });
                }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            return lstAllMessages;
        }

        private NameValueCollection GetMessageForRandom(bool isCustomUser)
        {
            var lstAllMessages = new NameValueCollection();
            try
            {
                MessageModel.LstDisplayManageMessageModel.ForEach(message =>
                {
                    if (lstAllMessages.GetValues(message.MessagesText) == null || !lstAllMessages
                            .GetValues(message.MessagesText).Any(x => x.Equals(message.MediaPath)))
                        message.SelectedQuery.Select(x => x.Content.QueryValue).ToList().ForEach(y =>
                        {
                            if (isCustomUser && MessageModel.MessageSetting.IsChkCustomFollowers)
                            {
                                if (StringHandler.IsEqualsIgnoreCase(y, "Custom"))
                                    lstAllMessages.Add(message.MessagesText == null ? "" : message.MessagesText,
                                        message.MediaPath);
                            }
                            else if (MessageModel.MessageSetting.IsChkRandomFollowers &&
                                     StringHandler.IsEqualsIgnoreCase("Random Follower", y))
                            {
                                var messages = message.MessagesText == null ? "" : message.MessagesText;
                                lstAllMessages.Add(messages, message.MediaPath);
                            }
                        });
                });
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            return lstAllMessages;
        }

        #endregion
    }
}