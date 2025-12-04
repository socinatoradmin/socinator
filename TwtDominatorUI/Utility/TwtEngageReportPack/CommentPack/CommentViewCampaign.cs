using System;
using System.Globalization;
using System.Windows;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using TwtDominatorCore.Interface;
using TwtDominatorCore.TDModels;
using TwtDominatorUI.TDViews.TwtEngage;

namespace TwtDominatorUI.Utility.TwtEngageReportPack.CommentPack
{
    public class CommentViewCampaign : ITDViewCampaign
    {
        public void ManageCampaign(TemplateModel templateDetails, CampaignDetails campaignDetails,
            bool isEditCampaignName,
            Visibility cancelEditVisibility, string campaignButtonContent, string templateId)
        {
            var objComment = Comment.GetSingletonObjectComment();


            if (campaignButtonContent == ConstantVariable.CreateCampaign())
                objComment.CampaignName =
                    $"{SocialNetworks.Twitter} {campaignDetails.SubModule} [{DateTime.Now.ToString(CultureInfo.InvariantCulture)}]";

            objComment.IsEditCampaignName = isEditCampaignName;
            objComment.CancelEditVisibility = cancelEditVisibility;
            objComment.TemplateId = templateId;

            objComment.CampaignName = campaignButtonContent == ConstantVariable.UpdateCampaign()
                ? campaignDetails.CampaignName
                : objComment.CampaignName;

            objComment.CampaignButtonContent = campaignButtonContent;
            objComment.SelectedAccountCount = campaignDetails.SelectedAccountList.Count +
                                              $" {"LangKeyAccountSelected".FromResourceDictionary()}";
            objComment.CommentFooter.list_SelectedAccounts = campaignDetails.SelectedAccountList;

            // Doubt FollowerModel FollowBack
            objComment.ObjViewModel.CommentModel =
                templateDetails.ActivitySettings.GetActivityModel<CommentModel>(objComment.ObjViewModel.Model);

            objComment.MainGrid.DataContext = objComment.ObjViewModel;

            TabSwitcher.ChangeTabIndex(3, 1);
        }
    }
}