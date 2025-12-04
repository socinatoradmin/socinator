using System;
using System.Globalization;
using System.Windows;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using GramDominatorCore.GDModel;
using GramDominatorCore.Interface;

namespace GramDominatorUI.Utility.GrowFollowers.BlockFollower
{
    internal class BlockFollowerViewCampaign : IGdViewCampaign
    {
        public void ManageCampaign(TemplateModel templateDetails, CampaignDetails campaignDetails,
            bool isEditCampaignName,
            Visibility cancelEditVisibility, string campaignButtonContent, string templateId)
        {
            var objBlockFollower = GDViews.GrowFollowers.BlockFollower.GetSingeltonObjectBlockFollower();
            if (campaignButtonContent == ConstantVariable.CreateCampaign())
                objBlockFollower.CampaignName =
                    $"{SocialNetworks.Instagram} {campaignDetails.SubModule} [{DateTime.Now.ToString(CultureInfo.InvariantCulture)}]"; //added new line
            objBlockFollower.IsEditCampaignName = isEditCampaignName;
            objBlockFollower.CancelEditVisibility = cancelEditVisibility;
            objBlockFollower.TemplateId = templateId;
            objBlockFollower.CampaignButtonContent = campaignButtonContent;
            objBlockFollower.CampaignName = campaignButtonContent == ConstantVariable.UpdateCampaign()
                ? campaignDetails.CampaignName
                : objBlockFollower.CampaignName; //updated line           
            objBlockFollower.SelectedAccountCount = campaignDetails.SelectedAccountList.Count +
                                                    $" {"LangKeyAccountSelected".FromResourceDictionary()}";
            objBlockFollower.BlockFollowerFooter.list_SelectedAccounts = campaignDetails.SelectedAccountList;

            objBlockFollower.ObjViewModel.BlockFollowerModel
                = templateDetails.ActivitySettings.GetActivityModel<BlockFollowerModel>(objBlockFollower.ObjViewModel
                    .Model);

            objBlockFollower.MainGrid.DataContext = objBlockFollower.ObjViewModel;

            TabSwitcher.ChangeTabIndex(1, 3);
        }
    }
}