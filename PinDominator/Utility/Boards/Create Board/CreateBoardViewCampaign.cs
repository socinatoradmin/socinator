using System;
using System.Globalization;
using System.Windows;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using PinDominatorCore.Interface;
using PinDominatorCore.PDModel;
using CreateBoards = PinDominator.PDViews.Boards.CreateBoard;

namespace PinDominator.Utility.Boards.Create_Board
{
    public class CreateBoardViewCampaign : IPdViewCampaign
    {
        public void ManageCampaign(TemplateModel templateDetails, CampaignDetails campaignDetails,
            bool isEditCampaignName,
            Visibility cancelEditVisibility, string campaignButtonContent, string templateId)
        {
            var objCreateBoard = CreateBoards.GetSingletonObjectCreateBoard();
            if (campaignButtonContent == ConstantVariable.CreateCampaign())
                objCreateBoard.CampaignName =
                    $"{SocialNetworks.Pinterest} {campaignDetails.SubModule} [{DateTime.Now.ToString(CultureInfo.InvariantCulture)}]";
            objCreateBoard.IsEditCampaignName = isEditCampaignName;
            objCreateBoard.CancelEditVisibility = Visibility.Visible;
            objCreateBoard.TemplateId = templateId;
            objCreateBoard.CampaignName = campaignButtonContent == ConstantVariable.UpdateCampaign()
                ? campaignDetails.CampaignName
                : objCreateBoard.CampaignName;
            objCreateBoard.CampaignButtonContent = campaignButtonContent;
            objCreateBoard.SelectedAccountCount = campaignDetails.SelectedAccountList.Count +
                                                  $" {"LangKeyAccountSelected".FromResourceDictionary()}";
            objCreateBoard.CreateBoardFooter.list_SelectedAccounts = campaignDetails.SelectedAccountList;

            objCreateBoard.ObjViewModel.BoardModel =
                templateDetails.ActivitySettings.GetActivityModel<BoardModel>(objCreateBoard.ObjViewModel.Model, true);

            objCreateBoard.MainGrid.DataContext = objCreateBoard.ObjViewModel;

            TabSwitcher.ChangeTabIndex(1, 0);
        }
    }
}