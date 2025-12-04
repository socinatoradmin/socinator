using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using FaceDominatorCore.FDModel.ScraperModel;
using FaceDominatorCore.Interface;
using System;
using System.Globalization;
using System.Windows;

namespace FaceDominatorUI.Utilities.Scraper.GroupScraper
{
    public class GroupScraperViewCampaign : IFdViewCampaign
    {
        public void ManageCampaign(TemplateModel templateDetails, CampaignDetails campaignDetails,
            bool isEditCampaignName,
            Visibility cancelEditVisibility, string campaignButtonContent, string templateId)
        {
            var singeltonObjectGroupScraper = FDViews.FbScraper.GroupScraper.GetSingeltonObjectGroupScraper();
            singeltonObjectGroupScraper.IsEditCampaignName = isEditCampaignName;
            singeltonObjectGroupScraper.CancelEditVisibility = cancelEditVisibility;
            singeltonObjectGroupScraper.TemplateId = templateId;

            if (campaignButtonContent == ConstantVariable.CreateCampaign())
                singeltonObjectGroupScraper.CampaignName =
                    $"{SocialNetworks.Facebook} {campaignDetails.SubModule} [{DateTime.Now.ToString(CultureInfo.InvariantCulture)}]";
            singeltonObjectGroupScraper.CampaignName = campaignButtonContent == ConstantVariable.UpdateCampaign()
                ? campaignDetails.CampaignName
                : singeltonObjectGroupScraper.CampaignName;

            //singeltonObjectGroupScraper.CampaignName = campaignDetails.CampaignName;
            singeltonObjectGroupScraper.CampaignButtonContent = campaignButtonContent;
            singeltonObjectGroupScraper.SelectedAccountCount =
                campaignDetails.SelectedAccountList.Count + $" {"LangKeyAccountSelected".FromResourceDictionary()}";
            singeltonObjectGroupScraper.GroupScraperFooter.list_SelectedAccounts = campaignDetails.SelectedAccountList;

            singeltonObjectGroupScraper.ObjViewModel.GroupScraperModel =
                templateDetails.ActivitySettings.GetActivityModel<GroupScraperModel>(singeltonObjectGroupScraper
                    .ObjViewModel.Model);

            singeltonObjectGroupScraper.MainGrid.DataContext = singeltonObjectGroupScraper.ObjViewModel;

            TabSwitcher.ChangeTabIndex(6, 2);
        }
    }
}