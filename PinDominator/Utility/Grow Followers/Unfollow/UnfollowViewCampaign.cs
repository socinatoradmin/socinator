using System;
using System.Globalization;
using System.Windows;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using PinDominator.PDViews.GrowFollowers;
using PinDominatorCore.Interface;
using PinDominatorCore.PDModel;

namespace PinDominator.Utility.Grow_Followers.Unfollow
{
    public class UnfollowViewCampaign : IPdViewCampaign
    {
        public void ManageCampaign(TemplateModel templateDetails, CampaignDetails campaignDetails,
            bool isEditCampaignName,
            Visibility cancelEditVisibility, string campaignButtonContent, string templateId)
        {
            var objUnFollower = UnFollower.GetSingeltonObjectUnFollower();
            if (campaignButtonContent == ConstantVariable.CreateCampaign())
                objUnFollower.CampaignName =
                    $"{SocialNetworks.Pinterest} {campaignDetails.SubModule} [{DateTime.Now.ToString(CultureInfo.InvariantCulture)}]";
            objUnFollower.IsEditCampaignName = isEditCampaignName;
            objUnFollower.CancelEditVisibility = Visibility.Visible;
            objUnFollower.CampaignButtonContent = campaignButtonContent;
            objUnFollower.SelectedAccountCount = campaignDetails.SelectedAccountList.Count +
                                                 $" {"LangKeyAccountSelected".FromResourceDictionary()}";
            objUnFollower.TemplateId = templateId;
            objUnFollower.UnFollowFooter.list_SelectedAccounts = campaignDetails.SelectedAccountList;
            objUnFollower.CampaignName = campaignButtonContent == ConstantVariable.UpdateCampaign()
                ? campaignDetails.CampaignName
                : objUnFollower.CampaignName;

            objUnFollower.ObjViewModel.UnfollowerModel =
                templateDetails.ActivitySettings.GetActivityModel<UnfollowerModel>(objUnFollower.ObjViewModel.Model,
                    true);
            objUnFollower.MainGrid.DataContext = objUnFollower.ObjViewModel;

            TabSwitcher.ChangeTabIndex(2, 1);
        }
    }
}