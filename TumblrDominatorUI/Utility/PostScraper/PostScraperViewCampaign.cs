using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using Newtonsoft.Json;
using System;
using System.Globalization;
using System.Windows;
using TumblrDominatorCore.Interface;
using TumblrDominatorCore.Models;

namespace TumblrDominatorUI.Utility.PostScraper
{
    public class PostScraperViewCampaign : ITumblrViewCampaign
    {
        public void ManageCampaign(TemplateModel templateDetails, CampaignDetails campaignDetails,
            bool isEditCampaignName,
            Visibility cancelEditVisibility, string campaignButtonContent, string templateId)
        {
            var objPostScraper = TumblrView.Scraper.PostScraper.GetSingletonObjectPostScraperConfig();
            if (campaignButtonContent == ConstantVariable.CreateCampaign())
                objPostScraper.CampaignName =
                    $"{SocialNetworks.Tumblr} {campaignDetails.SubModule} [{DateTime.Now.ToString(CultureInfo.InvariantCulture)}]";

            objPostScraper.IsEditCampaignName = isEditCampaignName;
            objPostScraper.CancelEditVisibility = cancelEditVisibility;
            objPostScraper.TemplateId = templateId;
            objPostScraper.CampaignName = campaignDetails.CampaignName;
            objPostScraper.CampaignButtonContent = campaignButtonContent;
            objPostScraper.SelectedAccountCount = campaignDetails.SelectedAccountList.Count +
                                                  $" {"LangKeyAccountSelected".FromResourceDictionary()}";
            objPostScraper.PostScraperFooter.list_SelectedAccounts = campaignDetails.SelectedAccountList;
            objPostScraper.ObjViewModel.PostScraperModel =
                JsonConvert.DeserializeObject<PostScraperModel>(templateDetails.ActivitySettings);

            objPostScraper.MainGrid.DataContext = objPostScraper.ObjViewModel;

            TabSwitcher.ChangeTabIndex(5, 0);
        }
    }
}