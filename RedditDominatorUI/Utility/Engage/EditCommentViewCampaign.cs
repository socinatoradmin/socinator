using CommonServiceLocator;
using DominatorHouseCore.Enums;
using DominatorHouseCore.FileManagers;
using DominatorHouseCore.Utility;
using Newtonsoft.Json;
using RedditDominatorCore.RDModel;
using RedditDominatorCore.Utility;
using RedditDominatorUI.RDViews.Engage;
using System;
using System.Globalization;
using System.Windows;

namespace RedditDominatorUI.Utility.Engage
{
    public class EditCommentViewCampaign : IRdViewCampaign
    {
        public void ViewCampaigns(string campaignId, string openCampaignType)
        {
            var campaignFileManager = InstanceProvider.GetInstance<ICampaignsFileManager>();
            var campaignDetails = campaignFileManager.GetCampaignById(campaignId);
            var templatesFileManager = InstanceProvider.GetInstance<ITemplatesFileManager>();
            var templateDetails = templatesFileManager.GetTemplateById(campaignDetails.TemplateId);
            var editComment = EditComment.GetSingletonObjectEditComment();
            if (openCampaignType == ConstantVariable.CreateCampaign())
                editComment.CampaignName =
                    $"{SocialNetworks.Reddit} {campaignDetails.SubModule} [{DateTime.Now.ToString(CultureInfo.InvariantCulture)}]";
            editComment.IsEditCampaignName = true;
            editComment.CancelEditVisibility = Visibility.Visible;
            editComment.TemplateId = campaignDetails.TemplateId;
            editComment.CampaignName = openCampaignType == ConstantVariable.UpdateCampaign()
                ? campaignDetails.CampaignName
                : editComment.CampaignName;
            editComment.CampaignButtonContent = openCampaignType;
            editComment.SelectedAccountCount = campaignDetails.SelectedAccountList.Count +
                                               $" {"LangKeyAccountSelected".FromResourceDictionary()}";
            editComment.EditCommentFooter.list_SelectedAccounts = campaignDetails.SelectedAccountList;
            editComment.ObjViewModel.EditCommentModel =
                JsonConvert.DeserializeObject<EditCommentModel>(templateDetails.ActivitySettings);
            editComment.MainGrid.DataContext = editComment.ObjViewModel;
            TabSwitcher.ChangeTabIndex(3, 2);
        }
    }
}