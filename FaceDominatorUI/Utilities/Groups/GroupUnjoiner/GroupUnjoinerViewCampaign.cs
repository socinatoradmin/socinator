using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using FaceDominatorCore.FDModel.GroupsModel;
using FaceDominatorCore.Interface;
using FaceDominatorCore.Utility;
using FaceDominatorUI.FDViews.FbGroups;
using System;
using System.Globalization;
using System.Windows;

namespace FaceDominatorUI.Utilities.Groups.GroupUnjoiner
{
    public class GroupUnjoinerViewCampaign : IFdViewCampaign
    {
        public void ManageCampaign(TemplateModel templateDetails, CampaignDetails campaignDetails,
            bool isEditCampaignName,
            Visibility cancelEditVisibility, string campaignButtonContent, string templateId)
        {
            var singeltonObjectGroupUnjoiner = GroupUnjoinerNew.GetSingeltonObjectGroupUnjoinerNew();
            singeltonObjectGroupUnjoiner.IsEditCampaignName = isEditCampaignName;
            singeltonObjectGroupUnjoiner.CancelEditVisibility = cancelEditVisibility;
            singeltonObjectGroupUnjoiner.TemplateId = templateId;

            if (campaignButtonContent == ConstantVariable.CreateCampaign())
                singeltonObjectGroupUnjoiner.CampaignName =
                    $"{SocialNetworks.Facebook} {campaignDetails.SubModule} [{DateTime.Now.ToString(CultureInfo.InvariantCulture)}]";
            singeltonObjectGroupUnjoiner.CampaignName = campaignButtonContent == ConstantVariable.UpdateCampaign()
                ? campaignDetails.CampaignName
                : singeltonObjectGroupUnjoiner.CampaignName;

            //objGroupUnjoiner.CampaignName = campaignDetails.CampaignName;
            singeltonObjectGroupUnjoiner.CampaignButtonContent = campaignButtonContent;
            singeltonObjectGroupUnjoiner.SelectedAccountCount =
                campaignDetails.SelectedAccountList.Count + $" {"LangKeyAccountSelected".FromResourceDictionary()}";
            singeltonObjectGroupUnjoiner.GroupJoinerFooter.list_SelectedAccounts = campaignDetails.SelectedAccountList;

            singeltonObjectGroupUnjoiner.ObjViewModel.GroupUnJoinerModel
                = templateDetails.ActivitySettings.GetActivityModelNonQueryList<GroupUnjoinerModelNew>(
                    singeltonObjectGroupUnjoiner.ObjViewModel.Model);

            singeltonObjectGroupUnjoiner.MainGrid.DataContext = singeltonObjectGroupUnjoiner.ObjViewModel;

            TabSwitcher.ChangeTabIndex(3, 1);
        }
    }
}