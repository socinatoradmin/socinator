using System;
using System.Globalization;
using System.Windows;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using PinDominatorCore.Interface;
using PinDominatorCore.PDModel;
using comment = PinDominator.PDViews.PinTryComment.Comment;

namespace PinDominator.Utility.Pin_Try_Comment.Comment
{
    public class CommentViewCampaigns : IPdViewCampaign
    {
        public void ManageCampaign(TemplateModel templateDetails, CampaignDetails campaignDetails,
            bool isEditCampaignName, Visibility cancelEditVisibility, string campaignButtonContent, string templateId)
        {
            var objComment = comment.GetSingeltonObjectComment();
            if (campaignButtonContent == ConstantVariable.CreateCampaign())
                objComment.CampaignName =
                    $"{SocialNetworks.Pinterest} {campaignDetails.SubModule} [{DateTime.Now.ToString(CultureInfo.InvariantCulture)}]";
            objComment.IsEditCampaignName = isEditCampaignName;
            objComment.CancelEditVisibility = Visibility.Visible;
            objComment.CampaignButtonContent = campaignButtonContent;
            objComment.SelectedAccountCount = campaignDetails.SelectedAccountList.Count +
                                              $" {"LangKeyAccountSelected".FromResourceDictionary()}";
            objComment.TemplateId = templateId;
            objComment.CommentFooter.list_SelectedAccounts = campaignDetails.SelectedAccountList;
            objComment.CampaignName = campaignButtonContent == ConstantVariable.UpdateCampaign()
                ? campaignDetails.CampaignName
                : objComment.CampaignName;

            objComment.ObjViewModel.CommentModel =
                templateDetails.ActivitySettings.GetActivityModel<CommentModel>(objComment.ObjViewModel.Model);
            objComment.MainGrid.DataContext = objComment.ObjViewModel;
            TabSwitcher.ChangeTabIndex(4, 0);
        }
    }
}