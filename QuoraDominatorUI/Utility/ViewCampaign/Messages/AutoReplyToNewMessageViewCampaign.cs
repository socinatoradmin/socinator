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
using QuoraDominatorUI.QDViews.Messages;

namespace QuoraDominatorUI.Utility.ViewCampaign.Messages
{
    public class AutoReplyToNewMessageViewCampaign : IViewCampaignsFactory
    {
        public void ViewCampaigns(string campaignId, string openCampaignType)
        {
            var campaignFileManager = InstanceProvider.GetInstance<ICampaignsFileManager>();
            var campaignDetails = campaignFileManager.GetCampaignById(campaignId);
            var templatesFileManager = InstanceProvider.GetInstance<ITemplatesFileManager>();
            var templateDetails = templatesFileManager.GetTemplateById(campaignDetails.TemplateId);
            var autoReplyToNewMessage = AutoReplyToNewMessage.GetSingeltonAutoReplyToNewMessage();
            if (openCampaignType == ConstantVariable.CreateCampaign())
                autoReplyToNewMessage.CampaignName =
                    $"{SocialNetworks.Quora} {campaignDetails.SubModule} [{DateTime.Now.ToString(CultureInfo.InvariantCulture)}]";
            autoReplyToNewMessage.IsEditCampaignName = true;
            autoReplyToNewMessage.CancelEditVisibility = Visibility.Visible;
            autoReplyToNewMessage.TemplateId = campaignDetails.TemplateId;
            autoReplyToNewMessage.CampaignButtonContent = openCampaignType;
            autoReplyToNewMessage.CampaignName = openCampaignType == ConstantVariable.UpdateCampaign()
                ? campaignDetails.CampaignName
                : autoReplyToNewMessage.CampaignName;

            autoReplyToNewMessage.SelectedAccountCount = campaignDetails.SelectedAccountList.Count +
                                                         $" {"LangKeyAccountSelected".FromResourceDictionary()}";
            autoReplyToNewMessage.AutoReplyToNewMessageFooter.list_SelectedAccounts =
                campaignDetails.SelectedAccountList;
            autoReplyToNewMessage.ObjViewModel.AutoReplyToNewMessageModel =
                JsonConvert.DeserializeObject<AutoReplyToNewMessageModel>(templateDetails.ActivitySettings);
            autoReplyToNewMessage.MainGrid.DataContext = autoReplyToNewMessage.ObjViewModel;
            TabSwitcher.ChangeTabIndex(5, 1);
        }
    }
}