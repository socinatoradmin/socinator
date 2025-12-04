using System;
using System.Globalization;
using System.Windows;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using GramDominatorCore.GDModel;
using GramDominatorCore.Interface;

namespace GramDominatorUI.Utility.InstaChat.AutoReplyToNewMessage
{
    internal class AutoReplyToNewMessageViewCampaign : IGdViewCampaign
    {
        public void ManageCampaign(TemplateModel templateDetails, CampaignDetails campaignDetails,
            bool isEditCampaignName,
            Visibility cancelEditVisibility, string campaignButtonContent, string templateId)
        {
            var objAutoReplyToNewMessage = GDViews.Instachats.AutoReplyToNewMessage.GetSingeltonAutoReplyToNewMessage();
            if (campaignButtonContent == ConstantVariable.CreateCampaign())
                objAutoReplyToNewMessage.CampaignName =
                    $"{SocialNetworks.Instagram} {campaignDetails.SubModule} [{DateTime.Now.ToString(CultureInfo.InvariantCulture)}]"; //added new line

            objAutoReplyToNewMessage.IsEditCampaignName = isEditCampaignName;
            objAutoReplyToNewMessage.CancelEditVisibility = cancelEditVisibility;
            objAutoReplyToNewMessage.TemplateId = templateId;
            objAutoReplyToNewMessage.CampaignButtonContent = campaignButtonContent;
            objAutoReplyToNewMessage.CampaignName = campaignButtonContent == ConstantVariable.UpdateCampaign()
                ? campaignDetails.CampaignName
                : objAutoReplyToNewMessage.CampaignName; //updated line          
            // objAutoReplyToNewMessage.CampaignName = campaignDetails.CampaignName;

            objAutoReplyToNewMessage.SelectedAccountCount = campaignDetails.SelectedAccountList.Count +
                                                            $" {"LangKeyAccountSelected".FromResourceDictionary()}";
            objAutoReplyToNewMessage.AutoReplyToNewMessageFooter.list_SelectedAccounts =
                campaignDetails.SelectedAccountList;

            objAutoReplyToNewMessage.ObjViewModel.AutoReplyToNewMessageModel =
                templateDetails.ActivitySettings.GetActivityModel<AutoReplyToNewMessageModel>(
                    objAutoReplyToNewMessage.ObjViewModel.Model, true);

            //  objAutoReplyToNewMessage.MainGrid.DataContext = objAutoReplyToNewMessage.ObjViewModel.AutoReplyToNewMessageModel;
            objAutoReplyToNewMessage.MainGrid.DataContext = objAutoReplyToNewMessage.ObjViewModel;

            TabSwitcher.ChangeTabIndex(3, 1);
        }
    }
}