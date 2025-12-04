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
    internal class RemoveCommentVotingViewCampaign : IRdViewCampaign
    {
        public void ViewCampaigns(string campaignId, string openCampaignType)
        {
            var campaignFileManager = InstanceProvider.GetInstance<ICampaignsFileManager>();
            var campaignDetails = campaignFileManager.GetCampaignById(campaignId);
            var templatesFileManager = InstanceProvider.GetInstance<ITemplatesFileManager>();
            var templateDetails = templatesFileManager.GetTemplateById(campaignDetails.TemplateId);
            var objRemovevote = RemovevoteForComment.GetSingletonObjectRemovevoteForComment();
            if (openCampaignType == ConstantVariable.CreateCampaign())
                objRemovevote.CampaignName =
                    $"{SocialNetworks.Reddit} {campaignDetails.SubModule} [{DateTime.Now.ToString(CultureInfo.InvariantCulture)}]";
            objRemovevote.IsEditCampaignName = true;
            objRemovevote.CancelEditVisibility = Visibility.Visible;
            objRemovevote.TemplateId = campaignDetails.TemplateId;
            objRemovevote.CampaignName = openCampaignType == ConstantVariable.UpdateCampaign()
                ? campaignDetails.CampaignName
                : objRemovevote.CampaignName;
            objRemovevote.CampaignButtonContent = openCampaignType;
            objRemovevote.SelectedAccountCount = campaignDetails.SelectedAccountList.Count + "Account Selected";
            objRemovevote.RemoveVoteFooter.list_SelectedAccounts = campaignDetails.SelectedAccountList;
            objRemovevote.ObjViewModel.RemoveVoteModel =
                JsonConvert.DeserializeObject<RemoveVoteModel>(templateDetails.ActivitySettings);
            objRemovevote.MainGrid.DataContext = objRemovevote.ObjViewModel;
            TabSwitcher.ChangeTabIndex(4, 5);
        }
    }
}