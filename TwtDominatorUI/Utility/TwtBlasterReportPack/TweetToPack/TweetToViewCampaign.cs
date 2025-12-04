using System;
using System.Globalization;
using System.Windows;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using TwtDominatorCore.Interface;
using TwtDominatorCore.TDModels;
using TwtDominatorUI.TDViews.TwtBlaster;

namespace TwtDominatorUI.Utility.TwtBlasterReportPack.TweetToPack
{
    public class TweetToViewCampaign : ITDViewCampaign
    {
        public void ManageCampaign(TemplateModel templateDetails, CampaignDetails campaignDetails,
            bool isEditCampaignName,
            Visibility cancelEditVisibility, string campaignButtonContent, string templateId)
        {
            var objTweetTo = TweetTo.GetSingletonObjectTweetTo();
            if (campaignButtonContent == ConstantVariable.CreateCampaign())
                objTweetTo.CampaignName =
                    $"{SocialNetworks.Twitter} {campaignDetails.SubModule} [{DateTime.Now.ToString(CultureInfo.InvariantCulture)}]";

            objTweetTo.IsEditCampaignName = isEditCampaignName;
            objTweetTo.CancelEditVisibility = cancelEditVisibility;
            objTweetTo.TemplateId = templateId;


            objTweetTo.CampaignName = campaignButtonContent == ConstantVariable.UpdateCampaign()
                ? campaignDetails.CampaignName
                : objTweetTo.CampaignName;

            objTweetTo.CampaignButtonContent = campaignButtonContent;
            objTweetTo.SelectedAccountCount = campaignDetails.SelectedAccountList.Count +
                                              $" {"LangKeyAccountSelected".FromResourceDictionary()}";
            objTweetTo.TweetToFooter.list_SelectedAccounts = campaignDetails.SelectedAccountList;

            // Doubt FollowerModel FollowBack
            objTweetTo.ObjViewModel.TweetToModel =
                templateDetails.ActivitySettings.GetActivityModel<TweetToModel>(objTweetTo.ObjViewModel.Model);

            objTweetTo.MainGrid.DataContext = objTweetTo.ObjViewModel;

            TabSwitcher.ChangeTabIndex(2, 3);
        }
    }
}