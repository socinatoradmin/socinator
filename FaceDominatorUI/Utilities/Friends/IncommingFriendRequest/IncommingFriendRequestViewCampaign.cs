using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using FaceDominatorCore.FDModel.FriendsModel;
using FaceDominatorCore.Interface;
using FaceDominatorCore.Utility;
using System;
using System.Globalization;
using System.Windows;

namespace FaceDominatorUI.Utilities.Friends.IncommingFriendRequest
{
    public class IncommingFriendRequestViewCampaign : IFdViewCampaign
    {
        public void ManageCampaign(TemplateModel templateDetails, CampaignDetails campaignDetails,
            bool isEditCampaignName,
            Visibility cancelEditVisibility, string campaignButtonContent, string templateId)
        {
            var singeltonObjectIncommingfriend =
                FDViews.FbFriends.IncommingFriendRequest.GetSingeltonObjectIncommingFriendRequest();
            singeltonObjectIncommingfriend.IsEditCampaignName = isEditCampaignName;
            singeltonObjectIncommingfriend.CancelEditVisibility = cancelEditVisibility; //CancelEditVisibility;
            singeltonObjectIncommingfriend.TemplateId = templateId;

            if (campaignButtonContent == ConstantVariable.CreateCampaign())
                singeltonObjectIncommingfriend.CampaignName =
                    $"{SocialNetworks.Facebook} {campaignDetails.SubModule} [{DateTime.Now.ToString(CultureInfo.InvariantCulture)}]";
            singeltonObjectIncommingfriend.CampaignName = campaignButtonContent == ConstantVariable.UpdateCampaign()
                ? campaignDetails.CampaignName
                : singeltonObjectIncommingfriend.CampaignName;

            //singeltonObjectIncommingfriend.CampaignName = campaignDetails.CampaignName;
            singeltonObjectIncommingfriend.CampaignButtonContent = campaignButtonContent;
            singeltonObjectIncommingfriend.SelectedAccountCount =
                campaignDetails.SelectedAccountList.Count + $" {"LangKeyAccountSelected".FromResourceDictionary()}";
            singeltonObjectIncommingfriend.ManageFriendsFooter.list_SelectedAccounts =
                campaignDetails.SelectedAccountList;

            singeltonObjectIncommingfriend.ObjViewModel.IncommingFriendRequestModel
                = templateDetails.ActivitySettings.GetActivityModelNonQueryList<IncommingFriendRequestModel>(
                    singeltonObjectIncommingfriend.ObjViewModel.Model);

            singeltonObjectIncommingfriend.MainGrid.DataContext = singeltonObjectIncommingfriend.ObjViewModel;

            TabSwitcher.ChangeTabIndex(1, 1);
        }
    }
}