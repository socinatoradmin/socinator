using System;
using System.Globalization;
using System.Windows;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using TwtDominatorCore.Interface;
using TwtDominatorCore.TDModels;
using TwtDominatorUI.TDViews.TwtBlaster;

namespace TwtDominatorUI.Utility.TwtBlasterReportPack.WelcomeTweetPack
{
    internal class WelcomeTweetViewCampaign : ITDViewCampaign
    {
        public void ManageCampaign(TemplateModel templateDetails, CampaignDetails campaignDetails,
            bool isEditCampaignName,
            Visibility cancelEditVisibility, string campaignButtonContent, string templateId)
        {
            var objWelcomeTweet = WelcomeTweet.GetSingletonObjectobjWelcomeTweet();
            if (campaignButtonContent == ConstantVariable.CreateCampaign())
                objWelcomeTweet.CampaignName =
                    $"{SocialNetworks.Twitter} {campaignDetails.SubModule} [{DateTime.Now.ToString(CultureInfo.InvariantCulture)}]";

            objWelcomeTweet.IsEditCampaignName = isEditCampaignName;
            objWelcomeTweet.CancelEditVisibility = cancelEditVisibility;
            objWelcomeTweet.TemplateId = templateId;

            objWelcomeTweet.CampaignName = campaignButtonContent == ConstantVariable.UpdateCampaign()
                ? campaignDetails.CampaignName
                : objWelcomeTweet.CampaignName;

            objWelcomeTweet.CampaignButtonContent = campaignButtonContent;
            objWelcomeTweet.SelectedAccountCount = campaignDetails.SelectedAccountList.Count +
                                                   $" {"LangKeyAccountSelected".FromResourceDictionary()}";
            objWelcomeTweet.WelcomeTweetFooter.list_SelectedAccounts = campaignDetails.SelectedAccountList;

            // Doubt FollowerModel FollowBack
            objWelcomeTweet.ObjViewModel.WelcomeTweetModel =
                templateDetails.ActivitySettings.GetActivityModel<WelcomeTweetModel>(objWelcomeTweet.ObjViewModel.Model,
                    true);

            objWelcomeTweet.MainGrid.DataContext = objWelcomeTweet.ObjViewModel;

            TabSwitcher.ChangeTabIndex(2, 2);
        }
    }
}