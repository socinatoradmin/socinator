using System;
using System.Globalization;
using System.Windows;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using GramDominatorCore.GDModel;
using GramDominatorCore.Interface;

namespace GramDominatorUI.Utility.InstaScrape.UserScraper
{
    internal class UserScraperViewCampaign : IGdViewCampaign
    {
        public void ManageCampaign(TemplateModel templateDetails, CampaignDetails campaignDetails,
            bool isEditCampaignName,
            Visibility cancelEditVisibility, string campaignButtonContent, string templateId)
        {
            var objUserScraper = GDViews.InstaScrape.UserScraper.GetSingeltonObjectUserScraper();
            if (campaignButtonContent == ConstantVariable.CreateCampaign())
                objUserScraper.CampaignName =
                    $"{SocialNetworks.Instagram} {campaignDetails.SubModule} [{DateTime.Now.ToString(CultureInfo.InvariantCulture)}]"; //added new line

            objUserScraper.IsEditCampaignName = isEditCampaignName;
            objUserScraper.CancelEditVisibility = cancelEditVisibility;
            objUserScraper.TemplateId = templateId;
            objUserScraper.CampaignButtonContent = campaignButtonContent;
            objUserScraper.CampaignName = campaignButtonContent == ConstantVariable.UpdateCampaign()
                ? campaignDetails.CampaignName
                : objUserScraper.CampaignName; //updated line          
            // objUserScraper.CampaignName = campaignDetails.CampaignName;           
            objUserScraper.SelectedAccountCount = campaignDetails.SelectedAccountList.Count +
                                                  $" {"LangKeyAccountSelected".FromResourceDictionary()}";
            objUserScraper.UserScraperFooter.list_SelectedAccounts = campaignDetails.SelectedAccountList;

            objUserScraper.ObjViewModel.UserScraperModel =
                templateDetails.ActivitySettings.GetActivityModel<UserScraperModel>(objUserScraper.ObjViewModel.Model);

            objUserScraper.MainGrid.DataContext = objUserScraper.ObjViewModel;

            TabSwitcher.ChangeTabIndex(5, 0);
        }
    }
}