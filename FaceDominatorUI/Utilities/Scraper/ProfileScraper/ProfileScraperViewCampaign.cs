using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using FaceDominatorCore.FDModel.ScraperModel;
using FaceDominatorCore.Interface;
using System;
using System.Globalization;
using System.Windows;

namespace FaceDominatorUI.Utilities.Scraper.ProfileScraper
{
    public class ProfileScraperViewCampaign : IFdViewCampaign
    {
        public void ManageCampaign(TemplateModel templateDetails, CampaignDetails campaignDetails,
            bool isEditCampaignName,
            Visibility cancelEditVisibility, string campaignButtonContent, string templateId)
        {
            var singeltonObjectProfileScraper = FDViews.FbScraper.ProfileScraper.GetSingeltonObjectProfileScraper();
            singeltonObjectProfileScraper.IsEditCampaignName = isEditCampaignName;
            singeltonObjectProfileScraper.CancelEditVisibility = cancelEditVisibility;
            singeltonObjectProfileScraper.TemplateId = templateId;

            if (campaignButtonContent == ConstantVariable.CreateCampaign())
                singeltonObjectProfileScraper.CampaignName =
                    $"{SocialNetworks.Facebook} {campaignDetails.SubModule} [{DateTime.Now.ToString(CultureInfo.InvariantCulture)}]";
            singeltonObjectProfileScraper.CampaignName = campaignButtonContent == ConstantVariable.UpdateCampaign()
                ? campaignDetails.CampaignName
                : singeltonObjectProfileScraper.CampaignName;

            //singeltonObjectProfileScraper.CampaignName = campaignDetails.CampaignName;
            singeltonObjectProfileScraper.CampaignButtonContent = campaignButtonContent;
            singeltonObjectProfileScraper.SelectedAccountCount =
                campaignDetails.SelectedAccountList.Count + $" {"LangKeyAccountSelected".FromResourceDictionary()}";
            singeltonObjectProfileScraper.ProfileScraperFooter.list_SelectedAccounts =
                campaignDetails.SelectedAccountList;

            singeltonObjectProfileScraper.ObjViewModel.ProfileScraperModel =
                templateDetails.ActivitySettings.GetActivityModel<ProfileScraperModel>(singeltonObjectProfileScraper
                    .ObjViewModel.Model);

            singeltonObjectProfileScraper.MainGrid.DataContext = singeltonObjectProfileScraper.ObjViewModel;

            TabSwitcher.ChangeTabIndex(6, 0);
        }
    }
}