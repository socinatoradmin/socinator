using System;
using System.Globalization;
using System.Windows;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using GramDominatorCore.GDModel;
using GramDominatorCore.Interface;

namespace GramDominatorUI.Utility.InstaScrape.DownloadPhotos
{
    internal class DownloadPhotosViewCampaign : IGdViewCampaign
    {
        public void ManageCampaign(TemplateModel templateDetails, CampaignDetails campaignDetails,
            bool isEditCampaignName,
            Visibility cancelEditVisibility, string campaignButtonContent, string templateId)
        {
            var objDownloadPhotos = GDViews.InstaScrape.DownloadPhotos.GetSingeltonObjectDownloadPhotos();
            if (campaignButtonContent == ConstantVariable.CreateCampaign())
                objDownloadPhotos.CampaignName =
                    $"{SocialNetworks.Instagram} {campaignDetails.SubModule} [{DateTime.Now.ToString(CultureInfo.InvariantCulture)}]"; //added new line


            objDownloadPhotos.IsEditCampaignName = isEditCampaignName;
            objDownloadPhotos.CancelEditVisibility = cancelEditVisibility;
            objDownloadPhotos.TemplateId = templateId;
            objDownloadPhotos.CampaignButtonContent = campaignButtonContent;
            objDownloadPhotos.CampaignName = campaignButtonContent == ConstantVariable.UpdateCampaign()
                ? campaignDetails.CampaignName
                : objDownloadPhotos.CampaignName; //updated line          
            //  objDownloadPhotos.CampaignName = campaignDetails.CampaignName;            
            objDownloadPhotos.SelectedAccountCount = campaignDetails.SelectedAccountList.Count +
                                                     $" {"LangKeyAccountSelected".FromResourceDictionary()}";
            objDownloadPhotos.DownloadPhotosFooterControl.list_SelectedAccounts = campaignDetails.SelectedAccountList;

            objDownloadPhotos.ObjViewModel.DownloadPhotosModel =
                templateDetails.ActivitySettings.GetActivityModel<DownloadPhotosModel>(objDownloadPhotos.ObjViewModel
                    .Model);

            objDownloadPhotos.MainGrid.DataContext = objDownloadPhotos.ObjViewModel;

            TabSwitcher.ChangeTabIndex(5, 2);
        }
    }
}