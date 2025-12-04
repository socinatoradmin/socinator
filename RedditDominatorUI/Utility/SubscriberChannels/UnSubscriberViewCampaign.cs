using CommonServiceLocator;
using DominatorHouseCore.Enums;
using DominatorHouseCore.FileManagers;
using DominatorHouseCore.Utility;
using Newtonsoft.Json;
using RedditDominatorCore.RDModel;
using RedditDominatorCore.Utility;
using RedditDominatorUI.RDViews.Subscribe;
using System;
using System.Globalization;
using System.Windows;

namespace RedditDominatorUI.Utility.SubscriberChannels
{
    internal class UnSubscriberViewCampaign : IRdViewCampaign
    {
        public void ViewCampaigns(string campaignId, string openCampaignType)
        {
            var campaignFileManager = InstanceProvider.GetInstance<ICampaignsFileManager>();
            var campaignDetails = campaignFileManager.GetCampaignById(campaignId);
            var templatesFileManager = InstanceProvider.GetInstance<ITemplatesFileManager>();
            var templateDetails = templatesFileManager.GetTemplateById(campaignDetails.TemplateId);
            var objUnSubscribe = UnSubscribe.GetSingletonObjectUnSubscribe();
            if (openCampaignType == ConstantVariable.CreateCampaign())
                objUnSubscribe.CampaignName =
                    $"{SocialNetworks.Reddit} {campaignDetails.SubModule} [{DateTime.Now.ToString(CultureInfo.InvariantCulture)}]";
            objUnSubscribe.IsEditCampaignName = true;
            objUnSubscribe.CancelEditVisibility = Visibility.Visible;
            objUnSubscribe.TemplateId = campaignDetails.TemplateId;
            objUnSubscribe.CampaignName = openCampaignType == ConstantVariable.UpdateCampaign()
                ? campaignDetails.CampaignName
                : objUnSubscribe.CampaignName;
            objUnSubscribe.CampaignButtonContent = openCampaignType;
            objUnSubscribe.SelectedAccountCount = campaignDetails.SelectedAccountList.Count +
                                                  $" {"LangKeyAccountSelected".FromResourceDictionary()}";
            objUnSubscribe.UnSubscribeFooter.list_SelectedAccounts = campaignDetails.SelectedAccountList;
            objUnSubscribe.ObjViewModel.UnSubscribeModel =
                JsonConvert.DeserializeObject<UnSubscribeModel>(templateDetails.ActivitySettings);
            objUnSubscribe.MainGrid.DataContext = objUnSubscribe.ObjViewModel;
            TabSwitcher.ChangeTabIndex(2, 1);
        }
    }
}