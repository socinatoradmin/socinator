using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using FaceDominatorCore.FDModel.MessageModel;
using FaceDominatorCore.Interface;
using FaceDominatorUI.FDViews.FbScraper;
using System;
using System.Globalization;
using System.Windows;

namespace FaceDominatorUI.Utilities.Messanger.SendGreetingsToFriends
{
    public class PlaceScraperViewCampaign : IFdViewCampaign
    {
        public void ManageCampaign(TemplateModel templateDetails, CampaignDetails campaignDetails,
            bool isEditCampaignName, Visibility cancelEditVisibility, string campaignButtonContent, string templateId)
        {
            var singeltonObjectPlaceScraper = PlaceScraper.GetSingeltonObjectMessageToPlaces();
            singeltonObjectPlaceScraper.IsEditCampaignName = isEditCampaignName;
            singeltonObjectPlaceScraper.CancelEditVisibility = cancelEditVisibility;
            singeltonObjectPlaceScraper.TemplateId = templateId;

            if (campaignButtonContent == ConstantVariable.CreateCampaign())
                singeltonObjectPlaceScraper.CampaignName =
                    $"{SocialNetworks.Facebook} {campaignDetails.SubModule} [{DateTime.Now.ToString(CultureInfo.InvariantCulture)}]";
            singeltonObjectPlaceScraper.CampaignName = campaignButtonContent == ConstantVariable.UpdateCampaign()
                ? campaignDetails.CampaignName
                : singeltonObjectPlaceScraper.CampaignName;

            //singeltonObjectPlaceScraper.CampaignName = campaignDetails.CampaignName;
            singeltonObjectPlaceScraper.CampaignButtonContent = campaignButtonContent;
            singeltonObjectPlaceScraper.SelectedAccountCount =
                campaignDetails.SelectedAccountList.Count + $" {"LangKeyAccountSelected".FromResourceDictionary()}";
            singeltonObjectPlaceScraper.MessageToFanpageFooter.list_SelectedAccounts =
                campaignDetails.SelectedAccountList;

            singeltonObjectPlaceScraper.ObjViewModel.PlaceScraperModel =
                templateDetails.ActivitySettings.GetActivityModel<PlaceScraperModel>(singeltonObjectPlaceScraper
                    .ObjViewModel.Model);

            singeltonObjectPlaceScraper.MainGrid.DataContext = singeltonObjectPlaceScraper.ObjViewModel;

            TabSwitcher.ChangeTabIndex(6, 7);
        }
    }
}