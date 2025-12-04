using System;
using System.Globalization;
using System.Windows;
using CommonServiceLocator;
using DominatorHouseCore.Enums;
using DominatorHouseCore.FileManagers;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Utility;
using Newtonsoft.Json;
using QuoraDominatorCore.Models;
using QuoraDominatorUI.QDViews.Voting;

namespace QuoraDominatorUI.Utility.ViewCampaign.Voting
{
    public class UpvoteAnswersViewCampaign : IViewCampaignsFactory
    {
        public void ViewCampaigns(string campaignId, string openCampaignType)
        {
            var campaignFileManager = InstanceProvider.GetInstance<ICampaignsFileManager>();
            var campaignDetails = campaignFileManager.GetCampaignById(campaignId);
            var templatesFileManager = InstanceProvider.GetInstance<ITemplatesFileManager>();
            var templateDetails = templatesFileManager.GetTemplateById(campaignDetails.TemplateId);
            var upvoteAnswers = UpvoteAnswers.GetSingeltonObjectUpvoteAnswers();
            if (openCampaignType == ConstantVariable.CreateCampaign())
                upvoteAnswers.CampaignName =
                    $"{SocialNetworks.Quora} {campaignDetails.SubModule} [{DateTime.Now.ToString(CultureInfo.InvariantCulture)}]";

            upvoteAnswers.IsEditCampaignName = true;
            upvoteAnswers.CancelEditVisibility = Visibility.Visible;
            upvoteAnswers.TemplateId = campaignDetails.TemplateId;
            upvoteAnswers.CampaignButtonContent = openCampaignType;
            upvoteAnswers.CampaignName = openCampaignType == ConstantVariable.UpdateCampaign()
                ? campaignDetails.CampaignName
                : upvoteAnswers.CampaignName;
            upvoteAnswers.SelectedAccountCount = campaignDetails.SelectedAccountList.Count +
                                                 $" {"LangKeyAccountSelected".FromResourceDictionary()}";
            upvoteAnswers.UpvoteAnswersFooter.list_SelectedAccounts = campaignDetails.SelectedAccountList;
            upvoteAnswers.ObjViewModel.UpvoteAnswersModel =
                JsonConvert.DeserializeObject<UpvoteAnswersModel>(templateDetails.ActivitySettings);
            upvoteAnswers.MainGrid.DataContext = upvoteAnswers.ObjViewModel;
            TabSwitcher.ChangeTabIndex(2, 0);
        }
    }
}