using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using FaceDominatorCore.FDModel.MessageModel;
using FaceDominatorCore.Interface;
using FaceDominatorCore.Utility;
using System;
using System.Globalization;
using System.Windows;

namespace FaceDominatorUI.Utilities.Messanger.AutoReplyMessage
{
    public class AutoReplyMessageViewCampaign : IFdViewCampaign
    {
        public void ManageCampaign(TemplateModel templateDetails, CampaignDetails campaignDetails,
            bool isEditCampaignName, Visibility cancelEditVisibility, string campaignButtonContent, string templateId)
        {
            var singeltonObjectBrodcastMessage =
                FDViews.FbMessanger.AutoReplyMessage.GetSingeltonObjectAutoReplyMessage();
            singeltonObjectBrodcastMessage.IsEditCampaignName = isEditCampaignName;
            singeltonObjectBrodcastMessage.CancelEditVisibility = cancelEditVisibility;
            singeltonObjectBrodcastMessage.TemplateId = templateId;

            if (campaignButtonContent == ConstantVariable.CreateCampaign())
                singeltonObjectBrodcastMessage.CampaignName =
                    $"{SocialNetworks.Facebook} {campaignDetails.SubModule} [{DateTime.Now.ToString(CultureInfo.InvariantCulture)}]";
            singeltonObjectBrodcastMessage.CampaignName = campaignButtonContent == ConstantVariable.UpdateCampaign()
                ? campaignDetails.CampaignName
                : singeltonObjectBrodcastMessage.CampaignName;

            //singeltonObjectBrodcastMessage.CampaignName = campaignDetails.CampaignName;
            singeltonObjectBrodcastMessage.CampaignButtonContent = campaignButtonContent;
            singeltonObjectBrodcastMessage.SelectedAccountCount =
                campaignDetails.SelectedAccountList.Count + $" {"LangKeyAccountSelected".FromResourceDictionary()}";
            singeltonObjectBrodcastMessage.SendRequestFooter.list_SelectedAccounts =
                campaignDetails.SelectedAccountList;

            singeltonObjectBrodcastMessage.ObjViewModel.AutoReplyMessageModel
                = templateDetails.ActivitySettings.GetActivityModelNonQueryList<AutoReplyMessageModel>(
                    singeltonObjectBrodcastMessage.ObjViewModel.Model);

            singeltonObjectBrodcastMessage.MainGrid.DataContext = singeltonObjectBrodcastMessage.ObjViewModel;

            TabSwitcher.ChangeTabIndex(2, 1);
        }
    }
}