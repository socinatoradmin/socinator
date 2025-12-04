using System;
using System.Globalization;
using System.Windows;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using GramDominatorCore.GDModel;
using GramDominatorCore.Interface;

namespace GramDominatorUI.Utility.InstaScrape.CommentScraper
{
    public class CommentScraperViewCampaign : IGdViewCampaign
    {
        public void ManageCampaign(TemplateModel templateDetails, CampaignDetails campaignDetails,
            bool isEditCampaignName,
            Visibility cancelEditVisibility, string campaignButtonContent, string templateId)
        {
            var objCommentScraper = GDViews.InstaScrape.CommentScraper.GetSingeltonObjectCommentScraper();
            if (campaignButtonContent == ConstantVariable.CreateCampaign())
                objCommentScraper.CampaignName =
                    $"{SocialNetworks.Instagram} {campaignDetails.SubModule} [{DateTime.Now.ToString(CultureInfo.InvariantCulture)}]"; //added new line

            objCommentScraper.IsEditCampaignName = isEditCampaignName;
            objCommentScraper.CancelEditVisibility = cancelEditVisibility;
            objCommentScraper.TemplateId = templateId;
            objCommentScraper.CampaignButtonContent = campaignButtonContent;
            objCommentScraper.CampaignName = campaignButtonContent == ConstantVariable.UpdateCampaign()
                ? campaignDetails.CampaignName
                : objCommentScraper.CampaignName; //updated line          
            // objCommentScraper.CampaignName = campaignDetails.CampaignName;         
            objCommentScraper.SelectedAccountCount = campaignDetails.SelectedAccountList.Count +
                                                     $" {"LangKeyAccountSelected".FromResourceDictionary()}";
            objCommentScraper.CommentScraperFooter.list_SelectedAccounts = campaignDetails.SelectedAccountList;

            objCommentScraper.ObjViewModel.CommentScraperModel =
                templateDetails.ActivitySettings.GetActivityModel<CommentScraperModel>(objCommentScraper.ObjViewModel
                    .Model);

            objCommentScraper.MainGrid.DataContext = objCommentScraper.ObjViewModel;

            TabSwitcher.ChangeTabIndex(5, 3);
        }
    }
}