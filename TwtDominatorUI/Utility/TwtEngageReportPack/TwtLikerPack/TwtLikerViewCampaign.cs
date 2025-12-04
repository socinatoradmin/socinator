using System;
using System.Globalization;
using System.Windows;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using TwtDominatorCore.Interface;
using TwtDominatorCore.TDModels;
using TwtDominatorUI.TDViews.TwtEngage;

namespace TwtDominatorUI.Utility.TwtEngageReportPack.TwtLikerPack
{
    public class TwtLikerViewCampaign : ITDViewCampaign
    {
        public void ManageCampaign(TemplateModel templateDetails, CampaignDetails campaignDetails,
            bool isEditCampaignName,
            Visibility cancelEditVisibility, string campaignButtonContent, string templateId)
        {
            var objTwtLiker = TwtLiker.GetSingletonObjectTwtLiker();

            if (campaignButtonContent == ConstantVariable.CreateCampaign())
                objTwtLiker.CampaignName =
                    $"{SocialNetworks.Twitter} {campaignDetails.SubModule} [{DateTime.Now.ToString(CultureInfo.InvariantCulture)}]";


            objTwtLiker.IsEditCampaignName = isEditCampaignName;
            objTwtLiker.CancelEditVisibility = cancelEditVisibility;
            objTwtLiker.TemplateId = templateId;

            objTwtLiker.CampaignName = campaignButtonContent == ConstantVariable.UpdateCampaign()
                ? campaignDetails.CampaignName
                : objTwtLiker.CampaignName;

            objTwtLiker.CampaignButtonContent = campaignButtonContent;
            objTwtLiker.SelectedAccountCount = campaignDetails.SelectedAccountList.Count +
                                               $" {"LangKeyAccountSelected".FromResourceDictionary()}";
            objTwtLiker.LikerFooter.list_SelectedAccounts = campaignDetails.SelectedAccountList;

            // Doubt FollowerModel FollowBack
            objTwtLiker.ObjViewModel.LikeModel =
                templateDetails.ActivitySettings.GetActivityModel<LikeModel>(objTwtLiker.ObjViewModel.Model);

            objTwtLiker.MainGrid.DataContext = objTwtLiker.ObjViewModel;

            TabSwitcher.ChangeTabIndex(3, 0);
        }
    }
}