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
using YoutubeDominatorCore.YoutubeModels.EngageModel;
using YoutubeDominatorUI.YDViews.Engage;

namespace YoutubeDominatorUI.Utility.LikeUtility
{
    public class LikeViewCampaign : IYdViewCampaign
    {
        public void ManageCampaign(TemplateModel templateDetails, CampaignDetails campaignDetails,
            bool isEditCampaignName,
            Visibility cancelEditVisibility, string campaignButtonContent, string templateId)
        {
            var moduleUIObject = Like.GetSingeltonObjectLike();
            if (campaignButtonContent == ConstantVariable.CreateCampaign())
                moduleUIObject.CampaignName =
                    $"{SocialNetworks.YouTube} {campaignDetails.SubModule} [{DateTime.Now.ToString(CultureInfo.InvariantCulture)}]";
            else if (campaignButtonContent == ConstantVariable.UpdateCampaign())
                moduleUIObject.CampaignName = campaignDetails.CampaignName;

            moduleUIObject.IsEditCampaignName = isEditCampaignName;
            moduleUIObject.CancelEditVisibility = cancelEditVisibility;
            moduleUIObject.TemplateId = templateId;
            moduleUIObject.CampaignButtonContent = campaignButtonContent;
            moduleUIObject.ObjViewModel.LikeModel =
                JsonConvert.DeserializeObject<LikeModel>(templateDetails.ActivitySettings);

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
            moduleUIObject.LikeFooter.list_SelectedAccounts = selectedAccounts;
            moduleUIObject.MainGrid.DataContext = moduleUIObject.ObjViewModel;
            TabSwitcher.ChangeTabIndex(1, 0);
        }
    }
}