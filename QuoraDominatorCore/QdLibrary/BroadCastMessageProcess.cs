using DominatorHouseCore;
using DominatorHouseCore.DatabaseHandler.DHTables;
using DominatorHouseCore.DatabaseHandler.QdTables.Accounts;
using DominatorHouseCore.Enums;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using DominatorHouseCore.Process;
using DominatorHouseCore.Process.JobLimits;
using DominatorHouseCore.Utility;
using QuoraDominatorCore.Factories;
using QuoraDominatorCore.Models;
using QuoraDominatorCore.QDFactories;
using QuoraDominatorCore.QdLibrary.DAL;
using QuoraDominatorCore.QdLibrary.QdFunctions;
using QuoraDominatorCore.QdUtility;
using QuoraDominatorCore.Request;
using QuoraDominatorCore.Response;
using System;
using System.Collections.Generic;
using System.Linq;

namespace QuoraDominatorCore.QdLibrary
{
    internal class BroadCastMessageProcess : QdJobProcessInteracted<InteractedMessage>
    {
        private readonly AutoReplyToNewMessageModel _autoreplymodel;
        private readonly BroadcastMessagesModel _broadcastMessagesModel;
        private IQuoraBrowserManager _browser;
        private readonly IQdHttpHelper _httpHelper;
        private readonly SendMessageToFollowerModel _sendmessagemodel;
        public readonly List<InteractedMessage> _users;
        protected readonly IQuoraFunctions quoraFunct;
        private IQDBrowserManagerFactory managerFactory;
        public BroadCastMessageProcess(IProcessScopeModel processScopeModel,
            IDbAccountServiceScoped accountServiceScoped,
            IExecutionLimitsManager executionLimitsManager, IQuoraFunctions qdFunc,
            IQdQueryScraperFactory queryScraperFactory, IQdHttpHelper qdHttpHelper, IQdLogInProcess qdLogInProcess)
            : base(processScopeModel, accountServiceScoped, executionLimitsManager, queryScraperFactory, qdHttpHelper,
                qdLogInProcess)
        {
            _httpHelper = qdHttpHelper;
            quoraFunct = qdFunc;
            DbAccountService = accountServiceScoped;
            managerFactory = qdLogInProcess.managerFactory;

            _users = DbAccountService.GetInteractedMessage().ToList();
            if (processScopeModel.ActivityType == ActivityType.BroadcastMessages)
                _broadcastMessagesModel = processScopeModel.GetActivitySettingsAs<BroadcastMessagesModel>();
            else if (processScopeModel.ActivityType == ActivityType.AutoReplyToNewMessage)
                _autoreplymodel = processScopeModel.GetActivitySettingsAs<AutoReplyToNewMessageModel>();

            else if (processScopeModel.ActivityType == ActivityType.SendMessageToFollower)
                _sendmessagemodel = processScopeModel.GetActivitySettingsAs<SendMessageToFollowerModel>();
        }

