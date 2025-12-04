using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using FaceDominatorCore.FDModel.ScraperModel;
using FaceDominatorCore.Interface;
using FaceDominatorCore.Utility;
using System;
using System.Globalization;
using System.Windows;

namespace FaceDominatorUI.Utilities.Scraper.PostScraper
{
    public class PostScraperViewCampaign : IFdViewCampaign
    {
        public void ManageCampaign(TemplateModel templateDetails, CampaignDetails campaignDetails,
            bool isEditCampaignName,
            Visibility cancelEditVisibility, string campaignButtonContent, string templateId)
        {
            var singeltonObjectPostScraper = FDViews.FbScraper.PostScraper.GetSingeltonObjectPostScraper();
            singeltonObjectPostScraper.IsEditCampaignName = isEditCampaignName;
            singeltonObjectPostScraper.CancelEditVisibility = cancelEditVisibility;
            singeltonObjectPostScraper.TemplateId = templateId;

            if (campaignButtonContent == ConstantVariable.CreateCampaign())
                singeltonObjectPostScraper.CampaignName =
                    $"{SocialNetworks.Facebook} {campaignDetails.SubModule} [{DateTime.Now.ToString(CultureInfo.InvariantCulture)}]";
            singeltonObjectPostScraper.CampaignName = campaignButtonContent == ConstantVariable.UpdateCampaign()
                ? campaignDetails.CampaignName
                : singeltonObjectPostScraper.CampaignName;

            //singeltonObjectPostScraper.CampaignName = campaignDetails.CampaignName;
            singeltonObjectPostScraper.CampaignButtonContent = campaignButtonContent;
            singeltonObjectPostScraper.SelectedAccountCount = campaignDetails.SelectedAccountList.Count +
                                                              $" {"LangKeyAccountSelected".FromResourceDictionary()}";
            singeltonObjectPostScraper.PostScraperFooter.list_SelectedAccounts = campaignDetails.SelectedAccountList;

            singeltonObjectPostScraper.ObjViewModel.PostScraperModel =
                templateDetails.ActivitySettings.GetActivityModelNonQueryList<PostScraperModel>(
                    singeltonObjectPostScraper.ObjViewModel.Model);

            singeltonObjectPostScraper.MainGrid.DataContext = singeltonObjectPostScraper.ObjViewModel;

            TabSwitcher.ChangeTabIndex(6, 5);
        }
    }
}