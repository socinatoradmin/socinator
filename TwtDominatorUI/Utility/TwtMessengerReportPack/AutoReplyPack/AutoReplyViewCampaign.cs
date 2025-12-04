using System;
using System.Globalization;
using System.Windows;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using TwtDominatorCore.Interface;
using TwtDominatorCore.TDModels;
using TwtDominatorUI.TDViews.TwtMessenger;

namespace TwtDominatorUI.Utility.TwtMessengerReportPack.AutoReplyPack
{
    public class AutoReplyViewCampaign : ITDViewCampaign
    {
        public void ManageCampaign(TemplateModel templateDetails, CampaignDetails campaignDetails,
            bool isEditCampaignName,
            Visibility cancelEditVisibility, string campaignButtonContent, string templateId)
        {
            var objAutoReply = AutoReply.GetSingletonObjectAutoReply();

            if (campaignButtonContent == ConstantVariable.CreateCampaign())
                objAutoReply.CampaignName =
                    $"{SocialNetworks.Twitter} {campaignDetails.SubModule} [{DateTime.Now.ToString(CultureInfo.InvariantCulture)}]";


            objAutoReply.IsEditCampaignName = isEditCampaignName;
            objAutoReply.CancelEditVisibility = cancelEditVisibility;
            objAutoReply.TemplateId = templateId;


            objAutoReply.CampaignName = campaignButtonContent == ConstantVariable.UpdateCampaign()
                ? campaignDetails.CampaignName
                : objAutoReply.CampaignName;

            objAutoReply.CampaignButtonContent = campaignButtonContent;
            objAutoReply.SelectedAccountCount = campaignDetails.SelectedAccountList.Count +
                                                $" {"LangKeyAccountSelected".FromResourceDictionary()}";
            objAutoReply.MessageFooter.list_SelectedAccounts = campaignDetails.SelectedAccountList;

            // Doubt FollowerModel FollowBack
            objAutoReply.ObjViewModel.MessageModel =
                templateDetails.ActivitySettings.GetActivityModel<MessageModel>(objAutoReply.ObjViewModel.Model, true);

            objAutoReply.MainGrid.DataContext = objAutoReply.ObjViewModel;

            TabSwitcher.ChangeTabIndex(4, 2);
        }
    }
}