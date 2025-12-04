using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using FaceDominatorCore.FDModel.ScraperModel;
using FaceDominatorCore.Interface;
using System;
using System.Globalization;
using System.Windows;

namespace FaceDominatorUI.Utilities.Scraper.FanpageScraper
{
    public class FanpageScraperViewCampaign : IFdViewCampaign
    {
        public void ManageCampaign(TemplateModel templateDetails, CampaignDetails campaignDetails,
            bool isEditCampaignName,
            Visibility cancelEditVisibility, string campaignButtonContent, string templateId)
        {
            var singeltonObjectFanpageScraper = FDViews.FbScraper.FanpageScraper.GetSingeltonObjectFanpageScraper();
            singeltonObjectFanpageScraper.IsEditCampaignName = isEditCampaignName;
            singeltonObjectFanpageScraper.CancelEditVisibility = cancelEditVisibility;
            singeltonObjectFanpageScraper.TemplateId = templateId;

            if (campaignButtonContent == ConstantVariable.CreateCampaign())
                singeltonObjectFanpageScraper.CampaignName =
                    $"{SocialNetworks.Facebook} {campaignDetails.SubModule} [{DateTime.Now.ToString(CultureInfo.InvariantCulture)}]";
            singeltonObjectFanpageScraper.CampaignName = campaignButtonContent == ConstantVariable.UpdateCampaign()
                ? campaignDetails.CampaignName
                : singeltonObjectFanpageScraper.CampaignName;

            //singeltonObjectFanpageScraper.CampaignName = campaignDetails.CampaignName;
            singeltonObjectFanpageScraper.CampaignButtonContent = campaignButtonContent;
            singeltonObjectFanpageScraper.SelectedAccountCount =
                campaignDetails.SelectedAccountList.Count + $" {"LangKeyAccountSelected".FromResourceDictionary()}";
            singeltonObjectFanpageScraper.FanpageScraperFooter.list_SelectedAccounts =
                campaignDetails.SelectedAccountList;

            singeltonObjectFanpageScraper.ObjViewModel.FanpageScraperModel
                = templateDetails.ActivitySettings.GetActivityModel<FanpageScraperModel>(singeltonObjectFanpageScraper
                    .ObjViewModel.Model);

            singeltonObjectFanpageScraper.MainGrid.DataContext = singeltonObjectFanpageScraper.ObjViewModel;

            TabSwitcher.ChangeTabIndex(6, 1);
        }
    }
}