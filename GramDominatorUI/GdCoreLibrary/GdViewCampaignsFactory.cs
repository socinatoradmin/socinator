using DominatorHouseCore.Enums;
using DominatorHouseCore.FileManagers;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Utility;
using GramDominatorCore.Utility;
using System;
using System.Windows;

namespace GramDominatorUI.GDCoreLibrary
{
    public class GdViewCampaignsFactory : IViewCampaignsFactory
    {
        public void ViewCampaigns(string campaignId, string openCampaignType)
        {
            var campaignFileManager = InstanceProvider.GetInstance<ICampaignsFileManager>();
            var campaignDetails = campaignFileManager.GetCampaignById(campaignId);
            // CampaignDetails campaignDetails = CampaignsFileManager.GetCampaignById(campaignId);

            var templatesFileManager = InstanceProvider.GetInstance<ITemplatesFileManager>();
            var templateModel = templatesFileManager.GetTemplateById(campaignDetails.TemplateId);

            var activityType = (ActivityType) Enum.Parse(typeof(ActivityType), templateModel.ActivityType);

            var baseFactory = InstagramInitialize.GetModuleLibrary(activityType);

            var view = baseFactory.GdUtilityFactory().GdViewCampaign;

            view.ManageCampaign(templateModel, campaignDetails, true, Visibility.Visible, openCampaignType,
                campaignDetails.TemplateId);
        }
    }
}