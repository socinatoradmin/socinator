using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using Newtonsoft.Json;
using System;
using System.Globalization;
using System.Windows;
using TumblrDominatorCore.Interface;
using TumblrDominatorCore.Models;

namespace TumblrDominatorUI.Utility.Comment
{
    public class CommentViewCampaign : ITumblrViewCampaign
    {
        public void ManageCampaign(TemplateModel templateDetails, CampaignDetails campaignDetails,
            bool isEditCampaignName,
            Visibility cancelEditVisibility, string campaignButtonContent, string templateId)
        {
            var objComment = TumblrView.Liker.Comment.GetSingeltonObjectComment();

            if (campaignButtonContent == ConstantVariable.CreateCampaign())
                objComment.CampaignName =
                    $"{SocialNetworks.Tumblr} {campaignDetails.SubModule} [{DateTime.Now.ToString(CultureInfo.InvariantCulture)}]";

            objComment.IsEditCampaignName = isEditCampaignName;
            objComment.CampaignButtonContent = campaignButtonContent;
            objComment.CancelEditVisibility = cancelEditVisibility;
            objComment.TemplateId = templateId;
            objComment.CampaignName = campaignDetails.CampaignName;
            objComment.SelectedAccountCount = campaignDetails.SelectedAccountList.Count +
                                              $" {"LangKeyAccountSelected".FromResourceDictionary()}";
            objComment.CommentFooter.list_SelectedAccounts = campaignDetails.SelectedAccountList;
            objComment.ObjViewModel.CommentModel =
                JsonConvert.DeserializeObject<CommentModel>(templateDetails.ActivitySettings);

            objComment.MainGrid.DataContext = objComment.ObjViewModel;

            TabSwitcher.ChangeTabIndex(2, 2);
        }
    }
}