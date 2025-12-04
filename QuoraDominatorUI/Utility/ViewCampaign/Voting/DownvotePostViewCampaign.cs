using CommonServiceLocator;
using DominatorHouseCore.Enums;
using DominatorHouseCore.FileManagers;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Utility;
using Newtonsoft.Json;
using QuoraDominatorCore.Models;
using QuoraDominatorUI.QDViews.Voting;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace QuoraDominatorUI.Utility.ViewCampaign.Voting
{
    public class DownvotePostViewCampaign : IViewCampaignsFactory
    {
        public void ViewCampaigns(string campaignId, string openCampaignType)
        {
            var campaignFileManager = InstanceProvider.GetInstance<ICampaignsFileManager>();
            var campaignDetails = campaignFileManager.GetCampaignById(campaignId);
            var templatesFileManager = InstanceProvider.GetInstance<ITemplatesFileManager>();
            var templateDetails = templatesFileManager.GetTemplateById(campaignDetails.TemplateId);
            var downvotePosts = DownvotePost.GetSingeltonInstance();
            if (openCampaignType == ConstantVariable.CreateCampaign())
                downvotePosts.CampaignName =
                    $"{SocialNetworks.Quora} {campaignDetails.SubModule} [{DateTime.Now.ToString(CultureInfo.InvariantCulture)}]";

            downvotePosts.IsEditCampaignName = true;
            downvotePosts.CancelEditVisibility = Visibility.Visible;
            downvotePosts.TemplateId = campaignDetails.TemplateId;
            downvotePosts.CampaignButtonContent = openCampaignType;
            downvotePosts.CampaignName = openCampaignType == ConstantVariable.UpdateCampaign()
                ? campaignDetails.CampaignName
                : downvotePosts.CampaignName;
            downvotePosts.SelectedAccountCount = campaignDetails.SelectedAccountList.Count +
                                                 $" {"LangKeyAccountSelected".FromResourceDictionary()}";
            downvotePosts.DownvotePostsFooter.list_SelectedAccounts = campaignDetails.SelectedAccountList;
            downvotePosts.ObjViewModel.DownvotePostModel =
                JsonConvert.DeserializeObject<DownvotePostModel>(templateDetails.ActivitySettings);
            downvotePosts.MainGrid.DataContext = downvotePosts.ObjViewModel;
            TabSwitcher.ChangeTabIndex(2, 4);
        }
    }
}
