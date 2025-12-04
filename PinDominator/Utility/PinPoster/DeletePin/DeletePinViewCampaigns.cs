using System;
using System.Globalization;
using System.Windows;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using PinDominator.PDViews.PinPoster;
using PinDominatorCore.Interface;
using PinDominatorCore.PDModel;

namespace PinDominator.Utility.PinPoster.DeletePin
{
    public class DeletePinViewCampaigns : IPdViewCampaign
    {
        public void ManageCampaign(TemplateModel templateDetails, CampaignDetails campaignDetails,
            bool isEditCampaignName, Visibility cancelEditVisibility, string campaignButtonContent, string templateId)
        {
            var objDeletePins = DeletePins.GetSingletonObjectDeletePins();
            if (campaignButtonContent == ConstantVariable.CreateCampaign())
                objDeletePins.CampaignName =
                    $"{SocialNetworks.Pinterest} {campaignDetails.SubModule} [{DateTime.Now.ToString(CultureInfo.InvariantCulture)}]";
            objDeletePins.IsEditCampaignName = isEditCampaignName;
            objDeletePins.CancelEditVisibility = Visibility.Visible;
            objDeletePins.CampaignButtonContent = campaignButtonContent;
            objDeletePins.SelectedAccountCount = campaignDetails.SelectedAccountList.Count +
                                                 $" {"LangKeyAccountSelected".FromResourceDictionary()}";
            objDeletePins.TemplateId = templateId;
            objDeletePins.DeletePinsFooter.list_SelectedAccounts = campaignDetails.SelectedAccountList;
            objDeletePins.CampaignName = campaignButtonContent == ConstantVariable.UpdateCampaign()
                ? campaignDetails.CampaignName
                : objDeletePins.CampaignName;

            objDeletePins.ObjViewModel.DeletePinModel =
                templateDetails.ActivitySettings.GetActivityModel<DeletePinModel>(objDeletePins.ObjViewModel.Model,
                    true);
            objDeletePins.MainGrid.DataContext = objDeletePins.ObjViewModel;
            TabSwitcher.ChangeTabIndex(3, 1);
        }
    }
}