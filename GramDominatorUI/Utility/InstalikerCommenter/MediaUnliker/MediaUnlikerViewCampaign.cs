using System;
using System.Globalization;
using System.Windows;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using GramDominatorCore.GDModel;
using GramDominatorCore.Interface;

namespace GramDominatorUI.Utility.InstalikerCommenter.MediaUnliker
{
    internal class MediaUnlikerViewCampaign : IGdViewCampaign
    {
        public void ManageCampaign(TemplateModel templateDetails, CampaignDetails campaignDetails,
            bool isEditCampaignName,
            Visibility cancelEditVisibility, string campaignButtonContent, string templateId)
        {
            var objMediaUnliker = GDViews.InstaLikeComment.MediaUnliker.GetSingeltonObjectMediaUnliker();
            if (campaignButtonContent == ConstantVariable.CreateCampaign())
                objMediaUnliker.CampaignName =
                    $"{SocialNetworks.Instagram} {campaignDetails.SubModule} [{DateTime.Now.ToString(CultureInfo.InvariantCulture)}]"; //added new line

            objMediaUnliker.IsEditCampaignName = isEditCampaignName;
            objMediaUnliker.CancelEditVisibility = cancelEditVisibility;
            objMediaUnliker.TemplateId = templateId;
            objMediaUnliker.CampaignButtonContent = campaignButtonContent;
            objMediaUnliker.CampaignName = campaignButtonContent == ConstantVariable.UpdateCampaign()
                ? campaignDetails.CampaignName
                : objMediaUnliker.CampaignName; //updated line          
            // objMediaUnliker.CampaignName = campaignDetails.CampaignName;           
            objMediaUnliker.SelectedAccountCount = campaignDetails.SelectedAccountList.Count +
                                                   $" {"LangKeyAccountSelected".FromResourceDictionary()}";
            objMediaUnliker.MediaUnlikerFooter.list_SelectedAccounts = campaignDetails.SelectedAccountList;

            objMediaUnliker.ObjViewModel.MediaUnlikerModel =
                templateDetails.ActivitySettings.GetActivityModel<MediaUnlikerModel>(objMediaUnliker.ObjViewModel.Model,
                    true);

            // objMediaUnliker.MainGrid.DataContext = objMediaUnliker.ObjViewModel.MediaUnlikerModel;
            objMediaUnliker.MainGrid.DataContext = objMediaUnliker.ObjViewModel;

            TabSwitcher.ChangeTabIndex(4, 3);
        }
    }
}