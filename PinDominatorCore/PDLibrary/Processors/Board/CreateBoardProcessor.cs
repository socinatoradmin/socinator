using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using DominatorHouseCore.Process;
using DominatorHouseCore.Utility;
using PinDominatorCore.PDLibrary.DAL;
using System.Linq;

namespace PinDominatorCore.PDLibrary.Processors.Board
{
    public class CreateBoardProcessor : BasePinterestBoardProcessor
    {
        public CreateBoardProcessor(IPdJobProcess jobProcess, IDbGlobalService globalService,
            IDbCampaignService campaignService, IPinFunction objPinFunct) :
            base(jobProcess, globalService, campaignService, objPinFunct)
        {
        }

        protected override void Process(QueryInfo queryInfo, ref JobProcessResult jobProcessResult)
        {
            JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();

            StartProcessForListOfBoardsToCreate(ref jobProcessResult, ModuleSetting.BoardDetails.ToList());

            if (!jobProcessResult.IsProcessCompleted)
                GlobusLogHelper.log.Info(Log.NoMoreDataToPerform, JobProcess.DominatorAccountModel.AccountBaseModel.AccountNetwork,
                    JobProcess.DominatorAccountModel.AccountBaseModel.UserName, ActivityType);

            jobProcessResult.HasNoResult = true;
        }
    }
}