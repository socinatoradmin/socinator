using System;
using System.Globalization;
using System.Windows;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using GramDominatorCore.GDModel;
using GramDominatorCore.Interface;

namespace GramDominatorUI.Utility.InstalikerCommenter.ReplyComment
{
    internal class ReplyCommentsViewCampaign : IGdViewCampaign
    {
        public void ManageCampaign(TemplateModel templateDetails, CampaignDetails campaignDetails,
            bool isEditCampaignName, Visibility cancelEditVisibility, string campaignButtonContent, string templateId)
        {
            var objReplyCommentModel = GDViews.InstaLikeComment.ReplyComment.SingletonReplyComment();
            if (campaignButtonContent == ConstantVariable.CreateCampaign())
                objReplyCommentModel.CampaignName =
                    $"{SocialNetworks.Instagram} {campaignDetails.SubModule} [{DateTime.Now.ToString(CultureInfo.InvariantCulture)}]"; //added new line

            objReplyCommentModel.IsEditCampaignName = isEditCampaignName;
            objReplyCommentModel.CancelEditVisibility = cancelEditVisibility;
            objReplyCommentModel.TemplateId = templateId;
            objReplyCommentModel.CampaignButtonContent = campaignButtonContent;
            objReplyCommentModel.CampaignName = campaignButtonContent == ConstantVariable.UpdateCampaign()
                ? campaignDetails.CampaignName
                : objReplyCommentModel.CampaignName; //updated line          
            // objMediaUnliker.CampaignName = campaignDetails.CampaignName;           
            objReplyCommentModel.SelectedAccountCount = campaignDetails.SelectedAccountList.Count +
                                                        $" {"LangKeyAccountSelected".FromResourceDictionary()}";
            objReplyCommentModel.ReplyCommentFooter.list_SelectedAccounts = campaignDetails.SelectedAccountList;

            objReplyCommentModel.ObjViewModel.ReplyCommentModel =
                templateDetails.ActivitySettings.GetActivityModel<ReplyCommentModel>(
                    objReplyCommentModel.ObjViewModel.Model, true);

            // objMediaUnliker.MainGrid.DataContext = objMediaUnliker.ObjViewModel.MediaUnlikerModel;
            objReplyCommentModel.MainGrid.DataContext = objReplyCommentModel.ObjViewModel;

            TabSwitcher.ChangeTabIndex(4, 4);
        }
    }
}