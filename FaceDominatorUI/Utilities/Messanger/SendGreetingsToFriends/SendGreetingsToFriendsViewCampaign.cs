using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using FaceDominatorCore.FDModel.MessageModel;
using FaceDominatorCore.Interface;
using FaceDominatorCore.Utility;
using System;
using System.Globalization;
using System.Windows;

namespace FaceDominatorUI.Utilities.Messanger.SendGreetingsToFriends
{
    public class SendGreetingsToFriendsViewCampaign : IFdViewCampaign
    {
        public void ManageCampaign(TemplateModel templateDetails, CampaignDetails campaignDetails,
            bool isEditCampaignName, Visibility cancelEditVisibility, string campaignButtonContent, string templateId)
        {
            var singeltonObjectSendGreetingsToFriends =
                FDViews.FbMessanger.SendGreetingsToFriends.GetSingeltonObjectSendGreetingsToFriends();
            singeltonObjectSendGreetingsToFriends.IsEditCampaignName = isEditCampaignName;
            singeltonObjectSendGreetingsToFriends.CancelEditVisibility = cancelEditVisibility;
            singeltonObjectSendGreetingsToFriends.TemplateId = templateId;

            if (campaignButtonContent == ConstantVariable.CreateCampaign())
                singeltonObjectSendGreetingsToFriends.CampaignName =
                    $"{SocialNetworks.Facebook} {campaignDetails.SubModule} [{DateTime.Now.ToString(CultureInfo.InvariantCulture)}]";
            singeltonObjectSendGreetingsToFriends.CampaignName =
                campaignButtonContent == ConstantVariable.UpdateCampaign()
                    ? campaignDetails.CampaignName
                    : singeltonObjectSendGreetingsToFriends.CampaignName;

            //singeltonObjectSendGreetingsToFriends.CampaignName = campaignDetails.CampaignName;
            singeltonObjectSendGreetingsToFriends.CampaignButtonContent = campaignButtonContent;
            singeltonObjectSendGreetingsToFriends.SelectedAccountCount =
                campaignDetails.SelectedAccountList.Count + $" {"LangKeyAccountSelected".FromResourceDictionary()}";
            singeltonObjectSendGreetingsToFriends.SendRequestFooter.list_SelectedAccounts =
                campaignDetails.SelectedAccountList;

            singeltonObjectSendGreetingsToFriends.ObjViewModel.SendGreetingsToFriendsModel
                = templateDetails.ActivitySettings.GetActivityModelNonQueryList<SendGreetingsToFriendsModel>(
                    singeltonObjectSendGreetingsToFriends.ObjViewModel.Model);

            singeltonObjectSendGreetingsToFriends.MainGrid.DataContext =
                singeltonObjectSendGreetingsToFriends.ObjViewModel;

            TabSwitcher.ChangeTabIndex(2, 3);
        }
    }
}