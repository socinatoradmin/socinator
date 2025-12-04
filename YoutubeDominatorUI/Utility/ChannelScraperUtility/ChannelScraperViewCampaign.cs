using DominatorHouseCore.Enums;
using DominatorHouseCore.Enums.YdQuery;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using Newtonsoft.Json;
using System;
using System.Globalization;
using System.Linq;
using System.Windows;
using YoutubeDominatorCore.Interface;
using YoutubeDominatorCore.YDUtility;
using YoutubeDominatorCore.YoutubeModels.ScraperModel;
using YoutubeDominatorUI.YDViews.Scraper;

namespace YoutubeDominatorUI.Utility.ChannelScraperUtility
{
    public class ChannelScraperViewCampaign : IYdViewCampaign
    {
        public void ManageCampaign(TemplateModel templateDetails, CampaignDetails campaignDetails,
            bool isEditCampaignName,
            Visibility cancelEditVisibility, string campaignButtonContent, string templateId)
        {
            var moduleUIObject = ChannelScraper.GetSingeltonObjectChannelScraper();
            if (campaignButtonContent == ConstantVariable.CreateCampaign())
                moduleUIObject.CampaignName =
                    $"{SocialNetworks.YouTube} {campaignDetails.SubModule} [{DateTime.Now.ToString(CultureInfo.InvariantCulture)}]";
            else if (campaignButtonContent == ConstantVariable.UpdateCampaign())
                moduleUIObject.CampaignName = campaignDetails.CampaignName;

            moduleUIObject.IsEditCampaignName = isEditCampaignName;
            moduleUIObject.CancelEditVisibility = cancelEditVisibility;
            moduleUIObject.TemplateId = templateId;
            moduleUIObject.CampaignButtonContent = campaignButtonContent;
            moduleUIObject.ObjViewModel.ChannelScraperModel =
                JsonConvert.DeserializeObject<ChannelScraperModel>(templateDetails.ActivitySettings);

            // Remove those accounts which are not in campaign details bcz of these accounts would be selected for other campaign
            moduleUIObject.ObjViewModel.Model.ListSelectDestination.DeepCloneObject().ForEach(x =>
            {
                if (!campaignDetails.SelectedAccountList.Contains(x.AccountName))
                    moduleUIObject.ObjViewModel.Model.ListSelectDestination.RemoveAt(
                        moduleUIObject.ObjViewModel.Model.ListSelectDestination.IndexOf(
                            moduleUIObject.ObjViewModel.Model.ListSelectDestination.FirstOrDefault(z =>
                                z.AccountName == x.AccountName)));
            });

            #region Remove this code before next 2nd-3rd release

            if (moduleUIObject.Model.ListQueryType.Count == 2)
                Enum.GetValues(typeof(YdScraperParameters)).Cast<YdScraperParameters>().ToList().ForEach(query =>
                {
                    if (query == YdScraperParameters.YTVideoCommenters)
                        moduleUIObject.Model.ListQueryType.Add(Application.Current
                            .FindResource(query.GetDescriptionAttr()).ToString());
                });

            #endregion

            var selectedAccounts =
                new YoutubeUtilities().SelectedAccountsName(moduleUIObject.ObjViewModel.Model.ListSelectDestination);
            moduleUIObject.SelectedAccountCount = selectedAccounts.Count + " Account" +
                                                  (selectedAccounts.Count < 2 ? "" : "s") + " Selected";
            moduleUIObject.ChannelScraperFooter.list_SelectedAccounts = selectedAccounts;
            moduleUIObject.MainGrid.DataContext = moduleUIObject.ObjViewModel;
            TabSwitcher.ChangeTabIndex(3, 1);
        }
    }
}