using System;
using System.Globalization;
using System.Windows;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using PinDominatorCore.Interface;
using PinDominatorCore.PDModel;
using userScraper = PinDominator.PDViews.PinScrape.UserScraper;

namespace PinDominator.Utility.PinScrap.User_Scraper
{
    public class UserScraperViewCampaign : IPdViewCampaign
    {
        public void ManageCampaign(TemplateModel templateDetails, CampaignDetails campaignDetails,
            bool isEditCampaignName,
            Visibility cancelEditVisibility, string campaignButtonContent, string templateId)
        {
            var objUserScraper = userScraper.GetSingeltonObjectUserScraper();
            if (campaignButtonContent == ConstantVariable.CreateCampaign())
                objUserScraper.CampaignName =
                    $"{SocialNetworks.Pinterest} {campaignDetails.SubModule} [{DateTime.Now.ToString(CultureInfo.InvariantCulture)}]";
            objUserScraper.IsEditCampaignName = isEditCampaignName;
            objUserScraper.CancelEditVisibility = Visibility.Visible;
            objUserScraper.CampaignButtonContent = campaignButtonContent;
            objUserScraper.SelectedAccountCount = campaignDetails.SelectedAccountList.Count +
                                                  $" {"LangKeyAccountSelected".FromResourceDictionary()}";
            objUserScraper.TemplateId = templateId;
            objUserScraper.UserScraperFooterControl.list_SelectedAccounts = campaignDetails.SelectedAccountList;
            objUserScraper.CampaignName = campaignButtonContent == ConstantVariable.UpdateCampaign()
                ? campaignDetails.CampaignName
                : objUserScraper.CampaignName;

            objUserScraper.ObjViewModel.UserScraperModel =
                templateDetails.ActivitySettings.GetActivityModel<UserScraperModel>(objUserScraper.ObjViewModel.Model);
            objUserScraper.MainGrid.DataContext = objUserScraper.ObjViewModel;
            TabSwitcher.ChangeTabIndex(6, 0);
        }
    }
}