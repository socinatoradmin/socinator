using System;
using System.Globalization;
using System.Windows;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using PinDominatorCore.Interface;
using PinDominatorCore.PDModel;
using followBack = PinDominator.PDViews.GrowFollowers.FollowBack;

namespace PinDominator.Utility.Grow_Followers.Follow_Back
{
    public class FollowBackViewCampaign : IPdViewCampaign
    {
        public void ManageCampaign(TemplateModel templateDetails, CampaignDetails campaignDetails,
            bool isEditCampaignName,
            Visibility cancelEditVisibility, string campaignButtonContent, string templateId)
        {
            var objFollowBack = followBack.GetSingeltonObjectFollowBack();
            if (campaignButtonContent == ConstantVariable.CreateCampaign())
                objFollowBack.CampaignName =
                    $"{SocialNetworks.Pinterest} {campaignDetails.SubModule} [{DateTime.Now.ToString(CultureInfo.InvariantCulture)}]";
            objFollowBack.IsEditCampaignName = isEditCampaignName;
            objFollowBack.CancelEditVisibility = Visibility.Visible;
            objFollowBack.CampaignButtonContent = campaignButtonContent;
            objFollowBack.SelectedAccountCount = campaignDetails.SelectedAccountList.Count +
                                                 $" {"LangKeyAccountSelected".FromResourceDictionary()}";
            objFollowBack.TemplateId = templateId;
            objFollowBack.FollowBackFooter.list_SelectedAccounts = campaignDetails.SelectedAccountList;
            objFollowBack.CampaignName = campaignButtonContent == ConstantVariable.UpdateCampaign()
                ? campaignDetails.CampaignName
                : objFollowBack.CampaignName;

            objFollowBack.ObjViewModel.FollowBackModel =
                templateDetails.ActivitySettings.GetActivityModel<FollowBackModel>(objFollowBack.ObjViewModel.Model,
                    true);
            objFollowBack.MainGrid.DataContext = objFollowBack.ObjViewModel;

            TabSwitcher.ChangeTabIndex(2, 2);
        }
    }
}