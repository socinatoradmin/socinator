using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using FaceDominatorCore.FDModel.MessageModel;
using FaceDominatorCore.Interface;
using FaceDominatorUI.FDViews.FbMessanger;
using System;
using System.Globalization;
using System.Windows;

namespace FaceDominatorUI.Utilities.Messanger
{
    public class BrodcastMessageViewCampaign : IFdViewCampaign
    {
        public void ManageCampaign(TemplateModel templateDetails, CampaignDetails campaignDetails,
            bool isEditCampaignName,
            Visibility cancelEditVisibility, string campaignButtonContent, string templateId)
        {
            var singeltonObjectBrodcastMessage = BrodcastMessage.GetSingeltonObjectBrodcastMessage();
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
            singeltonObjectBrodcastMessage.BrodCastMessageFooter.list_SelectedAccounts =
                campaignDetails.SelectedAccountList;

            singeltonObjectBrodcastMessage.ObjViewModel.BrodcastMessageModel
                = templateDetails.ActivitySettings.GetActivityModel<BrodcastMessageModel>(singeltonObjectBrodcastMessage
                    .ObjViewModel.Model);

            singeltonObjectBrodcastMessage.MainGrid.DataContext = singeltonObjectBrodcastMessage.ObjViewModel;

            TabSwitcher.ChangeTabIndex(2, 0);
        }
    }
}