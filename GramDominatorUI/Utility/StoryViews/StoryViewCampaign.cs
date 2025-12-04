using System;
using System.Globalization;
using System.Windows;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using GramDominatorCore.GDModel;
using GramDominatorCore.Interface;
using GramDominatorUI.GDViews.StoryViews;

namespace GramDominatorUI.Utility.StoryViews
{
    public class StoryViewCampaign : IGdViewCampaign
    {
        public void ManageCampaign(TemplateModel templateDetails, CampaignDetails campaignDetails,
            bool isEditCampaignName, Visibility cancelEditVisibility, string campaignButtonContent, string templateId)
        {
            var objStoryViewer = StoryViewers.GetSingletonViewers();
            if (campaignButtonContent == ConstantVariable.CreateCampaign())
                objStoryViewer.CampaignName =
                    $"{SocialNetworks.Instagram} {campaignDetails.SubModule} [{DateTime.Now.ToString(CultureInfo.InvariantCulture)}]";

            objStoryViewer.IsEditCampaignName = isEditCampaignName;
            objStoryViewer.CancelEditVisibility = cancelEditVisibility;
            objStoryViewer.TemplateId = templateId;
            objStoryViewer.CampaignButtonContent = campaignButtonContent;
            objStoryViewer.CampaignName = campaignButtonContent == ConstantVariable.UpdateCampaign()
                ? campaignDetails.CampaignName
                : objStoryViewer.CampaignName;

            objStoryViewer.SelectedAccountCount = campaignDetails.SelectedAccountList.Count +
                                                  $" {"LangKeyAccountSelected".FromResourceDictionary()}";
            objStoryViewer.storyFooter.list_SelectedAccounts = campaignDetails.SelectedAccountList;

            objStoryViewer.ObjViewModel.StoryModel =
                templateDetails.ActivitySettings.GetActivityModel<StoryModel>(objStoryViewer.ObjViewModel.Model);

            objStoryViewer.MainGrid.DataContext = objStoryViewer.ObjViewModel;

            TabSwitcher.ChangeTabIndex(6, 0);
        }
    }
}