        public override JobProcessResult PostScrapeProcess(ScrapeResultNew scrapeResult)
        {
            var jobProcessResult = new JobProcessResult();
            _browser = managerFactory.QdBrowserManager();
            try
            {
                JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                var quoraUser = (QuoraUser) scrapeResult.ResultUser;
                var messagedata = string.Empty;
                var IsSuccess = false;
                if (ActivityType == ActivityType.BroadcastMessages)
                {
                    GlobusLogHelper.log.Info(Log.CustomMessage,
                            DominatorAccountModel.AccountBaseModel.AccountNetwork,
                            DominatorAccountModel.AccountBaseModel.UserName, ActivityType,
                            $"Trying to Broadcast Message to {quoraUser.Username}.");
                    var messagess = _broadcastMessagesModel.LstDisplayManageMessageModel[0].MessagesText;
                    var url = $"{QdConstants.HomePageUrl}/profile/" + quoraUser.Username;
                    
                    if (_broadcastMessagesModel.IsSpintaxChecked)
                    {
                        messagess.ForEach(message =>{messagedata = " " + SpinTexHelper.GetSpinText(messagess) + " ";});
                        if (DominatorAccountModel.IsRunProcessThroughBrowser)
                        {
                            _browser.SearchByCustomUrl(DominatorAccountModel, url);
                            IsSuccess = _browser.BroadCastMessage(messagedata, DominatorAccountModel, ActivityType, quoraUser.Username);
                        }
                        else
                        {
                            var BasePostData = quoraFunct.GetBasePostData(url, DominatorAccountModel, string.Empty);
                            var BroadCastMessageResponse = quoraFunct.SendMessage(DominatorAccountModel, BasePostData, quoraFunct.GetUserId(url), messagedata, string.Empty, url);
                            IsSuccess = BroadCastMessageResponse.Success;
                        }
                        messagess = messagedata;
                    }
                    else
                    {
                        if (DominatorAccountModel.IsRunProcessThroughBrowser)
                        {
                            _browser.SearchByCustomUrl(DominatorAccountModel, url);
                            IsSuccess = _browser.BroadCastMessage(messagess, DominatorAccountModel, ActivityType, quoraUser.Username);
                        }
                        else
                        {
                            var BasePostData = quoraFunct.GetBasePostData(url, DominatorAccountModel, string.Empty);
                            var BroadCastMessageResponse = quoraFunct.SendMessage(DominatorAccountModel, BasePostData, quoraFunct.GetUserId(url), messagess, string.Empty, url);
                            IsSuccess = BroadCastMessageResponse.Success;
                        }
                    }
                    if (IsSuccess)
                    {
                        GlobusLogHelper.log.Info(Log.ActivitySuccessful,
                            DominatorAccountModel.AccountBaseModel.AccountNetwork,
                            DominatorAccountModel.AccountBaseModel.UserName, ActivityType,
                            $"Message has been sucessfully sent to {quoraUser.Username}.");
                        AddMessageDataToDataBase(scrapeResult, messagess);
                    }
                    else
                    {
                        GlobusLogHelper.log.Info(Log.ActivityFailed,
                            DominatorAccountModel.AccountBaseModel.AccountNetwork,
                            DominatorAccountModel.AccountBaseModel.UserName, ActivityType, $"Message option is not there for {quoraUser.Username}");
                        jobProcessResult.IsProcessSuceessfull = false;
                    }
                    JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                }
                else if (ActivityType == ActivityType.AutoReplyToNewMessage)
                {
                    GlobusLogHelper.log.Info(Log.CustomMessage,
                            DominatorAccountModel.AccountBaseModel.AccountNetwork,
                            DominatorAccountModel.AccountBaseModel.UserName, ActivityType,
                            $"Trying to AutoReplyToNewMessage: {quoraUser.Username}");
                    var autoReplyMessage = _autoreplymodel.Message;
                    if (DominatorAccountModel.IsRunProcessThroughBrowser)
                    {
                        _browser.SearchByCustomUrl(DominatorAccountModel, quoraUser.Url);
                        IsSuccess = _browser.BroadCastMessage(autoReplyMessage, DominatorAccountModel, ActivityType,
                            quoraUser.Username);
                    }
                    else
                    {
                        var url = $"{QdConstants.HomePageUrl}/profile/" + quoraUser.Username;
                        var BasePostData = quoraFunct.GetBasePostData(quoraUser.Url, DominatorAccountModel, string.Empty);
                        var BroadCastMessageResponse = quoraFunct.SendMessage(DominatorAccountModel, BasePostData,quoraFunct.GetUserId(url), autoReplyMessage, string.Empty,url,quoraUser?.Url?.Split('/').LastOrDefault(x=>x!=string.Empty));
                        IsSuccess = BroadCastMessageResponse.Success;
                    }
                    if (IsSuccess)
                    {
                        GlobusLogHelper.log.Info(Log.ActivitySuccessful,
                            DominatorAccountModel.AccountBaseModel.AccountNetwork,
                            DominatorAccountModel.AccountBaseModel.UserName, ActivityType,
                            $"Message has been sucessfully sent to {quoraUser.Username}.");
                        AddMessageDataToDataBase(scrapeResult, autoReplyMessage);
                    }
                    else
                    {
                        GlobusLogHelper.log.Info(Log.ActivityFailed,
                            DominatorAccountModel.AccountBaseModel.AccountNetwork,
                            DominatorAccountModel.AccountBaseModel.UserName, ActivityType, "");
                        jobProcessResult.IsProcessSuceessfull = false;
                    }
                }

                else
                {
                    var url = $"{QdConstants.HomePageUrl}/profile/" + quoraUser.Username;
                    var messagesToFollower = _sendmessagemodel.Message;
                    if (!DominatorAccountModel.IsRunProcessThroughBrowser)
                    {
                        var BasePostData = quoraFunct.GetBasePostData(url, DominatorAccountModel, string.Empty);
                        var BroadCastMessageResponse = quoraFunct.SendMessage(DominatorAccountModel, BasePostData, quoraFunct.GetUserId(url),messagesToFollower,string.Empty,url);
                        IsSuccess = BroadCastMessageResponse.Success;
                    }
                    else
                    {
                        _browser.SearchByCustomUrl(DominatorAccountModel, url);
                        IsSuccess = _browser.BroadCastMessage(messagesToFollower, DominatorAccountModel, ActivityType,
                            quoraUser.Username);
                    }
                    if (IsSuccess)
                    {
                        GlobusLogHelper.log.Info(Log.ActivitySuccessful,
                            DominatorAccountModel.AccountBaseModel.AccountNetwork,
                            DominatorAccountModel.AccountBaseModel.UserName, ActivityType,
                            $"Message has been sucessfully sent to {quoraUser.Username}.");
                        AddMessageDataToDataBase(scrapeResult, messagesToFollower);
                    }
                    else
                    {
                        GlobusLogHelper.log.Info(Log.ActivityFailed,
                            DominatorAccountModel.AccountBaseModel.AccountNetwork,
                            DominatorAccountModel.AccountBaseModel.UserName, ActivityType, "");
                        jobProcessResult.IsProcessSuceessfull = false;
                    }
                }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
            //finally { if (_browser != null && _browser.BrowserWindow != null) _browser.CloseBrowser(); }
            DelayBeforeNextActivity();
            return jobProcessResult;
        }


        public JobProcessResult PostScrapeProcessnew(ScrapeResultNew scrapeResult)
        {
            var jobProcessResult = new JobProcessResult();
            try
            {
                var quoraUser = (QuoraUser) scrapeResult.ResultUser;
                JobCancellationTokenSource.Token.ThrowIfCancellationRequested();


                try
                {
                    GlobusLogHelper.log.Info(Log.StartedActivity, DominatorAccountModel.AccountBaseModel.AccountNetwork,
                        DominatorAccountModel.AccountBaseModel.UserName, ActivityType,
                        scrapeResult.ResultUser.Username);

                    #region

                    JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                    var userPageResponse = _httpHelper.GetRequest($"{QdConstants.HomePageUrl}/profile/" + quoraUser.Username)
                        .Response;
                    var postcontent = new BasePostData(userPageResponse);
                    var parentCid = Utilities.GetBetween(userPageResponse, "ActionBarWeb\", \"", "\", \"").Trim();
                    var v = Utilities.GetBetween(userPageResponse,
                        "[\"unified_view/action_bar/base\", \"ActionBarWeb\", ", "}], [\"unified");
                    var jsInit = "{" + Utilities.GetBetween(v, "{", ", {").Replace(" ", "");
                    jsInit = Uri.EscapeDataString(jsInit);
                    var parentdom = Utilities.GetBetween(userPageResponse, "</span></span></div></div><div id='",
                        "><div class='icon_action_bar'");
                    var parentDomid = Utilities.GetBetween(parentdom, "id='", "'");
                    var postdata = "json=%7B%22args%22%3A%5B%5D%2C%22kwargs%22%3A%7B%7D%7D&revision=" +
                                   postcontent.Revision + "&formkey=" + postcontent.FormKey + "&postkey=" +
                                   postcontent.PostKey + "&window_id=" + postcontent.WindowId +
                                   "&referring_controller=user&referring_action=profile&parent_cid=" + parentCid +
                                   "&parent_domid=" + parentDomid +
                                   "&domids_to_remove=%5B%5D&__hmac=YlYPeMg3Y9i9%2Bv&__method=load_menu&__e2e_action_id=f4ienus80m&js_init=" +
                                   jsInit + "&__metadata=%7B%7D";


                    var checkCanMessage = _httpHelper
                        .PostRequest($"{QdConstants.HomePageUrl}/webnode2/server_call_POST?_h=YlYPeMg3Y9i9%2Bv&_m=load_menu",
                            postdata).Response;

                    JobCancellationTokenSource.Token.ThrowIfCancellationRequested();

                    if (checkCanMessage.Contains(">Message<") || checkCanMessage.Contains(">Messages<"))
                    {
                        IncrementCounters();
                        var newJsInit = Utilities.GetBetween(checkCanMessage, "ViewModalMessage", "{}]");
                        var newJsInit2 = Utilities.GetBetween(newJsInit, ", {", "}");
                        var orgjsInit = ("{" + newJsInit2 + "}").Replace("\\", "").Replace(" ", "");
                        orgjsInit = Uri.EscapeDataString(orgjsInit);

                        StartSendingMessages(scrapeResult, postcontent, orgjsInit, quoraUser, checkCanMessage);
                    }
                    else
                    {
                        GlobusLogHelper.log.Info(Log.ActivityFailed,
                            DominatorAccountModel.AccountBaseModel.AccountNetwork,
                            DominatorAccountModel.AccountBaseModel.UserName, ActivityType,
                            $"Message option is not there for {scrapeResult.ResultUser.Username}");
                    }

                    #endregion
                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                }

                DelayBeforeNextActivity();
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            return jobProcessResult;
        }

        public virtual void StartSendingMessages(ScrapeResultNew scrapeResult, BasePostData postcontent,
            string orgjsInit,
            QuoraUser quoraUser, string checkCanMessage = null)
        {
        }

        public virtual SendMessageResponseHandler SendMessages(DominatorAccountModel accountModel,
            BasePostData postcontent,
            string jsInit, string message)
        {
            return new SendMessageResponseHandler(null);
        }

        public void ShowSucessLogAndSaveToDb(ScrapeResultNew scrapeResult, string message, QuoraUser quoraUser,
            SendMessageResponseHandler result)
        {
            if (result.Success)
            {
                GlobusLogHelper.log.Info(Log.ActivitySuccessful, DominatorAccountModel.AccountBaseModel.AccountNetwork,
                    DominatorAccountModel.AccountBaseModel.UserName, ActivityType,
                    $"Message has been sucessfully sent to {quoraUser.Username}.");
                AddMessageDataToDataBase(scrapeResult, message);
            }
        }

        private void AddMessageDataToDataBase(ScrapeResultNew scrapeResult, string message)
        {
            try
            {
                var quorauser = (QuoraUser) scrapeResult.ResultUser;

                #region Add to AccountDB

                var objAccount =
                    new InteractedMessage
                    {
                        ActivityType = ActivityType.ToString(),
                        SinAccUsername = DominatorAccountModel.AccountBaseModel.UserName,
                        InteractionDate = DateTime.Now,
                        Message = message,
                        Username = quorauser.Username,
                        InteractionTimeStamp = GetEpochTime()
                    };
                if (ActivityType == ActivityType.BroadcastMessages)
                {
                    objAccount.QueryType = scrapeResult.QueryInfo.QueryType;
                    objAccount.QueryValue = scrapeResult.QueryInfo.QueryValue;
                }

                DbAccountService.Add(objAccount);
                _users.Add(objAccount);

                #endregion

                #region Add to CampaignDb

                var objCampaigns =
                    new DominatorHouseCore.DatabaseHandler.QdTables.Campaigns.InteractedMessage
                    {
                        ActivityType = ActivityType.ToString(),
                        SinAccUsername = DominatorAccountModel.AccountBaseModel.UserName,
                        InteractionDate = DateTime.Now,
                        Message = message,
                        Username = quorauser.Username,
                        InteractionTimeStamp = GetEpochTime()
                    };
                if (ActivityType == ActivityType.BroadcastMessages)
                {
                    objCampaigns.QueryType = scrapeResult.QueryInfo.QueryType;
                    objCampaigns.QueryValue = scrapeResult.QueryInfo.QueryValue;
                }

                IDbCampaignService dbCampaignService = new DbCampaignService(CampaignId);
                dbCampaignService.Add(objCampaigns);

                #endregion

                #region Add to PrivateBlacklistDB 

                if (ActivityType == ActivityType.BroadcastMessages &&
                    _broadcastMessagesModel.IsChkBroadCastPrivateBlacklist
                    || ActivityType == ActivityType.SendMessageToFollower &&
                    _sendmessagemodel.IschkSendMessagePrivateBlacklist
                    || ActivityType == ActivityType.AutoReplyToNewMessage &&
                    _autoreplymodel.IsChkAutoReplyPrivateBlacklist)
                    DbAccountService.Add(
                        new PrivateBlacklist
                        {
                            UserName = quorauser.Username,
                            UserId = quorauser.UserId,
                            InteractionTimeStamp = DateTime.Now.GetCurrentEpochTime()
                        });

                #endregion

                #region Add to GroupBlacklist DB

                if (_broadcastMessagesModel != null &&
                    _broadcastMessagesModel.IsChkBroadCastGroupBlacklist || _autoreplymodel != null &&
                                                                         _autoreplymodel.IsChkAutoReplyGroupBlacklist
                                                                         || _sendmessagemodel != null &&
                                                                         _sendmessagemodel
                                                                             .IschkSendMessageGroupBlacklist)
                {
                    IDbGlobalService dbGlobalService = new DbGlobalService();
                    dbGlobalService.Add(new BlackListUser
                    {
                        UserName = quorauser.Username,
                        UserId = quorauser.UserId,
                        AddedDateTime = DateTime.Now
                    });
                }

                #endregion
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        public virtual void AddToPrivateBlacklistDB(QuoraUser quorauser, string message)
        {
        }

        public virtual void AddToGroupBlacklistDB(QuoraUser quorauser, string message)
        {
        }

        public override void StartOtherConfiguration(ScrapeResultNew scrapeResult)
        {
            JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
        }

        public void CheckForNewMessage()
        {
        }

        public void SendOtherConfigMessage(ScrapeResultNew scrapeResult, List<string> lstMessage)
        {
            var quoraUser = (QuoraUser) scrapeResult.ResultUser;

            try
            {
                GlobusLogHelper.log.Info(Log.StartedActivity, DominatorAccountModel.AccountBaseModel.AccountNetwork,
                    DominatorAccountModel.AccountBaseModel.UserName, "Other Configuration",
                    scrapeResult.ResultUser.Username);

                var postdata =
                    "json=%7B%22args%22%3A%5B%5D%2C%22kwargs%22%3A%7B%7D%7D&revision=2632e70624cb9a1d5d661d281246b09c3c449417&formkey=" +
                    quoraUser.Formkey + "&postkey=" + quoraUser.PostKey + "&window_id=" + quoraUser.WindowId +
                    "&referring_controller=user&referring_action=profile&parent_cid=hlBjci2&parent_domid=uSeGXb&domids_to_remove=%5B%5D&__vcon_json=%5B%22YlYPeMg3Y9i9%2Bv%22%5D&__vcon_method=load_menu&__e2e_action_id=f0623d22ak&js_init=%7B%22oid%22%3A" +
                    quoraUser.UserId +
                    "%2C%22is_sticky%22%3Afalse%2C%22sticky_offsets%22%3A%7B%22top%22%3A300%7D%2C%22lazy_loaded%22%3Atrue%2C%22sdvars%22%3A%22AAEAAFAlbvv%2BNdVzDYzQZVXcS4rWiTPj%2F6fQjy%2BIyMr463BXpRe1ABNmRPNfhq3kpJoEUrZY2Hak%5CnjR1Z3s3aD%2Bm48PvCg0fIRKXKsGQUvVqHU76cKqRIkWuJBvpJ05KXlYqmuLLiXVjw7o2bkCsAKHnV%5Cn61x7dDdOpAyk4GHAWx9IRJzG%2F1mlOwU5BZw2TqPBhaeLFQ%3D%3D%5Cn%22%7D&__metadata=%7B%7D";

                JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                var checkCanMessage = _httpHelper
                    .PostRequest($"{QdConstants.HomePageUrl}/webnode2/server_call_POST?_v=YlYPeMg3Y9i9%2Bv&_m=load_menu",
                        postdata).Response;
                JobCancellationTokenSource.Token.ThrowIfCancellationRequested();

                if (checkCanMessage.Contains(">Message<") || checkCanMessage.Contains(">Messages<"))
                    lstMessage.ForEach(message =>
                    {
                        JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                        var response = quoraFunct.SendMessage(DominatorAccountModel, quoraUser.Formkey,
                            quoraUser.PostKey, quoraUser.WindowId, quoraUser.UserId, message);
                        JobCancellationTokenSource.Token.ThrowIfCancellationRequested();

                        if (response.Success) AddMessageDataToDataBase(scrapeResult, message);
                    });
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error(Log.CustomMessage, DominatorAccountModel.AccountBaseModel.AccountNetwork,
                    DominatorAccountModel.AccountBaseModel.UserName, "Other Configuration", "Message sending fail.");
                ex.DebugLog();
            }
        }
    }
}