using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using Newtonsoft.Json;
using System;
using System.Globalization;
using System.Windows;
using TumblrDominatorCore.Interface;
using TumblrDominatorCore.Models;
using TumblrDominatorUI.TumblrView.Message;

namespace TumblrDominatorUI.Utility.Message
{
    public class MessageViewCampaign : ITumblrViewCampaign
    {
        public void ManageCampaign(TemplateModel templateDetails, CampaignDetails campaignDetails,
            bool isEditCampaignName,
            Visibility cancelEditVisibility, string campaignButtonContent, string templateId)
        {
            var objMessages = BroadcastMessages.GetSingeltonBroadcastMessages();

            if (campaignButtonContent == ConstantVariable.CreateCampaign())
                objMessages.CampaignName =
                    $"{SocialNetworks.Tumblr} {campaignDetails.SubModule} [{DateTime.Now.ToString(CultureInfo.InvariantCulture)}]";

            objMessages.IsEditCampaignName = isEditCampaignName;
            objMessages.CancelEditVisibility = cancelEditVisibility;
            objMessages.TemplateId = templateId;
            objMessages.CampaignName = campaignDetails.CampaignName;
            objMessages.CampaignButtonContent = campaignButtonContent;
            objMessages.SelectedAccountCount = campaignDetails.SelectedAccountList.Count +
                                               $" {"LangKeyAccountSelected".FromResourceDictionary()}";
            objMessages.BrodCastMessageFooter.list_SelectedAccounts = campaignDetails.SelectedAccountList;
            objMessages.ObjViewModel.BroadcastMessagesModel =
                JsonConvert.DeserializeObject<BroadcastMessagesModel>(templateDetails.ActivitySettings);

            objMessages.MainGrid.DataContext = objMessages.ObjViewModel;

            TabSwitcher.ChangeTabIndex(4, 0);
        }
    }
}