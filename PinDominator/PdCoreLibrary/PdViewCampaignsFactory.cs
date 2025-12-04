using System;
using System.Windows;
using CommonServiceLocator;
using DominatorHouseCore;
using DominatorHouseCore.Enums;
using DominatorHouseCore.FileManagers;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using PinDominatorCore.Utility;

namespace PinDominator.PdCoreLibrary
{
    public class PdViewCampaignsFactory : IViewCampaignsFactory
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

                var baseFactory = PinterestInitialize.GetModuleLibrary(activityType);

                var view = baseFactory.PdUtilityFactory().PdViewCampaign;

                view.ManageCampaign(templateDetails, campaignDetails, true, Visibility.Visible, openCampaignType,
                    campaignDetails.TemplateId);
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        public void ViewCampaigns(TemplateModel templateDetails, CampaignDetails campaignDetails,
            bool isEditCampaignName, Visibility cancelEditVisibility, string campaignButtonContent,
            string templateId)
        {
            var activityType = (ActivityType) Enum.Parse(typeof(ActivityType), templateDetails.ActivityType);

            var baseFactory = PinterestInitialize.GetModuleLibrary(activityType);

            var view = baseFactory.PdUtilityFactory().PdViewCampaign;

            view.ManageCampaign(templateDetails, campaignDetails, true, Visibility.Visible, campaignButtonContent,
                campaignDetails.TemplateId);
        }
    }
}