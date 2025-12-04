using System;
using System.Globalization;
using System.Windows;
using CommonServiceLocator;
using DominatorHouseCore.Enums;
using DominatorHouseCore.FileManagers;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Utility;
using Newtonsoft.Json;
using QuoraDominatorCore.Models;
using QuoraDominatorUI.QDViews.GrowFollowers;

namespace QuoraDominatorUI.Utility.ViewCampaign.GrowFollower
{
    public class FollowViewCampaign : IViewCampaignsFactory
    {
        public void ViewCampaigns(string campaignId, string openCampaignType)
        {
            var campaignFileManager = InstanceProvider.GetInstance<ICampaignsFileManager>();
            var campaignDetails = campaignFileManager.GetCampaignById(campaignId);
            var templatesFileManager = InstanceProvider.GetInstance<ITemplatesFileManager>();
            var templateDetails = templatesFileManager.GetTemplateById(campaignDetails.TemplateId);
            var objFollower = Follower.GetSingeltonObjectFollower();
            if (openCampaignType == ConstantVariable.CreateCampaign())
                objFollower.CampaignName =
                    $"{SocialNetworks.Quora} {campaignDetails.SubModule} [{DateTime.Now.ToString(CultureInfo.InvariantCulture)}]";

            objFollower.IsEditCampaignName = true;
            objFollower.CancelEditVisibility = Visibility.Visible;
            objFollower.TemplateId = campaignDetails.TemplateId;
            objFollower.CampaignButtonContent = openCampaignType;
            objFollower.CampaignName = openCampaignType == ConstantVariable.UpdateCampaign()
                ? campaignDetails.CampaignName
                : objFollower.CampaignName;
            objFollower.SelectedAccountCount = campaignDetails.SelectedAccountList.Count +
                                               $" {"LangKeyAccountSelected".FromResourceDictionary()}";
            objFollower.FollowFooter.list_SelectedAccounts = campaignDetails.SelectedAccountList;
            objFollower.ObjViewModel.FollowerModel =
                JsonConvert.DeserializeObject<FollowerModel>(templateDetails.ActivitySettings);
            objFollower.MainGrid.DataContext = objFollower.ObjViewModel;
            TabSwitcher.ChangeTabIndex(1, 0);
        }
    }
}