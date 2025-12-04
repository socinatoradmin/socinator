using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using FaceDominatorCore.FDModel.LikerCommentorModel;
using FaceDominatorCore.Interface;
using FaceDominatorUI.FDViews.FbLikerCommentor;
using System;
using System.Globalization;
using System.Windows;

namespace FaceDominatorUI.Utilities.LikerCommentor.FanpageLiker
{
    public class FanpageLikerViewCampaign : IFdViewCampaign
    {
        public void ManageCampaign(TemplateModel templateDetails, CampaignDetails campaignDetails,
            bool isEditCampaignName,
            Visibility cancelEditVisibility, string campaignButtonContent, string templateId)
        {
            var singeltonObjectFanapgeLiker = FanapgeLiker.GetSingeltonObjectFanapgeLiker();
            singeltonObjectFanapgeLiker.IsEditCampaignName = isEditCampaignName;
            singeltonObjectFanapgeLiker.CancelEditVisibility = cancelEditVisibility;
            singeltonObjectFanapgeLiker.TemplateId = templateId;

            if (campaignButtonContent == ConstantVariable.CreateCampaign())
                singeltonObjectFanapgeLiker.CampaignName =
                    $"{SocialNetworks.Facebook} {campaignDetails.SubModule} [{DateTime.Now.ToString(CultureInfo.InvariantCulture)}]";
            singeltonObjectFanapgeLiker.CampaignName = campaignButtonContent == ConstantVariable.UpdateCampaign()
                ? campaignDetails.CampaignName
                : singeltonObjectFanapgeLiker.CampaignName;

            //objFanapgeLiker.CampaignName = campaignDetails.CampaignName;
            singeltonObjectFanapgeLiker.CampaignButtonContent = campaignButtonContent;
            singeltonObjectFanapgeLiker.SelectedAccountCount =
                campaignDetails.SelectedAccountList.Count + $" {"LangKeyAccountSelected".FromResourceDictionary()}";
            singeltonObjectFanapgeLiker.LikeFooter.list_SelectedAccounts = campaignDetails.SelectedAccountList;

            singeltonObjectFanapgeLiker.ObjViewModel.FanpageLikerModel
                = templateDetails.ActivitySettings.GetActivityModel<FanpageLikerModel>(singeltonObjectFanapgeLiker
                    .ObjViewModel.Model);

            singeltonObjectFanapgeLiker.MainGrid.DataContext = singeltonObjectFanapgeLiker.ObjViewModel;

            TabSwitcher.ChangeTabIndex(4, 0);
        }
    }
}