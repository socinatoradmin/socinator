using System;
using System.Globalization;
using System.Windows;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using GramDominatorCore.GDModel;
using GramDominatorCore.Interface;
using GramDominatorUI.GDViews.InstaPoster;

namespace GramDominatorUI.Utility.InstaPoster.Reposter
{
    internal class ReposterViewCampaign : IGdViewCampaign
    {
        public void ManageCampaign(TemplateModel templateDetails, CampaignDetails campaignDetails,
            bool isEditCampaignName,
            Visibility cancelEditVisibility, string campaignButtonContent, string templateId)
        {
            var objRePoster = RePoster.GetSingeltonObjectRePoster();
            if (campaignButtonContent == ConstantVariable.CreateCampaign())
                objRePoster.CampaignName =
                    $"{SocialNetworks.Instagram} {campaignDetails.SubModule} [{DateTime.Now.ToString(CultureInfo.InvariantCulture)}]"; //added new line

            objRePoster.IsEditCampaignName = isEditCampaignName;
            objRePoster.CancelEditVisibility = cancelEditVisibility;
            objRePoster.TemplateId = templateId;
            objRePoster.CampaignButtonContent = campaignButtonContent;
            objRePoster.CampaignName = campaignButtonContent == ConstantVariable.UpdateCampaign()
                ? campaignDetails.CampaignName
                : objRePoster.CampaignName; //updated line          
            //  objRePoster.CampaignName = campaignDetails.CampaignName;

            objRePoster.SelectedAccountCount = campaignDetails.SelectedAccountList.Count +
                                               $" {"LangKeyAccountSelected".FromResourceDictionary()}";
            objRePoster.RePosterFooter.list_SelectedAccounts = campaignDetails.SelectedAccountList;

            objRePoster.ObjViewModel.RePosterModel =
                templateDetails.ActivitySettings.GetActivityModel<RePosterModel>(objRePoster.ObjViewModel.Model);

            // objRePoster.MainGrid.DataContext = objRePoster.ObjViewModel.RePosterModel;
            objRePoster.MainGrid.DataContext = objRePoster.ObjViewModel;

            TabSwitcher.ChangeTabIndex(2, 0);
        }
    }
}