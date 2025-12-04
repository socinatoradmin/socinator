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
    internal class SubscriberViewCampaign : IRdViewCampaign
    {
        public void ViewCampaigns(string campaignId, string openCampaignType)
        {
            var campaignFileManager = InstanceProvider.GetInstance<ICampaignsFileManager>();
            var campaignDetails = campaignFileManager.GetCampaignById(campaignId);
            var templatesFileManager = InstanceProvider.GetInstance<ITemplatesFileManager>();
            var templateDetails = templatesFileManager.GetTemplateById(campaignDetails.TemplateId);
            var objSubscribe = Subscribe.GetSingletonObjectSubscribe();
            if (openCampaignType == ConstantVariable.CreateCampaign())
                objSubscribe.CampaignName =
                    $"{SocialNetworks.Reddit} {campaignDetails.SubModule} [{DateTime.Now.ToString(CultureInfo.InvariantCulture)}]";
            objSubscribe.IsEditCampaignName = true;
            objSubscribe.CancelEditVisibility = Visibility.Visible;
            objSubscribe.TemplateId = campaignDetails.TemplateId;
            objSubscribe.CampaignName = openCampaignType == ConstantVariable.UpdateCampaign()
                ? campaignDetails.CampaignName
                : objSubscribe.CampaignName;
            objSubscribe.CampaignButtonContent = openCampaignType;
            objSubscribe.SelectedAccountCount = campaignDetails.SelectedAccountList.Count +
                                                $" {"LangKeyAccountSelected".FromResourceDictionary()}";
            objSubscribe.SubscribeFooter.list_SelectedAccounts = campaignDetails.SelectedAccountList;
            objSubscribe.ObjViewModel.SubscribeModel =
                JsonConvert.DeserializeObject<SubscribeModel>(templateDetails.ActivitySettings);
            objSubscribe.MainGrid.DataContext = objSubscribe.ObjViewModel;
            TabSwitcher.ChangeTabIndex(2, 0);
        }
    }
}