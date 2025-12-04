using System;
using System.Globalization;
using System.Windows;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using TwtDominatorCore.Interface;
using TwtDominatorCore.TDModels;
using TwtDominatorUI.TDViews.TwtBlaster;

namespace TwtDominatorUI.Utility.TwtBlasterReportPack.RetweetPack
{
    public class RetweetViewCampaign : ITDViewCampaign
    {
        public void ManageCampaign(TemplateModel templateDetails, CampaignDetails campaignDetails,
            bool isEditCampaignName,
            Visibility cancelEditVisibility, string campaignButtonContent, string templateId)
        {
            var objRetweet = Retweet.GetSingletonObjectRetweet();

            if (campaignButtonContent == ConstantVariable.CreateCampaign())
                objRetweet.CampaignName =
                    $"{SocialNetworks.Twitter} {campaignDetails.SubModule} [{DateTime.Now.ToString(CultureInfo.InvariantCulture)}]";


            objRetweet.IsEditCampaignName = isEditCampaignName;
            objRetweet.CancelEditVisibility = cancelEditVisibility;
            objRetweet.TemplateId = templateId;

            objRetweet.CampaignName = campaignButtonContent == ConstantVariable.UpdateCampaign()
                ? campaignDetails.CampaignName
                : objRetweet.CampaignName;


            objRetweet.CampaignButtonContent = campaignButtonContent;
            objRetweet.SelectedAccountCount = campaignDetails.SelectedAccountList.Count +
                                              $" {"LangKeyAccountSelected".FromResourceDictionary()}";
            objRetweet.RetweetFooter.list_SelectedAccounts = campaignDetails.SelectedAccountList;

            // Doubt FollowerModel FollowBack
            objRetweet.ObjViewModel.RetweetModel =
                templateDetails.ActivitySettings.GetActivityModel<RetweetModel>(objRetweet.ObjViewModel.Model);

            objRetweet.MainGrid.DataContext = objRetweet.ObjViewModel;

            TabSwitcher.ChangeTabIndex(2, 1);
        }
    }
}