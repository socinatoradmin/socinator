using CommonServiceLocator;
using DominatorHouseCore.Enums;
using DominatorHouseCore.FileManagers;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using MahApps.Metro.Controls.Dialogs;
using Newtonsoft.Json;
using RedditDominatorCore.RDModel;
using RedditDominatorCore.Utility;
using RedditDominatorUI.RDViews.AutoFeedActivity;
using System;
using System.Globalization;
using System.Windows;

namespace RedditDominatorUI.Utility.AutoActivity
{
    public class AutoActivityViewCampaign : IRdViewCampaign
    {
        private readonly IGenericFileManager genericFileManager;
        public AutoActivityViewCampaign()
        {
            genericFileManager = InstanceProvider.GetInstance<IGenericFileManager>();
        }
        public void ViewCampaigns(string campaignId, string openCampaignType)
        {
            if (genericFileManager == null) return;
            if (genericFileManager != null)
            {
                var OtherModel = genericFileManager.GetModel<RedditOtherConfigModel>(ConstantVariable.GetOtherRedditSettingsFile()) ?? new RedditOtherConfigModel();
                if (OtherModel != null && !OtherModel.IsEnableFeedActivity)
                {
                    Dialog.ShowDialog(Application.Current.MainWindow, "Enable Auto Activity",
                    "Please Enable Auto Activity For Reddit From Other Configuration To Start Auto Activity.");
                    return;
                }
            }
            var campaignFileManager = InstanceProvider.GetInstance<ICampaignsFileManager>();
            var campaignDetails = campaignFileManager.GetCampaignById(campaignId);
            var templatesFileManager = InstanceProvider.GetInstance<ITemplatesFileManager>();
            var templateDetails = templatesFileManager.GetTemplateById(campaignDetails.TemplateId);
            var autoActivity = RedditPostAutoActivity.GetSingletonInstance();
            if (openCampaignType == ConstantVariable.CreateCampaign())
                autoActivity.CampaignName =
                    $"{SocialNetworks.Reddit} {campaignDetails.SubModule} [{DateTime.Now.ToString(CultureInfo.InvariantCulture)}]";
            autoActivity.IsEditCampaignName = true;
            autoActivity.CancelEditVisibility = Visibility.Visible;
            autoActivity.TemplateId = campaignDetails.TemplateId;
            autoActivity.CampaignName = openCampaignType == ConstantVariable.UpdateCampaign()
                ? campaignDetails.CampaignName
                : autoActivity.CampaignName;
            autoActivity.CampaignButtonContent = openCampaignType;
            autoActivity.SelectedAccountCount = campaignDetails.SelectedAccountList.Count +
                                               $" {"LangKeyAccountSelected".FromResourceDictionary()}";
            autoActivity.AutoActivityFooter.list_SelectedAccounts = campaignDetails.SelectedAccountList;
            autoActivity.ObjViewModel.AutoActivityModel =
                JsonConvert.DeserializeObject<PostAutoActivityModel>(templateDetails.ActivitySettings);
            autoActivity.MainGrid.DataContext = autoActivity.ObjViewModel;
            TabSwitcher.ChangeTabIndex(7, 0);
        }
    }
}
