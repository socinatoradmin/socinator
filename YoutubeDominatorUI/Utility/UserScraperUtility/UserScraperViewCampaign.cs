using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using YoutubeDominatorCore.Interface;
using YoutubeDominatorCore.YoutubeModel;
using YoutubeDominatorUI.YDViews.GrowSubscribers;
using YoutubeDominatorUI.YDViews.Scraper;

namespace YoutubeDominatorUI.Utility.UserScraperUtility
{
    public class UserScraperViewCampaign : IYdViewCampaign
    {
        public void ManageCampaign(TemplateModel templateDetails, CampaignDetails campaignDetails, bool isEditCampaignName,
           Visibility cancelEditVisibility, string campaignButtonContent, string templateId)
        {
            UserScraper objUserScraper = UserScraper.GetSingeltonObjectUserScraper();
            objUserScraper.IsEditCampaignName = isEditCampaignName;
            objUserScraper.CancelEditVisibility = cancelEditVisibility;
            objUserScraper.TemplateId = templateId;
            objUserScraper.CampaignName = campaignDetails.CampaignName;
            objUserScraper.CampaignButtonContent = campaignButtonContent;
            objUserScraper.SelectedAccountCount = campaignDetails.SelectedAccountList.Count + " Account Selected";
            objUserScraper.UserScraperFooter.list_SelectedAccounts = campaignDetails.SelectedAccountList;
            objUserScraper.ObjViewModel.userScraperModel = JsonConvert.DeserializeObject<UserScraperModel>(templateDetails.ActivitySettings);
            objUserScraper.MainGrid.DataContext = objUserScraper.ObjViewModel;
            TabSwitcher.ChangeTabIndex(1, 0);

        }
    }
}
