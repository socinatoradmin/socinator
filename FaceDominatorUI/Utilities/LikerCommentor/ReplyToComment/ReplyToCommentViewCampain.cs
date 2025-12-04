using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using FaceDominatorCore.FDModel.LikerCommentorModel;
using FaceDominatorCore.Interface;
using System;
using System.Globalization;
using System.Windows;

namespace FaceDominatorUI.Utilities.LikerCommentor.ReplyToComment
{
    public class ReplyToCommentViewCampain : IFdViewCampaign
    {
        public void ManageCampaign(TemplateModel templateDetails, CampaignDetails campaignDetails,
            bool isEditCampaignName,
            Visibility cancelEditVisibility, string campaignButtonContent, string templateId)
        {
            var singeltonObjectReplyToComment =
                FDViews.FbLikerCommentor.ReplyToComment.GetSingeltonObjectReplyToComment();
            singeltonObjectReplyToComment.IsEditCampaignName = isEditCampaignName;
            singeltonObjectReplyToComment.CancelEditVisibility = cancelEditVisibility;
            singeltonObjectReplyToComment.TemplateId = templateId;

            if (campaignButtonContent == ConstantVariable.CreateCampaign())
                singeltonObjectReplyToComment.CampaignName =
                    $"{SocialNetworks.Facebook} {campaignDetails.SubModule} [{DateTime.Now.ToString(CultureInfo.InvariantCulture)}]";
            singeltonObjectReplyToComment.CampaignName = campaignButtonContent == ConstantVariable.UpdateCampaign()
                ? campaignDetails.CampaignName
                : singeltonObjectReplyToComment.CampaignName;

            singeltonObjectReplyToComment.CampaignName = campaignDetails.CampaignName;
            singeltonObjectReplyToComment.CampaignButtonContent = campaignButtonContent;
            singeltonObjectReplyToComment.SelectedAccountCount =
                campaignDetails.SelectedAccountList.Count + $" {"LangKeyAccountSelected".FromResourceDictionary()}";
            singeltonObjectReplyToComment.ReplyToCommentsFooter.list_SelectedAccounts =
                campaignDetails.SelectedAccountList;

            singeltonObjectReplyToComment.ObjViewModel.ReplyToCommentsModel
                = templateDetails.ActivitySettings.GetActivityModel<ReplyToCommentModel>(singeltonObjectReplyToComment
                    .ObjViewModel.Model);

            singeltonObjectReplyToComment.MainGrid.DataContext = singeltonObjectReplyToComment.ObjViewModel;

            TabSwitcher.ChangeTabIndex(4, 4);
        }
    }
}