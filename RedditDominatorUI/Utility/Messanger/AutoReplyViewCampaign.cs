using CommonServiceLocator;
using DominatorHouseCore.Enums;
using DominatorHouseCore.FileManagers;
using DominatorHouseCore.Utility;
using Newtonsoft.Json;
using RedditDominatorCore.RDModel;
using RedditDominatorCore.Utility;
using RedditDominatorUI.RDViews.Messanger;
using System;
using System.Globalization;
using System.Windows;

namespace RedditDominatorUI.Utility.Messanger
{
    class AutoReplyViewCampaign : IRdViewCampaign
    {
        public void ViewCampaigns(string campaignId, string openCampaignType)
        {
            var campaignFileManager = InstanceProvider.GetInstance<ICampaignsFileManager>();
            var campaignDetails = campaignFileManager.GetCampaignById(campaignId);
            var templatesFileManager = InstanceProvider.GetInstance<ITemplatesFileManager>();
            var templateDetails = templatesFileManager.GetTemplateById(campaignDetails.TemplateId);
            var objAutoReply = AutoReply.GetSingletonObjectAutoReply();
            if (openCampaignType == ConstantVariable.CreateCampaign())
                objAutoReply.CampaignName =
                    $"{SocialNetworks.Reddit} {campaignDetails.SubModule} [{DateTime.Now.ToString(CultureInfo.InvariantCulture)}]";
            objAutoReply.IsEditCampaignName = true;
            objAutoReply.CancelEditVisibility = Visibility.Visible;
            objAutoReply.TemplateId = campaignDetails.TemplateId;
            objAutoReply.CampaignName = openCampaignType == ConstantVariable.UpdateCampaign()
                ? campaignDetails.CampaignName
                : objAutoReply.CampaignName;
            objAutoReply.CampaignButtonContent = openCampaignType;
            objAutoReply.SelectedAccountCount = campaignDetails.SelectedAccountList.Count +
                                                       $" {"LangKeyAccountSelected".FromResourceDictionary()}";

            objAutoReply.MessageFooter.list_SelectedAccounts = campaignDetails.SelectedAccountList;
            objAutoReply.ObjViewModel.MessageModel =
                JsonConvert.DeserializeObject<AutoReplyModel>(templateDetails.ActivitySettings);
            objAutoReply.MainGrid.DataContext = objAutoReply.ObjViewModel;
            TabSwitcher.ChangeTabIndex(5, 1);
        }
    }
}
