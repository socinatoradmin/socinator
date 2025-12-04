using System;
using System.Globalization;
using System.Windows;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using TwtDominatorCore.Interface;
using TwtDominatorCore.TDModels;
using TwtDominatorUI.TDViews.TwtMessenger;

namespace TwtDominatorUI.Utility.TwtMessengerReportPack.BroadCastMessagePack
{
    public class BroadCastMessageViewCampaign : ITDViewCampaign
    {
        public void ManageCampaign(TemplateModel templateDetails, CampaignDetails campaignDetails,
            bool isEditCampaignName,
            Visibility cancelEditVisibility, string campaignButtonContent, string templateId)
        {
            var objBroadCastMessage = BroadCastMessage.GetSingletonObjectBroadCastMessage();

            if (campaignButtonContent == ConstantVariable.CreateCampaign())
                objBroadCastMessage.CampaignName =
                    $"{SocialNetworks.Twitter} {campaignDetails.SubModule} [{DateTime.Now.ToString(CultureInfo.InvariantCulture)}]";

            objBroadCastMessage.IsEditCampaignName = isEditCampaignName;
            objBroadCastMessage.CancelEditVisibility = cancelEditVisibility;
            objBroadCastMessage.TemplateId = templateId;


            objBroadCastMessage.CampaignName = campaignButtonContent == ConstantVariable.UpdateCampaign()
                ? campaignDetails.CampaignName
                : objBroadCastMessage.CampaignName;

            objBroadCastMessage.CampaignButtonContent = campaignButtonContent;
            objBroadCastMessage.SelectedAccountCount = campaignDetails.SelectedAccountList.Count +
                                                       $" {"LangKeyAccountSelected".FromResourceDictionary()}";
            objBroadCastMessage.BroadcastMessageFooter.list_SelectedAccounts = campaignDetails.SelectedAccountList;

            // Doubt FollowerModel FollowBack
            objBroadCastMessage.ObjViewModel.MessageModel =
                templateDetails.ActivitySettings.GetActivityModel<MessageModel>(objBroadCastMessage.ObjViewModel.Model,
                    true);

            objBroadCastMessage.MainGrid.DataContext = objBroadCastMessage.ObjViewModel;

            TabSwitcher.ChangeTabIndex(4, 0);
        }
    }
}