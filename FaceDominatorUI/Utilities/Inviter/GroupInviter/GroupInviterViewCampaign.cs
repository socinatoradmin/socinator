using DominatorHouseCore;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using FaceDominatorCore.FDModel.InviterModel;
using FaceDominatorCore.Interface;
using FaceDominatorCore.Utility;
using Newtonsoft.Json;
using System;
using System.Globalization;
using System.Windows;

namespace FaceDominatorUI.Utilities.Inviter.GroupInviter
{
    public class GroupInviterViewCampaign : IFdViewCampaign
    {
        public void ManageCampaign(TemplateModel templateDetails, CampaignDetails campaignDetails,
            bool isEditCampaignName, Visibility cancelEditVisibility, string campaignButtonContent, string templateId)
        {
            var singeltonObjectGroupInviter = FDViews.FbInviter.GroupInviter.GetSingeltonObjectGroupInviter();
            singeltonObjectGroupInviter.IsEditCampaignName = isEditCampaignName;
            singeltonObjectGroupInviter.CancelEditVisibility = cancelEditVisibility;
            singeltonObjectGroupInviter.TemplateId = templateId;

            if (campaignButtonContent == ConstantVariable.CreateCampaign())
                singeltonObjectGroupInviter.CampaignName =
                    $"{SocialNetworks.Facebook} {campaignDetails.SubModule} [{DateTime.Now.ToString(CultureInfo.InvariantCulture)}]";
            singeltonObjectGroupInviter.CampaignName = campaignButtonContent == ConstantVariable.UpdateCampaign()
                ? campaignDetails.CampaignName
                : singeltonObjectGroupInviter.CampaignName;

            //objFanapgeLiker.CampaignName = campaignDetails.CampaignName;
            singeltonObjectGroupInviter.CampaignButtonContent = campaignButtonContent;
            singeltonObjectGroupInviter.SelectedAccountCount =
                campaignDetails.SelectedAccountList.Count + $" {"LangKeyAccountSelected".FromResourceDictionary()}";
            singeltonObjectGroupInviter.GroupInviterFooter.list_SelectedAccounts = campaignDetails.SelectedAccountList;

            var getModel = JsonConvert.DeserializeObject<GroupInviterModel>(templateDetails.ActivitySettings);

            if ("LangKeySocinator".FromResourceDictionary() == "Tunto Socianator")
                try
                {
                    var lastModel = singeltonObjectGroupInviter.ObjViewModel.Model;

                    getModel.JobConfiguration.CopyJobConfigWith(lastModel.JobConfiguration);

                    var listOldQuery = getModel.ListKeywordsNonQuery;
                    getModel.ListKeywordsNonQuery = lastModel.ListKeywordsNonQuery;

                    getModel.SavedQueries.ModifySavedQueries(getModel.ListKeywordsNonQuery, listOldQuery);
                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                }

            singeltonObjectGroupInviter.ObjViewModel.GroupInviterModel
                = templateDetails.ActivitySettings.GetActivityModelNonQueryList<GroupInviterModel>(
                    singeltonObjectGroupInviter.ObjViewModel.Model);

            singeltonObjectGroupInviter.MainGrid.DataContext = singeltonObjectGroupInviter.ObjViewModel;

            TabSwitcher.ChangeTabIndex(5, 0);
        }
    }
}