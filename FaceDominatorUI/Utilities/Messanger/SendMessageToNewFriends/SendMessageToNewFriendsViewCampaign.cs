using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using FaceDominatorCore.FDModel.MessageModel;
using FaceDominatorCore.Interface;
using FaceDominatorCore.Utility;
using FaceDominatorUI.FDViews.FbMessanger;
using System;
using System.Globalization;
using System.Windows;

namespace FaceDominatorUI.Utilities.Messanger.SendMessageToNewFriends
{
    public class SendMessageToNewFriendsViewCampaign : IFdViewCampaign
    {
        public void ManageCampaign(TemplateModel templateDetails, CampaignDetails campaignDetails,
            bool isEditCampaignName, Visibility cancelEditVisibility, string campaignButtonContent, string templateId)
        {
            var singeltonObjectMessageRecentFriends = MessageRecentFriends.GetSingeltonObjectMessageRecentFriends();
            singeltonObjectMessageRecentFriends.IsEditCampaignName = isEditCampaignName;
            singeltonObjectMessageRecentFriends.CancelEditVisibility = cancelEditVisibility;
            singeltonObjectMessageRecentFriends.TemplateId = templateId;

            if (campaignButtonContent == ConstantVariable.CreateCampaign())
                singeltonObjectMessageRecentFriends.CampaignName =
                    $"{SocialNetworks.Facebook} {campaignDetails.SubModule} [{DateTime.Now.ToString(CultureInfo.InvariantCulture)}]";
            singeltonObjectMessageRecentFriends.CampaignName =
                campaignButtonContent == ConstantVariable.UpdateCampaign()
                    ? campaignDetails.CampaignName
                    : singeltonObjectMessageRecentFriends.CampaignName;

            //singeltonObjectMessageRecentFriends.CampaignName = campaignDetails.CampaignName;
            singeltonObjectMessageRecentFriends.CampaignButtonContent = campaignButtonContent;
            singeltonObjectMessageRecentFriends.SelectedAccountCount =
                campaignDetails.SelectedAccountList.Count + $" {"LangKeyAccountSelected".FromResourceDictionary()}";
            singeltonObjectMessageRecentFriends.SendRequestFooter.list_SelectedAccounts =
                campaignDetails.SelectedAccountList;

            singeltonObjectMessageRecentFriends.ObjViewModel.MessageRecentFriendsModel
                = templateDetails.ActivitySettings.GetActivityModelNonQueryList<MessageRecentFriendsModel>(
                    singeltonObjectMessageRecentFriends.ObjViewModel.Model);

            singeltonObjectMessageRecentFriends.MainGrid.DataContext = singeltonObjectMessageRecentFriends.ObjViewModel;

            TabSwitcher.ChangeTabIndex(2, 2);
        }
    }
}