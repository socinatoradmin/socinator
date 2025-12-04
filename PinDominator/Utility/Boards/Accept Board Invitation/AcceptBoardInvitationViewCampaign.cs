using System;
using System.Globalization;
using System.Windows;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using PinDominator.PDViews.Boards;
using PinDominatorCore.Interface;
using PinDominatorCore.PDModel;

namespace PinDominator.Utility.Boards.Accept_Board_Invitation
{
    public class AcceptBoardInvitationViewCampaign : IPdViewCampaign
    {
        public void ManageCampaign(TemplateModel templateDetails, CampaignDetails campaignDetails,
            bool isEditCampaignName,
            Visibility cancelEditVisibility, string campaignButtonContent, string templateId)
        {
            var objAcceptBoardInvitation = AcceptBoardInvitation.GetSingletonObjectAcceptBoardInvitation();
            if (campaignButtonContent == ConstantVariable.CreateCampaign())
                objAcceptBoardInvitation.CampaignName =
                    $"{SocialNetworks.Pinterest} {campaignDetails.SubModule} [{DateTime.Now.ToString(CultureInfo.InvariantCulture)}]";
            objAcceptBoardInvitation.IsEditCampaignName = isEditCampaignName;
            objAcceptBoardInvitation.CancelEditVisibility = Visibility.Visible;
            objAcceptBoardInvitation.TemplateId = templateId;
            objAcceptBoardInvitation.CampaignName = campaignButtonContent == ConstantVariable.UpdateCampaign()
                ? campaignDetails.CampaignName
                : objAcceptBoardInvitation.CampaignName;
            objAcceptBoardInvitation.CampaignButtonContent = campaignButtonContent;
            objAcceptBoardInvitation.SelectedAccountCount =
                campaignDetails.SelectedAccountList.Count + $" {"LangKeyAccountSelected".FromResourceDictionary()}";
            objAcceptBoardInvitation.AcceptBoardInvitationFooter.list_SelectedAccounts =
                campaignDetails.SelectedAccountList;

            objAcceptBoardInvitation.ObjViewModel.AcceptBoardInvitationModel
                = templateDetails.ActivitySettings.GetActivityModel<AcceptBoardInvitationModel>(
                    objAcceptBoardInvitation.ObjViewModel.Model, true);

            objAcceptBoardInvitation.MainGrid.DataContext = objAcceptBoardInvitation.ObjViewModel;

            TabSwitcher.ChangeTabIndex(1, 1);
        }
    }
}