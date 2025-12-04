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
using QuoraDominatorUI.QDViews.Scrape;

namespace QuoraDominatorUI.Utility.ViewCampaign.Scrape
{
    public class QuestionsScraperViewCampaign : IViewCampaignsFactory
    {
        public void ViewCampaigns(string campaignId, string openCampaignType)
        {
            var campaignFileManager = InstanceProvider.GetInstance<ICampaignsFileManager>();
            var campaignDetails = campaignFileManager.GetCampaignById(campaignId);
            var templatesFileManager = InstanceProvider.GetInstance<ITemplatesFileManager>();
            var templateDetails = templatesFileManager.GetTemplateById(campaignDetails.TemplateId);
            var questionsScraper = QuestionsScraper.GetSingeltonObjectQuestionsScraper();
            if (openCampaignType == ConstantVariable.CreateCampaign())
                questionsScraper.CampaignName =
                    $"{SocialNetworks.Quora} {campaignDetails.SubModule} [{DateTime.Now.ToString(CultureInfo.InvariantCulture)}]";

            questionsScraper.IsEditCampaignName = true;
            questionsScraper.CancelEditVisibility = Visibility.Visible;
            questionsScraper.TemplateId = campaignDetails.TemplateId;
            questionsScraper.CampaignButtonContent = openCampaignType;
            questionsScraper.CampaignName = openCampaignType == ConstantVariable.UpdateCampaign()
                ? campaignDetails.CampaignName
                : questionsScraper.CampaignName;
            questionsScraper.SelectedAccountCount = campaignDetails.SelectedAccountList.Count +
                                                    $" {"LangKeyAccountSelected".FromResourceDictionary()}";
            questionsScraper.QuestionsScraperFooter.list_SelectedAccounts = campaignDetails.SelectedAccountList;
            questionsScraper.ObjViewModel.QuestionsScraperModel =
                JsonConvert.DeserializeObject<QuestionsScraperModel>(templateDetails.ActivitySettings);
            questionsScraper.MainGrid.DataContext = questionsScraper.ObjViewModel;
            TabSwitcher.ChangeTabIndex(4, 1);
        }
    }
}