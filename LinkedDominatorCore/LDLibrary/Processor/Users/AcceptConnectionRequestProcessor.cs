using System;
using ThreadUtils;
using DominatorHouseCore;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using DominatorHouseCore.Process;
using DominatorHouseCore.Utility;
using LinkedDominatorCore.Factories;
using LinkedDominatorCore.LDLibrary.DAL;
using LinkedDominatorCore.LDModel.GrowConnection;
using LinkedDominatorCore.LDUtility;

namespace LinkedDominatorCore.LDLibrary.Processor.Users
{
    public class AcceptConnectionRequestProcessor : BaseLinkedinUserProcessor, IQueryProcessor
    {
        public AcceptConnectionRequestProcessor(ILdJobProcess jobProcess,
            IDbCampaignService campaignService, ILdFunctionFactory ldFunctionFactory,
            IProcessScopeModel processScopeModel, IDelayService delayService) :
            base(jobProcess, campaignService, ldFunctionFactory, delayService, processScopeModel)
        {
            AcceptConnectionRequestModel = processScopeModel.GetActivitySettingsAs<AcceptConnectionRequestModel>();
        }

        public AcceptConnectionRequestModel AcceptConnectionRequestModel { get; set; }


        protected override void Process(QueryInfo queryInfo, ref JobProcessResult jobProcessResult)
        {
            try
            {
                var start = 0;

                #region MyRegion

                var tempQueryInfo = AcceptConnectionRequestModel.IsChkAllInvitations
                    ? new QueryInfo {QueryType = "Accept Invitations"}
                    : new QueryInfo {QueryType = "Ignore Invitations"};

                const string invitationApi =
                    "https://www.linkedin.com/voyager/api/relationships/invitationViews?q=receivedInvitation";


                while (!jobProcessResult.IsProcessCompleted && !jobProcessResult.HasNoResult)
                {
                    var actionUrl = IsBrowser && start > 0
                        ? ""
                        : $"{invitationApi}&count=40&start={start.ToString()}&nc={Utils.GenerateNc()}";

                    var objReceivedInvitationsResponseHandler = LdFunctions.SearchForLinkedinInvitations(actionUrl);
                    LdJobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                    if (objReceivedInvitationsResponseHandler == null)
                        jobProcessResult.HasNoResult = true;
                    if (objReceivedInvitationsResponseHandler != null && objReceivedInvitationsResponseHandler.Success)
                    {
                        if (objReceivedInvitationsResponseHandler.UsersList.Count > 0)
                        {
                            #region Filter BlacklistedUsers

                            if (AcceptConnectionRequestModel.IsChkSkipBlackListedUser &&
                                (AcceptConnectionRequestModel.IsChkPrivateBlackList ||
                                 AcceptConnectionRequestModel.IsChkGroupBlackList))
                                FilterBlacklistedUsers(objReceivedInvitationsResponseHandler.UsersList,
                                    AcceptConnectionRequestModel.IsChkPrivateBlackList,
                                    AcceptConnectionRequestModel.IsChkGroupBlackList);

                            #endregion

                            if (objReceivedInvitationsResponseHandler.UsersList.Count > 0)
                                ProcessLinkedinUsersFromUserList(tempQueryInfo, ref jobProcessResult,
                                    objReceivedInvitationsResponseHandler.UsersList);
                            else
                                GlobusLogHelper.log.Info(Log.CustomMessage,
                                    DominatorAccountModel.AccountBaseModel.AccountNetwork,
                                    DominatorAccountModel.AccountBaseModel.UserName, ActivityType,
                                    "sorry no results found navigating to next page...");
                        }
                        else
                        {
                            GlobusLogHelper.log.Info(Log.CustomMessage,
                                DominatorAccountModel.AccountBaseModel.AccountNetwork,
                                DominatorAccountModel.AccountBaseModel.UserName, ActivityType,
                                "sorry no results found navigating to next page...");
                        }

                        start = start + 40;
                        //here we running loop upto 1000 count we assuming that account maximum have
                        // upto 1000 pending connection invitation otherwise loop keep running infinite times in unitTest
                        if (start > 1000)
                        {
                            jobProcessResult.HasNoResult = true;
                            break;
                        }
                    }
                    else
                    {
                        jobProcessResult.HasNoResult = true;
                    }
                }

                #endregion
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
    }
}