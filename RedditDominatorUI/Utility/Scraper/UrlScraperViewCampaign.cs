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
    public class UrlScraperViewCampaign : IRdViewCampaign
    {
        public void ViewCampaigns(string campaignId, string openCampaignType)
        {
            var campaignFileManager = InstanceProvider.GetInstance<ICampaignsFileManager>();
            var campaignDetails = campaignFileManager.GetCampaignById(campaignId);
            var templatesFileManager = InstanceProvider.GetInstance<ITemplatesFileManager>();
            var templateDetails = templatesFileManager.GetTemplateById(campaignDetails.TemplateId);
            var objUrlScraper = UrlScraper.GetSingletonObjectUrlScraper();
            if (openCampaignType == ConstantVariable.CreateCampaign())
                objUrlScraper.CampaignName =
                    $"{SocialNetworks.Reddit} {campaignDetails.SubModule} [{DateTime.Now.ToString(CultureInfo.InvariantCulture)}]";
            objUrlScraper.IsEditCampaignName = true;
            objUrlScraper.CancelEditVisibility = Visibility.Visible;
            objUrlScraper.TemplateId = campaignDetails.TemplateId;
            objUrlScraper.CampaignName = openCampaignType == ConstantVariable.UpdateCampaign()
                ? campaignDetails.CampaignName
                : objUrlScraper.CampaignName;
            objUrlScraper.CampaignButtonContent = openCampaignType;
            objUrlScraper.SelectedAccountCount = campaignDetails.SelectedAccountList.Count +
                                                 $" {"LangKeyAccountSelected".FromResourceDictionary()}";
            objUrlScraper.UrlScraperFooter.list_SelectedAccounts = campaignDetails.SelectedAccountList;
            objUrlScraper.ObjViewModel.UrlScraperModel =
                JsonConvert.DeserializeObject<UrlScraperModel>(templateDetails.ActivitySettings);
            objUrlScraper.MainGrid.DataContext = objUrlScraper.ObjViewModel;
            TabSwitcher.ChangeTabIndex(6, 0);
        }
    }
}