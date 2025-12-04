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
    public class AnswersScraperViewCampaign : IViewCampaignsFactory
    {
        public void ViewCampaigns(string campaignId, string openCampaignType)
        {
            var campaignFileManager = InstanceProvider.GetInstance<ICampaignsFileManager>();
            var campaignDetails = campaignFileManager.GetCampaignById(campaignId);
            var templatesFileManager = InstanceProvider.GetInstance<ITemplatesFileManager>();
            var templateDetails = templatesFileManager.GetTemplateById(campaignDetails.TemplateId);
            var answersScraper = AnswersScraper.GetSingeltonObjectAnswersScraper();
            if (openCampaignType == ConstantVariable.CreateCampaign())
                answersScraper.CampaignName =
                    $"{SocialNetworks.Quora} {campaignDetails.SubModule} [{DateTime.Now.ToString(CultureInfo.InvariantCulture)}]";

            answersScraper.IsEditCampaignName = true;
            answersScraper.CancelEditVisibility = Visibility.Visible;
            answersScraper.TemplateId = campaignDetails.TemplateId;
            answersScraper.CampaignButtonContent = openCampaignType;
            answersScraper.CampaignName = openCampaignType == ConstantVariable.UpdateCampaign()
                ? campaignDetails.CampaignName
                : answersScraper.CampaignName;
            answersScraper.SelectedAccountCount = campaignDetails.SelectedAccountList.Count +
                                                  $" {"LangKeyAccountSelected".FromResourceDictionary()}";
            answersScraper.AnswersScraperFooter.list_SelectedAccounts = campaignDetails.SelectedAccountList;
            answersScraper.ObjViewModel.AnswersScraperModel =
                JsonConvert.DeserializeObject<AnswersScraperModel>(templateDetails.ActivitySettings);
            answersScraper.MainGrid.DataContext = answersScraper.ObjViewModel;
            TabSwitcher.ChangeTabIndex(4, 2);
        }
    }
}