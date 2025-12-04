using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using FaceDominatorCore.FDModel.InviterModel;
using FaceDominatorCore.Interface;
using FaceDominatorCore.Utility;
using System;
using System.Globalization;
using System.Windows;

namespace FaceDominatorUI.Utilities.Inviter.EventInviter
{
    public class EventInviterViewCampaign : IFdViewCampaign
    {
        public void ManageCampaign(TemplateModel templateDetails, CampaignDetails campaignDetails,
            bool isEditCampaignName, Visibility cancelEditVisibility, string campaignButtonContent, string templateId)
        {
            var singeltonObjectEventInviter = FDViews.FbInviter.EventInviter.GetSingeltonObjectEventInviter();
            singeltonObjectEventInviter.IsEditCampaignName = isEditCampaignName;
            singeltonObjectEventInviter.CancelEditVisibility = cancelEditVisibility;
            singeltonObjectEventInviter.TemplateId = templateId;

            if (campaignButtonContent == ConstantVariable.CreateCampaign())
                singeltonObjectEventInviter.CampaignName =
                    $"{SocialNetworks.Facebook} {campaignDetails.SubModule} [{DateTime.Now.ToString(CultureInfo.InvariantCulture)}]";
            singeltonObjectEventInviter.CampaignName = campaignButtonContent == ConstantVariable.UpdateCampaign()
                ? campaignDetails.CampaignName
                : singeltonObjectEventInviter.CampaignName;

            //objEventInviter.CampaignName = campaignDetails.CampaignName;
            singeltonObjectEventInviter.CampaignButtonContent = campaignButtonContent;
            singeltonObjectEventInviter.SelectedAccountCount =
                campaignDetails.SelectedAccountList.Count + $" {"LangKeyAccountSelected".FromResourceDictionary()}";
            singeltonObjectEventInviter.EventInviterFooter.list_SelectedAccounts = campaignDetails.SelectedAccountList;

            singeltonObjectEventInviter.ObjViewModel.EventInviterModel
                = templateDetails.ActivitySettings.GetActivityModelNonQueryList<EventInviterModel>(
                    singeltonObjectEventInviter.ObjViewModel.Model);

            singeltonObjectEventInviter.MainGrid.DataContext = singeltonObjectEventInviter.ObjViewModel;

            TabSwitcher.ChangeTabIndex(5, 2);
        }
    }
}