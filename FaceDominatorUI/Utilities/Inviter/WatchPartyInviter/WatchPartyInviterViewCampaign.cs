using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using FaceDominatorCore.FDModel.InviterModel;
using FaceDominatorCore.Interface;
using FaceDominatorCore.Utility;
using System;
using System.Globalization;
using System.Windows;

namespace FaceDominatorUI.Utilities.Inviter.WatchPartyInviter
{
    public class WatchPartyInviterViewCampaign : IFdViewCampaign
    {
        public void ManageCampaign(TemplateModel templateDetails, CampaignDetails campaignDetails,
            bool isEditCampaignName, Visibility cancelEditVisibility, string campaignButtonContent, string templateId)
        {
            var singeltonObjectWatchPartyInviter =
                FDViews.FbInviter.WatchPartyInviter.GetSingeltonObjectWatchPartyInviter();
            singeltonObjectWatchPartyInviter.IsEditCampaignName = isEditCampaignName;
            singeltonObjectWatchPartyInviter.CancelEditVisibility = cancelEditVisibility;
            singeltonObjectWatchPartyInviter.TemplateId = templateId;

            if (campaignButtonContent == ConstantVariable.CreateCampaign())
                singeltonObjectWatchPartyInviter.CampaignName =
                    $"{SocialNetworks.Facebook} {campaignDetails.SubModule} [{DateTime.Now.ToString(CultureInfo.InvariantCulture)}]";
            singeltonObjectWatchPartyInviter.CampaignName = campaignButtonContent == ConstantVariable.UpdateCampaign()
                ? campaignDetails.CampaignName
                : singeltonObjectWatchPartyInviter.CampaignName;

            //objWatchPartyInviter.CampaignName = campaignDetails.CampaignName;
            singeltonObjectWatchPartyInviter.CampaignButtonContent = campaignButtonContent;
            singeltonObjectWatchPartyInviter.SelectedAccountCount =
                campaignDetails.SelectedAccountList.Count + $" {"LangKeyAccountSelected".FromResourceDictionary()}";
            singeltonObjectWatchPartyInviter.PageInviterFooter.list_SelectedAccounts =
                campaignDetails.SelectedAccountList;

            singeltonObjectWatchPartyInviter.ObjViewModel.WatchPartyInviterModel
                = templateDetails.ActivitySettings.GetActivityModelNonQueryList<WatchPartyInviterModel>(
                    singeltonObjectWatchPartyInviter.ObjViewModel.Model);

            singeltonObjectWatchPartyInviter.MainGrid.DataContext = singeltonObjectWatchPartyInviter.ObjViewModel;

            TabSwitcher.ChangeTabIndex(5, 3);
        }
    }
}