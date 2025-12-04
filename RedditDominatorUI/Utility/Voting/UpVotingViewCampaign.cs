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
    internal class UpVotingViewCampaign : IRdViewCampaign
    {
        public void ViewCampaigns(string campaignId, string openCampaignType)
        {
            var campaignFileManager = InstanceProvider.GetInstance<ICampaignsFileManager>();
            var campaignDetails = campaignFileManager.GetCampaignById(campaignId);
            var templatesFileManager = InstanceProvider.GetInstance<ITemplatesFileManager>();
            var templateDetails = templatesFileManager.GetTemplateById(campaignDetails.TemplateId);
            var objUpvote = Upvote.GetSingletonObjectUpvote();
            if (openCampaignType == ConstantVariable.CreateCampaign())
                objUpvote.CampaignName =
                    $"{SocialNetworks.Reddit} {campaignDetails.SubModule} [{DateTime.Now.ToString(CultureInfo.InvariantCulture)}]";
            objUpvote.IsEditCampaignName = true;
            objUpvote.CancelEditVisibility = Visibility.Visible;
            objUpvote.TemplateId = campaignDetails.TemplateId;
            objUpvote.CampaignName = openCampaignType == ConstantVariable.UpdateCampaign()
                ? campaignDetails.CampaignName
                : objUpvote.CampaignName;
            objUpvote.CampaignButtonContent = openCampaignType;
            objUpvote.SelectedAccountCount = campaignDetails.SelectedAccountList.Count +
                                             $" {"LangKeyAccountSelected".FromResourceDictionary()}";
            objUpvote.UpvoteFooter.list_SelectedAccounts = campaignDetails.SelectedAccountList;
            objUpvote.ObjViewModel.UpvoteModel =
                JsonConvert.DeserializeObject<UpvoteModel>(templateDetails.ActivitySettings);
            objUpvote.MainGrid.DataContext = objUpvote.ObjViewModel;
            TabSwitcher.ChangeTabIndex(4, 0);
        }
    }
}