using DominatorHouseCore;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using DominatorHouseCore.Process;
using DominatorHouseCore.Utility;
using FaceDominatorCore.FDEnums;
using FaceDominatorCore.FDLibrary.DAL;
using FaceDominatorCore.FDLibrary.FdClassLibrary;
using FaceDominatorCore.FDLibrary.FdFunctions;
using FaceDominatorCore.FDLibrary.FdFunctions.FdBrowserManager;
using FaceDominatorCore.FDLibrary.FdProcesses;
using FaceDominatorCore.Interface;
using System;
using System.Collections.Generic;

namespace FaceDominatorCore.FDLibrary.FdProcessors.BroadCastProcessor
{
    public class AutoReplyToNewMessageProcessor : BaseAutoReplyToNewMessageProcessor
    {
        public AutoReplyToNewMessageProcessor(IFdJobProcess jobProcess, IDbAccountServiceScoped dbAccountService,
            IDbCampaignServiceScoped campaignService, IFdRequestLibrary objFdRequestLibrary,
            IFdBrowserManager browserManager, IProcessScopeModel processScopeModel)
            : base(jobProcess, dbAccountService, campaignService, objFdRequestLibrary, browserManager, processScopeModel)
        {
        }

        protected override void Process(QueryInfo queryInfo, ref JobProcessResult jobProcessResult)
        {
            JobProcessResult jobProcessResultReplyRequest = new JobProcessResult()
            {
                IsProcessCompleted = !JobProcess.ModuleSetting.AutoReplyOptionModel.IsMessageRequestChecked
            };

            JobProcessResult jobProcessResultReply = new JobProcessResult()
            {
                IsProcessCompleted = !JobProcess.ModuleSetting.AutoReplyOptionModel.IsFriendsMessageChecked
            };

            List<string> listOption = new List<string>()
                            {
                                "FilterAndStartFinalProcessForAutoReplyRequest",
                                "FilterAndStartFinalProcessForAutoReply"
                            };

            if (!JobProcess.ModuleSetting.AutoReplyOptionModel.IsMessageRequestChecked)
                listOption.Remove("FilterAndStartFinalProcessForAutoReplyRequest");
            else if (!JobProcess.ModuleSetting.AutoReplyOptionModel.IsFriendsMessageChecked)
                listOption.Remove("FilterAndStartFinalProcessForAutoReply");

            while (!jobProcessResult.IsProcessCompleted)
            {
                try
                {

                    JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();


                    string currentAction = listOption.GetRandomItem();

                    if (currentAction == "FilterAndStartFinalProcessForAutoReplyRequest"
                        && !jobProcessResultReplyRequest.IsProcessCompleted)
                    {
                        FilterAndStartFinalProcessForAutoReplyRequest(ref jobProcessResult, "Reply to Message Requests");
                        if (jobProcessResult.HasNoResult == true)
                        {
                            jobProcessResultReplyRequest.IsProcessCompleted = true;
                            jobProcessResult.IsProcessCompleted = false;
                            jobProcessResult.HasNoResult = false;
                            listOption.Remove("FilterAndStartFinalProcessForAutoReplyRequest");
                        }
                    }

                    JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();

                    if (currentAction == "FilterAndStartFinalProcessForAutoReply"
                        && !jobProcessResultReply.IsProcessCompleted)
                    {
                        FilterAndStartFinalProcessForAutoReply(ref jobProcessResult, "Reply to Connected Friends");
                        if (jobProcessResult.HasNoResult == true)
                        {
                            jobProcessResultReply.IsProcessCompleted = true;
                            jobProcessResult.IsProcessCompleted = false;
                            jobProcessResult.HasNoResult = false;
                            listOption.Remove("FilterAndStartFinalProcessForAutoReply");
                        }
                    }

                    if (jobProcessResultReply.IsProcessCompleted &&
                        jobProcessResultReplyRequest.IsProcessCompleted)
                    {
                        jobProcessResult.HasNoResult = true;
                        jobProcessResult.IsProcessCompleted = true;
                    }

                }
                catch (OperationCanceledException)
                {
                    throw new OperationCanceledException("Requested Cancelled !");
                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                }
            }
        }

