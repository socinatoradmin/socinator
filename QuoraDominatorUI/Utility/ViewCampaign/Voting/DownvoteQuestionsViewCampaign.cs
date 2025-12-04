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
    public class DownvoteQuestionsViewCampaign : IViewCampaignsFactory
    {
        public void ViewCampaigns(string campaignId, string openCampaignType)
        {
            var campaignFileManager = InstanceProvider.GetInstance<ICampaignsFileManager>();
            var campaignDetails = campaignFileManager.GetCampaignById(campaignId);
            var templatesFileManager = InstanceProvider.GetInstance<ITemplatesFileManager>();
            var templateDetails = templatesFileManager.GetTemplateById(campaignDetails.TemplateId);
            var downvoteQuestions = DownvoteQuestions.GetSingeltonObjectDownvote();
            if (openCampaignType == ConstantVariable.CreateCampaign())
                downvoteQuestions.CampaignName =
                    $"{SocialNetworks.Quora} {campaignDetails.SubModule} [{DateTime.Now.ToString(CultureInfo.InvariantCulture)}]";

            downvoteQuestions.IsEditCampaignName = true;
            downvoteQuestions.CancelEditVisibility = Visibility.Visible;
            downvoteQuestions.TemplateId = campaignDetails.TemplateId;
            downvoteQuestions.CampaignButtonContent = openCampaignType;
            downvoteQuestions.CampaignName = openCampaignType == ConstantVariable.UpdateCampaign()
                ? campaignDetails.CampaignName
                : downvoteQuestions.CampaignName;
            downvoteQuestions.SelectedAccountCount = campaignDetails.SelectedAccountList.Count +
                                                     $" {"LangKeyAccountSelected".FromResourceDictionary()}";
            downvoteQuestions.DownvoteQuestionsFooter.list_SelectedAccounts = campaignDetails.SelectedAccountList;
            downvoteQuestions.ObjViewModel.DownvoteQuestionsModel =
                JsonConvert.DeserializeObject<DownvoteQuestionsModel>(templateDetails.ActivitySettings);
            downvoteQuestions.MainGrid.DataContext = downvoteQuestions.ObjViewModel;
            TabSwitcher.ChangeTabIndex(2, 2);
        }
    }
}