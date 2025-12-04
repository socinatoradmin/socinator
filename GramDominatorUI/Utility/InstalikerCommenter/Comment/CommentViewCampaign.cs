using System;
using System.Globalization;
using System.Windows;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using GramDominatorCore.GDModel;
using GramDominatorCore.Interface;

namespace GramDominatorUI.Utility.InstalikerCommenter.Comment
{
    internal class CommentViewCampaign : IGdViewCampaign
    {
        public void ManageCampaign(TemplateModel templateDetails, CampaignDetails campaignDetails,
            bool isEditCampaignName,
            Visibility cancelEditVisibility, string campaignButtonContent, string templateId)
        {
            var objComment = GDViews.InstaLikeComment.Comment.GetSingeltonObjectComment();
            if (campaignButtonContent == ConstantVariable.CreateCampaign())
                objComment.CampaignName =
                    $"{SocialNetworks.Instagram} {campaignDetails.SubModule} [{DateTime.Now.ToString(CultureInfo.InvariantCulture)}]"; //added new line

            objComment.IsEditCampaignName = isEditCampaignName;
            objComment.CancelEditVisibility = cancelEditVisibility;
            objComment.TemplateId = templateId;
            objComment.CampaignButtonContent = campaignButtonContent;
            objComment.CampaignName = campaignButtonContent == ConstantVariable.UpdateCampaign()
                ? campaignDetails.CampaignName
                : objComment.CampaignName; //updated line          
            //  objComment.CampaignName = campaignDetails.CampaignName;
            objComment.SelectedAccountCount = campaignDetails.SelectedAccountList.Count +
                                              $" {"LangKeyAccountSelected".FromResourceDictionary()}";
            objComment.CommentFooter.list_SelectedAccounts = campaignDetails.SelectedAccountList;

            objComment.ObjViewModel.CommentModel =
                templateDetails.ActivitySettings.GetActivityModel<CommentModel>(objComment.ObjViewModel.Model);

            // objComment.MainGrid.DataContext = objComment.ObjViewModel.CommentModel;
            objComment.MainGrid.DataContext = objComment.ObjViewModel;

            TabSwitcher.ChangeTabIndex(4, 1);
        }
    }
}