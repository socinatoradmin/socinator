using CommonServiceLocator;
using DominatorHouseCore;
using DominatorHouseCore.Enums;
using DominatorHouseCore.FileManagers;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Utility;
using RedditDominatorCore.Utility;
using System;

namespace RedditDominatorUI.RdCoreLibrary
{
    public class RdViewCampaignsFactory : IViewCampaignsFactory
    {
        public void ViewCampaigns(string campaignId, string openCampaignType)
        {
            try
            {
                var campaignFileManager = InstanceProvider.GetInstance<ICampaignsFileManager>();
                var campaignDetails = campaignFileManager.GetCampaignById(campaignId);

                var templatesFileManager = InstanceProvider.GetInstance<ITemplatesFileManager>();
                var templateDetails = templatesFileManager.GetTemplateById(campaignDetails.TemplateId);

                var activityType = (ActivityType)Enum.Parse(typeof(ActivityType), templateDetails.ActivityType);

                var baseFactory = RedditInitialize.GetModuleLibrary(activityType);

                baseFactory.RdUtilityFactory().RdViewCampaign.ViewCampaigns(campaignId, openCampaignType);
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }
    }
}