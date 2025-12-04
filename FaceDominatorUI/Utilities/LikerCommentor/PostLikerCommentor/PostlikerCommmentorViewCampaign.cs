using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using FaceDominatorCore.FDModel.LikerCommentorModel;
using FaceDominatorCore.Interface;
using FaceDominatorCore.Utility;
using System;
using System.Globalization;
using System.Windows;

namespace FaceDominatorUI.Utilities.LikerCommentor.PostLikerCommentor
{
    public class PostlikerCommmentorViewCampaign : IFdViewCampaign
    {
        public void ManageCampaign(TemplateModel templateDetails, CampaignDetails campaignDetails,
            bool isEditCampaignName,
            Visibility cancelEditVisibility, string campaignButtonContent, string templateId)
        {
            var singeltonObjectPostLikerCommentor =
                FDViews.FbLikerCommentor.PostLikerCommentor.GetSingeltonObjectPostLikerCommentor();
            singeltonObjectPostLikerCommentor.IsEditCampaignName = isEditCampaignName;
            singeltonObjectPostLikerCommentor.CancelEditVisibility = cancelEditVisibility;
            singeltonObjectPostLikerCommentor.TemplateId = templateId;

            if (campaignButtonContent == ConstantVariable.CreateCampaign())
                singeltonObjectPostLikerCommentor.CampaignName =
                    $"{SocialNetworks.Facebook} {campaignDetails.SubModule} [{DateTime.Now.ToString(CultureInfo.InvariantCulture)}]";
            singeltonObjectPostLikerCommentor.CampaignName = campaignButtonContent == ConstantVariable.UpdateCampaign()
                ? campaignDetails.CampaignName
                : singeltonObjectPostLikerCommentor.CampaignName;

            //singeltonObjectPostLikerCommentor.CampaignName = campaignDetails.CampaignName;
            singeltonObjectPostLikerCommentor.CampaignButtonContent = campaignButtonContent;
            singeltonObjectPostLikerCommentor.SelectedAccountCount =
                campaignDetails.SelectedAccountList.Count + $" {"LangKeyAccountSelected".FromResourceDictionary()}";
            singeltonObjectPostLikerCommentor.LikeCommentFooter.list_SelectedAccounts =
                campaignDetails.SelectedAccountList;

            singeltonObjectPostLikerCommentor.ObjViewModel.PostLikerCommentorModel
                = templateDetails.ActivitySettings.GetActivityModelNonQueryList<PostLikerCommentorModel>(
                    singeltonObjectPostLikerCommentor.ObjViewModel.Model);

            singeltonObjectPostLikerCommentor.MainGrid.DataContext = singeltonObjectPostLikerCommentor.ObjViewModel;

            TabSwitcher.ChangeTabIndex(4, 1);
        }
    }
}