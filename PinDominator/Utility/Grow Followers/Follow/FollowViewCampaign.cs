using System;
using System.Globalization;
using System.Windows;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using PinDominator.PDViews.GrowFollowers;
using PinDominatorCore.Interface;
using PinDominatorCore.PDModel;

namespace PinDominator.Utility.Grow_Followers.Follow
{
    public class FollowViewCampaign : IPdViewCampaign
    {
        public void ManageCampaign(TemplateModel templateDetails, CampaignDetails campaignDetails,
            bool isEditCampaignName,
            Visibility cancelEditVisibility, string campaignButtonContent, string templateId)
        {
            var objFollower = Follower.GetSingeltonObjectFollower();
            if (campaignButtonContent == ConstantVariable.CreateCampaign())
                objFollower.CampaignName =
                    $"{SocialNetworks.Pinterest} {campaignDetails.SubModule} [{DateTime.Now.ToString(CultureInfo.InvariantCulture)}]";
            objFollower.IsEditCampaignName = isEditCampaignName;
            objFollower.CancelEditVisibility = Visibility.Visible;
            objFollower.TemplateId = templateId;
            objFollower.CampaignName = campaignButtonContent == ConstantVariable.UpdateCampaign()
                ? campaignDetails.CampaignName
                : objFollower.CampaignName;
            objFollower.CampaignButtonContent = campaignButtonContent;
            objFollower.SelectedAccountCount = campaignDetails.SelectedAccountList.Count +
                                               $" {"LangKeyAccountSelected".FromResourceDictionary()}";
            objFollower.FollowFooter.list_SelectedAccounts = campaignDetails.SelectedAccountList;

            objFollower.ObjViewModel.FollowerModel =
                templateDetails.ActivitySettings.GetActivityModel<FollowerModel>(objFollower.ObjViewModel.Model);

            objFollower.MainGrid.DataContext = objFollower.ObjViewModel;

            TabSwitcher.ChangeTabIndex(2, 0);
        }
    }
}