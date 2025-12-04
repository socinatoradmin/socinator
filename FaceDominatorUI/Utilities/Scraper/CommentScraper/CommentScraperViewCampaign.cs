using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using FaceDominatorCore.FDModel.ScraperModel;
using FaceDominatorCore.Interface;
using System;
using System.Globalization;
using System.Windows;

namespace FaceDominatorUI.Utilities.Scraper.CommentScraper
{
    public class CommentScraperViewCampaign : IFdViewCampaign
    {
        public void ManageCampaign(TemplateModel templateDetails, CampaignDetails campaignDetails,
            bool isEditCampaignName,
            Visibility cancelEditVisibility, string campaignButtonContent, string templateId)
        {
            var singeltonObjectCommentScraper = FDViews.FbScraper.CommentScraper.GetSingeltonObjectCommentScraper();
            singeltonObjectCommentScraper.IsEditCampaignName = isEditCampaignName;
            singeltonObjectCommentScraper.CancelEditVisibility = cancelEditVisibility;
            singeltonObjectCommentScraper.TemplateId = templateId;

            if (campaignButtonContent == ConstantVariable.CreateCampaign())
                singeltonObjectCommentScraper.CampaignName =
                    $"{SocialNetworks.Facebook} {campaignDetails.SubModule} [{DateTime.Now.ToString(CultureInfo.InvariantCulture)}]";
            singeltonObjectCommentScraper.CampaignName = campaignButtonContent == ConstantVariable.UpdateCampaign()
                ? campaignDetails.CampaignName
                : singeltonObjectCommentScraper.CampaignName;

            //singeltonObjectCommentScraper.CampaignName = campaignDetails.CampaignName;
            singeltonObjectCommentScraper.CampaignButtonContent = campaignButtonContent;
            singeltonObjectCommentScraper.SelectedAccountCount =
                campaignDetails.SelectedAccountList.Count + $" {"LangKeyAccountSelected".FromResourceDictionary()}";
            singeltonObjectCommentScraper.CommentScraperFooter.list_SelectedAccounts =
                campaignDetails.SelectedAccountList;

            singeltonObjectCommentScraper.ObjViewModel.CommentScraperModel
                = templateDetails.ActivitySettings.GetActivityModel<CommentScraperModel>(singeltonObjectCommentScraper
                    .ObjViewModel.Model);

            singeltonObjectCommentScraper.MainGrid.DataContext = singeltonObjectCommentScraper.ObjViewModel;

            TabSwitcher.ChangeTabIndex(6, 3);
        }
    }
}