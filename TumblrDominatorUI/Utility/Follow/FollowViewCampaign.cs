using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using Newtonsoft.Json;
using System;
using System.Globalization;
using System.Windows;
using TumblrDominatorCore.Interface;
using TumblrDominatorCore.Models;
using TumblrDominatorUI.TumblrView.GrowFollowers;

namespace TumblrDominatorUI.Utility.Follow
{
    public class FollowViewCampaign : ITumblrViewCampaign
    {
        public void ManageCampaign(TemplateModel templateDetails, CampaignDetails campaignDetails,
            bool isEditCampaignName,
            Visibility cancelEditVisibility, string campaignButtonContent, string templateId)
        {
            var objFollower = Follower.GetSingeltonObjectFollower();

            if (campaignButtonContent == ConstantVariable.CreateCampaign())
                objFollower.CampaignName =
                    $"{SocialNetworks.Tumblr} {campaignDetails.SubModule} [{DateTime.Now.ToString(CultureInfo.InvariantCulture)}]";

            objFollower.IsEditCampaignName = isEditCampaignName;
            objFollower.CancelEditVisibility = cancelEditVisibility;
            objFollower.TemplateId = templateId;
            objFollower.CampaignName = campaignDetails.CampaignName;
            objFollower.CampaignButtonContent = campaignButtonContent;
            objFollower.SelectedAccountCount = campaignDetails.SelectedAccountList.Count +
                                               $" {"LangKeyAccountSelected".FromResourceDictionary()}";
            objFollower.FollowFooter.list_SelectedAccounts = campaignDetails.SelectedAccountList;
            objFollower.ObjViewModel.FollowerModel =
                JsonConvert.DeserializeObject<FollowerModel>(templateDetails.ActivitySettings);

            objFollower.MainGrid.DataContext = objFollower.ObjViewModel;

            TabSwitcher.ChangeTabIndex(1, 0);
        }
    }
}