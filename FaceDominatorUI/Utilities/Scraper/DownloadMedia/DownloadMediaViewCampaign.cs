using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using FaceDominatorCore.FDModel.ScraperModel;
using FaceDominatorCore.Interface;
using FaceDominatorCore.Utility;
using FaceDominatorUI.FDViews.FbScraper;
using System;
using System.Globalization;
using System.Windows;

namespace FaceDominatorUI.Utilities.Scraper.DownloadMedia
{
    public class DownloadMediaViewCampaign : IFdViewCampaign
    {
        public void ManageCampaign(TemplateModel templateDetails, CampaignDetails campaignDetails,
            bool isEditCampaignName,
            Visibility cancelEditVisibility, string campaignButtonContent, string templateId)
        {
            var singeltonObjectReplyToComment = DownloadPhotos.GetSingeltonObjectCurrentDownloadMedia();
            singeltonObjectReplyToComment.IsEditCampaignName = isEditCampaignName;
            singeltonObjectReplyToComment.CancelEditVisibility = cancelEditVisibility;
            singeltonObjectReplyToComment.TemplateId = templateId;

            if (campaignButtonContent == ConstantVariable.CreateCampaign())
                singeltonObjectReplyToComment.CampaignName =
                    $"{SocialNetworks.Facebook} {campaignDetails.SubModule} [{DateTime.Now.ToString(CultureInfo.InvariantCulture)}]";
            singeltonObjectReplyToComment.CampaignName = campaignButtonContent == ConstantVariable.UpdateCampaign()
                ? campaignDetails.CampaignName
                : singeltonObjectReplyToComment.CampaignName;

            //singeltonObjectReplyToComment.CampaignName = campaignDetails.CampaignName;
            singeltonObjectReplyToComment.CampaignButtonContent = campaignButtonContent;
            singeltonObjectReplyToComment.SelectedAccountCount =
                campaignDetails.SelectedAccountList.Count + $" {"LangKeyAccountSelected".FromResourceDictionary()}";
            singeltonObjectReplyToComment.PostScraperFooter.list_SelectedAccounts = campaignDetails.SelectedAccountList;

            singeltonObjectReplyToComment.ObjViewModel.DownloadPhotosModel
                = templateDetails.ActivitySettings.GetActivityModelNonQueryList<DownloadPhotosModel>(
                    singeltonObjectReplyToComment.ObjViewModel.Model);

            singeltonObjectReplyToComment.MainGrid.DataContext = singeltonObjectReplyToComment.ObjViewModel;

            TabSwitcher.ChangeTabIndex(6, 6);
        }
    }
}