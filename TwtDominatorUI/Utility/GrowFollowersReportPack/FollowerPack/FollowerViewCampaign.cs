using System;
using System.Globalization;
using System.Windows;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using TwtDominatorCore.Interface;
using TwtDominatorCore.TDModels;
using TwtDominatorUI.TDViews.GrowFollowers;

namespace TwtDominatorUI.Utility.GrowFollowersReportPack.FollowerPack
{
    public class FollowerViewCampaign : ITDViewCampaign
    {
        public void ManageCampaign(TemplateModel templateDetails, CampaignDetails campaignDetails,
            bool isEditCampaignName,
            Visibility cancelEditVisibility, string campaignButtonContent, string templateId)
        {
            var objFollow = Follower.GetSingletonObjectFollower();
            if (campaignButtonContent == ConstantVariable.CreateCampaign())
                objFollow.CampaignName =
                    $"{SocialNetworks.Twitter} {campaignDetails.SubModule} [{DateTime.Now.ToString(CultureInfo.InvariantCulture)}]";
            objFollow.IsEditCampaignName = isEditCampaignName;
            objFollow.CancelEditVisibility = cancelEditVisibility;
            objFollow.TemplateId = templateId;

            objFollow.CampaignName = campaignButtonContent == ConstantVariable.UpdateCampaign()
                ? campaignDetails.CampaignName
                : objFollow.CampaignName;

            objFollow.CampaignButtonContent = campaignButtonContent;
            objFollow.SelectedAccountCount = campaignDetails.SelectedAccountList.Count +
                                             $" {"LangKeyAccountSelected".FromResourceDictionary()}";
            objFollow.FollowFooter.list_SelectedAccounts = campaignDetails.SelectedAccountList;

            // Doubt FollowerModel FollowBack
            objFollow.ObjViewModel.FollowerModel =
                templateDetails.ActivitySettings.GetActivityModel<FollowerModel>(objFollow.ObjViewModel.Model);

            objFollow.MainGrid.DataContext = objFollow.ObjViewModel;

            TabSwitcher.ChangeTabIndex(1, 0);
        }
    }
}