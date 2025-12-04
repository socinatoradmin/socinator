using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using FaceDominatorCore.FDModel.LikerCommentorModel;
using FaceDominatorCore.Interface;
using FaceDominatorCore.Utility;
using System;
using System.Globalization;
using System.Windows;

namespace FaceDominatorUI.Utilities.LikerCommentor.PostLiker
{
    public class PostlikerViewCampaign : IFdViewCampaign
    {
        public void ManageCampaign(TemplateModel templateDetails, CampaignDetails campaignDetails,
            bool isEditCampaignName,
            Visibility cancelEditVisibility, string campaignButtonContent, string templateId)
        {
            var singeltonObjectPostLikerCommentor = FDViews.FbLikerCommentor.PostLiker.GetSingeltonObjectPostLiker();
            singeltonObjectPostLikerCommentor.IsEditCampaignName = isEditCampaignName;
            singeltonObjectPostLikerCommentor.CancelEditVisibility = cancelEditVisibility;
            singeltonObjectPostLikerCommentor.TemplateId = templateId;

            if (campaignButtonContent == ConstantVariable.CreateCampaign())
                singeltonObjectPostLikerCommentor.CampaignName =
                    $"{SocialNetworks.Facebook} {campaignDetails.SubModule} [{DateTime.Now.ToString(CultureInfo.InvariantCulture)}]";
            singeltonObjectPostLikerCommentor.CampaignName = campaignButtonContent == ConstantVariable.UpdateCampaign()
                ? campaignDetails.CampaignName
                : singeltonObjectPostLikerCommentor.CampaignName;

            //objPostLikerCommentor.CampaignName = campaignDetails.CampaignName;
            singeltonObjectPostLikerCommentor.CampaignButtonContent = campaignButtonContent;
            singeltonObjectPostLikerCommentor.SelectedAccountCount =
                campaignDetails.SelectedAccountList.Count + $" {"LangKeyAccountSelected".FromResourceDictionary()}";
            singeltonObjectPostLikerCommentor.LikeCommentFooter.list_SelectedAccounts =
                campaignDetails.SelectedAccountList;

            singeltonObjectPostLikerCommentor.ObjViewModel.PostLikerModel
                = templateDetails.ActivitySettings.GetActivityModelNonQueryList<PostLikerModel>(
                    singeltonObjectPostLikerCommentor.ObjViewModel.Model);

            if (!singeltonObjectPostLikerCommentor.ObjViewModel.PostLikerModel.IsActionasPageChecked &&
                !singeltonObjectPostLikerCommentor.ObjViewModel.PostLikerModel.IsActionasOwnAccountChecked)
                singeltonObjectPostLikerCommentor.ObjViewModel.PostLikerModel.IsActionasOwnAccountChecked = true;

            singeltonObjectPostLikerCommentor.MainGrid.DataContext = singeltonObjectPostLikerCommentor.ObjViewModel;

            TabSwitcher.ChangeTabIndex(4, 1);
        }
    }
}