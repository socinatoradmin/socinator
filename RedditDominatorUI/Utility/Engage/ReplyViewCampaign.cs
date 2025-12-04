using CommonServiceLocator;
using DominatorHouseCore.Enums;
using DominatorHouseCore.FileManagers;
using DominatorHouseCore.Utility;
using Newtonsoft.Json;
using RedditDominatorCore.RDModel;
using RedditDominatorCore.Utility;
using RedditDominatorUI.RDViews.Engage;
using System;
using System.Globalization;
using System.Windows;

namespace RedditDominatorUI.Utility.Engage
{
    public class ReplyViewCampaign : IRdViewCampaign
    {
        public void ViewCampaigns(string campaignId, string openCampaignType)
        {
            var campaignFileManager = InstanceProvider.GetInstance<ICampaignsFileManager>();
            var campaignDetails = campaignFileManager.GetCampaignById(campaignId);
            var templatesFileManager = InstanceProvider.GetInstance<ITemplatesFileManager>();
            var templateDetails = templatesFileManager.GetTemplateById(campaignDetails.TemplateId);
            var objReply = Reply.GetSingeltonObjectReply();
            if (openCampaignType == ConstantVariable.CreateCampaign())
                objReply.CampaignName =
                    $"{SocialNetworks.Reddit} {campaignDetails.SubModule} [{DateTime.Now.ToString(CultureInfo.InvariantCulture)}]";
            objReply.IsEditCampaignName = true;
            objReply.CancelEditVisibility = Visibility.Visible;
            objReply.TemplateId = campaignDetails.TemplateId;
            objReply.CampaignName = openCampaignType == ConstantVariable.UpdateCampaign()
                ? campaignDetails.CampaignName
                : objReply.CampaignName;
            objReply.CampaignButtonContent = openCampaignType;
            objReply.SelectedAccountCount = campaignDetails.SelectedAccountList.Count +
                                            $" {"LangKeyAccountSelected".FromResourceDictionary()}";
            objReply.ReplyFooter.list_SelectedAccounts = campaignDetails.SelectedAccountList;
            objReply.ObjViewModel.ReplyModel =
                JsonConvert.DeserializeObject<ReplyModel>(templateDetails.ActivitySettings);
            objReply.MainGrid.DataContext = objReply.ObjViewModel;
            TabSwitcher.ChangeTabIndex(3, 1);
        }
    }
}