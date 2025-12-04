using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using Newtonsoft.Json;
using System;
using System.Globalization;
using System.Windows;
using TumblrDominatorCore.Interface;
using TumblrDominatorCore.Models;
using TumblrDominatorUI.TumblrView.Liker;

namespace TumblrDominatorUI.Utility.Unlike
{
    public class UnLikeViewCampaign : ITumblrViewCampaign
    {
        public void ManageCampaign(TemplateModel templateDetails, CampaignDetails campaignDetails,
            bool isEditCampaignName,
            Visibility cancelEditVisibility, string campaignButtonContent, string templateId)
        {
            var objUnfollow = UnLike.GetSingeltonObjectUnLike();
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
            objUnfollow.UnLikeFooter.list_SelectedAccounts = campaignDetails.SelectedAccountList;
            objUnfollow.ObjViewModel.UnLikeModel =
                JsonConvert.DeserializeObject<UnLikeModel>(templateDetails.ActivitySettings);

            objUnfollow.MainGrid.DataContext = objUnfollow.ObjViewModel;

            TabSwitcher.ChangeTabIndex(2, 1);
        }
    }
}