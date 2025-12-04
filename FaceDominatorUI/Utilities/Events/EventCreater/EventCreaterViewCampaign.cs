using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using FaceDominatorCore.FDModel.FbEvents;
using FaceDominatorCore.Interface;
using FaceDominatorCore.Utility;
using FaceDominatorUI.FDViews.FbEvents;
using System;
using System.Globalization;
using System.Windows;

namespace FaceDominatorUI.Utilities.Events.EventCreater
{
    internal class EventCreaterViewCampaign : IFdViewCampaign
    {
        public void ManageCampaign(TemplateModel templateDetails, CampaignDetails campaignDetails,
            bool isEditCampaignName,
            Visibility cancelEditVisibility, string campaignButtonContent, string templateId)
        {
            var singeltonObjectEventCreator = EventCreator.GetSingeltonObjectEventCreator();
            singeltonObjectEventCreator.IsEditCampaignName = isEditCampaignName;
            singeltonObjectEventCreator.CancelEditVisibility = cancelEditVisibility;
            singeltonObjectEventCreator.TemplateId = templateId;

            if (campaignButtonContent == ConstantVariable.CreateCampaign())
                singeltonObjectEventCreator.CampaignName =
                    $"{SocialNetworks.Facebook} {campaignDetails.SubModule} [{DateTime.Now.ToString(CultureInfo.InvariantCulture)}]";
            singeltonObjectEventCreator.CampaignName = campaignButtonContent == ConstantVariable.UpdateCampaign()
                ? campaignDetails.CampaignName
                : singeltonObjectEventCreator.CampaignName;
            //singeltonObjectEventCreator.CampaignName = campaignDetails.CampaignName;
            singeltonObjectEventCreator.CampaignButtonContent = campaignButtonContent;
            singeltonObjectEventCreator.SelectedAccountCount =
                campaignDetails.SelectedAccountList.Count + $" {"LangKeyAccountSelected".FromResourceDictionary()}";
            singeltonObjectEventCreator.EventCreaterFooter.list_SelectedAccounts = campaignDetails.SelectedAccountList;

            singeltonObjectEventCreator.ObjViewModel.EventCreatorModel =
                templateDetails.ActivitySettings.GetActivityModelNonQueryList<EventCreatorModel>(
                    singeltonObjectEventCreator.ObjViewModel.Model);

            singeltonObjectEventCreator.MainGrid.DataContext = singeltonObjectEventCreator.ObjViewModel;

            TabSwitcher.ChangeTabIndex(5, 1);
        }
    }
}