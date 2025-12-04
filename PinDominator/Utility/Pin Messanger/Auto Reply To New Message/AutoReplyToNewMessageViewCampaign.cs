using System;
using System.Globalization;
using System.Windows;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using PinDominator.PDViews.PinMessenger;
using PinDominatorCore.Interface;
using PinDominatorCore.PDModel;

namespace PinDominator.Utility.Pin_Messanger.Auto_Reply_To_New_Message
{
    public class AutoReplyToNewMessageViewCampaign : IPdViewCampaign
    {
        public void ManageCampaign(TemplateModel templateDetails, CampaignDetails campaignDetails,
            bool isEditCampaignName, Visibility cancelEditVisibility, string campaignButtonContent, string templateId)
        {
            var objAutoReplyToNewMessage = AutoReplyToNewMessage.GetSingletonObjectAutoReplyToNewMessage();
            if (campaignButtonContent == ConstantVariable.CreateCampaign())
                objAutoReplyToNewMessage.CampaignName =
                    $"{SocialNetworks.Pinterest} {campaignDetails.SubModule} [{DateTime.Now.ToString(CultureInfo.InvariantCulture)}]";
            objAutoReplyToNewMessage.IsEditCampaignName = isEditCampaignName;
            objAutoReplyToNewMessage.CancelEditVisibility = Visibility.Visible;
            objAutoReplyToNewMessage.TemplateId = templateId;
            objAutoReplyToNewMessage.CampaignName = campaignButtonContent == ConstantVariable.UpdateCampaign()
                ? campaignDetails.CampaignName
                : objAutoReplyToNewMessage.CampaignName;
            objAutoReplyToNewMessage.CampaignButtonContent = campaignButtonContent;
            objAutoReplyToNewMessage.SelectedAccountCount =
                campaignDetails.SelectedAccountList.Count + $" {"LangKeyAccountSelected".FromResourceDictionary()}";
            objAutoReplyToNewMessage.AutoReplyToNewMessageFooter.list_SelectedAccounts =
                campaignDetails.SelectedAccountList;

            objAutoReplyToNewMessage.ObjViewModel.AutoReplyToNewMessageModel =
                templateDetails.ActivitySettings.GetActivityModel<AutoReplyToNewMessageModel>(
                    objAutoReplyToNewMessage.ObjViewModel.Model, true);

            objAutoReplyToNewMessage.MainGrid.DataContext = objAutoReplyToNewMessage.ObjViewModel;

            TabSwitcher.ChangeTabIndex(5, 1);
        }
    }
}