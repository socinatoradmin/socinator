using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using FaceDominatorCore.FDModel.LikerCommentorModel;
using FaceDominatorCore.Interface;
using System;
using System.Globalization;
using System.Windows;

namespace FaceDominatorUI.Utilities.LikerCommentor.CommentLiker
{
    public class CommentLikerViewCampaign : IFdViewCampaign
    {
        public void ManageCampaign(TemplateModel templateDetails, CampaignDetails campaignDetails,
            bool isEditCampaignName,
            Visibility cancelEditVisibility, string campaignButtonContent, string templateId)
        {
            var singeltonObjectCommentScraper = FDViews.FbLikerCommentor.CommentLiker.GetSingeltonObjectCommentLiker();
            singeltonObjectCommentScraper.IsEditCampaignName = isEditCampaignName;
            singeltonObjectCommentScraper.CancelEditVisibility = cancelEditVisibility;
            singeltonObjectCommentScraper.TemplateId = templateId;

            if (campaignButtonContent == ConstantVariable.CreateCampaign())
                singeltonObjectCommentScraper.CampaignName =
                    $"{SocialNetworks.Facebook} {campaignDetails.SubModule} [{DateTime.Now.ToString(CultureInfo.InvariantCulture)}]";
            singeltonObjectCommentScraper.CampaignName = campaignButtonContent == ConstantVariable.UpdateCampaign()
                ? campaignDetails.CampaignName
                : singeltonObjectCommentScraper.CampaignName;

            //objCommentScraper.CampaignName = campaignDetails.CampaignName;
            singeltonObjectCommentScraper.CampaignButtonContent = campaignButtonContent;
            singeltonObjectCommentScraper.SelectedAccountCount =
                campaignDetails.SelectedAccountList.Count + $" {"LangKeyAccountSelected".FromResourceDictionary()}";
            singeltonObjectCommentScraper.CommentScraperFooter.list_SelectedAccounts =
                campaignDetails.SelectedAccountList;

            singeltonObjectCommentScraper.ObjViewModel.CommentLikerModule
                = templateDetails.ActivitySettings.GetActivityModel<CommentLikerModule>(singeltonObjectCommentScraper
                    .ObjViewModel.Model);
            ;

            singeltonObjectCommentScraper.MainGrid.DataContext = singeltonObjectCommentScraper.ObjViewModel;

            TabSwitcher.ChangeTabIndex(4, 3);
        }
    }
}