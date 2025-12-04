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

namespace TumblrDominatorUI.Utility.Unfollow
{
    public class UnfollowViewCampaign : ITumblrViewCampaign
    {
        public void ManageCampaign(TemplateModel templateDetails, CampaignDetails campaignDetails,
            bool isEditCampaignName,
            Visibility cancelEditVisibility, string campaignButtonContent, string templateId)
        {
            var objUnfollow = UnFollower.GetSingeltonObjectUnfollower();
            if (campaignButtonContent == ConstantVariable.CreateCampaign())
                objUnfollow.CampaignName =
                    $"{SocialNetworks.Tumblr} {campaignDetails.SubModule} [{DateTime.Now.ToString(CultureInfo.InvariantCulture)}]";
            objUnfollow.IsEditCampaignName = isEditCampaignName;
            objUnfollow.CancelEditVisibility = cancelEditVisibility;
            objUnfollow.TemplateId = templateId;
            objUnfollow.CampaignName = campaignDetails.CampaignName;
            objUnfollow.CampaignButtonContent = campaignButtonContent;
            objUnfollow.SelectedAccountCount = campaignDetails.SelectedAccountList.Count +
                                               $" {"LangKeyAccountSelected".FromResourceDictionary()}";
            objUnfollow.UnFollowFooter.list_SelectedAccounts = campaignDetails.SelectedAccountList;
            objUnfollow.ObjViewModel.UnfollowerModel =
                JsonConvert.DeserializeObject<UnfollowerModel>(templateDetails.ActivitySettings);

            objUnfollow.MainGrid.DataContext = objUnfollow.ObjViewModel;

            TabSwitcher.ChangeTabIndex(1, 1);
        }
    }
}