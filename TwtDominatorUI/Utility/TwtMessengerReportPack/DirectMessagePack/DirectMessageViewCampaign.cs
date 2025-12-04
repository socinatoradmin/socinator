using System.Windows;
using DominatorHouseCore.Models;
using TwtDominatorCore.Interface;

namespace TwtDominatorUI.Utility.TwtMessengerReportPack.DirectMessagePack
{
    public class DirectMessageViewCampaign : ITDViewCampaign
    {
        public void ManageCampaign(TemplateModel templateDetails, CampaignDetails campaignDetails,
            bool isEditCampaignName,
            Visibility cancelEditVisibility, string campaignButtonContent, string templateId)
        {
            //TDViews.GrowFollowers.FollowBack objFollowBack = TDViews.GrowFollowers.FollowBack.GetSingletonObjectFollowBack();
            //objFollowBack.IsEditCampaignName = isEditCampaignName;
            //objFollowBack.CancelEditVisibility = cancelEditVisibility;
            //objFollowBack.TemplateId = templateId;
            //objFollowBack.CampaignName = campaignDetails.CampaignName;
            //objFollowBack.CampaignButtonContent = campaignButtonContent;
            //objFollowBack.SelectedAccountCount = campaignDetails.SelectedAccountList.Count + $" {"LangKeyAccountSelected".FromResourceDictionary()}";
            //objFollowBack.FollowBackFooter.list_SelectedAccounts = campaignDetails.SelectedAccountList;

            //// Doubt FollowerModel FollowBack
            //objFollowBack.ObjViewModel.FollowerModel =
            //    JsonConvert.DeserializeObject<FollowerModel>(templateDetails.ActivitySettings);

            //objFollowBack.MainGrid.DataContext = objFollowBack.ObjViewModel.FollowerModel;

            //TabSwitcher.ChangeTabIndex(1, 0);
        }
    }
}