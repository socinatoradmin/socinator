using System;
using System.Globalization;
using System.Windows;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using PinDominator.PDViews.PinMessenger;
using PinDominatorCore.Interface;
using PinDominatorCore.PDModel;

namespace PinDominator.Utility.Pin_Messanger.Send_Message_To_New_Followers
{
    public class SendMessageToNewFollowrsViewCampaigns : IPdViewCampaign
    {
        public void ManageCampaign(TemplateModel templateDetails, CampaignDetails campaignDetails,
            bool isEditCampaignName, Visibility cancelEditVisibility, string campaignButtonContent, string templateId)
        {
            var objSendMessageToNewFollowers = SendMessageToNewFollowers.GetSingletonObjectSendMessageToNewFollowers();
            if (campaignButtonContent == ConstantVariable.CreateCampaign())
                objSendMessageToNewFollowers.CampaignName =
                    $"{SocialNetworks.Pinterest} {campaignDetails.SubModule} [{DateTime.Now.ToString(CultureInfo.InvariantCulture)}]";
            objSendMessageToNewFollowers.IsEditCampaignName = isEditCampaignName;
            objSendMessageToNewFollowers.CancelEditVisibility = Visibility.Visible;
            objSendMessageToNewFollowers.TemplateId = templateId;
            objSendMessageToNewFollowers.CampaignName = campaignButtonContent == ConstantVariable.UpdateCampaign()
                ? campaignDetails.CampaignName
                : objSendMessageToNewFollowers.CampaignName;
            objSendMessageToNewFollowers.CampaignButtonContent = campaignButtonContent;
            objSendMessageToNewFollowers.SelectedAccountCount =
                campaignDetails.SelectedAccountList.Count + $" {"LangKeyAccountSelected".FromResourceDictionary()}";
            objSendMessageToNewFollowers.SendMessageToNewFollowersFooter.list_SelectedAccounts =
                campaignDetails.SelectedAccountList;

            objSendMessageToNewFollowers.ObjViewModel.SendMessageToNewFollowersModel =
                templateDetails.ActivitySettings.GetActivityModel<SendMessageToNewFollowersModel>(
                    objSendMessageToNewFollowers.ObjViewModel.Model, true);

            objSendMessageToNewFollowers.MainGrid.DataContext = objSendMessageToNewFollowers.ObjViewModel;

            TabSwitcher.ChangeTabIndex(5, 2);
        }
    }
}