using System;
using System.Globalization;
using System.Windows;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using TwtDominatorCore.Interface;
using TwtDominatorCore.TDModels;
using TwtDominatorUI.TDViews.TwtEngage;

namespace TwtDominatorUI.Utility.TwtEngageReportPack.TwtUnLikerPack
{
    public class TwtUnLikerViewCampaign : ITDViewCampaign
    {
        public void ManageCampaign(TemplateModel templateDetails, CampaignDetails campaignDetails,
            bool isEditCampaignName,
            Visibility cancelEditVisibility, string campaignButtonContent, string templateId)
        {
            var objTwtUnLiker = TwtUnLiker.GetSingletonObjectTwtUnLiker();


            if (campaignButtonContent == ConstantVariable.CreateCampaign())
                objTwtUnLiker.CampaignName =
                    $"{SocialNetworks.Twitter} {campaignDetails.SubModule} [{DateTime.Now.ToString(CultureInfo.InvariantCulture)}]";


            objTwtUnLiker.IsEditCampaignName = isEditCampaignName;
            objTwtUnLiker.CancelEditVisibility = cancelEditVisibility;
            objTwtUnLiker.TemplateId = templateId;

            objTwtUnLiker.CampaignName = campaignButtonContent == ConstantVariable.UpdateCampaign()
                ? campaignDetails.CampaignName
                : objTwtUnLiker.CampaignName;

            objTwtUnLiker.CampaignButtonContent = campaignButtonContent;
            objTwtUnLiker.SelectedAccountCount = campaignDetails.SelectedAccountList.Count +
                                                 $" {"LangKeyAccountSelected".FromResourceDictionary()}";
            objTwtUnLiker.UnLikerFooter.list_SelectedAccounts = campaignDetails.SelectedAccountList;

            // Doubt FollowerModel FollowBack
            objTwtUnLiker.ObjViewModel.UnLikeModel =
                templateDetails.ActivitySettings.GetActivityModel<UnLikeModel>(objTwtUnLiker.ObjViewModel.Model);

            objTwtUnLiker.MainGrid.DataContext = objTwtUnLiker.ObjViewModel;

            TabSwitcher.ChangeTabIndex(3, 2);
        }
    }
}