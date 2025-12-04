using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using FaceDominatorCore.FDModel.FriendsModel;
using FaceDominatorCore.Interface;
using System;
using System.Globalization;
using System.Windows;

namespace FaceDominatorUI.Utilities.Friends.SendFriendRequest
{
    public class SendFriendRequestViewCampaigny : IFdViewCampaign
    {
        public void ManageCampaign(TemplateModel templateDetails, CampaignDetails campaignDetails,
            bool isEditCampaignName,
            Visibility cancelEditVisibility, string campaignButtonContent, string templateId)
        {
            var singeltonObjectFriendRequest =
                FDViews.FbFriends.SendFriendRequest.GetSingeltonObjectSendFriendRequest();
            singeltonObjectFriendRequest.IsEditCampaignName = isEditCampaignName;
            singeltonObjectFriendRequest.CancelEditVisibility = cancelEditVisibility;
            singeltonObjectFriendRequest.TemplateId = templateId;

            if (campaignButtonContent == ConstantVariable.CreateCampaign())
                singeltonObjectFriendRequest.CampaignName =
                    $"{SocialNetworks.Facebook} {campaignDetails.SubModule} [{DateTime.Now.ToString(CultureInfo.InvariantCulture)}]";
            singeltonObjectFriendRequest.CampaignName = campaignButtonContent == ConstantVariable.UpdateCampaign()
                ? campaignDetails.CampaignName
                : singeltonObjectFriendRequest.CampaignName;

            //objSendFriendRequest.CampaignName = campaignDetails.CampaignName;
            singeltonObjectFriendRequest.CampaignButtonContent = campaignButtonContent;
            singeltonObjectFriendRequest.SelectedAccountCount =
                campaignDetails.SelectedAccountList.Count + $" {"LangKeyAccountSelected".FromResourceDictionary()}";
            singeltonObjectFriendRequest.SendRequestFooter.list_SelectedAccounts = campaignDetails.SelectedAccountList;

            singeltonObjectFriendRequest.ObjViewModel.SendFriendRequestModel
                = templateDetails.ActivitySettings.GetActivityModel<SendFriendRequestModel>(singeltonObjectFriendRequest
                    .ObjViewModel.Model);

            singeltonObjectFriendRequest.MainGrid.DataContext = singeltonObjectFriendRequest.ObjViewModel;

            TabSwitcher.ChangeTabIndex(1, 0);
        }
    }
}