using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using Newtonsoft.Json;
using System;
using System.Globalization;
using System.Windows;
using TumblrDominatorCore.Interface;
using TumblrDominatorCore.Models;

namespace TumblrDominatorUI.Utility.UserScraper
{
    public class UserScraperViewCampaign : ITumblrViewCampaign
    {
        public void ManageCampaign(TemplateModel templateDetails, CampaignDetails campaignDetails,
            bool isEditCampaignName,
            Visibility cancelEditVisibility, string campaignButtonContent, string templateId)
        {
            var objUserScraper = TumblrView.Scraper.UserScraper.GetSingletonObjectUserScraperConfig();
            if (campaignButtonContent == ConstantVariable.CreateCampaign())
                objUserScraper.CampaignName =
                    $"{SocialNetworks.Tumblr} {campaignDetails.SubModule} [{DateTime.Now.ToString(CultureInfo.InvariantCulture)}]";
            objUserScraper.IsEditCampaignName = isEditCampaignName;
            objUserScraper.CancelEditVisibility = cancelEditVisibility;
            objUserScraper.TemplateId = templateId;
            objUserScraper.CampaignName = campaignDetails.CampaignName;
            objUserScraper.CampaignButtonContent = campaignButtonContent;
            objUserScraper.SelectedAccountCount = campaignDetails.SelectedAccountList.Count +
                                                  $" {"LangKeyAccountSelected".FromResourceDictionary()}";
            objUserScraper.UserScraperFooter.list_SelectedAccounts = campaignDetails.SelectedAccountList;
            objUserScraper.ObjViewModel.UserScraperModel =
                JsonConvert.DeserializeObject<UserScraperModel>(templateDetails.ActivitySettings);

            objUserScraper.MainGrid.DataContext = objUserScraper.ObjViewModel;

            TabSwitcher.ChangeTabIndex(5, 1);
        }
    }
}