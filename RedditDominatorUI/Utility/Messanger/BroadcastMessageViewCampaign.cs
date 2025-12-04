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
    public class BroadcastMessageViewCampaign : IRdViewCampaign
    {
        public void ViewCampaigns(string campaignId, string openCampaignType)
        {
            var campaignFileManager = InstanceProvider.GetInstance<ICampaignsFileManager>();
            var campaignDetails = campaignFileManager.GetCampaignById(campaignId);
            var templatesFileManager = InstanceProvider.GetInstance<ITemplatesFileManager>();
            var templateDetails = templatesFileManager.GetTemplateById(campaignDetails.TemplateId);
            var objBroadcastMessage = BroadcastMessage.GetSingeltonBroadcastMessages();
            if (openCampaignType == ConstantVariable.CreateCampaign())
                objBroadcastMessage.CampaignName =
                    $"{SocialNetworks.Reddit} {campaignDetails.SubModule} [{DateTime.Now.ToString(CultureInfo.InvariantCulture)}]";
            objBroadcastMessage.IsEditCampaignName = true;
            objBroadcastMessage.CancelEditVisibility = Visibility.Visible;
            objBroadcastMessage.TemplateId = campaignDetails.TemplateId;
            objBroadcastMessage.CampaignName = openCampaignType == ConstantVariable.UpdateCampaign()
                ? campaignDetails.CampaignName
                : objBroadcastMessage.CampaignName;
            objBroadcastMessage.CampaignButtonContent = openCampaignType;
            objBroadcastMessage.SelectedAccountCount = campaignDetails.SelectedAccountList.Count +
                                                       $" {"LangKeyAccountSelected".FromResourceDictionary()}";
            objBroadcastMessage.BrodcastMessageFooter.list_SelectedAccounts = campaignDetails.SelectedAccountList;
            objBroadcastMessage.ObjViewModel.BrodcastMessageModel =
                JsonConvert.DeserializeObject<BrodcastMessageModel>(templateDetails.ActivitySettings);
            objBroadcastMessage.MainGrid.DataContext = objBroadcastMessage.ObjViewModel;
            TabSwitcher.ChangeTabIndex(5, 0);
        }
    }
}