using DominatorHouseCore.Enums;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using DominatorHouseCore.Process;
using DominatorHouseCore.Utility;
using PinDominatorCore.PDLibrary.DAL;
using PinDominatorCore.PDModel;
using PinDominatorCore.Response;
using System.Collections.Generic;

namespace PinDominatorCore.PDLibrary.Processors.Pin
{
    public class CustomPinProcessor : BasePinterestPinProcessor
    {
        private PinInfoPtResponseHandler _pinInfoPtResponseHandler;
        public CustomPinProcessor(IPdJobProcess jobProcess, IDbGlobalService globalService,
            IDbCampaignService campaignService, IPinFunction objPinFunct) :
            base(jobProcess, globalService, campaignService, objPinFunct)
        { }

        protected override void Process(QueryInfo queryInfo, ref JobProcessResult jobProcessResult)
        {
            //To check Is Account Selected For Repin Query
            if (JobProcess.ActivityType == ActivityType.Repin && IsAccountSelectedForRepinQuery(queryInfo))
                return;
            JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
            //if (JobProcess.DominatorAccountModel.IsRunProcessThroughBrowser)
            //    _pinInfoPtResponseHandler = BrowserManager.SearchByCustomPin(JobProcess.DominatorAccountModel, queryInfo.QueryValue, JobProcess.JobCancellationTokenSource);
            //else
                _pinInfoPtResponseHandler = PinFunction.GetPinDetails(queryInfo.QueryValue, JobProcess.DominatorAccountModel,ModuleSetting.PostFilterModel.FilterComments);

            if (_pinInfoPtResponseHandler == null || !_pinInfoPtResponseHandler.Success)
                GlobusLogHelper.log.Info(Log.NoMoreDataToPerform, JobProcess.DominatorAccountModel.AccountBaseModel.AccountNetwork,
                    JobProcess.DominatorAccountModel.UserName, ActivityType);
            else
            {
                var lstPin = new List<PinterestPin>() { _pinInfoPtResponseHandler};
                if (ActivityType != ActivityType.Repin && ActivityType != ActivityType.Comment)
                    lstPin = AlreadyInteractedPin(queryInfo, lstPin);

                lstPin = FilterBlackListUser(TemplateModel, lstPin);
                if (lstPin.Count != 0)
                    StartProcessForListOfPins(queryInfo, ref jobProcessResult, lstPin);
                jobProcessResult.HasNoResult = true;
            }
        }
    }
}