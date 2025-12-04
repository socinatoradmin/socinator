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
    public class SendMessageToFollowerViewCampaign : IViewCampaignsFactory
    {
        public void ViewCampaigns(string campaignId, string openCampaignType)
        {
            var campaignFileManager = InstanceProvider.GetInstance<ICampaignsFileManager>();
            var campaignDetails = campaignFileManager.GetCampaignById(campaignId);
            var templatesFileManager = InstanceProvider.GetInstance<ITemplatesFileManager>();
            var templateDetails = templatesFileManager.GetTemplateById(campaignDetails.TemplateId);
            var sendMessageToFollower = SendMessageToFollower.GetSingeltonSendMessageToFollower();
            if (openCampaignType == ConstantVariable.CreateCampaign())
                sendMessageToFollower.CampaignName =
                    $"{SocialNetworks.Quora} {campaignDetails.SubModule} [{DateTime.Now.ToString(CultureInfo.InvariantCulture)}]";

            sendMessageToFollower.IsEditCampaignName = true;
            sendMessageToFollower.CancelEditVisibility = Visibility.Visible;
            sendMessageToFollower.TemplateId = campaignDetails.TemplateId;
            sendMessageToFollower.CampaignButtonContent = openCampaignType;
            sendMessageToFollower.CampaignName = openCampaignType == ConstantVariable.UpdateCampaign()
                ? campaignDetails.CampaignName
                : sendMessageToFollower.CampaignName;
            sendMessageToFollower.SelectedAccountCount = campaignDetails.SelectedAccountList.Count +
                                                         $" {"LangKeyAccountSelected".FromResourceDictionary()}";
            sendMessageToFollower.SendMessageToFollowFooter.list_SelectedAccounts = campaignDetails.SelectedAccountList;
            sendMessageToFollower.ObjViewModel.SendMessageToFollowerModel =
                JsonConvert.DeserializeObject<SendMessageToFollowerModel>(templateDetails.ActivitySettings);
            sendMessageToFollower.MainGrid.DataContext = sendMessageToFollower.ObjViewModel;
            TabSwitcher.ChangeTabIndex(5, 2);
        }
    }
}