using System;
using System.Globalization;
using System.Windows;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using PinDominatorCore.Interface;
using PinDominatorCore.PDModel;
using pinScraper = PinDominator.PDViews.PinScrape.PinScraper;

namespace PinDominator.Utility.PinScrap.Pin_Scraper
{
    public class PinScraperViewCampaigns : IPdViewCampaign
    {
        public void ManageCampaign(TemplateModel templateDetails, CampaignDetails campaignDetails,
            bool isEditCampaignName, Visibility cancelEditVisibility, string campaignButtonContent, string templateId)
        {
            var objpinScraper = pinScraper.GetSingletonObjectPinScraper();
            if (campaignButtonContent == ConstantVariable.CreateCampaign())
                objpinScraper.CampaignName =
                    $"{SocialNetworks.Pinterest} {campaignDetails.SubModule} [{DateTime.Now.ToString(CultureInfo.InvariantCulture)}]";
            objpinScraper.IsEditCampaignName = isEditCampaignName;
            objpinScraper.CancelEditVisibility = Visibility.Visible;
            objpinScraper.CampaignButtonContent = campaignButtonContent;
            objpinScraper.SelectedAccountCount = campaignDetails.SelectedAccountList.Count +
                                                 $" {"LangKeyAccountSelected".FromResourceDictionary()}";
            objpinScraper.TemplateId = templateId;
            objpinScraper.PinScraperFooterControl.list_SelectedAccounts = campaignDetails.SelectedAccountList;
            objpinScraper.CampaignName = campaignButtonContent == ConstantVariable.UpdateCampaign()
                ? campaignDetails.CampaignName
                : objpinScraper.CampaignName;

            objpinScraper.ObjViewModel.PinScraperModel =
                templateDetails.ActivitySettings.GetActivityModel<PinScraperModel>(objpinScraper.ObjViewModel.Model);
            objpinScraper.MainGrid.DataContext = objpinScraper.ObjViewModel;
            TabSwitcher.ChangeTabIndex(6, 2);
        }
    }
}