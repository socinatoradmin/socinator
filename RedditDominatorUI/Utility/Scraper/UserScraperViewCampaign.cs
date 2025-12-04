using CommonServiceLocator;
using DominatorHouseCore.Enums;
using DominatorHouseCore.FileManagers;
using DominatorHouseCore.Utility;
using Newtonsoft.Json;
using RedditDominatorCore.RDModel;
using RedditDominatorCore.Utility;
using RedditDominatorUI.RDViews.UrlScraper;
using System;
using System.Globalization;
using System.Windows;

namespace RedditDominatorUI.Utility.Scraper
{
    public class UserScraperViewCampaign : IRdViewCampaign
    {
        public void ViewCampaigns(string campaignId, string openCampaignType)
        {
            var campaignFileManager = InstanceProvider.GetInstance<ICampaignsFileManager>();
            var campaignDetails = campaignFileManager.GetCampaignById(campaignId);
            var templatesFileManager = InstanceProvider.GetInstance<ITemplatesFileManager>();
            var templateDetails = templatesFileManager.GetTemplateById(campaignDetails.TemplateId);
            var objUserScraper = UserScraper.GetSingletonObjectUserScraper();
            if (openCampaignType == ConstantVariable.CreateCampaign())
                objUserScraper.CampaignName =
                    $"{SocialNetworks.Reddit} {campaignDetails.SubModule} [{DateTime.Now.ToString(CultureInfo.InvariantCulture)}]";
            objUserScraper.IsEditCampaignName = true;
            objUserScraper.CancelEditVisibility = Visibility.Visible;
            objUserScraper.TemplateId = campaignDetails.TemplateId;
            objUserScraper.CampaignName = openCampaignType == ConstantVariable.UpdateCampaign()
                ? campaignDetails.CampaignName
                : objUserScraper.CampaignName;
            objUserScraper.CampaignButtonContent = openCampaignType;
            objUserScraper.SelectedAccountCount = campaignDetails.SelectedAccountList.Count +
                                                  $" {"LangKeyAccountSelected".FromResourceDictionary()}";
            objUserScraper.UserScraperFooter.list_SelectedAccounts = campaignDetails.SelectedAccountList;
            objUserScraper.ObjViewModel.UserScraperModel =
                JsonConvert.DeserializeObject<UserScraperModel>(templateDetails.ActivitySettings);
            objUserScraper.MainGrid.DataContext = objUserScraper.ObjViewModel;
            TabSwitcher.ChangeTabIndex(6, 1);
        }
    }
}