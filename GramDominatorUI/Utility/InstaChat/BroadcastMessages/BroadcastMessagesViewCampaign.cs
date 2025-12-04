using System;
using System.Globalization;
using System.Windows;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using GramDominatorCore.GDModel;
using GramDominatorCore.Interface;

namespace GramDominatorUI.Utility.InstaChat.BroadcastMessages
{
    internal class BroadcastMessagesViewCampaign : IGdViewCampaign
    {
        public void ManageCampaign(TemplateModel templateDetails, CampaignDetails campaignDetails,
            bool isEditCampaignName,
            Visibility cancelEditVisibility, string campaignButtonContent, string templateId)
        {
            var objBroadcastMessages = GDViews.Instachats.BroadcastMessages.GetSingeltonBroadcastMessages();
            if (campaignButtonContent == ConstantVariable.CreateCampaign())
                objBroadcastMessages.CampaignName =
                    $"{SocialNetworks.Instagram} {campaignDetails.SubModule} [{DateTime.Now.ToString(CultureInfo.InvariantCulture)}]"; //added new line

            objBroadcastMessages.IsEditCampaignName = isEditCampaignName;
            objBroadcastMessages.CancelEditVisibility = cancelEditVisibility;
            objBroadcastMessages.TemplateId = templateId;
            objBroadcastMessages.CampaignButtonContent = campaignButtonContent;
            objBroadcastMessages.CampaignName = campaignButtonContent == ConstantVariable.UpdateCampaign()
                ? campaignDetails.CampaignName
                : objBroadcastMessages.CampaignName; //updated line          
            // objBroadcastMessages.CampaignName = campaignDetails.CampaignName;  
            objBroadcastMessages.SelectedAccountCount = campaignDetails.SelectedAccountList.Count +
                                                        $" {"LangKeyAccountSelected".FromResourceDictionary()}";
            objBroadcastMessages.BrodCastMessageFooter.list_SelectedAccounts = campaignDetails.SelectedAccountList;

            objBroadcastMessages.ObjViewModel.BroadcastMessagesModel =
                templateDetails.ActivitySettings.GetActivityModel<BroadcastMessagesModel>(
                    objBroadcastMessages.ObjViewModel.Model, true);

            // objBroadcastMessages.MainGrid.DataContext = objBroadcastMessages.ObjViewModel.BroadcastMessagesModel;
            objBroadcastMessages.MainGrid.DataContext = objBroadcastMessages.ObjViewModel;

            TabSwitcher.ChangeTabIndex(3, 0);
        }
    }
}