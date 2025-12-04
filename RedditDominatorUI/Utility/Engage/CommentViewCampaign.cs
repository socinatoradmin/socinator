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
    public class CommentViewCampaign : IRdViewCampaign
    {
        public void ViewCampaigns(string campaignId, string openCampaignType)
        {
            var campaignFileManager = InstanceProvider.GetInstance<ICampaignsFileManager>();
            var campaignDetails = campaignFileManager.GetCampaignById(campaignId);
            var templatesFileManager = InstanceProvider.GetInstance<ITemplatesFileManager>();
            var templateDetails = templatesFileManager.GetTemplateById(campaignDetails.TemplateId);
            var objComment = Comment.GetSingeltonObjectComment();
            if (openCampaignType == ConstantVariable.CreateCampaign())
                objComment.CampaignName =
                    $"{SocialNetworks.Reddit} {campaignDetails.SubModule} [{DateTime.Now.ToString(CultureInfo.InvariantCulture)}]";
            objComment.IsEditCampaignName = true;
            objComment.CancelEditVisibility = Visibility.Visible;
            objComment.TemplateId = campaignDetails.TemplateId;
            objComment.CampaignName = openCampaignType == ConstantVariable.UpdateCampaign()
                ? campaignDetails.CampaignName
                : objComment.CampaignName;
            objComment.CampaignButtonContent = openCampaignType;
            objComment.SelectedAccountCount = campaignDetails.SelectedAccountList.Count +
                                              $" {"LangKeyAccountSelected".FromResourceDictionary()}";
            objComment.CommentFooter.list_SelectedAccounts = campaignDetails.SelectedAccountList;
            objComment.ObjViewModel.CommentModel =
                JsonConvert.DeserializeObject<CommentModel>(templateDetails.ActivitySettings);
            objComment.MainGrid.DataContext = objComment.ObjViewModel;
            TabSwitcher.ChangeTabIndex(3, 0);
        }
    }
}