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
    public class CommentScraperViewCampaign : ITumblrViewCampaign
    {
        public void ManageCampaign(TemplateModel templateDetails, CampaignDetails campaignDetails,
            bool isEditCampaignName,
            Visibility cancelEditVisibility, string campaignButtonContent, string templateId)
        {
            var objCommentScraper = TumblrView.Scraper.CommentScraper.GetSingletonObjectCommentScraperConfig();

            if (campaignButtonContent == ConstantVariable.CreateCampaign())
                objCommentScraper.CampaignName =
                    $"{SocialNetworks.Tumblr} {campaignDetails.SubModule} [{DateTime.Now.ToString(CultureInfo.InvariantCulture)}]";

            objCommentScraper.IsEditCampaignName = isEditCampaignName;
            objCommentScraper.CancelEditVisibility = cancelEditVisibility;
            objCommentScraper.TemplateId = templateId;
            objCommentScraper.CampaignName = campaignDetails.CampaignName;
            objCommentScraper.CampaignButtonContent = campaignButtonContent;
            objCommentScraper.SelectedAccountCount = campaignDetails.SelectedAccountList.Count +
                                                     $" {"LangKeyAccountSelected".FromResourceDictionary()}";
            objCommentScraper.CommentScraperFooter.list_SelectedAccounts = campaignDetails.SelectedAccountList;
            objCommentScraper.ObjViewModel.CommentScraperModel =
                JsonConvert.DeserializeObject<CommentScraperModel>(templateDetails.ActivitySettings);

            objCommentScraper.MainGrid.DataContext = objCommentScraper.ObjViewModel;

            TabSwitcher.ChangeTabIndex(5, 2);
        }
    }
}