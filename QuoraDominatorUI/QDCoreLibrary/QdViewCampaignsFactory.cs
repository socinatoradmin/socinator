using System;
using CommonServiceLocator;
using DominatorHouseCore;
using DominatorHouseCore.Enums;
using DominatorHouseCore.FileManagers;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Utility;
using QuoraDominatorCore.QdUtility;

namespace QuoraDominatorUI.QDCoreLibrary
{
    public class QdViewCampaignsFactory : IViewCampaignsFactory
    {
        public void ViewCampaigns(string campaignId, string openCampaignType)
        {
            try
            {
                var campaignFileManager = InstanceProvider.GetInstance<ICampaignsFileManager>();
                var campaignDetails = campaignFileManager.GetCampaignById(campaignId);

                var templatesFileManager = InstanceProvider.GetInstance<ITemplatesFileManager>();
                var templateDetails = templatesFileManager.GetTemplateById(campaignDetails.TemplateId);

                var activityType = (ActivityType) Enum.Parse(typeof(ActivityType), templateDetails.ActivityType);

                var baseFactory = QuoraInitialize.GetModuleLibrary(activityType);

                baseFactory.QdUtilityFactory().QdViewCampaign.ViewCampaigns(campaignId, openCampaignType);
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }
    }
}