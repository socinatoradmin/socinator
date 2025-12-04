using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using GramDominatorCore.GDModel;
using GramDominatorCore.Interface;
using GramDominatorUI.GDViews.StoryViews;
using System;
using System.Globalization;
using System.Windows;

namespace GramDominatorUI.Utility.AddStory
{
    public class AddStoryViewCampaign: IGdViewCampaign
    {
        public void ManageCampaign(TemplateModel templateDetails, CampaignDetails campaignDetails,
            bool isEditCampaignName, Visibility cancelEditVisibility, string campaignButtonContent, string templateId)
        {
            var AddStoryView = AddInstaStory.Instance;
            if (campaignButtonContent == ConstantVariable.CreateCampaign())
                AddStoryView.CampaignName =
                    $"{SocialNetworks.Instagram} {campaignDetails.SubModule} [{DateTime.Now.ToString(CultureInfo.InvariantCulture)}]";

            AddStoryView.IsEditCampaignName = isEditCampaignName;
            AddStoryView.CancelEditVisibility = cancelEditVisibility;
            AddStoryView.TemplateId = templateId;
            AddStoryView.CampaignButtonContent = campaignButtonContent;
            AddStoryView.CampaignName = campaignButtonContent == ConstantVariable.UpdateCampaign()
                ? campaignDetails.CampaignName
                : AddStoryView.CampaignName;

            AddStoryView.SelectedAccountCount = campaignDetails.SelectedAccountList.Count +
                                                  $" {"LangKeyAccountSelected".FromResourceDictionary()}";
            AddStoryView.AddstoryFooter.list_SelectedAccounts = campaignDetails.SelectedAccountList;
            AddStoryView.ObjViewModel.AddStory =
                templateDetails.ActivitySettings.GetActivityModel<AddInstaStoryModel>(AddStoryView.ObjViewModel.Model);
            AddStoryView.MainGrid.DataContext = AddStoryView.ObjViewModel;

            TabSwitcher.ChangeTabIndex(6, 1);
        }
    }
}
