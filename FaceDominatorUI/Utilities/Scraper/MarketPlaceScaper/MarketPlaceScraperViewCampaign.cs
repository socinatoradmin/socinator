using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using FaceDominatorCore.FDModel.ScraperModel;
using FaceDominatorCore.Interface;
using FaceDominatorUI.FDViews.FbScraper;
using System;
using System.Globalization;
using System.Windows;

namespace FaceDominatorUI.Utilities.Scraper.MarketPlaceScaper
{
    public class MarketPlaceScraperViewCampaign : IFdViewCampaign
    {
        public void ManageCampaign(TemplateModel templateDetails, CampaignDetails campaignDetails,
            bool isEditCampaignName,
            Visibility cancelEditVisibility, string campaignButtonContent, string templateId)
        {
            var singeltonObjectMarketPlaceScraper = MarketPlaceScraper.GetSingeltonObjectMarketplaceScraper();
            singeltonObjectMarketPlaceScraper.IsEditCampaignName = isEditCampaignName;
            singeltonObjectMarketPlaceScraper.CancelEditVisibility = cancelEditVisibility;
            singeltonObjectMarketPlaceScraper.TemplateId = templateId;

            if (campaignButtonContent == ConstantVariable.CreateCampaign())
                singeltonObjectMarketPlaceScraper.CampaignName =
                    $"{SocialNetworks.Facebook} {campaignDetails.SubModule} [{DateTime.Now.ToString(CultureInfo.InvariantCulture)}]";
            singeltonObjectMarketPlaceScraper.CampaignName = campaignButtonContent == ConstantVariable.UpdateCampaign()
                ? campaignDetails.CampaignName
                : singeltonObjectMarketPlaceScraper.CampaignName;

            //singeltonObjectMarketPlaceScraper.CampaignName = campaignDetails.CampaignName;
            singeltonObjectMarketPlaceScraper.CampaignButtonContent = campaignButtonContent;
            singeltonObjectMarketPlaceScraper.SelectedAccountCount =
                campaignDetails.SelectedAccountList.Count + $" {"LangKeyAccountSelected".FromResourceDictionary()}";
            singeltonObjectMarketPlaceScraper.MarketplaceScraperFooter.list_SelectedAccounts =
                campaignDetails.SelectedAccountList;

            singeltonObjectMarketPlaceScraper.ObjViewModel.MarketPlaceScraperModel =
                templateDetails.ActivitySettings.GetActivityModel<MarketPlaceScraperModel>(
                    singeltonObjectMarketPlaceScraper.ObjViewModel.Model);

            singeltonObjectMarketPlaceScraper.MainGrid.DataContext = singeltonObjectMarketPlaceScraper.ObjViewModel;

            TabSwitcher.ChangeTabIndex(6, 6);
        }
    }
}