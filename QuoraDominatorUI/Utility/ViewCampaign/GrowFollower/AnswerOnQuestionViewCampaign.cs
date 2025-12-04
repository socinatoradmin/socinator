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
using QuoraDominatorUI.QDViews.Answers;

namespace QuoraDominatorUI.Utility.ViewCampaign.GrowFollower
{
    public class AnswerOnQuestionViewCampaign : IViewCampaignsFactory
    {
        public void ViewCampaigns(string campaignId, string openCampaignType)
        {
            var campaignDetails =
                InstanceProvider.GetInstance<ICampaignsFileManager>().GetCampaignById(campaignId);
            var templatesFileManager = InstanceProvider.GetInstance<ITemplatesFileManager>();
            var templateDetails = templatesFileManager.GetTemplateById(campaignDetails.TemplateId);
            var _ansOnQuestion = AnswerOnQuestion.GetSingeltonAnswerOnQuestion();
            if (openCampaignType == ConstantVariable.CreateCampaign())
                _ansOnQuestion.CampaignName =
                    $"{SocialNetworks.Quora} {campaignDetails.SubModule} [{DateTime.Now.ToString(CultureInfo.InvariantCulture)}]";
            _ansOnQuestion.IsEditCampaignName = true;
            _ansOnQuestion.CancelEditVisibility = Visibility.Visible;
            _ansOnQuestion.TemplateId = campaignDetails.TemplateId;
            _ansOnQuestion.CampaignButtonContent = openCampaignType;
            _ansOnQuestion.CampaignName = openCampaignType == ConstantVariable.UpdateCampaign()
                ? campaignDetails.CampaignName
                : _ansOnQuestion.CampaignName;
            _ansOnQuestion.SelectedAccountCount = campaignDetails.SelectedAccountList.Count +
                                                  $" {"LangKeyAccountSelected".FromResourceDictionary()}";
            _ansOnQuestion.AnswersQuestionFooter.list_SelectedAccounts = campaignDetails.SelectedAccountList;
            _ansOnQuestion.ObjViewModel.AnswerQuestionModel =
                JsonConvert.DeserializeObject<AnswerQuestionModel>(templateDetails.ActivitySettings);
            _ansOnQuestion.MainGrid.DataContext = _ansOnQuestion.ObjViewModel;
            TabSwitcher.ChangeTabIndex(6, 0);
        }
    }
}