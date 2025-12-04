using System;
using System.Globalization;
using System.Windows;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using GramDominatorCore.GDModel;
using GramDominatorCore.Interface;

namespace GramDominatorUI.Utility.GrowFollowers.Follower
{
    public class FollowerViewCampaign : IGdViewCampaign
    {
        public void ManageCampaign(TemplateModel templateDetails, CampaignDetails campaignDetails,
            bool isEditCampaignName,
            Visibility cancelEditVisibility, string campaignButtonContent, string templateId)
        {
            var objFollower = GDViews.GrowFollowers.Follower.GetSingeltonObjectFollower();
            if (campaignButtonContent == ConstantVariable.CreateCampaign())
                objFollower.CampaignName =
                    $"{SocialNetworks.Instagram} {campaignDetails.SubModule} [{DateTime.Now.ToString(CultureInfo.InvariantCulture)}]"; //added new line

            objFollower.IsEditCampaignName = isEditCampaignName;
            objFollower.CancelEditVisibility = cancelEditVisibility;
            objFollower.TemplateId = templateId;
            objFollower.CampaignButtonContent = campaignButtonContent;
            objFollower.CampaignName = campaignButtonContent == ConstantVariable.UpdateCampaign()
                ? campaignDetails.CampaignName
                : objFollower.CampaignName; //updated line                    
            objFollower.SelectedAccountCount = campaignDetails.SelectedAccountList.Count +
                                               $" {"LangKeyAccountSelected".FromResourceDictionary()}";
            objFollower.FollowFooter.list_SelectedAccounts = campaignDetails.SelectedAccountList;

            objFollower.ObjViewModel.FollowerModel =
                templateDetails.ActivitySettings.GetActivityModel<FollowerModel>(objFollower.ObjViewModel.Model);
            objFollower.MainGrid.DataContext = objFollower.ObjViewModel;
            TabSwitcher.ChangeTabIndex(1, 0);
        }
    }
}