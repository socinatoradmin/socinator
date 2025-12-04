using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using FaceDominatorCore.FDModel.MessageModel;
using FaceDominatorCore.Interface;
using FaceDominatorUI.FDViews.FbMessanger;
using System;
using System.Globalization;
using System.Windows;

namespace FaceDominatorUI.Utilities.Messanger.SendGreetingsToFriends
{
    public class MessageToFanpagesViewCampaign : IFdViewCampaign
    {
        public void ManageCampaign(TemplateModel templateDetails, CampaignDetails campaignDetails,
            bool isEditCampaignName, Visibility cancelEditVisibility, string campaignButtonContent, string templateId)
        {
            var singeltonObjectMessageToFanpages = MessageToFanpages.GetSingeltonObjectMessageToFanpages();
            singeltonObjectMessageToFanpages.IsEditCampaignName = isEditCampaignName;
            singeltonObjectMessageToFanpages.CancelEditVisibility = cancelEditVisibility;
            singeltonObjectMessageToFanpages.TemplateId = templateId;

            if (campaignButtonContent == ConstantVariable.CreateCampaign())
                singeltonObjectMessageToFanpages.CampaignName =
                    $"{SocialNetworks.Facebook} {campaignDetails.SubModule} [{DateTime.Now.ToString(CultureInfo.InvariantCulture)}]";
            singeltonObjectMessageToFanpages.CampaignName = campaignButtonContent == ConstantVariable.UpdateCampaign()
                ? campaignDetails.CampaignName
                : singeltonObjectMessageToFanpages.CampaignName;

            //singeltonObjectMessageToFanpages.CampaignName = campaignDetails.CampaignName;
            singeltonObjectMessageToFanpages.CampaignButtonContent = campaignButtonContent;
            singeltonObjectMessageToFanpages.SelectedAccountCount =
                campaignDetails.SelectedAccountList.Count + $" {"LangKeyAccountSelected".FromResourceDictionary()}";
            singeltonObjectMessageToFanpages.MessageToFanpageFooter.list_SelectedAccounts =
                campaignDetails.SelectedAccountList;

            singeltonObjectMessageToFanpages.ObjViewModel.MessageToFanpagesModel
                = templateDetails.ActivitySettings.GetActivityModel<MessageToFanpagesModel>(
                    singeltonObjectMessageToFanpages.ObjViewModel.Model);

            singeltonObjectMessageToFanpages.MainGrid.DataContext = singeltonObjectMessageToFanpages.ObjViewModel;

            TabSwitcher.ChangeTabIndex(2, 4);
        }
    }
}