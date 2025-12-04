using CommonServiceLocator;
using DominatorHouseCore.Enums;
using DominatorHouseCore.FileManagers;
using DominatorHouseCore.Utility;
using Newtonsoft.Json;
using RedditDominatorCore.RDModel;
using RedditDominatorCore.Utility;
using RedditDominatorUI.RDViews.Voting;
using System;
using System.Globalization;
using System.Windows;

namespace RedditDominatorUI.Utility.Voting
{
    internal class DownVoteCommentViewCampaign : IRdViewCampaign
    {
        public void ViewCampaigns(string campaignId, string openCampaignType)
        {
            var campaignFileManager = InstanceProvider.GetInstance<ICampaignsFileManager>();
            var campaignDetails = campaignFileManager.GetCampaignById(campaignId);
            var templatesFileManager = InstanceProvider.GetInstance<ITemplatesFileManager>();
            var templateDetails = templatesFileManager.GetTemplateById(campaignDetails.TemplateId);
            var objDownvote = DownvoteForComment.GetSingeltonObjectDownvoteForComment();
            if (openCampaignType == ConstantVariable.CreateCampaign())
                objDownvote.CampaignName =
                    $"{SocialNetworks.Reddit} {campaignDetails.SubModule} [{DateTime.Now.ToString(CultureInfo.InvariantCulture)}]";
            objDownvote.IsEditCampaignName = true;
            objDownvote.CancelEditVisibility = Visibility.Visible;
            objDownvote.TemplateId = campaignDetails.TemplateId;
            objDownvote.CampaignName = openCampaignType == ConstantVariable.UpdateCampaign()
                ? campaignDetails.CampaignName
                : objDownvote.CampaignName;
            objDownvote.CampaignButtonContent = openCampaignType;
            objDownvote.SelectedAccountCount = campaignDetails.SelectedAccountList.Count +
                                               $" {"LangKeyAccountSelected".FromResourceDictionary()}";
            objDownvote.DownvoteFooter.list_SelectedAccounts = campaignDetails.SelectedAccountList;
            objDownvote.ObjViewModel.DownvoteModel =
                JsonConvert.DeserializeObject<DownvoteModel>(templateDetails.ActivitySettings);
            objDownvote.MainGrid.DataContext = objDownvote.ObjViewModel;
            TabSwitcher.ChangeTabIndex(4, 4);
        }
    }
}