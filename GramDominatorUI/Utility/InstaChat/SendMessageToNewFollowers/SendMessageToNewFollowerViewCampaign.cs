using System;
using System.Globalization;
using System.Windows;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using GramDominatorCore.GDModel;
using GramDominatorCore.Interface;
using GramDominatorUI.GDViews.Instachats;
using Newtonsoft.Json;

namespace GramDominatorUI.Utility.InstaChat.SendMessageToNewFollowers
{
    internal class SendMessageToNewFollowerViewCampaign : IGdViewCampaign
    {
        public void ManageCampaign(TemplateModel templateDetails, CampaignDetails campaignDetails,
            bool isEditCampaignName,
            Visibility cancelEditVisibility, string campaignButtonContent, string templateId)
        {
            var objSendMessageToFollower = SendMessageToFollower.GetSingeltonSendMessageToFollower();
            if (campaignButtonContent == ConstantVariable.CreateCampaign())
                objSendMessageToFollower.CampaignName =
                    $"{SocialNetworks.Instagram} {campaignDetails.SubModule} [{DateTime.Now.ToString(CultureInfo.InvariantCulture)}]"; //added new line

            objSendMessageToFollower.IsEditCampaignName = isEditCampaignName;
            objSendMessageToFollower.CancelEditVisibility = cancelEditVisibility;
            objSendMessageToFollower.TemplateId = templateId;
            objSendMessageToFollower.CampaignButtonContent = campaignButtonContent;
            objSendMessageToFollower.CampaignName = campaignButtonContent == ConstantVariable.UpdateCampaign()
                ? campaignDetails.CampaignName
                : objSendMessageToFollower.CampaignName; //updated line          
            // objSendMessageToFollower.CampaignName = campaignDetails.CampaignName;
            objSendMessageToFollower.SelectedAccountCount = campaignDetails.SelectedAccountList.Count +
                                                            $" {"LangKeyAccountSelected".FromResourceDictionary()}";
            objSendMessageToFollower.SendMessageToFollowFooter.list_SelectedAccounts =
                campaignDetails.SelectedAccountList;

            var getModel = JsonConvert.DeserializeObject<SendMessageToFollowerModel>(templateDetails.ActivitySettings);

            if ("LangKeySocinator".FromResourceDictionary() == "Tunto Socianator")
                getModel.JobConfiguration.CopyJobConfigWith(
                    objSendMessageToFollower.ObjViewModel.Model.JobConfiguration);

            objSendMessageToFollower.ObjViewModel.SendMessageToFollowerModel =
                templateDetails.ActivitySettings.GetActivityModel<SendMessageToFollowerModel>(
                    objSendMessageToFollower.ObjViewModel.Model, true);

            // objBroadcastMessages.MainGrid.DataContext = objBroadcastMessages.ObjViewModel.BroadcastMessagesModel;
            objSendMessageToFollower.MainGrid.DataContext = objSendMessageToFollower.ObjViewModel;

            TabSwitcher.ChangeTabIndex(3, 2);
        }
    }
}