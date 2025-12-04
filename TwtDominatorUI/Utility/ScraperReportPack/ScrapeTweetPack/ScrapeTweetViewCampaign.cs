using System;
using System.Globalization;
using System.Windows;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using TwtDominatorCore.Interface;
using TwtDominatorCore.TDModels;
using TwtDominatorUI.TDViews.Scraper;

namespace TwtDominatorUI.Utility.ScraperReportPack.ScrapeTweetPack
{
    public class ScrapeTweetViewCampaign : ITDViewCampaign
    {
        public void ManageCampaign(TemplateModel templateDetails, CampaignDetails campaignDetails,
            bool isEditCampaignName,
            Visibility cancelEditVisibility, string campaignButtonContent, string templateId)
        {
            var objScrapeTweet = ScrapeTweet.GetSingletonObjectScrapeTweet();
            if (campaignButtonContent == ConstantVariable.CreateCampaign())
                objScrapeTweet.CampaignName =
                    $"{SocialNetworks.Twitter} {campaignDetails.SubModule} [{DateTime.Now.ToString(CultureInfo.InvariantCulture)}]";

            objScrapeTweet.IsEditCampaignName = isEditCampaignName;
            objScrapeTweet.CancelEditVisibility = cancelEditVisibility;
            objScrapeTweet.TemplateId = templateId;

            objScrapeTweet.CampaignName = campaignButtonContent == ConstantVariable.UpdateCampaign()
                ? campaignDetails.CampaignName
                : objScrapeTweet.CampaignName;


            objScrapeTweet.CampaignButtonContent = campaignButtonContent;
            objScrapeTweet.SelectedAccountCount = campaignDetails.SelectedAccountList.Count +
                                                  $" {"LangKeyAccountSelected".FromResourceDictionary()}";
            objScrapeTweet.ScrapeTweetFooter.list_SelectedAccounts = campaignDetails.SelectedAccountList;

            // Doubt FollowerModel FollowBack
            objScrapeTweet.ObjViewModel.ScrapeTweetModel =
                templateDetails.ActivitySettings.GetActivityModel<ScrapeTweetModel>(objScrapeTweet.ObjViewModel.Model);

            objScrapeTweet.MainGrid.DataContext = objScrapeTweet.ObjViewModel;

            TabSwitcher.ChangeTabIndex(5, 1);
        }
    }
}