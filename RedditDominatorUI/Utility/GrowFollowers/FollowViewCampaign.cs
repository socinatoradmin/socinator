using CommonServiceLocator;
using DominatorHouseCore.Enums;
using DominatorHouseCore.FileManagers;
using DominatorHouseCore.Utility;
using Newtonsoft.Json;
using RedditDominatorCore.RDModel;
using RedditDominatorCore.Utility;
using RedditDominatorUI.RDViews.GrowFollowers;
using System;
using System.Globalization;
using System.Windows;

namespace RedditDominatorUI.Utility.GrowFollowers
{
    internal class FollowViewCampaign : IRdViewCampaign
    {
        public void ViewCampaigns(string campaignId, string openCampaignType)
        {
            var campaignFileManager = InstanceProvider.GetInstance<ICampaignsFileManager>();
            var campaignDetails = campaignFileManager.GetCampaignById(campaignId);
            var templatesFileManager = InstanceProvider.GetInstance<ITemplatesFileManager>();
            var templateDetails = templatesFileManager.GetTemplateById(campaignDetails.TemplateId);
            var objFollower = Follow.GetSingletonObjectFollow();
            if (openCampaignType == ConstantVariable.CreateCampaign())
                objFollower.CampaignName =
                    $"{SocialNetworks.Reddit} {campaignDetails.SubModule} [{DateTime.Now.ToString(CultureInfo.InvariantCulture)}]";
            objFollower.IsEditCampaignName = true;
            objFollower.CancelEditVisibility = Visibility.Visible;
            objFollower.TemplateId = campaignDetails.TemplateId;
            objFollower.CampaignName = openCampaignType == ConstantVariable.UpdateCampaign()
                ? campaignDetails.CampaignName
                : objFollower.CampaignName;
            objFollower.CampaignButtonContent = openCampaignType;
            objFollower.SelectedAccountCount = campaignDetails.SelectedAccountList.Count +
                                               $" {"LangKeyAccountSelected".FromResourceDictionary()}";
            objFollower.FollowFooter.list_SelectedAccounts = campaignDetails.SelectedAccountList;
            objFollower.ObjViewModel.FollowModel =
                JsonConvert.DeserializeObject<FollowModel>(templateDetails.ActivitySettings);
            objFollower.MainGrid.DataContext = objFollower.ObjViewModel;
            TabSwitcher.ChangeTabIndex(1, 0);
        }
    }
}