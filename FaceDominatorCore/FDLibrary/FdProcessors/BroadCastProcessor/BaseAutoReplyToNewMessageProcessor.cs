using CommonServiceLocator;
using DominatorHouseCore;
using DominatorHouseCore.Enums;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using DominatorHouseCore.Process;
using DominatorHouseCore.Utility;
using FaceDominatorCore.FDLibrary.DAL;
using FaceDominatorCore.FDLibrary.FdClassLibrary;
using FaceDominatorCore.FDLibrary.FdFunctions;
using FaceDominatorCore.FDLibrary.FdFunctions.FdBrowserManager;
using FaceDominatorCore.FDLibrary.FdProcesses;
using FaceDominatorCore.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using Unity;

namespace FaceDominatorCore.FDLibrary.FdProcessors.BroadCastProcessor
{
    public class BaseAutoReplyToNewMessageProcessor : BaseFbProcessor
    {
        private readonly IAccountScopeFactory _accountScopeFactory;

        private IResponseHandler UserInfoResponseHandler;

        public BaseAutoReplyToNewMessageProcessor(IFdJobProcess jobProcess, IDbAccountServiceScoped dbAccountService,
            IDbCampaignServiceScoped campaignService, IFdRequestLibrary objFdRequestLibrary,
            IFdBrowserManager browserManager, IProcessScopeModel processScopeModel)
            : base(jobProcess, dbAccountService, campaignService, objFdRequestLibrary, browserManager, processScopeModel)
        {
            _accountScopeFactory = InstanceProvider.GetInstance<IAccountScopeFactory>();
        }

        protected override void Process(QueryInfo queryInfo, ref JobProcessResult jobProcessResult)
        {
        }

        protected void AppplyFilterAndStartFinalProcessForReply(ref JobProcessResult jobProcessResult, List<FdMessageDetails> lstUserIds, string query)
        {
            List<FdMessageDetails> lstFilteredId;
            JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
            if (IsAutoReplyFilterApplied())
            {
                lstFilteredId = ApplyAutoReplyFilter(lstUserIds);

                GlobusLogHelper.log.Info(Log.FilterApplied, AccountModel.AccountBaseModel.AccountNetwork, AccountModel.AccountBaseModel.UserName, _ActivityType, lstFilteredId.Count);
            }
            else
                lstFilteredId = lstUserIds;

            foreach (var messageDetail in lstFilteredId)
            {
                try
                {
                    JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();

                    var facebookUser = new FacebookUser
                    {
                        UserId = string.IsNullOrEmpty(messageDetail.OtherUserFbid)
                        ? messageDetail.MessageSenderId
                        : messageDetail.OtherUserFbid,
                        ProfileId = messageDetail.MessageSenderId,
                        Familyname = messageDetail.MessageSenderName,
                        ScrapedProfileUrl = messageDetail.ProfileUrl,
                    };

                    if (AccountModel.IsRunProcessThroughBrowser)
                    {
                        var userSpecificWindow = _accountScopeFactory[$"{AccountModel.AccountId}{facebookUser}"]
                              .Resolve<IFdBrowserManager>();

                        UserInfoResponseHandler = userSpecificWindow.GetFullUserDetails(AccountModel, facebookUser);

                        userSpecificWindow.CloseBrowser(AccountModel);

                        //messageDetail.Message = Browsermanager.GetLastUserMessage(AccountModel, facebookUser, messageDetail.IsOldUi);

                        //if (string.IsNullOrEmpty(messageDetail.Message))
                        //    continue;

                    }
                    else
                        UserInfoResponseHandler = ObjFdRequestLibrary.GetDetailedInfoUserMobileScraper
                                (facebookUser, AccountModel, true, false);

                    JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();

                    FilterData(ref jobProcessResult,
                        messageDetail, UserInfoResponseHandler.ObjFdScraperResponseParameters.FacebookUser, query);

                    if (JobProcess.IsStopped())
                        break;

                }
                catch (OperationCanceledException)
                {
                    throw;
                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                }
            }
        }


