using DominatorHouseCore.Enums;
using DominatorHouseCore.Enums.YdQuery;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using System;
using System.Globalization;
using System.Linq;
using System.Windows;
using YoutubeDominatorCore.Interface;
using YoutubeDominatorCore.YDUtility;
using YoutubeDominatorCore.YoutubeModels.GrowSubscribersModel;
using YoutubeDominatorUI.YDViews.GrowSubscribers;

namespace YoutubeDominatorUI.Utility.SubscribeUtility
{
    public class SubscribeViewCampaign : IYdViewCampaign
    {
        public void ManageCampaign(TemplateModel templateDetails, CampaignDetails campaignDetails,
            bool isEditCampaignName,
            Visibility cancelEditVisibility, string campaignButtonContent, string templateId)
        {
            var moduleUIObject = Subscribe.GetSingeltonObjectSubscribe();
            if (campaignButtonContent == ConstantVariable.CreateCampaign())
                moduleUIObject.CampaignName =
                    $"{SocialNetworks.YouTube} {campaignDetails.SubModule} [{DateTime.Now.ToString(CultureInfo.InvariantCulture)}]";
            else if (campaignButtonContent == ConstantVariable.UpdateCampaign())
                moduleUIObject.CampaignName = campaignDetails.CampaignName;

            moduleUIObject.IsEditCampaignName = isEditCampaignName;
            moduleUIObject.CancelEditVisibility = cancelEditVisibility;
            moduleUIObject.TemplateId = templateId;
            moduleUIObject.CampaignButtonContent = campaignButtonContent;
            moduleUIObject.ObjViewModel.SubscribeModel =
                templateDetails.ActivitySettings.GetActivityModel<SubscribeModel>(moduleUIObject.ObjViewModel.Model);

            #region Remove this code before next 2nd-3rd release

            if (moduleUIObject.Model.ListQueryType.Count == 2)
                Enum.GetValues(typeof(YdScraperParameters)).Cast<YdScraperParameters>().ToList().ForEach(query =>
                {
                    if (query == YdScraperParameters.YTVideoCommenters)
                        moduleUIObject.Model.ListQueryType.Add(Application.Current
                            .FindResource(query.GetDescriptionAttr()).ToString());
                });

            #endregion

            // Remove those account which are not in campaign details bcz of these accounts would be selected for other campaign
            moduleUIObject.ObjViewModel.Model.ListSelectDestination.DeepCloneObject().ForEach(x =>
            {
                if (!campaignDetails.SelectedAccountList.Contains(x.AccountName))
                    moduleUIObject.ObjViewModel.Model.ListSelectDestination.RemoveAt(
                        moduleUIObject.ObjViewModel.Model.ListSelectDestination.IndexOf(
                            moduleUIObject.ObjViewModel.Model.ListSelectDestination.FirstOrDefault(z =>
                                z.AccountName == x.AccountName)));
            });

            var selectedAccounts =
                new YoutubeUtilities().SelectedAccountsName(moduleUIObject.ObjViewModel.Model.ListSelectDestination);

            moduleUIObject.SelectedAccountCount = selectedAccounts.Count + " Account" +
                                                  (selectedAccounts.Count < 2 ? "" : "s") + " Selected";
            moduleUIObject.SubscribeFooter.list_SelectedAccounts = selectedAccounts;
            moduleUIObject.MainGrid.DataContext = moduleUIObject.ObjViewModel;
            TabSwitcher.ChangeTabIndex(2, 0);
        }
    }
}