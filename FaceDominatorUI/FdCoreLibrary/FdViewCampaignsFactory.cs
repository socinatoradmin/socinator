using CommonServiceLocator;
using DominatorHouseCore.Enums;
using DominatorHouseCore.FileManagers;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Utility;
using FaceDominatorCore.Utility;
using System;
using System.Windows;

namespace FaceDominatorUI.FdCoreLibrary
{
    public class FdViewCampaignsFactory : IViewCampaignsFactory
    {
        public void ViewCampaigns(string campaignId, string openCampaignType)
        {
            var campaignFileManager = InstanceProvider.GetInstance<ICampaignsFileManager>();
            var campaignDetails = campaignFileManager.GetCampaignById(campaignId);

            var templatesFileManager = InstanceProvider.GetInstance<ITemplatesFileManager>();

            var templateDetails = templatesFileManager.GetTemplateById(campaignDetails.TemplateId);

            var activityType = (ActivityType)Enum.Parse(typeof(ActivityType), templateDetails.ActivityType);

            var baseFactory = FacebookInitialize.GetModuleLibrary(activityType);

            var view = baseFactory.FdUtilityFactory().FdViewCampaign;

            view.ManageCampaign(templateDetails,
                campaignDetails,
                true,
                Visibility.Visible,
                openCampaignType,
                campaignDetails.TemplateId);
        }
    }
}