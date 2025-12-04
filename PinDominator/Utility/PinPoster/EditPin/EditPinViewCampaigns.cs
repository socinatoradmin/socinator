using System;
using System.Globalization;
using System.Windows;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using PinDominatorCore.Interface;
using PinDominatorCore.PDModel;
using editPin = PinDominator.PDViews.PinPoster.EditPin;

namespace PinDominator.Utility.PinPoster.EditPin
{
    public class EditPinViewCampaigns : IPdViewCampaign
    {
        public void ManageCampaign(TemplateModel templateDetails, CampaignDetails campaignDetails,
            bool isEditCampaignName, Visibility cancelEditVisibility, string campaignButtonContent, string templateId)
        {
            var objEditPin = editPin.GetSingletonObjectEditPin();
            if (campaignButtonContent == ConstantVariable.CreateCampaign())
                objEditPin.CampaignName =
                    $"{SocialNetworks.Pinterest} {campaignDetails.SubModule} [{DateTime.Now.ToString(CultureInfo.InvariantCulture)}]";
            objEditPin.IsEditCampaignName = isEditCampaignName;
            objEditPin.CancelEditVisibility = Visibility.Visible;
            objEditPin.CampaignButtonContent = campaignButtonContent;
            objEditPin.SelectedAccountCount = campaignDetails.SelectedAccountList.Count +
                                              $" {"LangKeyAccountSelected".FromResourceDictionary()}";
            objEditPin.TemplateId = templateId;
            objEditPin.EditPinFooterControl.list_SelectedAccounts = campaignDetails.SelectedAccountList;
            objEditPin.CampaignName = campaignButtonContent == ConstantVariable.UpdateCampaign()
                ? campaignDetails.CampaignName
                : objEditPin.CampaignName;

            objEditPin.ObjViewModel.EditPinModel =
                templateDetails.ActivitySettings.GetActivityModel<EditPinModel>(objEditPin.ObjViewModel.Model, true);
            objEditPin.MainGrid.DataContext = objEditPin.ObjViewModel;
            TabSwitcher.ChangeTabIndex(3, 2);
        }
    }
}