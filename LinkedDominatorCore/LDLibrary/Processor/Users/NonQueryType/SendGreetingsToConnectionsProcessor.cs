using System;
using System.Collections.Generic;
using System.Linq;
using CommonServiceLocator;
using ThreadUtils;
using DominatorHouseCore;
using DominatorHouseCore.FileManagers;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using DominatorHouseCore.Process;
using DominatorHouseCore.Utility;
using LinkedDominatorCore.Factories;
using LinkedDominatorCore.LDLibrary.DAL;
using LinkedDominatorCore.LDModel;
using LinkedDominatorCore.LDModel.Messenger;
using Newtonsoft.Json;

namespace LinkedDominatorCore.LDLibrary.Processor.Users.NonQueryType
{
    internal class SendGreetingsToConnectionsProcessor : BaseLinkedinUserProcessor, IQueryProcessor
    {
        public SendGreetingsToConnectionsProcessor(ILdJobProcess jobProcess,
            IDbCampaignService campaignService, ILdFunctionFactory ldFunctionFactory, IDelayService delayService,
            IProcessScopeModel processScopeModel) :
            base(jobProcess, campaignService, ldFunctionFactory, delayService, processScopeModel)
        {
            var templatesFileManager = InstanceProvider.GetInstance<ITemplatesFileManager>();
            SendGreetingsToConnectionsModel = JsonConvert.DeserializeObject<SendGreetingsToConnectionsModel>(
                templatesFileManager.Get().FirstOrDefault(x => x.Id == LdJobProcess.TemplateId).ActivitySettings);
        }

        private SendGreetingsToConnectionsModel SendGreetingsToConnectionsModel { get; }

        protected override void Process(QueryInfo queryInfo, ref JobProcessResult jobProcessResult)
        {
            try
            {
                var Start = 0;
                var notificationType = "NEW";
                GlobusLogHelper.log.Info(Log.CustomMessage, DominatorAccountModel.AccountBaseModel.AccountNetwork,
                    DominatorAccountModel.AccountBaseModel.UserName, ActivityType,
                    "getting notifications to send greetings.");


                while (!jobProcessResult.IsProcessCompleted && !jobProcessResult.HasNoResult)
                {
                    //https://www.linkedin.com/voyager/api/identity/cards?count=40&q=notifications&start="
                    var notificationApi = IsBrowser
                        ? "https://www.linkedin.com/notifications/"
                        : $"https://www.linkedin.com/voyager/api/notifications/dash/cards?count=40&decorationId=com.linkedin.voyager.dash.deco.identity.notifications.CardsCollection-23&q=notifications&segmentUrn=urn%3Ali%3Afsd_notificationSegment%3A{notificationType}&start={Start}";
                    var objNotificationDetailsHandler = LdFunctions.GetNotificationDetails(notificationApi);

                    LdJobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                    RemoveNonCheckGreetings(objNotificationDetailsHandler.LstNotificationDetails);
                    if (objNotificationDetailsHandler.Success)
                    {
                        if (objNotificationDetailsHandler.LstNotificationDetails.Count > 0)
                        {
                            #region Remove Already Sent Greetings

                            try
                            {
                                var Lst_GreetingAlreadySent =
                                    DbAccountService.GetInteractedUsers(ActivityTypeString).ToList();
                                objNotificationDetailsHandler.LstNotificationDetails.RemoveAll(x =>
                                Lst_GreetingAlreadySent.Any(y=>
                                !IsBrowser?y.ProfileId==x.ProfileId:y.UserProfileUrl==x.ProfileUrl));
                            }
                            catch (Exception ex)
                            {
                                ex.DebugLog();
                            }

                            #endregion

                            #region Filter events(Notifications) for connections having no profile picture

                            if (SendGreetingsToConnectionsModel.LDUserFilterModel.IsCheckedFilterProfileImageCheckbox)
                            {
                                objNotificationDetailsHandler.LstNotificationDetails.RemoveAll(x =>
                                    x.ProfilePicUrl == null);
                                GlobusLogHelper.log.Info(Log.CustomMessage,
                                    DominatorAccountModel.AccountBaseModel.AccountNetwork,
                                    DominatorAccountModel.AccountBaseModel.UserName, ActivityType,
                                    "successfully filtered events for connections having no profile picture");
                            }

                            #endregion

                            #region Filter BlacklistedUsers

                            if (SendGreetingsToConnectionsModel.IsChkSkipBlackListedUser &&
                                (SendGreetingsToConnectionsModel.IsChkPrivateBlackList ||
                                 SendGreetingsToConnectionsModel.IsChkGroupBlackList))
                                FilterBlacklistedUsers(objNotificationDetailsHandler.LstNotificationDetails,
                                    SendGreetingsToConnectionsModel.IsChkPrivateBlackList,
                                    SendGreetingsToConnectionsModel.IsChkGroupBlackList);

                            #endregion

                            if (objNotificationDetailsHandler.LstNotificationDetails.Count > 0)
                                ProcessLinkedinUsersFromUserList(QueryInfo.NoQuery, ref jobProcessResult,
                                    objNotificationDetailsHandler.LstNotificationDetails);
                            else
                                GlobusLogHelper.log.Info(Log.CustomMessage,
                                    DominatorAccountModel.AccountBaseModel.AccountNetwork,
                                    DominatorAccountModel.AccountBaseModel.UserName, ActivityType,
                                    "sorry no results found navigating to next page...");
                        }
                        else if (objNotificationDetailsHandler.ToString().Contains("elements\":[]") ||
                                 IsBrowser && objNotificationDetailsHandler.LstNotificationDetails.Count == 0)
                        {
                            jobProcessResult.HasNoResult = true;
                            GlobusLogHelper.log.Info(Log.CustomMessage,
                                DominatorAccountModel.AccountBaseModel.AccountNetwork,
                                DominatorAccountModel.AccountBaseModel.UserName, ActivityType,
                                "Sorry! no new notifications found.");
                        }

                        #region Pagination

                        if (IsBrowser)
                            break;

                        if (!jobProcessResult.IsProcessCompleted && !jobProcessResult.HasNoResult)
                        {
                            Start = notificationType == "NEW" ? 0 : Start + 40;
                            notificationType = "EARLIER";
                        }

                        #endregion
                    }
                    else
                    {
                        jobProcessResult.HasNoResult = true;
                        GlobusLogHelper.log.Info(Log.CustomMessage,
                            DominatorAccountModel.AccountBaseModel.AccountNetwork,
                            DominatorAccountModel.AccountBaseModel.UserName, ActivityType,
                            "Sorry! no new notifications found.");
                    }
                }
            }
            catch (OperationCanceledException)
            {
                throw new OperationCanceledException("Operation Cancelled!");
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        private void RemoveNonCheckGreetings(List<LinkedinUser> listGreetings)
        {
            try
            {
                if (!SendGreetingsToConnectionsModel.IsCheckedBirthdayGreeting)
                    listGreetings.RemoveAll(x => x.NotificationType.Contains("Birthday Greeting"));
                if (!SendGreetingsToConnectionsModel.IsCheckedNewJobGreeting)
                    listGreetings.RemoveAll(x => x.NotificationType.Contains("New Job Greeting"));
                if (!SendGreetingsToConnectionsModel.IsCheckedWorkAnniversaryGreeting)
                    listGreetings.RemoveAll(x => x.NotificationType.Contains("Work Anniversary Greeting"));
            }
            catch (Exception exception)
            {
                exception.DebugLog();
            }
        }
    }
}