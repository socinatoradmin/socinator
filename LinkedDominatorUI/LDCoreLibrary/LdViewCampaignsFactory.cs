using System;
using System.Windows;
using CommonServiceLocator;
using DominatorHouseCore.Enums;
using DominatorHouseCore.FileManagers;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using LinkedDominatorCore.Utility;

namespace LinkedDominatorUI.LDCoreLibrary
{
    public class LdViewCampaignsFactory : IViewCampaignsFactory
    {
        public void ViewCampaigns(string campaignId, string openCampaignType)
        {
            var campaignFileManager = InstanceProvider.GetInstance<ICampaignsFileManager>();
            var campaignDetails = campaignFileManager.GetCampaignById(campaignId);

            var templatesFileManager = InstanceProvider.GetInstance<ITemplatesFileManager>();
            var templateDetails = templatesFileManager.GetTemplateById(campaignDetails.TemplateId);

            //if (openCampaignType == ConstantVariable.CreateCampaign())
            //    ModuleUIObject.CampaignName = $"{SocialNetworks} {campaignDetails.SubModule.ToString()} [{DateTime.Now.ToString(CultureInfo.InvariantCulture)}]";

            var activityType = (ActivityType) Enum.Parse(typeof(ActivityType), templateDetails.ActivityType);

            var baseFactory = LinkedInInitialize.GetModuleLibrary(activityType);

            var view = baseFactory.LdUtilityFactory().LdViewCampaign;

            view.ManageCampaign(templateDetails, campaignDetails, true, Visibility.Visible, openCampaignType,
                campaignDetails.TemplateId);
        }

        public void ViewCampaigns(TemplateModel templateDetails, CampaignDetails campaignDetails,
            bool isEditCampaignName, Visibility cancelEditVisibility, string campaignButtonContent,
            string templateId)
        {
            var ActivityType = (ActivityType) Enum.Parse(typeof(ActivityType), templateDetails.ActivityType);

            var baseFactory = LinkedInInitialize.GetModuleLibrary(ActivityType);

            var view = baseFactory.LdUtilityFactory().LdViewCampaign;

            view.ManageCampaign(templateDetails, campaignDetails, true, Visibility.Visible, campaignButtonContent,
                campaignDetails.TemplateId);
        }
    }
}