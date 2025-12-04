using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using Newtonsoft.Json;
using System;
using System.Globalization;
using System.Linq;
using System.Windows;
using YoutubeDominatorCore.Interface;
using YoutubeDominatorCore.YDUtility;
using YoutubeDominatorCore.YoutubeModels.GrowSubscribersModel;
using YoutubeDominatorUI.YDViews.GrowSubscribers;

namespace YoutubeDominatorUI.Utility.UnsubscribeUtility
{
    public class UnsubscribeViewCampaign : IYdViewCampaign
    {
        public void ManageCampaign(TemplateModel templateDetails, CampaignDetails campaignDetails,
            bool isEditCampaignName,
            Visibility cancelEditVisibility, string campaignButtonContent, string templateId)
        {
            var moduleUIObject = Unsubscribe.GetSingeltonObjectUnsubscribe();
            if (campaignButtonContent == ConstantVariable.CreateCampaign())
                moduleUIObject.CampaignName =
                    $"{SocialNetworks.YouTube} {campaignDetails.SubModule} [{DateTime.Now.ToString(CultureInfo.InvariantCulture)}]";
            else if (campaignButtonContent == ConstantVariable.UpdateCampaign())
                moduleUIObject.CampaignName = campaignDetails.CampaignName;

            moduleUIObject.IsEditCampaignName = isEditCampaignName;
            moduleUIObject.CancelEditVisibility = cancelEditVisibility;
            moduleUIObject.TemplateId = templateId;
            moduleUIObject.CampaignButtonContent = campaignButtonContent;
            moduleUIObject.ObjViewModel.UnsubscribeModel =
                JsonConvert.DeserializeObject<UnsubscribeModel>(templateDetails.ActivitySettings);

            // Remove those accounts which are not in campaign details bcz of these accounts would be selected for other campaign
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
            moduleUIObject.UnSubscribeFooter.list_SelectedAccounts = selectedAccounts;
            moduleUIObject.MainGrid.DataContext = moduleUIObject.ObjViewModel;
            TabSwitcher.ChangeTabIndex(2, 1);
        }
    }
}