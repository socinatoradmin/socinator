using System;
using System.Globalization;
using System.Windows;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using TwtDominatorCore.Interface;
using TwtDominatorCore.TDModels;
using TwtDominatorUI.TDViews.Scraper;

namespace TwtDominatorUI.Utility.ScraperReportPack.ScrapeUserPack
{
    public class ScrapeUserViewCampaign : ITDViewCampaign
    {
        public void ManageCampaign(TemplateModel templateDetails, CampaignDetails campaignDetails,
            bool isEditCampaignName,
            Visibility cancelEditVisibility, string campaignButtonContent, string templateId)
        {
            var objScrapeUser = ScrapeUser.GetSingletonObjectScrapeUser();

            if (campaignButtonContent == ConstantVariable.CreateCampaign())
                objScrapeUser.CampaignName =
                    $"{SocialNetworks.Twitter} {campaignDetails.SubModule} [{DateTime.Now.ToString(CultureInfo.InvariantCulture)}]";

            objScrapeUser.IsEditCampaignName = isEditCampaignName;
            objScrapeUser.CancelEditVisibility = cancelEditVisibility;
            objScrapeUser.TemplateId = templateId;

            objScrapeUser.CampaignName = campaignButtonContent == ConstantVariable.UpdateCampaign()
                ? campaignDetails.CampaignName
                : objScrapeUser.CampaignName;


            objScrapeUser.CampaignButtonContent = campaignButtonContent;
            objScrapeUser.SelectedAccountCount = campaignDetails.SelectedAccountList.Count +
                                                 $" {"LangKeyAccountSelected".FromResourceDictionary()}";
            objScrapeUser.ScrapeUserFooter.list_SelectedAccounts = campaignDetails.SelectedAccountList;

            // Doubt FollowerModel FollowBack
            objScrapeUser.ObjViewModel.ScrapeUserModel =
                templateDetails.ActivitySettings.GetActivityModel<ScrapeUserModel>(objScrapeUser.ObjViewModel.Model);

            objScrapeUser.MainGrid.DataContext = objScrapeUser.ObjViewModel;

            TabSwitcher.ChangeTabIndex(5, 0);
        }
    }
}