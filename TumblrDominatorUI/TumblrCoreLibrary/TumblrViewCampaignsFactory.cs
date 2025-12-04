using CommonServiceLocator;
using DominatorHouseCore;
using DominatorHouseCore.Enums;
using DominatorHouseCore.FileManagers;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Utility;
using System;
using System.Windows;
using TumblrDominatorCore.TmblrUtility;

namespace TumblrDominatorUI.TumblrCoreLibrary
{
    public class TumblrViewCampaignsFactory : IViewCampaignsFactory
    {
        public void ViewCampaigns(string campaignId, string openCampaignType)
        {
            var campaignFileManager = InstanceProvider.GetInstance<ICampaignsFileManager>();
            var campaignDetails = campaignFileManager.GetCampaignById(campaignId);

            var templatesFileManager = InstanceProvider.GetInstance<ITemplatesFileManager>();
            var templateDetails = templatesFileManager.GetTemplateById(campaignDetails.TemplateId);

            var activityType = (ActivityType)Enum.Parse(typeof(ActivityType), templateDetails.ActivityType);

            try
            {
                var baseFactory = TumblrInitialize.GetModuleLibrary(activityType);

                var view = baseFactory.TumblrUtilityFactory().TumblrViewCampaign;

                view.ManageCampaign(templateDetails,
                    campaignDetails,
                    true,
                    Visibility.Visible,
                    openCampaignType,
                    campaignDetails.TemplateId);
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }
    }
}