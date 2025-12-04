using System;
using System.Globalization;
using System.Windows;
using CommonServiceLocator;
using DominatorHouseCore.Enums;
using DominatorHouseCore.FileManagers;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Utility;
using Newtonsoft.Json;
using QuoraDominatorCore.Models;
using QuoraDominatorUI.QDViews.Scrape;

namespace QuoraDominatorUI.Utility.ViewCampaign.Scrape
{
    public class UserScraperViewCampaign : IViewCampaignsFactory
    {
        public void ViewCampaigns(string campaignId, string openCampaignType)
        {
            var campaignFileManager = InstanceProvider.GetInstance<ICampaignsFileManager>();
            var campaignDetails = campaignFileManager.GetCampaignById(campaignId);
            var templatesFileManager = InstanceProvider.GetInstance<ITemplatesFileManager>();
            var templateDetails = templatesFileManager.GetTemplateById(campaignDetails.TemplateId);
            var userScraper = UserScraper.GetSingeltonObjectUserScraper();
            if (openCampaignType == ConstantVariable.CreateCampaign())
                userScraper.CampaignName =
                    $"{SocialNetworks.Quora} {campaignDetails.SubModule} [{DateTime.Now.ToString(CultureInfo.InvariantCulture)}]";

            userScraper.IsEditCampaignName = true;
            userScraper.CancelEditVisibility = Visibility.Visible;
            userScraper.TemplateId = campaignDetails.TemplateId;
            userScraper.CampaignButtonContent = openCampaignType;
            userScraper.CampaignName = openCampaignType == ConstantVariable.UpdateCampaign()
                ? campaignDetails.CampaignName
                : userScraper.CampaignName;
            userScraper.SelectedAccountCount = campaignDetails.SelectedAccountList.Count +
                                               $" {"LangKeyAccountSelected".FromResourceDictionary()}";
            userScraper.UserScraperFooter.list_SelectedAccounts = campaignDetails.SelectedAccountList;
            userScraper.ObjViewModel.UserScraperModel =
                JsonConvert.DeserializeObject<UserScraperModel>(templateDetails.ActivitySettings);
            userScraper.MainGrid.DataContext = userScraper.ObjViewModel;
            TabSwitcher.ChangeTabIndex(4, 0);
        }
    }
}