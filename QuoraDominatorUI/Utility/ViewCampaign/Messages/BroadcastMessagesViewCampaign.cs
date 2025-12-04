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
    public class BroadcastMessagesViewCampaign : IViewCampaignsFactory
    {
        public void ViewCampaigns(string campaignId, string openCampaignType)
        {
            var campaignFileManager = InstanceProvider.GetInstance<ICampaignsFileManager>();
            var campaignDetails = campaignFileManager.GetCampaignById(campaignId);
            var templatesFileManager = InstanceProvider.GetInstance<ITemplatesFileManager>();
            var templateDetails = templatesFileManager.GetTemplateById(campaignDetails.TemplateId);
            var broadcastMessages = BroadcastMessages.GetSingeltonBroadcastMessages();
            if (openCampaignType == ConstantVariable.CreateCampaign())
                broadcastMessages.CampaignName =
                    $"{SocialNetworks.Quora} {campaignDetails.SubModule} [{DateTime.Now.ToString(CultureInfo.InvariantCulture)}]";

            broadcastMessages.IsEditCampaignName = true;
            broadcastMessages.CancelEditVisibility = Visibility.Visible;
            broadcastMessages.TemplateId = campaignDetails.TemplateId;
            broadcastMessages.CampaignButtonContent = openCampaignType;
            broadcastMessages.CampaignName = openCampaignType == ConstantVariable.UpdateCampaign()
                ? campaignDetails.CampaignName
                : broadcastMessages.CampaignName;
            broadcastMessages.SelectedAccountCount = campaignDetails.SelectedAccountList.Count +
                                                     $" {"LangKeyAccountSelected".FromResourceDictionary()}";
            broadcastMessages.BrodCastMessageFooter.list_SelectedAccounts = campaignDetails.SelectedAccountList;
            broadcastMessages.ObjViewModel.BroadcastMessagesModel =
                JsonConvert.DeserializeObject<BroadcastMessagesModel>(templateDetails.ActivitySettings);
            broadcastMessages.MainGrid.DataContext = broadcastMessages.ObjViewModel;
            TabSwitcher.ChangeTabIndex(5, 0);
        }
    }
}