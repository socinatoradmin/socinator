using System;
using System.Globalization;
using System.Windows;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using PinDominator.PDViews.PinScrape;
using PinDominatorCore.Interface;
using PinDominatorCore.PDModel;

namespace PinDominator.Utility.PinScrap.Board_Scraper
{
    public class BoardScraperViewCampagns : IPdViewCampaign
    {
        public void ManageCampaign(TemplateModel templateDetails, CampaignDetails campaignDetails,
            bool isEditCampaignName, Visibility cancelEditVisibility, string campaignButtonContent, string templateId)
        {
            var objBoardScraper = BoardScraper.GetSingeltonObjectBoardScraper();
            if (campaignButtonContent == ConstantVariable.CreateCampaign())
                objBoardScraper.CampaignName =
                    $"{SocialNetworks.Pinterest} {campaignDetails.SubModule} [{DateTime.Now.ToString(CultureInfo.InvariantCulture)}]";
            objBoardScraper.IsEditCampaignName = isEditCampaignName;
            objBoardScraper.CancelEditVisibility = Visibility.Visible;
            objBoardScraper.CampaignButtonContent = campaignButtonContent;
            objBoardScraper.SelectedAccountCount = campaignDetails.SelectedAccountList.Count +
                                                   $" {"LangKeyAccountSelected".FromResourceDictionary()}";
            objBoardScraper.TemplateId = templateId;
            objBoardScraper.BoardScraperFooterControl.list_SelectedAccounts = campaignDetails.SelectedAccountList;
            objBoardScraper.CampaignName = campaignButtonContent == ConstantVariable.UpdateCampaign()
                ? campaignDetails.CampaignName
                : objBoardScraper.CampaignName;

            objBoardScraper.ObjViewModel.BoardScraperModel =
                templateDetails.ActivitySettings.GetActivityModel<BoardScraperModel>(objBoardScraper.ObjViewModel
                    .Model);
            objBoardScraper.MainGrid.DataContext = objBoardScraper.ObjViewModel;
            TabSwitcher.ChangeTabIndex(6, 1);
        }
    }
}