using System;
using System.Globalization;
using System.Windows;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using TwtDominatorCore.Interface;
using TwtDominatorCore.TDModels;
using TwtDominatorUI.TDViews.GrowFollowers;

namespace TwtDominatorUI.Utility.GrowFollowersReportPack.MuteUsersPack
{
    public class MuteUsersViewCampaign : ITDViewCampaign
    {
        public void ManageCampaign(TemplateModel templateDetails, CampaignDetails campaignDetails,
            bool isEditCampaignName,
            Visibility cancelEditVisibility, string campaignButtonContent, string templateId)
        {
            var objMuteUsers = MuteUsers.GetSingletonObjectMuteUsers();
            if (campaignButtonContent == ConstantVariable.CreateCampaign())
                objMuteUsers.CampaignName =
                    $"{SocialNetworks.Twitter} {campaignDetails.SubModule} [{DateTime.Now.ToString(CultureInfo.InvariantCulture)}]";


            objMuteUsers.IsEditCampaignName = isEditCampaignName;
            objMuteUsers.CancelEditVisibility = cancelEditVisibility;
            objMuteUsers.TemplateId = templateId;


            objMuteUsers.CampaignName = campaignButtonContent == ConstantVariable.UpdateCampaign()
                ? campaignDetails.CampaignName
                : objMuteUsers.CampaignName;

            objMuteUsers.CampaignButtonContent = campaignButtonContent;
            objMuteUsers.SelectedAccountCount = campaignDetails.SelectedAccountList.Count +
                                                $" {"LangKeyAccountSelected".FromResourceDictionary()}";
            objMuteUsers.MuteFooter.list_SelectedAccounts = campaignDetails.SelectedAccountList;

            // Doubt FollowerModel FollowBack
            objMuteUsers.ObjViewModel.MuteModel =
                templateDetails.ActivitySettings.GetActivityModel<MuteModel>(objMuteUsers.ObjViewModel.Model);

            objMuteUsers.MainGrid.DataContext = objMuteUsers.ObjViewModel;

            TabSwitcher.ChangeTabIndex(1, 2);
        }
    }
}