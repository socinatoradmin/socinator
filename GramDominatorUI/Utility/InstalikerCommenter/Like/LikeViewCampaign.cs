using System;
using System.Globalization;
using System.Windows;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using GramDominatorCore.GDModel;
using GramDominatorCore.Interface;

namespace GramDominatorUI.Utility.InstalikerCommenter.Like
{
    internal class LikeViewCampaign : IGdViewCampaign
    {
        public void ManageCampaign(TemplateModel templateDetails, CampaignDetails campaignDetails,
            bool isEditCampaignName,
            Visibility cancelEditVisibility, string campaignButtonContent, string templateId)
        {
            var objLike = GDViews.InstaLikeComment.Like.GetSingeltonObjectLike();
            if (campaignButtonContent == ConstantVariable.CreateCampaign())
                objLike.CampaignName =
                    $"{SocialNetworks.Instagram} {campaignDetails.SubModule} [{DateTime.Now.ToString(CultureInfo.InvariantCulture)}]"; //added new line

            objLike.IsEditCampaignName = isEditCampaignName;
            objLike.CancelEditVisibility = cancelEditVisibility;
            objLike.TemplateId = templateId;
            objLike.CampaignButtonContent = campaignButtonContent;
            objLike.CampaignName = campaignButtonContent == ConstantVariable.UpdateCampaign()
                ? campaignDetails.CampaignName
                : objLike.CampaignName; //updated line          
            //objLike.CampaignName = campaignDetails.CampaignName;
            objLike.SelectedAccountCount = campaignDetails.SelectedAccountList.Count +
                                           $" {"LangKeyAccountSelected".FromResourceDictionary()}";
            objLike.LikeFooter.list_SelectedAccounts = campaignDetails.SelectedAccountList;

            objLike.ObjViewModel.LikeModel =
                templateDetails.ActivitySettings.GetActivityModel<LikeModel>(objLike.ObjViewModel.Model);

            // objLike.MainGrid.DataContext = objLike.ObjViewModel.LikeModel;
            objLike.MainGrid.DataContext = objLike.ObjViewModel;

            TabSwitcher.ChangeTabIndex(4, 0);
        }
    }
}