using System;
using System.Globalization;
using System.Windows;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using TwtDominatorCore.Interface;
using TwtDominatorCore.TDModels;
using TwtDominatorUI.TDViews.TwtMessenger;

namespace TwtDominatorUI.Utility.TwtMessengerReportPack.ReplyToNewFollowersPack
{
    public class ReplyToNewFollowersViewCampaign : ITDViewCampaign
    {
        public void ManageCampaign(TemplateModel templateDetails, CampaignDetails campaignDetails,
            bool isEditCampaignName,
            Visibility cancelEditVisibility, string campaignButtonContent, string templateId)
        {
            var objReplyToNewFollowers = ReplyToNewFollowers.GetSingletonObjectReplyToNewFollowers();


            if (campaignButtonContent == ConstantVariable.CreateCampaign())
                objReplyToNewFollowers.CampaignName =
                    $"{SocialNetworks.Twitter} {campaignDetails.SubModule} [{DateTime.Now.ToString(CultureInfo.InvariantCulture)}]";


            objReplyToNewFollowers.IsEditCampaignName = isEditCampaignName;
            objReplyToNewFollowers.CancelEditVisibility = cancelEditVisibility;
            objReplyToNewFollowers.TemplateId = templateId;


            objReplyToNewFollowers.CampaignName = campaignButtonContent == ConstantVariable.UpdateCampaign()
                ? campaignDetails.CampaignName
                : objReplyToNewFollowers.CampaignName;

            objReplyToNewFollowers.CampaignButtonContent = campaignButtonContent;
            objReplyToNewFollowers.SelectedAccountCount = campaignDetails.SelectedAccountList.Count +
                                                          $" {"LangKeyAccountSelected".FromResourceDictionary()}";
            objReplyToNewFollowers.MessageFooter.list_SelectedAccounts = campaignDetails.SelectedAccountList;

            // Doubt FollowerModel FollowBack
            objReplyToNewFollowers.ObjViewModel.MessageModel =
                templateDetails.ActivitySettings.GetActivityModel<MessageModel>(
                    objReplyToNewFollowers.ObjViewModel.Model, true);

            objReplyToNewFollowers.MainGrid.DataContext = objReplyToNewFollowers.ObjViewModel;

            TabSwitcher.ChangeTabIndex(4, 1);
        }
    }
}