        private void FilterAndStartFinalProcessForAutoReplyRequest(ref JobProcessResult jobProcessResult, string query)
        {
            JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();

            IResponseHandler IncommingMessageResponseHandler = null;

            while (!jobProcessResult.IsProcessCompleted && !jobProcessResult.HasNoResult)
            {
                try
                {
                    JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                    IncommingMessageResponseHandler = AccountModel.IsRunProcessThroughBrowser
                        ? /*Browsermanager.ScrollWindowAndGetUnRepliedMessages(AccountModel, MessageType.Pending, 5, 0)*/
                         Browsermanager.GetPendingUserChat(AccountModel).Result
                        : ObjFdRequestLibrary.GetMessageRequestDetails(AccountModel, IncommingMessageResponseHandler, MessageType.Pending);

                    if (IncommingMessageResponseHandler.Status)
                    {
                        List<FdMessageDetails> lstUserIds = IncommingMessageResponseHandler.ObjFdScraperResponseParameters
                            .MessageDetailsList;

                        GlobusLogHelper.log.Info(Log.FoundXResults, AccountModel.AccountBaseModel.AccountNetwork, AccountModel.AccountBaseModel.UserName, lstUserIds.Count, "Reply to Message Requests", "", _ActivityType);
                        JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                        AppplyFilterAndStartFinalProcessForReply(ref jobProcessResult, lstUserIds, query);
                        jobProcessResult.maxId = IncommingMessageResponseHandler.PageletData;
                        if (IncommingMessageResponseHandler.HasMoreResults == false)
                        {
                            GlobusLogHelper.log.Info(Log.CustomMessage, AccountModel.AccountBaseModel.AccountNetwork, AccountModel.AccountBaseModel.UserName, _ActivityType, "No more users available who sent message requests");
                            jobProcessResult.HasNoResult = true;
                        }
                    }
                    else
                    {
                        jobProcessResult.HasNoResult = true;
                        jobProcessResult.maxId = null;
                    }
                }
                catch (OperationCanceledException)
                {
                    throw;
                }
                catch (Exception)
                {
                    //ex.DebugLog();
                }
            }
        }

        private void FilterAndStartFinalProcessForAutoReply(ref JobProcessResult jobProcessResult, string query)
        {
            IResponseHandler responseHandler = null;

            while (!jobProcessResult.IsProcessCompleted && !jobProcessResult.HasNoResult)
            {
                try
                {
                    JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                    if (AccountModel.IsRunProcessThroughBrowser)
                        Browsermanager.LoadPageSource(AccountModel, $"{FdConstants.FbHomeUrl}messages/", true);
                    responseHandler = AccountModel.IsRunProcessThroughBrowser
                        ? Browsermanager.ScrollWindowAndGetUnRepliedMessages(AccountModel, MessageType.Inbox, 5, 0)
                        : ObjFdRequestLibrary.GetMessageRequestDetails(AccountModel, responseHandler, MessageType.Inbox);

                    if (responseHandler.Status)
                    {
                        List<FdMessageDetails> lstUserIds = responseHandler.ObjFdScraperResponseParameters
                            .MessageDetailsList;

                        GlobusLogHelper.log.Info(Log.FoundXResults, AccountModel.AccountBaseModel.AccountNetwork, AccountModel.AccountBaseModel.UserName, lstUserIds.Count, "Reply to Connected Friends", "", _ActivityType);

                        JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();

                        AppplyFilterAndStartFinalProcessForReply(ref jobProcessResult, lstUserIds, query);

                        jobProcessResult.maxId = responseHandler.PageletData;
                        if (responseHandler.HasMoreResults == false)
                        {
                            GlobusLogHelper.log.Info(Log.CustomMessage, AccountModel.AccountBaseModel.AccountNetwork, AccountModel.AccountBaseModel.UserName, _ActivityType, "No more friends available who sent messages");
                            jobProcessResult.HasNoResult = true;
                        }
                    }
                    else
                    {
                        jobProcessResult.HasNoResult = true;
                        jobProcessResult.maxId = null;
                    }
                }
                catch (OperationCanceledException)
                {
                    throw;
                }
                catch (Exception)
                {
                    //ex.DebugLog();
                }
            }
        }



        //private void FilterAndStartFinalProcessForAutoReplyFromPage(JobProcessResult jobProcessResult, string query)
        //{

        //    List<string> listOfOwnPages = GetOwnPages();

        //    foreach (var ownPage in listOfOwnPages)
        //    {
        //        string timeStampPrecise = null;

        //        string pageurl = ownPage;
        //        if (!pageurl.Contains(FdConstants.FbHomeUrl))
        //            pageurl = FdConstants.FbHomeUrl + pageurl;

        //        FanpageDetails fanpageDetails = ObjFdRequestLibrary.GetPageDetailsFromUrl(AccountModel, pageurl);

        //        while (!jobProcessResult.IsProcessCompleted && !jobProcessResult.HasNoResult)
        //        {
        //            try
        //            {
        //                var incommingPageMessageResponseHandler = ObjFdRequestLibrary.GetMessageDetailsFromPage
        //                    (AccountModel, ref timeStampPrecise, MessageType.Pending, fanpageDetails.FanPageID);

        //                if (incommingPageMessageResponseHandler.Success)
        //                {
        //                    List<FdMessageDetails> lstUserIds = incommingPageMessageResponseHandler.IncommingMessageList;


