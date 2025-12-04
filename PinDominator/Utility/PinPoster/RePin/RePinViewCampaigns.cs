using System;
using System.Globalization;
using System.Windows;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using PinDominatorCore.Interface;
using PinDominatorCore.PDModel;
using rePin = PinDominator.PDViews.PinPoster.RePin;

namespace PinDominator.Utility.PinPoster.RePin
{
    public class RePinViewCampaigns : IPdViewCampaign
    {
        public void ManageCampaign(TemplateModel templateDetails, CampaignDetails campaignDetails,
            bool isEditCampaignName, Visibility cancelEditVisibility, string campaignButtonContent, string templateId)
        {
            var objrePin = rePin.GetSingletonObjectRePin﻿();
            if (campaignButtonContent == ConstantVariable.CreateCampaign())
                objrePin.CampaignName =
                    $"{SocialNetworks.Pinterest} {campaignDetails.SubModule} [{DateTime.Now.ToString(CultureInfo.InvariantCulture)}]";
            objrePin.IsEditCampaignName = isEditCampaignName;
            objrePin.CancelEditVisibility = Visibility.Visible;
            objrePin.CampaignButtonContent = campaignButtonContent;
            objrePin.SelectedAccountCount = campaignDetails.SelectedAccountList.Count +
                                            $" {"LangKeyAccountSelected".FromResourceDictionary()}";
            objrePin.TemplateId = templateId;
            objrePin.RePinFooterControl.list_SelectedAccounts = campaignDetails.SelectedAccountList;
            objrePin.CampaignName = campaignButtonContent == ConstantVariable.UpdateCampaign()
                ? campaignDetails.CampaignName
                : objrePin.CampaignName;

            objrePin.ObjViewModel.RePinModel =
                templateDetails.ActivitySettings.GetActivityModel<RePinModel>(objrePin.ObjViewModel.Model);
            objrePin.MainGrid.DataContext = objrePin.ObjViewModel;
            TabSwitcher.ChangeTabIndex(3, 0);
        }
    }
}