using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using DominatorHouseCore.Process;
using DominatorHouseCore.Utility;
using PinDominatorCore.PDLibrary.DAL;
using PinDominatorCore.PDModel;
using System.Collections.Generic;

namespace PinDominatorCore.PDLibrary.Processors.Board
{
    public class CustomBoardProcessor : BasePinterestBoardProcessor
    {
        public CustomBoardProcessor(IPdJobProcess jobProcess, IDbGlobalService globalService,
            IDbCampaignService campaignService, IPinFunction objPinFunct) :
            base(jobProcess, globalService, campaignService, objPinFunct)
        {
        }

        protected override void Process(QueryInfo queryInfo, ref JobProcessResult jobProcessResult)
        {
            JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
            var arrUser = queryInfo.QueryValue.Split('/');
            if (arrUser.Length >= 5 && string.IsNullOrEmpty(arrUser[4]) || arrUser.Length < 5)
            {
                GlobusLogHelper.log.Info(Log.CustomMessage, JobProcess.DominatorAccountModel.AccountBaseModel.AccountNetwork,
                    JobProcess.DominatorAccountModel.AccountBaseModel.UserName, ActivityType,
                    string.Format("LangKeyCheckSomeUrlSeemsIncorrect".FromResourceDictionary(), "LangKeyBoard".FromResourceDictionary()));
                jobProcessResult.IsProcessCompleted = true;
                return;
            }

            var boardInfo = JobProcess.DominatorAccountModel.IsRunProcessThroughBrowser
                ? BrowserManager.SearchByCustomBoard(JobProcess.DominatorAccountModel, queryInfo.QueryValue, JobProcess.JobCancellationTokenSource)
                : PinFunction.GetBoardDetails(queryInfo.QueryValue, JobProcess.DominatorAccountModel);

            if (boardInfo == null || !boardInfo.Success)
            {
                GlobusLogHelper.log.Info(Log.NoMoreDataToPerform, JobProcess.DominatorAccountModel.AccountBaseModel.AccountNetwork,
                    JobProcess.DominatorAccountModel.UserName, ActivityType);
                jobProcessResult.HasNoResult = true;
            }
            else
            {
                var boardList = new List<PinterestBoard>();
                boardList.Add(boardInfo);
                StartProcessForListOfBoards(queryInfo, ref jobProcessResult, boardList);
                jobProcessResult.HasNoResult = true;
            }
        }
    }
}