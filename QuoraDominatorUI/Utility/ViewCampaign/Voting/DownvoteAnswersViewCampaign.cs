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
    public class DownvoteAnswersViewCampaign : IViewCampaignsFactory
    {
        public void ViewCampaigns(string campaignId, string openCampaignType)
        {
            var campaignFileManager = InstanceProvider.GetInstance<ICampaignsFileManager>();
            var campaignDetails = campaignFileManager.GetCampaignById(campaignId);
            var templatesFileManager = InstanceProvider.GetInstance<ITemplatesFileManager>();
            var templateDetails = templatesFileManager.GetTemplateById(campaignDetails.TemplateId);
            var downvoteAnswers = DownvoteAnswers.GetSingeltonObjectDownvoteAnswers();
            if (openCampaignType == ConstantVariable.CreateCampaign())
                downvoteAnswers.CampaignName =
                    $"{SocialNetworks.Quora} {campaignDetails.SubModule} [{DateTime.Now.ToString(CultureInfo.InvariantCulture)}]";

            downvoteAnswers.IsEditCampaignName = true;
            downvoteAnswers.CancelEditVisibility = Visibility.Visible;
            downvoteAnswers.TemplateId = campaignDetails.TemplateId;
            downvoteAnswers.CampaignButtonContent = openCampaignType;
            downvoteAnswers.CampaignName = openCampaignType == ConstantVariable.UpdateCampaign()
                ? campaignDetails.CampaignName
                : downvoteAnswers.CampaignName;
            downvoteAnswers.SelectedAccountCount = campaignDetails.SelectedAccountList.Count +
                                                   $" {"LangKeyAccountSelected".FromResourceDictionary()}";
            downvoteAnswers.Footer.list_SelectedAccounts = campaignDetails.SelectedAccountList;
            downvoteAnswers.ObjViewModel.DownvoteAnswersModel =
                JsonConvert.DeserializeObject<DownvoteAnswersModel>(templateDetails.ActivitySettings);
            downvoteAnswers.MainGrid.DataContext = downvoteAnswers.ObjViewModel;
            TabSwitcher.ChangeTabIndex(2, 1);
        }
    }
}