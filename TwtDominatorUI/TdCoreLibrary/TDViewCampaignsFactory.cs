using System;
using System.Windows;
using CommonServiceLocator;
using DominatorHouseCore.Enums;
using DominatorHouseCore.FileManagers;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Utility;
using TwtDominatorCore.TDUtility;

namespace TwtDominatorUI.TdCoreLibrary
{
    public class TDViewCampaignsFactory : IViewCampaignsFactory
    {
        public void ViewCampaigns(string campaignId, string openCampaignType)
        {
            var campaignFileManager = InstanceProvider.GetInstance<ICampaignsFileManager>();
            var campaignDetails = campaignFileManager.GetCampaignById(campaignId);
            var TemplatesFileManager = InstanceProvider.GetInstance<ITemplatesFileManager>();
            var templateDetails = TemplatesFileManager.GetTemplateById(campaignDetails.TemplateId);
            var activityType = (ActivityType) Enum.Parse(typeof(ActivityType), templateDetails.ActivityType);
            var baseFactory = TDInitialise.GetModuleLibrary(activityType);
            var view = baseFactory.TDUtilityFactory().TDViewCampaign;

            view.ManageCampaign(templateDetails,
                campaignDetails,
                true,
                Visibility.Visible,
                openCampaignType,
                campaignDetails.TemplateId);
        }

        //public void ViewCampaigns(TemplateModel templateDetails, CampaignDetails campaignDetails,
        //    bool isEditCampaignName, Visibility cancelEditVisibility, string campaignButtonContent,
        //    string templateId)
        //{

        //    var ActivityType = (ActivityType)Enum.Parse(typeof(ActivityType), templateDetails.ActivityType);

        //    var baseFactory = TDInitialise.GetModuleLibrary(ActivityType);

        //    var view = baseFactory.TDUtilityFactory().TDViewCampaign;

        //    view.ManageCampaign(templateDetails, campaignDetails, true, Visibility.Visible, campaignButtonContent,
        //        campaignDetails.TemplateId);
        //}
    }
}