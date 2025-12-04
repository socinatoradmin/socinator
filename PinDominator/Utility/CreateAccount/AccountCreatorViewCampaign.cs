using System;
using System.Globalization;
using System.Windows;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using PinDominator.PDViews.CreateAccount;
using PinDominatorCore.Interface;
using PinDominatorCore.PDModel;

namespace PinDominator.Utility.CreateAccount
{
    public class AccountCreatorViewCampaign : IPdViewCampaign
    {
        public void ManageCampaign(TemplateModel templateDetails, CampaignDetails campaignDetails,
            bool isEditCampaignName, Visibility cancelEditVisibility, string campaignButtonContent, string templateId)
        {
            var objAccountCreator = AccountCreator.GetSingletonAccountCreator();
            if (campaignButtonContent == ConstantVariable.CreateCampaign())
                objAccountCreator.CampaignName =
                    $"{SocialNetworks.Pinterest} {campaignDetails.SubModule} [{DateTime.Now.ToString(CultureInfo.InvariantCulture)}]";
            objAccountCreator.IsEditCampaignName = isEditCampaignName;
            objAccountCreator.CancelEditVisibility = Visibility.Visible;
            objAccountCreator.CampaignButtonContent = campaignButtonContent;
            objAccountCreator.SelectedAccountCount = campaignDetails.SelectedAccountList.Count + $" {"LangKeyAccountSelected".FromResourceDictionary()}";
            objAccountCreator.TemplateId = templateId;
            objAccountCreator._footerControl.list_SelectedAccounts = campaignDetails.SelectedAccountList;
            objAccountCreator.CampaignName = campaignButtonContent == ConstantVariable.UpdateCampaign()
                ? campaignDetails.CampaignName
                : objAccountCreator.CampaignName;

            objAccountCreator.ObjViewModel.AccountCreatorModel =
                templateDetails.ActivitySettings.GetActivityModel<AccountCreatorModel>(
                    objAccountCreator.ObjViewModel.Model, true);
            objAccountCreator.MainGrid.DataContext = objAccountCreator.ObjViewModel;
            TabSwitcher.ChangeTabIndex(0, 2);
        }
    }
}