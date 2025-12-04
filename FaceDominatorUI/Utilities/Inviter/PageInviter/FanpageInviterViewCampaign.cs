using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using FaceDominatorCore.FDModel.InviterModel;
using FaceDominatorCore.Interface;
using FaceDominatorCore.Utility;
using System;
using System.Globalization;
using System.Windows;

namespace FaceDominatorUI.Utilities.Inviter.PageInviter
{
    public class FanpageInviterViewCampaign : IFdViewCampaign
    {
        public void ManageCampaign(TemplateModel templateDetails, CampaignDetails campaignDetails,
            bool isEditCampaignName, Visibility cancelEditVisibility, string campaignButtonContent, string templateId)
        {
            var singeltonObjectFanapgeLiker = FDViews.FbInviter.PageInviter.GetSingeltonObjectPageInviter();
            singeltonObjectFanapgeLiker.IsEditCampaignName = isEditCampaignName;
            singeltonObjectFanapgeLiker.CancelEditVisibility = cancelEditVisibility;
            singeltonObjectFanapgeLiker.TemplateId = templateId;

            if (campaignButtonContent == ConstantVariable.CreateCampaign())
                singeltonObjectFanapgeLiker.CampaignName =
                    $"{SocialNetworks.Facebook} {campaignDetails.SubModule} [{DateTime.Now.ToString(CultureInfo.InvariantCulture)}]";
            singeltonObjectFanapgeLiker.CampaignName = campaignButtonContent == ConstantVariable.UpdateCampaign()
                ? campaignDetails.CampaignName
                : singeltonObjectFanapgeLiker.CampaignName;

            //objFanapgeLiker.CampaignName = campaignDetails.CampaignName;
            singeltonObjectFanapgeLiker.CampaignButtonContent = campaignButtonContent;
            singeltonObjectFanapgeLiker.SelectedAccountCount =
                campaignDetails.SelectedAccountList.Count + $" {"LangKeyAccountSelected".FromResourceDictionary()}";
            singeltonObjectFanapgeLiker.PageInviterFooter.list_SelectedAccounts = campaignDetails.SelectedAccountList;

            singeltonObjectFanapgeLiker.ObjViewModel.FanpageInviterModel
                = templateDetails.ActivitySettings.GetActivityModelNonQueryList<FanpageInviterModel>(
                    singeltonObjectFanapgeLiker.ObjViewModel.Model);

            singeltonObjectFanapgeLiker.MainGrid.DataContext = singeltonObjectFanapgeLiker.ObjViewModel;

            TabSwitcher.ChangeTabIndex(5, 1);
        }
    }
}