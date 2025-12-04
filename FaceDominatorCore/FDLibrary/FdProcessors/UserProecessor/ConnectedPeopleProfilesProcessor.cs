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

namespace FaceDominatorCore.FDLibrary.FdProcessors.UserProecessor
{
    public class ConnectedPeopleProfilesProcessor : BaseFbUserProcessor
    {
        public ConnectedPeopleProfilesProcessor(IFdJobProcess jobProcess, IDbAccountServiceScoped dbAccountService,
            IDbCampaignServiceScoped campaignService, IFdRequestLibrary objFdRequestLibrary, IFdBrowserManager browserManager,
            IProcessScopeModel processScopeModel)
            : base(jobProcess, dbAccountService, campaignService, objFdRequestLibrary, browserManager,
                processScopeModel)
        {
        }

        protected override void Process(QueryInfo queryInfo, ref JobProcessResult jobProcessResult)
        {
            if (AccountModel.IsRunProcessThroughBrowser)
                Browsermanager.OpenMessengerWindow(AccountModel);

            IResponseHandler responseHandler = null;

            while (!jobProcessResult.IsProcessCompleted && !jobProcessResult.HasNoResult)
            {
                try
                {
                    JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();

                    responseHandler = AccountModel.IsRunProcessThroughBrowser
                        ? Browsermanager.ScrollWindowAndGetUnRepliedMessages(AccountModel, MessageType.Inbox, 5, 0)
                        : ObjFdRequestLibrary.GetMessageRequestDetails(AccountModel, responseHandler, MessageType.Inbox);

                    if (responseHandler.Status)
                    {
                        List<FdMessageDetails> fdMessageDetails = responseHandler.ObjFdScraperResponseParameters
                            .MessageDetailsList;
                        List<FacebookUser> facebookUsers = new List<FacebookUser>();
                        GlobusLogHelper.log.Info(Log.FoundXResults, AccountModel.AccountBaseModel.AccountNetwork, AccountModel.AccountBaseModel.UserName, fdMessageDetails.Count, queryInfo.QueryType, "", _ActivityType);

                        JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                        if (JobProcess.ModuleSetting.GenderAndLocationFilter.IsTimeLimitChecked)
                        {
                            fdMessageDetails.ForEach(x =>
                            {
                                if (JobProcess.ModuleSetting.GenderAndLocationFilter.TimeLimitOfDays < (DateTime.Now - x.InteractionDate).Days)
                                    facebookUsers.Add(new FacebookUser()
                                    {
                                        UserId = (x.MessageSenderId == AccountModel.AccountBaseModel.UserId) ? x.MessageReceiverId : x.MessageSenderId,
                                        Username = (x.MessageSenderId == AccountModel.AccountBaseModel.UserId) ? x.MessageReceiverName : x.MessageSenderName,
                                        ProfileUrl = x.ProfileUrl
                                    });
                            });

                            GlobusLogHelper.log.Info(Log.CustomMessage, AccountModel.AccountBaseModel.AccountNetwork,
                                      AccountModel.AccountBaseModel.UserName, _ActivityType, string.Format("LangKeyTotalNoConversions".FromResourceDictionary(), $"{facebookUsers.Count}"));


                        }
                        else
                        {
                            fdMessageDetails.ForEach(x => facebookUsers.Add(new FacebookUser()
                            {
                                UserId = (x.MessageSenderId == AccountModel.AccountBaseModel.UserId) ? x.MessageReceiverId : x.MessageSenderId,
                                Username = (x.MessageSenderId == AccountModel.AccountBaseModel.UserId) ? x.MessageReceiverName : x.MessageSenderName,
                                ProfileUrl = x.ProfileUrl
                            }));
                        }

                        ProcessDataOfUsers(queryInfo, ref jobProcessResult, facebookUsers);

                        jobProcessResult.maxId = responseHandler.PageletData;
                        if (!responseHandler.HasMoreResults)
                        {
                            GlobusLogHelper.log.Info(Log.CustomMessage, AccountModel.AccountBaseModel.AccountNetwork,
                                AccountModel.AccountBaseModel.UserName, _ActivityType, "LangKeyNoMoreConversionsFound".FromResourceDictionary());
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
    }
}