        private void FilterData(ref JobProcessResult jobProcessResult, FdMessageDetails message, FacebookUser objFacebookUser, string query
            )
        {
            JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();

            if (_ActivityType == ActivityType.Unfriend)
            {
                if (JobProcess.ModuleSetting.UnfriendOptionModel.IsFilterApplied)
                {
                    DateTime interactionDate;

                    if (DateTime.TryParse(objFacebookUser.InteractionDate, out interactionDate)
                        && _ActivityType == ActivityType.Unfriend)
                    {
                        var hours = (DateTime.Now - interactionDate).Hours;
                        var days = (DateTime.Now - interactionDate).Days;

                        if (days < JobProcess.ModuleSetting.UnfriendOptionModel.DaysBefore)
                        {
                            JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                            GlobusLogHelper.log.Info(Log.CustomMessage, AccountModel.AccountBaseModel.AccountNetwork, AccountModel.AccountBaseModel.UserName, _ActivityType, $"User with url {FdConstants.FbHomeUrl}{objFacebookUser.UserId} dosent match with filter condition");
                            return;
                        }
                        else if (days == JobProcess.ModuleSetting.UnfriendOptionModel.DaysBefore && hours < JobProcess.ModuleSetting.UnfriendOptionModel.HoursBefore)
                        {
                            JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                            GlobusLogHelper.log.Info(Log.CustomMessage, AccountModel.AccountBaseModel.AccountNetwork, AccountModel.AccountBaseModel.UserName, _ActivityType, $"User with url {FdConstants.FbHomeUrl}{objFacebookUser.UserId} dosent match with filter condition");
                            return;
                        }
                    }
                }
            }

            JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();


            if (JobProcess.ModuleSetting.GenderAndLocationFilter.IsFilterByGender)
            {
                //selected all and User Should not be neither male nor female
                if (JobProcess.ModuleSetting.GenderAndLocationFilter.SelectMaleUser
                    && JobProcess.ModuleSetting.GenderAndLocationFilter.SelectFemaleUser
                    && (objFacebookUser.Gender != Gender.Male.ToString() && objFacebookUser.Gender != Gender.Female.ToString()))
                {
                    JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                    GlobusLogHelper.log.Info(Log.CustomMessage, AccountModel.AccountBaseModel.AccountNetwork, AccountModel.AccountBaseModel.UserName, _ActivityType, $"User with profile url {FdConstants.FbHomeUrl}{objFacebookUser.UserId} dosent match with filter condition.");
                    return;
                }
                else if (!(JobProcess.ModuleSetting.GenderAndLocationFilter.SelectMaleUser
                    && JobProcess.ModuleSetting.GenderAndLocationFilter.SelectFemaleUser))
                {
                    //selected Male And Came User To be Female
                    if (JobProcess.ModuleSetting.GenderAndLocationFilter.SelectMaleUser
                    && objFacebookUser.Gender != Gender.Male.ToString())
                    {
                        JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                        GlobusLogHelper.log.Info(Log.CustomMessage, AccountModel.AccountBaseModel.AccountNetwork, AccountModel.AccountBaseModel.UserName, _ActivityType, $"User with profile url {FdConstants.FbHomeUrl}{objFacebookUser.UserId} dosent match with filter condition.");
                        return;
                    }
                    //selected Female and came user is not be Female
                    else if (JobProcess.ModuleSetting.GenderAndLocationFilter.SelectFemaleUser
                        && objFacebookUser.Gender != Gender.Female.ToString())
                    {
                        JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                        GlobusLogHelper.log.Info(Log.CustomMessage, AccountModel.AccountBaseModel.AccountNetwork, AccountModel.AccountBaseModel.UserName, _ActivityType, $"User with profile url {FdConstants.FbHomeUrl}{objFacebookUser.UserId} dosent match with filter condition.");
                        return;
                    }
                }
            }

            JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();

            if (JobProcess.ModuleSetting.GenderAndLocationFilter.IsLocationFilterChecked)
            {
                if (!JobProcess.ModuleSetting.GenderAndLocationFilter.ListLocationUrl.Any(x =>
                        System.Text.RegularExpressions.Regex.IsMatch(x, $@"\b{objFacebookUser.Currentcity.Replace(",", " ").Split(' ').FirstOrDefault()}\b", System.Text.RegularExpressions.RegexOptions.IgnoreCase) ||
                        System.Text.RegularExpressions.Regex.IsMatch(x, $@"\b{objFacebookUser.Hometown.Replace(",", " ").Split(' ').FirstOrDefault()}\b", System.Text.RegularExpressions.RegexOptions.IgnoreCase)))
                {
                    JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                    GlobusLogHelper.log.Info(Log.CustomMessage, AccountModel.AccountBaseModel.AccountNetwork, AccountModel.AccountBaseModel.UserName, _ActivityType, $"User with url {FdConstants.FbHomeUrl}{objFacebookUser.UserId} dosent match with filter condition");
                    return;
                }
            }

            JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();


            if (JobProcess.ModuleSetting.GenderAndLocationFilter.IsMutualFriendFilterChecked &&
                _ActivityType == ActivityType.IncommingFriendRequest)
            {
                var mutualFriendList = ObjFdRequestLibrary.GetAllMutualFriends(AccountModel, objFacebookUser.UserId);

                if (JobProcess.ModuleSetting.GenderAndLocationFilter.IsNoOfMutualFriend)
                {
                    if (mutualFriendList.Count <= JobProcess.ModuleSetting.GenderAndLocationFilter.TotalNoOfMutualFriend)
                    {
                        JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                        GlobusLogHelper.log.Info(Log.CustomMessage, AccountModel.AccountBaseModel.AccountNetwork, AccountModel.AccountBaseModel.UserName, _ActivityType, $"User with url {FdConstants.FbHomeUrl}{objFacebookUser.UserId} dosent match with filter condition");
                        return;
                    }
                }
                if (JobProcess.ModuleSetting.GenderAndLocationFilter.IsFriendOfFriend)
                {
                    if (mutualFriendList.FirstOrDefault
                        (x => JobProcess.ModuleSetting.GenderAndLocationFilter.ListFriends.Any(y => x.UserId == y)) == null)
                    {
                        JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                        GlobusLogHelper.log.Info(Log.CustomMessage, AccountModel.AccountBaseModel.AccountNetwork, AccountModel.AccountBaseModel.UserName, _ActivityType, $"User with url {FdConstants.FbHomeUrl}{objFacebookUser.UserId} dosent match with filter condition");
                        return;
                    }
                }
            }

            JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();

            FilterAndStartFinalProcessForEachUserAutoReply(ref jobProcessResult, message, objFacebookUser, query);

        }

