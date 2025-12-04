using System;
using System.Globalization;
using System.Windows;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using PinDominator.PDViews.PinMessenger;
using PinDominatorCore.Interface;
using PinDominatorCore.PDModel;

namespace PinDominator.Utility.Pin_Messanger.Broadcast_Messages
{
    public class BroadCastMessagesViewCampaigns : IPdViewCampaign
    {
        public void ManageCampaign(TemplateModel templateDetails, CampaignDetails campaignDetails,
            bool isEditCampaignName, Visibility cancelEditVisibility, string campaignButtonContent, string templateId)
        {
            var objBroadcastMessages = BroadcastMessages.GetSingletonObjectBroadcastMessages();
            if (campaignButtonContent == ConstantVariable.CreateCampaign())
                objBroadcastMessages.CampaignName =
                    $"{SocialNetworks.Pinterest} {campaignDetails.SubModule} [{DateTime.Now.ToString(CultureInfo.InvariantCulture)}]";
            objBroadcastMessages.IsEditCampaignName = isEditCampaignName;
            objBroadcastMessages.CancelEditVisibility = Visibility.Visible;
            objBroadcastMessages.TemplateId = templateId;
            objBroadcastMessages.CampaignName = campaignButtonContent == ConstantVariable.UpdateCampaign()
                ? campaignDetails.CampaignName
                : objBroadcastMessages.CampaignName;
            objBroadcastMessages.CampaignButtonContent = campaignButtonContent;
            objBroadcastMessages.SelectedAccountCount = campaignDetails.SelectedAccountList.Count +
                                                        $" {"LangKeyAccountSelected".FromResourceDictionary()}";
            objBroadcastMessages.BroadcastMessagesFooter.list_SelectedAccounts = campaignDetails.SelectedAccountList;

            objBroadcastMessages.ObjViewModel.BroadcastMessagesModel =
                templateDetails.ActivitySettings.GetActivityModel<BroadcastMessagesModel>(objBroadcastMessages
                    .ObjViewModel.Model);

            objBroadcastMessages.MainGrid.DataContext = objBroadcastMessages.ObjViewModel;

            TabSwitcher.ChangeTabIndex(5, 0);
        }
    }
}