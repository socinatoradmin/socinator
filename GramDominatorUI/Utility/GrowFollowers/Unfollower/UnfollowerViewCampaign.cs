using System;
using System.Globalization;
using System.Windows;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using GramDominatorCore.GDModel;
using GramDominatorCore.Interface;
using GramDominatorUI.GDViews.GrowFollowers;

namespace GramDominatorUI.Utility.GrowFollowers.Unfollower
{
    internal class UnfollowerViewCampaign : IGdViewCampaign
    {
        public void ManageCampaign(TemplateModel templateDetails, CampaignDetails campaignDetails,
            bool isEditCampaignName,
            Visibility cancelEditVisibility, string campaignButtonContent, string templateId)
        {
            var objUnFollower = UnFollower.GetSingeltonObjectUnfollower();
            if (campaignButtonContent == ConstantVariable.CreateCampaign())
                objUnFollower.CampaignName =
                    $"{SocialNetworks.Instagram} {campaignDetails.SubModule} [{DateTime.Now.ToString(CultureInfo.InvariantCulture)}]"; //added new line

            objUnFollower.IsEditCampaignName = isEditCampaignName;
            objUnFollower.CancelEditVisibility = cancelEditVisibility;
            objUnFollower.TemplateId = templateId;
            objUnFollower.CampaignButtonContent = campaignButtonContent;
            objUnFollower.CampaignName = campaignButtonContent == ConstantVariable.UpdateCampaign()
                ? campaignDetails.CampaignName
                : objUnFollower.CampaignName; //updated line          
            //objUnFollower.CampaignName = campaignDetails.CampaignName;           
            objUnFollower.SelectedAccountCount = campaignDetails.SelectedAccountList.Count +
                                                 $" {"LangKeyAccountSelected".FromResourceDictionary()}";
            objUnFollower.UnFollowFooter.list_SelectedAccounts = campaignDetails.SelectedAccountList;

            objUnFollower.ObjViewModel.UnfollowerModel =
                templateDetails.ActivitySettings.GetActivityModel<UnfollowerModel>(objUnFollower.ObjViewModel.Model,
                    true);

            objUnFollower.MainGrid.DataContext = objUnFollower.ObjViewModel;

            TabSwitcher.ChangeTabIndex(1, 1);
        }
    }
}

//if (openCampaignType == ConstantVariable.CreateCampaign())
//ModuleUIObject.CampaignName = $"{SocialNetworks} {campaignDetails.SubModule.ToString()} [{DateTime.Now.ToString(CultureInfo.InvariantCulture)}]";