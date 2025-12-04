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
    internal class ChannelScraperViewCampaign : IRdViewCampaign
    {
        public void ViewCampaigns(string campaignId, string openCampaignType)
        {
            var campaignFileManager = InstanceProvider.GetInstance<ICampaignsFileManager>();
            var campaignDetails = campaignFileManager.GetCampaignById(campaignId);
            var templatesFileManager = InstanceProvider.GetInstance<ITemplatesFileManager>();
            var templateDetails = templatesFileManager.GetTemplateById(campaignDetails.TemplateId);
            var objChannelScraper = ChannelScraper.GetSingletonObjectChannelScraper();
            if (openCampaignType == ConstantVariable.CreateCampaign())
                objChannelScraper.CampaignName =
                    $"{SocialNetworks.Reddit} {campaignDetails.SubModule} [{DateTime.Now.ToString(CultureInfo.InvariantCulture)}]";
            objChannelScraper.IsEditCampaignName = true;
            objChannelScraper.CancelEditVisibility = Visibility.Visible;
            objChannelScraper.TemplateId = campaignDetails.TemplateId;
            objChannelScraper.CampaignName = openCampaignType == ConstantVariable.UpdateCampaign()
                ? campaignDetails.CampaignName
                : objChannelScraper.CampaignName;
            objChannelScraper.CampaignButtonContent = openCampaignType;
            objChannelScraper.SelectedAccountCount = campaignDetails.SelectedAccountList.Count +
                                                     $" {"LangKeyAccountSelected".FromResourceDictionary()}";
            objChannelScraper.ChannelScraperFooter.list_SelectedAccounts = campaignDetails.SelectedAccountList;
            objChannelScraper.ObjViewModel.ChannelScraperModel =
                JsonConvert.DeserializeObject<ChannelScraperModel>(templateDetails.ActivitySettings);
            objChannelScraper.MainGrid.DataContext = objChannelScraper.ObjViewModel;
            TabSwitcher.ChangeTabIndex(6, 2);
        }
    }
}