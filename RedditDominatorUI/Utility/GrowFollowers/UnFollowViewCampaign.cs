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
    internal class UnFollowViewCampaign : IRdViewCampaign
    {
        public void ViewCampaigns(string campaignId, string openCampaignType)
        {
            var campaignFileManager = InstanceProvider.GetInstance<ICampaignsFileManager>();
            var campaignDetails = campaignFileManager.GetCampaignById(campaignId);
            var templatesFileManager = InstanceProvider.GetInstance<ITemplatesFileManager>();
            var templateDetails = templatesFileManager.GetTemplateById(campaignDetails.TemplateId);
            var objUnFollow = UnFollower.GetSingeltonObjectUnfollower();
            if (openCampaignType == ConstantVariable.CreateCampaign())
                objUnFollow.CampaignName =
                    $"{SocialNetworks.Reddit} {campaignDetails.SubModule} [{DateTime.Now.ToString(CultureInfo.InvariantCulture)}]";
            objUnFollow.IsEditCampaignName = true;
            objUnFollow.CancelEditVisibility = Visibility.Visible;
            objUnFollow.TemplateId = campaignDetails.TemplateId;
            objUnFollow.CampaignName = openCampaignType == ConstantVariable.UpdateCampaign()
                ? campaignDetails.CampaignName
                : objUnFollow.CampaignName;
            objUnFollow.CampaignButtonContent = openCampaignType;
            objUnFollow.SelectedAccountCount = campaignDetails.SelectedAccountList.Count +
                                               $" {"LangKeyAccountSelected".FromResourceDictionary()}";
            objUnFollow.UnFollowFooter.list_SelectedAccounts = campaignDetails.SelectedAccountList;
            objUnFollow.ObjViewModel.UnfollowerModel =
                JsonConvert.DeserializeObject<UnfollowerModel>(templateDetails.ActivitySettings);
            objUnFollow.MainGrid.DataContext = objUnFollow.ObjViewModel;
            TabSwitcher.ChangeTabIndex(1, 1);
        }
    }
}