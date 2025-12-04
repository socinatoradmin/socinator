using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using GramDominatorCore.GDModel;
using GramDominatorCore.Interface;
using GramDominatorUI.GDViews.GrowFollowers;
using System;
using System.Globalization;
using System.Windows;

namespace GramDominatorUI.Utility.GrowFollowers.MakeCloseFriend
{
    public class CloseFriendViewCampaign: IGdViewCampaign
    {
        public void ManageCampaign(TemplateModel templateDetails, CampaignDetails campaignDetails,
            bool isEditCampaignName,
            Visibility cancelEditVisibility, string campaignButtonContent, string templateId)
        {
            var objCloseFriend = CloseFriendTab.GetSingletonInstance();
            if (campaignButtonContent == ConstantVariable.CreateCampaign())
                objCloseFriend.CampaignName =
                    $"{SocialNetworks.Instagram} {campaignDetails.SubModule} [{DateTime.Now.ToString(CultureInfo.InvariantCulture)}]"; //added new line

            objCloseFriend.IsEditCampaignName = isEditCampaignName;
            objCloseFriend.CancelEditVisibility = cancelEditVisibility;
            objCloseFriend.TemplateId = templateId;
            objCloseFriend.CampaignButtonContent = campaignButtonContent;
            objCloseFriend.CampaignName = campaignButtonContent == ConstantVariable.UpdateCampaign()
                ? campaignDetails.CampaignName
                : objCloseFriend.CampaignName; //updated line          
            //objUnFollower.CampaignName = campaignDetails.CampaignName;           
            objCloseFriend.SelectedAccountCount = campaignDetails.SelectedAccountList.Count +
                                                 $" {"LangKeyAccountSelected".FromResourceDictionary()}";
            objCloseFriend.CloseFriendFooter.list_SelectedAccounts = campaignDetails.SelectedAccountList;

            objCloseFriend.ObjViewModel.CloseFriend =
                templateDetails.ActivitySettings.GetActivityModel<CloseFriendModel>(objCloseFriend.ObjViewModel.Model,
                    true);

            objCloseFriend.MainGrid.DataContext = objCloseFriend.ObjViewModel;

            TabSwitcher.ChangeTabIndex(1, 4);
        }
    }
}
