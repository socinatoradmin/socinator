using System;
using System.Globalization;
using System.Windows;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using GramDominatorCore.GDModel;
using GramDominatorCore.Interface;

namespace GramDominatorUI.Utility.GrowFollowers.FollowBack
{
    internal class FollowBackViewCampaign : IGdViewCampaign
    {
        public void ManageCampaign(TemplateModel templateDetails, CampaignDetails campaignDetails,
            bool isEditCampaignName,
            Visibility cancelEditVisibility, string campaignButtonContent, string templateId)
        {
            var objFollowBack = GDViews.GrowFollowers.FollowBack.GetSingeltonObjectFollowBack();
            if (campaignButtonContent == ConstantVariable.CreateCampaign())
                objFollowBack.CampaignName =
                    $"{SocialNetworks.Instagram} {campaignDetails.SubModule} [{DateTime.Now.ToString(CultureInfo.InvariantCulture)}]"; //added new line

            objFollowBack.IsEditCampaignName = isEditCampaignName;
            objFollowBack.CancelEditVisibility = cancelEditVisibility;
            objFollowBack.TemplateId = templateId;
            objFollowBack.CampaignButtonContent = campaignButtonContent;
            objFollowBack.CampaignName = campaignButtonContent == ConstantVariable.UpdateCampaign()
                ? campaignDetails.CampaignName
                : objFollowBack.CampaignName; //updated line           
            objFollowBack.SelectedAccountCount = campaignDetails.SelectedAccountList.Count +
                                                 $" {"LangKeyAccountSelected".FromResourceDictionary()}";
            objFollowBack.FollowBackFooter.list_SelectedAccounts = campaignDetails.SelectedAccountList;

            objFollowBack.ObjViewModel.FollowBackModel =
                templateDetails.ActivitySettings.GetActivityModel<FollowBackModel>(objFollowBack.ObjViewModel.Model);

            objFollowBack.MainGrid.DataContext = objFollowBack.ObjViewModel;

            TabSwitcher.ChangeTabIndex(1, 2);
        }
    }
}