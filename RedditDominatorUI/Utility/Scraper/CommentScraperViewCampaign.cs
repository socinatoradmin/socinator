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
    public class CommentScraperViewCampaign : IRdViewCampaign
    {
        public void ViewCampaigns(string campaignId, string openCampaignType)
        {
            var campaignFileManager = InstanceProvider.GetInstance<ICampaignsFileManager>();
            var campaignDetails = campaignFileManager.GetCampaignById(campaignId);
            var templatesFileManager = InstanceProvider.GetInstance<ITemplatesFileManager>();
            var templateDetails = templatesFileManager.GetTemplateById(campaignDetails.TemplateId);
            var objCommentScraper = CommentScraper.GetSingletonObjectCommentScraper();
            if (openCampaignType == ConstantVariable.CreateCampaign())
                objCommentScraper.CampaignName =
                    $"{SocialNetworks.Reddit} {campaignDetails.SubModule} [{DateTime.Now.ToString(CultureInfo.InvariantCulture)}]";
            objCommentScraper.IsEditCampaignName = true;
            objCommentScraper.CancelEditVisibility = Visibility.Visible;
            objCommentScraper.TemplateId = campaignDetails.TemplateId;
            objCommentScraper.CampaignName = openCampaignType == ConstantVariable.UpdateCampaign()
                ? campaignDetails.CampaignName
                : objCommentScraper.CampaignName;
            objCommentScraper.CampaignButtonContent = openCampaignType;
            objCommentScraper.SelectedAccountCount = campaignDetails.SelectedAccountList.Count +
                                                     $" {"LangKeyAccountSelected".FromResourceDictionary()}";
            objCommentScraper.CommentScraperFooter.list_SelectedAccounts = campaignDetails.SelectedAccountList;
            objCommentScraper.ObjViewModel.CommentScraperModel =
                JsonConvert.DeserializeObject<CommentScraperModel>(templateDetails.ActivitySettings);
            objCommentScraper.MainGrid.DataContext = objCommentScraper.ObjViewModel;
            TabSwitcher.ChangeTabIndex(6, 3);
        }
    }
}