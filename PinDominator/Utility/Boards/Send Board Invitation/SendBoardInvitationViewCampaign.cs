using System;
using System.Globalization;
using System.Windows;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using PinDominator.PDViews.Boards;
using PinDominatorCore.Interface;
using PinDominatorCore.PDModel;

namespace PinDominator.Utility.Boards.Send_Board_Invitation
{
    public class SendBoardInvitationViewCampaign : IPdViewCampaign
    {
        public void ManageCampaign(TemplateModel templateDetails, CampaignDetails campaignDetails,
            bool isEditCampaignName,
            Visibility cancelEditVisibility, string campaignButtonContent, string templateId)
        {
            var objSendBoardInvitation = SendBoardInvitation.GetSingletonObjectSendBoardInvitation();
            if (campaignButtonContent == ConstantVariable.CreateCampaign())
                objSendBoardInvitation.CampaignName =
                    $"{SocialNetworks.Pinterest} {campaignDetails.SubModule} [{DateTime.Now.ToString(CultureInfo.InvariantCulture)}]";
            objSendBoardInvitation.IsEditCampaignName = isEditCampaignName;
            objSendBoardInvitation.CancelEditVisibility = Visibility.Visible;
            objSendBoardInvitation.TemplateId = templateId;
            objSendBoardInvitation.CampaignName = campaignButtonContent == ConstantVariable.UpdateCampaign()
                ? campaignDetails.CampaignName
                : objSendBoardInvitation.CampaignName;
            objSendBoardInvitation.CampaignButtonContent = campaignButtonContent;
            objSendBoardInvitation.SelectedAccountCount =
                campaignDetails.SelectedAccountList.Count + $" {"LangKeyAccountSelected".FromResourceDictionary()}";
            objSendBoardInvitation.SendBoardInvitationFooterControl.list_SelectedAccounts =
                campaignDetails.SelectedAccountList;

            objSendBoardInvitation.ObjViewModel.SendBoardInvitationModel =
                templateDetails.ActivitySettings.GetActivityModel<SendBoardInvitationModel>(
                    objSendBoardInvitation.ObjViewModel.Model, true);
            objSendBoardInvitation._footerControl.list_SelectedAccounts = objSendBoardInvitation.ObjViewModel
                .SendBoardInvitationModel.listOfSelectedAccounts;
            objSendBoardInvitation.SelectedAccountCount =
                $"{objSendBoardInvitation.ObjViewModel.SendBoardInvitationModel.listOfSelectedAccounts.Count} Account Selected";
            objSendBoardInvitation.MainGrid.DataContext = objSendBoardInvitation.ObjViewModel;

            TabSwitcher.ChangeTabIndex(1, 2);
        }
    }
}