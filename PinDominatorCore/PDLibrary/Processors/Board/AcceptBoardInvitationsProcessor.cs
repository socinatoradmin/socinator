using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using DominatorHouseCore.Process;
using DominatorHouseCore.Utility;
using PinDominatorCore.PDLibrary.DAL;
using PinDominatorCore.Response;

namespace PinDominatorCore.PDLibrary.Processors.Board
{
    public class AcceptBoardInvitationsProcessor : BasePinterestBoardProcessor
    {
        public AcceptBoardInvitationsProcessor(IPdJobProcess jobProcess, IDbGlobalService globalService,
            IDbCampaignService campaignService, IPinFunction objPinFunct) :
            base(jobProcess, globalService, campaignService, objPinFunct)
        {
        }

        protected override void Process(QueryInfo queryInfo, ref JobProcessResult jobProcessResult)
        {
            JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
            var invitationCount = 0;
            BoardInvitationsResponseHandler boardInvitationsResponseHandler;
            if (JobProcess.DominatorAccountModel.IsRunProcessThroughBrowser)
            {
                boardInvitationsResponseHandler = BrowserManager.SearchBoardsToAcceptInvitaion(JobProcess.DominatorAccountModel, JobProcess.JobCancellationTokenSource);
                invitationCount = boardInvitationsResponseHandler.BoardsList == null ?
                    0 : boardInvitationsResponseHandler.BoardsList.Count;
            }
            else
            {
                invitationCount = PinFunction.GetBoardInvitationsCount(JobProcess.DominatorAccountModel);
                boardInvitationsResponseHandler = PinFunction.GetBoardInvitations(JobProcess.DominatorAccountModel);
            }
            if (boardInvitationsResponseHandler == null || !boardInvitationsResponseHandler.Success
                                                        || boardInvitationsResponseHandler.BoardsList != null && boardInvitationsResponseHandler.BoardsList.Count == 0)
            {
                GlobusLogHelper.log.Info(Log.NoMoreDataToPerform,
                    JobProcess.DominatorAccountModel.AccountBaseModel.AccountNetwork,
                    JobProcess.DominatorAccountModel.UserName, ActivityType);
                jobProcessResult.HasNoResult = true;
                jobProcessResult.maxId = null;
            }
            else
            {
                jobProcessResult.maxId = boardInvitationsResponseHandler.BookMark;
                if (boardInvitationsResponseHandler.BoardsList != null 
                    && invitationCount >= boardInvitationsResponseHandler.BoardsList.Count)
                    jobProcessResult.HasNoResult = false;
                else
                    jobProcessResult.HasNoResult = true;

                StartProcessForListOfBoards(queryInfo, ref jobProcessResult, boardInvitationsResponseHandler.BoardsList);

                if (boardInvitationsResponseHandler.HasMoreResults == false)
                    jobProcessResult.HasNoResult = true;
            }
        }
    }
}