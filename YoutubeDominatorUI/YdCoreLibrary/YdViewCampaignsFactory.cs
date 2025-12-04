using CommonServiceLocator;
using DominatorHouseCore.Enums;
using DominatorHouseCore.FileManagers;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Utility;
using System;
using System.Windows;
using YoutubeDominatorCore.YDUtility;

namespace YoutubeDominatorUI.YdCoreLibrary
{
    public class YdViewCampaignsFactory : IViewCampaignsFactory
    {
        public void ViewCampaigns(string campaignId, string openCampaignType)
        {
            var campaignDetails =
                InstanceProvider.GetInstance<ICampaignsFileManager>().GetCampaignById(campaignId);

            var templatesFileManager = InstanceProvider.GetInstance<ITemplatesFileManager>();
            var templateDetails = templatesFileManager.GetTemplateById(campaignDetails.TemplateId);

            var activityType = (ActivityType)Enum.Parse(typeof(ActivityType), templateDetails.ActivityType);

            var baseFactory = YoutubeInitialize.GetModuleLibrary(activityType);

            var view = baseFactory.YdUtilityFactory().YdViewCampaign;

            view.ManageCampaign(templateDetails,
                campaignDetails,
                true,
                Visibility.Visible,
                openCampaignType,
                campaignDetails.TemplateId);
        }
    }
}