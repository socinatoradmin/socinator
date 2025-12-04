using System;
using System.Globalization;
using System.Windows;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using GramDominatorCore.GDModel;
using GramDominatorCore.Interface;

namespace GramDominatorUI.Utility.InstaScrape.HashtagsScraper
{
    internal class HashtagsScraperViewCampaign : IGdViewCampaign
    {
        public void ManageCampaign(TemplateModel templateDetails, CampaignDetails campaignDetails,
            bool isEditCampaignName,
            Visibility cancelEditVisibility, string campaignButtonContent, string templateId)
        {
            var objHashtagsScraper = GDViews.InstaScrape.HashtagsScraper.GetSingeltonObjectHashtagsScraper();
            if (campaignButtonContent == ConstantVariable.CreateCampaign())
                objHashtagsScraper.CampaignName =
                    $"{SocialNetworks.Instagram} {campaignDetails.SubModule} [{DateTime.Now.ToString(CultureInfo.InvariantCulture)}]"; //added new line

            objHashtagsScraper.IsEditCampaignName = isEditCampaignName;
            objHashtagsScraper.CancelEditVisibility = cancelEditVisibility;
            objHashtagsScraper.TemplateId = templateId;
            objHashtagsScraper.CampaignButtonContent = campaignButtonContent;
            objHashtagsScraper.CampaignName = campaignButtonContent == ConstantVariable.UpdateCampaign()
                ? campaignDetails.CampaignName
                : objHashtagsScraper.CampaignName; //updated line          
            // objHashtagsScraper.CampaignName = campaignDetails.CampaignName;            
            objHashtagsScraper.SelectedAccountCount = campaignDetails.SelectedAccountList.Count +
                                                      $" {"LangKeyAccountSelected".FromResourceDictionary()}";
            objHashtagsScraper.HashtagsScraperFooter.list_SelectedAccounts = campaignDetails.SelectedAccountList;

            objHashtagsScraper.ObjViewModel.HashtagsScraperModel =
                templateDetails.ActivitySettings.GetActivityModel<HashtagsScraperModel>(
                    objHashtagsScraper.ObjViewModel.Model, true);

            objHashtagsScraper.MainGrid.DataContext = objHashtagsScraper.ObjViewModel;

            TabSwitcher.ChangeTabIndex(5, 1);
        }
    }
}