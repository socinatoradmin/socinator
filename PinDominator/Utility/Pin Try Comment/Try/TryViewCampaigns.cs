using System;
using System.Globalization;
using System.Windows;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using PinDominatorCore.Interface;
using PinDominatorCore.PDModel;
using TryView = PinDominator.PDViews.PinTryComment.Try;

namespace PinDominator.Utility.Pin_Try_Comment.Try
{
    public class TryViewCampaigns : IPdViewCampaign
    {
        public void ManageCampaign(TemplateModel templateDetails, CampaignDetails campaignDetails,
            bool isEditCampaignName, Visibility cancelEditVisibility, string campaignButtonContent, string templateId)
        {
            var objTry = TryView.GetSingeltonObjectTry();
            if (campaignButtonContent == ConstantVariable.CreateCampaign())
                objTry.CampaignName =
                    $"{SocialNetworks.Pinterest} {campaignDetails.SubModule} [{DateTime.Now.ToString(CultureInfo.InvariantCulture)}]";
            objTry.IsEditCampaignName = isEditCampaignName;
            objTry.CancelEditVisibility = Visibility.Visible;
            objTry.CampaignButtonContent = campaignButtonContent;
            objTry.SelectedAccountCount = campaignDetails.SelectedAccountList.Count +
                                          $" {"LangKeyAccountSelected".FromResourceDictionary()}";
            objTry.TemplateId = templateId;
            objTry.TryFooter.list_SelectedAccounts = campaignDetails.SelectedAccountList;
            objTry.CampaignName = campaignButtonContent == ConstantVariable.UpdateCampaign()
                ? campaignDetails.CampaignName
                : objTry.CampaignName;

            objTry.ObjViewModel.TryModel =
                templateDetails.ActivitySettings.GetActivityModel<TryModel>(objTry.ObjViewModel.Model);
            objTry.MainGrid.DataContext = objTry.ObjViewModel;
            TabSwitcher.ChangeTabIndex(4, 1);
        }
    }
}