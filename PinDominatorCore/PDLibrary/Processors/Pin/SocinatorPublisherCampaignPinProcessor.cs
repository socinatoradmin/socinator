using System.Linq;
using DominatorHouseCore.Diagnostics;
using DominatorHouseCore.Enums;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using DominatorHouseCore.Process;
using DominatorHouseCore.Utility;
using PinDominatorCore.PDLibrary.DAL;

namespace PinDominatorCore.PDLibrary.Processors.Pin
{
    public class SocinatorPublisherCampaignPinProcessor : BasePinterestPinProcessor
    {
        public SocinatorPublisherCampaignPinProcessor(IPdJobProcess jobProcess, IDbGlobalService globalService,
            IDbCampaignService campaignService, IPinFunction objPinFunct) :
            base(jobProcess, globalService, campaignService, objPinFunct)
        {
        }

        protected override void Process(QueryInfo queryInfo, ref JobProcessResult jobProcessResult)
        {
            JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();

            // Here we will get PublishedPostDetailsModel which contains details of all the pins published in socio publisher 
            var details = PublisherInitialize.GetNetworksPublishedPost(queryInfo.QueryValue,
                SocialNetworks.Pinterest);

            if (details == null || details.Count == 0)
            {
                GlobusLogHelper.log.Info(Log.NoMoreDataToPerform,
                    JobProcess.DominatorAccountModel.AccountBaseModel.AccountNetwork,
                    JobProcess.DominatorAccountModel.UserName, ActivityType);
            }
            else
            {
                var listOfPinIds = details.Select(x => x.Link).ToList();
                if (ActivityType != ActivityType.Repin)
                    listOfPinIds = AlreadyInteractedPin(queryInfo, listOfPinIds);
                listOfPinIds = FilterBlackListUser(TemplateModel, listOfPinIds);
                StartProcessForListOfPins(queryInfo, ref jobProcessResult, listOfPinIds);
                jobProcessResult.HasNoResult = true;
            }
        }
    }
}