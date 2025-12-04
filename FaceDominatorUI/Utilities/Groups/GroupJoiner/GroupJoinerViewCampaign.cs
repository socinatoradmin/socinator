using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using FaceDominatorCore.FDModel.GroupsModel;
using FaceDominatorCore.Interface;
using System;
using System.Globalization;
using System.Windows;

namespace FaceDominatorUI.Utilities.Groups.GroupJoiner
{
    public class GroupJoinerViewCampaign : IFdViewCampaign
    {
        public void ManageCampaign(TemplateModel templateDetails, CampaignDetails campaignDetails,
            bool isEditCampaignName,
            Visibility cancelEditVisibility, string campaignButtonContent, string templateId)
        {
            var singeltonObjectFriendRequest = FDViews.FbGroups.GroupJoiner.GetSingeltonObjectGroupJoiner();
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
            singeltonObjectFriendRequest.GroupJoinerFooter.list_SelectedAccounts = campaignDetails.SelectedAccountList;

            singeltonObjectFriendRequest.ObjViewModel.GroupJoinerModel
                = templateDetails.ActivitySettings.GetActivityModel<GroupJoinerModel>(singeltonObjectFriendRequest
                    .ObjViewModel.Model);

            singeltonObjectFriendRequest.MainGrid.DataContext = singeltonObjectFriendRequest.ObjViewModel;

            TabSwitcher.ChangeTabIndex(3, 0);
        }
    }
}