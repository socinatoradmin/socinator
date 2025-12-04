using System;
using System.Globalization;
using System.Windows;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using GramDominatorCore.GDModel;
using GramDominatorCore.Interface;
using GramDominatorUI.GDViews.InstaLikeComment;

namespace GramDominatorUI.Utility.InstalikerCommenter.LikeComments
{
    public class LikeCommentViewCampaign : IGdViewCampaign
    {
        public void ManageCampaign(TemplateModel templateDetails, CampaignDetails campaignDetails,
            bool isEditCampaignName,
            Visibility cancelEditVisibility, string campaignButtonContent, string templateId)
        {
            var objLikeComment = LikeComment.GetSingeltonObjectLikeComment();
            if (campaignButtonContent == ConstantVariable.CreateCampaign())
                objLikeComment.CampaignName =
                    $"{SocialNetworks.Instagram} {campaignDetails.SubModule} [{DateTime.Now.ToString(CultureInfo.InvariantCulture)}]"; //added new line

            objLikeComment.IsEditCampaignName = isEditCampaignName;
            objLikeComment.CancelEditVisibility = cancelEditVisibility;
            objLikeComment.TemplateId = templateId;
            objLikeComment.CampaignButtonContent = campaignButtonContent;
            objLikeComment.CampaignName = campaignButtonContent == ConstantVariable.UpdateCampaign()
                ? campaignDetails.CampaignName
                : objLikeComment.CampaignName; //updated line          
            // objLikeComment.CampaignName = campaignDetails.CampaignName;
            objLikeComment.SelectedAccountCount = campaignDetails.SelectedAccountList.Count +
                                                  $" {"LangKeyAccountSelected".FromResourceDictionary()}";
            objLikeComment.LikeCommentFooter.list_SelectedAccounts = campaignDetails.SelectedAccountList;

            objLikeComment.ObjViewModel.LikeCommentModel =
                templateDetails.ActivitySettings.GetActivityModel<LikeCommentModel>(objLikeComment.ObjViewModel.Model);
            objLikeComment.MainGrid.DataContext = objLikeComment.ObjViewModel;

            TabSwitcher.ChangeTabIndex(4, 2);
        }
    }
}