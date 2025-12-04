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
    public class UpvotePostsViewCampaign : IViewCampaignsFactory
    {
        public void ViewCampaigns(string campaignId, string openCampaignType)
        {
            var campaignFileManager = InstanceProvider.GetInstance<ICampaignsFileManager>();
            var campaignDetails = campaignFileManager.GetCampaignById(campaignId);
            var templatesFileManager = InstanceProvider.GetInstance<ITemplatesFileManager>();
            var templateDetails = templatesFileManager.GetTemplateById(campaignDetails.TemplateId);
            var upvotePosts = UpvotePosts.GetSingeltonInstance();
            if (openCampaignType == ConstantVariable.CreateCampaign())
                upvotePosts.CampaignName =
                    $"{SocialNetworks.Quora} {campaignDetails.SubModule} [{DateTime.Now.ToString(CultureInfo.InvariantCulture)}]";

            upvotePosts.IsEditCampaignName = true;
            upvotePosts.CancelEditVisibility = Visibility.Visible;
            upvotePosts.TemplateId = campaignDetails.TemplateId;
            upvotePosts.CampaignButtonContent = openCampaignType;
            upvotePosts.CampaignName = openCampaignType == ConstantVariable.UpdateCampaign()
                ? campaignDetails.CampaignName
                : upvotePosts.CampaignName;
            upvotePosts.SelectedAccountCount = campaignDetails.SelectedAccountList.Count +
                                                 $" {"LangKeyAccountSelected".FromResourceDictionary()}";
            upvotePosts.UpvotePostsFooter.list_SelectedAccounts = campaignDetails.SelectedAccountList;
            upvotePosts.ObjViewModel.UpvotePostsModel =
                JsonConvert.DeserializeObject<UpvotePostsModel>(templateDetails.ActivitySettings);
            upvotePosts.MainGrid.DataContext = upvotePosts.ObjViewModel;
            TabSwitcher.ChangeTabIndex(2, 3);
        }
    }
}