        //                    GlobusLogHelper.log.Info(Log.FoundXResults,
        //                        AccountModel.AccountBaseModel.AccountNetwork,
        //                        AccountModel.AccountBaseModel.UserName, lstUserIds.Count,
        //                        "Reply to Message Requests", "", ActivityType);

        //                    AppplyFilterAndStartFinalProcessForReplyFromPage(ref jobProcessResult, lstUserIds, query, fanpageDetails);

        //                    jobProcessResult.maxId = incommingPageMessageResponseHandler.PaginationTimestamp;
        //                    if (incommingPageMessageResponseHandler.HasMoreResults == false)
        //                    {
        //                        GlobusLogHelper.log.Info(Log.CustomMessage,
        //                            AccountModel.AccountBaseModel.AccountNetwork,
        //                            AccountModel.AccountBaseModel.UserName, ActivityType,
        //                            $"No more users available who sent message requests");
        //                        jobProcessResult.HasNoResult = true;
        //                    }
        //                }
        //                else
        //                {
        //                    GlobusLogHelper.log.Info(Log.NoMoreDataToPerform,
        //                        AccountModel.AccountBaseModel.AccountNetwork,
        //                        AccountModel.AccountBaseModel.UserName, ActivityType);
        //                    jobProcessResult.HasNoResult = true;
        //                    jobProcessResult.maxId = null;
        //                }

        //            }
        //            catch (OperationCanceledException ex)
        //            {
        //                ex.DebugLog();
        //                throw new OperationCanceledException("Requested Cancelled !");
        //            }
        //            catch (Exception ex)
        //            {
        //                ex.DebugLog();
        //            }
        //        }
        //    }

        //    if (!jobProcessResult.IsProcessCompleted)
        //    {
        //        GlobusLogHelper.log.Info(Log.NoMoreDataToPerform, AccountModel.AccountBaseModel.AccountNetwork,
        //            AccountModel.AccountBaseModel.UserName, ActivityType);
        //    }
        //}

        //private List<string> GetOwnPages()
        //{
        //    return Regex.Split(JobProcess.ModuleSetting.AutoReplyOptionModel.OwnPages, "\r\n").ToList();
        //}

        //private void AppplyFilterAndStartFinalProcessForReplyFromPage(ref JobProcessResult jobProcessResult, List<FdMessageDetails> lstUserIds, string query, FanpageDetails fanpageDetails)
        //{

        //    List<FdMessageDetails> lstFilteredId;

        //    if (IsAutoReplyFilterApplied())
        //    {
        //        lstFilteredId = ApplyAutoReplyFilter(lstUserIds);

        //        GlobusLogHelper.log.Info(Log.FilterApplied, AccountModel.AccountBaseModel.AccountNetwork,
        //            AccountModel.AccountBaseModel.UserName, ActivityType, lstFilteredId.Count);

        //    }
        //    else
        //    {
        //        lstFilteredId = lstUserIds;
        //    }


        //    foreach (var post in lstFilteredId)
        //    {
        //        try
        //        {
        //            var facebookUser = new FacebookUser();

        //            facebookUser.UserId = post.MessageSenderId;

        //            var userInfoResponseHandler = ObjFdRequestLibrary.GetDetailedInfoUserMobileScraper(facebookUser, AccountModel, true, false);

        //            if (IsMessageFilterApplied())
        //            {

        //            }

        //            FilterAndStartFinalProcessForEachUserAutoReplyFromPage(out jobProcessResult,
        //                post, fanpageDetails, query, userInfoResponseHandler.ObjFacebookUser);

        //            if (JobProcess.IsStopped())
        //                break;

        //        }
        //        catch (Exception ex)
        //        {
        //            ex.DebugLog();
        //        }
        //    }
        //}

        //private void FilterAndStartFinalProcessForEachUserAutoReplyFromPage(out JobProcessResult jobProcessResult, FdMessageDetails message, FanpageDetails fanpageDetails, string query, FacebookUser objFacebookUser)
        //{
        //    try
        //    {
        //        QueryInfo objQuery = new QueryInfo();

        //        objQuery.QueryType = query;
        //        objQuery.QueryValue = message.MesageType.ToString();

        //        objFacebookUser.UserId = message.MessageSenderId;
        //        objFacebookUser.Username = message.MessageSenderName;
        //        objFacebookUser.OtherDetails = message.Message;

        //        jobProcessResult = JobProcess.FinalProcess(new ScrapeResultNew()
        //        {
        //            ResultUser = objFacebookUser,
        //            QueryInfo = objQuery,
        //            ResultPage = fanpageDetails
        //        });
        //    }
        //    catch (Exception ex)
        //    {
        //        ex.DebugLog();
        //        jobProcessResult = new JobProcessResult();
        //    }
        //}
        //private bool IsMessageFilterApplied() => false;

    }
}
