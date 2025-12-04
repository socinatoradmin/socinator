using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using FaceDominatorCore.FDModel.GroupsModel;
using FaceDominatorCore.Interface;
using FaceDominatorCore.Utility;
using System;
using System.Globalization;
using System.Windows;

namespace FaceDominatorUI.Utilities.Groups.MakeGroupAdmin
{
    public class MakeGroupAdminViewCampaign : IFdViewCampaign
    {
        public void ManageCampaign(TemplateModel templateDetails, CampaignDetails campaignDetails,
            bool isEditCampaignName, Visibility cancelEditVisibility, string campaignButtonContent, string templateId)
        {
            var singeltonObjectmakeGroupAdmin =
                FDViews.FbGroups.MakeGroupAdmin.GetSingletonMakeGroupAdmin();
            singeltonObjectmakeGroupAdmin.IsEditCampaignName = isEditCampaignName;
            singeltonObjectmakeGroupAdmin.CancelEditVisibility = cancelEditVisibility;
            singeltonObjectmakeGroupAdmin.TemplateId = templateId;

            if (campaignButtonContent == ConstantVariable.CreateCampaign())
                singeltonObjectmakeGroupAdmin.CampaignName =
                    $"{SocialNetworks.Facebook} {campaignDetails.SubModule} [{DateTime.Now.ToString(CultureInfo.InvariantCulture)}]";
            singeltonObjectmakeGroupAdmin.CampaignName = campaignButtonContent == ConstantVariable.UpdateCampaign()
                ? campaignDetails.CampaignName
                : singeltonObjectmakeGroupAdmin.CampaignName;

            //makeGroupAdmin.CampaignName = campaignDetails.CampaignName;
            singeltonObjectmakeGroupAdmin.CampaignButtonContent = campaignButtonContent;
            singeltonObjectmakeGroupAdmin.SelectedAccountCount =
                campaignDetails.SelectedAccountList.Count + $" {"LangKeyAccountSelected".FromResourceDictionary()}";
            singeltonObjectmakeGroupAdmin.MakeGroupAdminFooter.list_SelectedAccounts =
                campaignDetails.SelectedAccountList;

            singeltonObjectmakeGroupAdmin.ObjViewModel.MakeAdminModel
                = templateDetails.ActivitySettings.GetActivityModelNonQueryList<MakeAdminModel>(
                    singeltonObjectmakeGroupAdmin.ObjViewModel.Model);

            singeltonObjectmakeGroupAdmin.MainGrid.DataContext = singeltonObjectmakeGroupAdmin.ObjViewModel;

            TabSwitcher.ChangeTabIndex(3, 2);
        }
    }
}