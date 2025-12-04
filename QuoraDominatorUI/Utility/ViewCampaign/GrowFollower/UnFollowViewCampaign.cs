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
    public class UnFollowViewCampaign : IViewCampaignsFactory
    {
        public void ViewCampaigns(string campaignId, string openCampaignType)
        {
            var campaignFileManager = InstanceProvider.GetInstance<ICampaignsFileManager>();
            var campaignDetails = campaignFileManager.GetCampaignById(campaignId);
            var templatesFileManager = InstanceProvider.GetInstance<ITemplatesFileManager>();
            var templateDetails = templatesFileManager.GetTemplateById(campaignDetails.TemplateId);
            var objUnfollow = Unfollow.GetSingeltonObjectUnfollower();
            if (openCampaignType == ConstantVariable.CreateCampaign())
                objUnfollow.CampaignName =
                    $"{SocialNetworks.Quora} {campaignDetails.SubModule} [{DateTime.Now.ToString(CultureInfo.InvariantCulture)}]";

            objUnfollow.IsEditCampaignName = true;
            objUnfollow.CancelEditVisibility = Visibility.Visible;
            objUnfollow.TemplateId = campaignDetails.TemplateId;
            objUnfollow.CampaignButtonContent = openCampaignType;
            objUnfollow.CampaignName = openCampaignType == ConstantVariable.UpdateCampaign()
                ? campaignDetails.CampaignName
                : objUnfollow.CampaignName;

            objUnfollow.SelectedAccountCount = campaignDetails.SelectedAccountList.Count +
                                               $" {"LangKeyAccountSelected".FromResourceDictionary()}";
            objUnfollow.UnFollowFooter.list_SelectedAccounts = campaignDetails.SelectedAccountList;
            objUnfollow.ObjViewModel.UnfollowerModel =
                JsonConvert.DeserializeObject<UnfollowerModel>(templateDetails.ActivitySettings);
            objUnfollow.MainGrid.DataContext = objUnfollow.ObjViewModel;
            TabSwitcher.ChangeTabIndex(1, 1);
        }
    }
}