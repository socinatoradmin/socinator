using DominatorHouseCore;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using DominatorHouseCore.Process;
using DominatorHouseCore.Utility;
using Newtonsoft.Json;
using PinDominatorCore.PDLibrary.DAL;
using PinDominatorCore.PDModel;
using PinDominatorCore.Response;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PinDominatorCore.PDLibrary.Processors.Board
{
    public class SendBoardInvitationsProcessor : BasePinterestBoardProcessor
    {
        public SendBoardInvitationsProcessor(IPdJobProcess jobProcess, IDbGlobalService globalService,
            IDbCampaignService campaignService, IPinFunction objPinFunct) :
            base(jobProcess, globalService, campaignService, objPinFunct)
        {
        }

        protected override void Process(QueryInfo queryInfo, ref JobProcessResult jobProcessResult)
        {
            try
            {
                var sendBoardInvitationModel =
                    JsonConvert.DeserializeObject<SendBoardInvitationModel>(TemplateModel.ActivitySettings);
                queryInfo = new QueryInfo
                {
                    QueryType = "Send Board Invitation",
                    QueryValue = "All"
                };
                var boardCollaborators = sendBoardInvitationModel.BoardCollaboratorDetails.Where(
                    boardCollaborator => boardCollaborator.Account == JobProcess.DominatorAccountModel.UserName);

                foreach (var info in boardCollaborators)
                {
                    BoardInfoPtResponseHandler boardInfo;
                    var failedCount = 0;
                    TryAgain:
                    if (JobProcess.DominatorAccountModel.IsRunProcessThroughBrowser)
                        boardInfo = BrowserManager.SearchByCustomBoard(JobProcess.DominatorAccountModel,
                            info.BoardUrl, JobProcess.JobCancellationTokenSource);
                    else
                        boardInfo = PinFunction.GetBoardDetails(info.BoardUrl, JobProcess.DominatorAccountModel);
                    while (failedCount++ <= 3 && (boardInfo == null || !boardInfo.Success))
                        goto TryAgain;
                    PinterestBoard board = boardInfo;
                    board.EmailToCollaborate = info.Email;

                    if (boardInfo == null || !boardInfo.Success)
                        GlobusLogHelper.log.Info(Log.NoMoreDataToPerform, JobProcess.DominatorAccountModel.AccountBaseModel.AccountNetwork,
                            JobProcess.DominatorAccountModel.UserName, ActivityType);
                    else
                    {
                        var boardList = new List<PinterestBoard> {board};
                        StartProcessForListOfBoards(queryInfo, ref jobProcessResult, boardList);
                    }
                    jobProcessResult.HasNoResult = true;
                }
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception ex)
            {
                jobProcessResult.HasNoResult = true;
                ex.DebugLog();
            }
        }
    }
}