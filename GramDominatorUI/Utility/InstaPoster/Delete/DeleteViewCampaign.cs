using System;
using System.Globalization;
using System.Windows;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using GramDominatorCore.GDModel;
using GramDominatorCore.Interface;
using GramDominatorUI.GDViews.InstaPoster;

namespace GramDominatorUI.Utility.InstaPoster.Delete
{
    internal class DeleteViewCampaign : IGdViewCampaign
    {
        public void ManageCampaign(TemplateModel templateDetails, CampaignDetails campaignDetails,
            bool isEditCampaignName,
            Visibility cancelEditVisibility, string campaignButtonContent, string templateId)
        {
            var objDeletePosts = DeletePosts.GetSingeltonObjectDeletePosts();
            if (campaignButtonContent == ConstantVariable.CreateCampaign())
                objDeletePosts.CampaignName =
                    $"{SocialNetworks.Instagram} {campaignDetails.SubModule} [{DateTime.Now.ToString(CultureInfo.InvariantCulture)}]"; //added new line

            objDeletePosts.IsEditCampaignName = isEditCampaignName;
            objDeletePosts.CancelEditVisibility = cancelEditVisibility;
            objDeletePosts.TemplateId = templateId;
            objDeletePosts.CampaignButtonContent = campaignButtonContent;
            objDeletePosts.CampaignName = campaignButtonContent == ConstantVariable.UpdateCampaign()
                ? campaignDetails.CampaignName
                : objDeletePosts.CampaignName; //updated line          
            //  objDeletePosts.CampaignName = campaignDetails.CampaignName;

            objDeletePosts.SelectedAccountCount = campaignDetails.SelectedAccountList.Count +
                                                  $" {"LangKeyAccountSelected".FromResourceDictionary()}";
            objDeletePosts.DeletePostsFooter.list_SelectedAccounts = campaignDetails.SelectedAccountList;

            objDeletePosts.ObjViewModel.DeletePostModel =
                templateDetails.ActivitySettings.GetActivityModel<DeletePostModel>(objDeletePosts.ObjViewModel.Model,
                    true);

            // objDeletePosts.MainGrid.DataContext = objDeletePosts.ObjViewModel.DeletePostModel;
            objDeletePosts.MainGrid.DataContext = objDeletePosts.ObjViewModel;

            TabSwitcher.ChangeTabIndex(2, 1);
        }
    }
}