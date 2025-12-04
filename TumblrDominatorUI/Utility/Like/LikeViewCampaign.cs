using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using Newtonsoft.Json;
using System;
using System.Globalization;
using System.Windows;
using TumblrDominatorCore.Interface;
using TumblrDominatorCore.Models;

namespace TumblrDominatorUI.Utility.Like
{
    public class LikeViewCampaign : ITumblrViewCampaign
    {
        public void ManageCampaign(TemplateModel templateDetails, CampaignDetails campaignDetails,
            bool isEditCampaignName,
            Visibility cancelEditVisibility, string campaignButtonContent, string templateId)
        {
            var objLike = TumblrView.Liker.Like.GetSingeltonObjectLike();
            if (campaignButtonContent == ConstantVariable.CreateCampaign())
                objLike.CampaignName =
                    $"{SocialNetworks.Tumblr} {campaignDetails.SubModule} [{DateTime.Now.ToString(CultureInfo.InvariantCulture)}]";

            objLike.IsEditCampaignName = isEditCampaignName;
            objLike.CancelEditVisibility = cancelEditVisibility;
            objLike.TemplateId = templateId;
            objLike.CampaignName = campaignDetails.CampaignName;
            objLike.CampaignButtonContent = campaignButtonContent;
            objLike.SelectedAccountCount = campaignDetails.SelectedAccountList.Count +
                                           $" {"LangKeyAccountSelected".FromResourceDictionary()}";
            objLike.LikeFooter.list_SelectedAccounts = campaignDetails.SelectedAccountList;
            objLike.ObjViewModel.LikeModel =
                JsonConvert.DeserializeObject<LikeModel>(templateDetails.ActivitySettings);

            objLike.MainGrid.DataContext = objLike.ObjViewModel;

            TabSwitcher.ChangeTabIndex(2, 0);
        }
    }
}