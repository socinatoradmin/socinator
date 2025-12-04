using DominatorHouseCore;
using DominatorHouseCore.Enums.FdQuery;
using DominatorHouseCore.Interfaces;
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

namespace FaceDominatorCore.FDLibrary.FdProcessors.InviterProcessor
{
    public class WpInviterForGroupMembers : BaseFbInviterProcessor
    {

        public WpInviterForGroupMembers(IFdJobProcess jobProcess, IDbAccountServiceScoped dbAccountService,
            IDbCampaignServiceScoped campaignService, IFdRequestLibrary objFdRequestLibrary,
            IFdBrowserManager browserManager, IProcessScopeModel processScopeModel)
            : base(jobProcess, dbAccountService, campaignService, objFdRequestLibrary, browserManager, processScopeModel)
        {

        }


        protected override void Process(QueryInfo queryInfo, ref JobProcessResult jobProcessResult)
        {
            JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();

            try
            {
                JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();

                var objListWatchPartyUrl = JobProcess.ModuleSetting.InviterDetailsModel.ListWatchPartyUrls;

                foreach (var watchPart in objListWatchPartyUrl)
                {
                    FacebookPostDetails objFacebookDetails = new FacebookPostDetails { Id = watchPart };

                    ObjFdRequestLibrary.GetPostDetailNew(AccountModel, objFacebookDetails, true);

                    if (objFacebookDetails.EntityType == FbEntityTypes.Group)
                        StartInviteForGroupMembers(queryInfo, ref jobProcessResult, objFacebookDetails);
                    else
                        jobProcessResult.HasNoResult = true;

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



        protected void StartInviteForGroupMembers(QueryInfo queryInfo, ref JobProcessResult jobProcessResult, IEntity objEntity)
        {
            string groupUrl = FdConstants.FbHomeUrl + ((FacebookPostDetails)objEntity).OwnerId;

            IResponseHandler objGroupMembersResponseHandler = null;

            try
            {
                while (!jobProcessResult.IsProcessCompleted && !jobProcessResult.HasNoResult)
                {
                    JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();

                    try
                    {

                        objGroupMembersResponseHandler = ObjFdRequestLibrary.GetGroupMembers
                                    (AccountModel, groupUrl, objGroupMembersResponseHandler);

                        JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();

                        if (objGroupMembersResponseHandler.Status)
                        {
                            List<FacebookUser> lstFacebookUser = objGroupMembersResponseHandler.ObjFdScraperResponseParameters.ListUser;

                            GlobusLogHelper.log.Info(Log.FoundXResults, AccountModel.AccountBaseModel.AccountNetwork,
                                AccountModel.AccountBaseModel.UserName, lstFacebookUser.Count, queryInfo.QueryType,
                                        queryInfo.QueryValue, _ActivityType);

                            ProcessDataForInviter(queryInfo, ref jobProcessResult, lstFacebookUser, objEntity);
                            jobProcessResult.maxId = objGroupMembersResponseHandler.PageletData;

                            jobProcessResult.HasNoResult = !objGroupMembersResponseHandler.HasMoreResults;

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
                    catch (Exception ex)
                    {
                        ex.DebugLog();
                    }
                }
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
}