        protected void FilterAndStartFinalProcessForEachUserAutoReply(ref JobProcessResult jobProcessResult, FdMessageDetails message, FacebookUser objFacebookUser, string query)
        {
            try
            {
                JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();

                QueryInfo objQuery = new QueryInfo { QueryType = query, QueryValue = message.MesageType.ToString() };

                objFacebookUser.UserId = message.MessageSenderId;
                objFacebookUser.Username = message.MessageSenderName;
                objFacebookUser.Username = string.IsNullOrEmpty(objFacebookUser.Username) ? objFacebookUser.Familyname : message.MessageSenderName;
                objFacebookUser.OtherDetails = message.Message;

                jobProcessResult = JobProcess.FinalProcess(new ScrapeResultNew()
                {
                    ResultUser = objFacebookUser,
                    QueryInfo = objQuery
                });
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
                jobProcessResult = new JobProcessResult();
            }
        }



        private List<FdMessageDetails> ApplyAutoReplyFilter(List<FdMessageDetails> message)
        {
            List<FdMessageDetails> lstFilteredMessage = new List<FdMessageDetails>();

            foreach (var post in message)
            {
                if (JobProcess.ModuleSetting.AutoReplyOptionModel.IsFilterApplied)
                {
                    var hours = (DateTime.Now - post.InteractionDate).Hours;
                    var days = (DateTime.Now - post.InteractionDate).Days;

                    if (days < JobProcess.ModuleSetting.AutoReplyOptionModel.DaysBefore)
                        continue;
                    else if (days == JobProcess.ModuleSetting.AutoReplyOptionModel.DaysBefore && hours <= JobProcess.ModuleSetting.AutoReplyOptionModel.HoursBefore)
                        continue;

                }

                if (JobProcess.ModuleSetting.AutoReplyOptionModel.IsFilterByIncommingMessageText)
                    if (!JobProcess.ModuleSetting.LstMessage.Any(x => post.Message.Contains(x)))
                        continue;

                lstFilteredMessage.Add(post);
            }

            return lstFilteredMessage;
        }

        private bool IsAutoReplyFilterApplied()
        {
            try
            {
                JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                return JobProcess.ModuleSetting.AutoReplyOptionModel.IsFilterApplied ||
                       JobProcess.ModuleSetting.AutoReplyOptionModel.IsFilterByIncommingMessageText;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
            return false;
        }
    }
}
