using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using Newtonsoft.Json;
using System;
using System.Globalization;
using System.Windows;
using TumblrDominatorCore.Interface;
using TumblrDominatorCore.Models;

namespace TumblrDominatorUI.Utility.Reblog
{
    public class ReblogViewCampaign : ITumblrViewCampaign
    {
        public void ManageCampaign(TemplateModel templateDetails, CampaignDetails campaignDetails,
            bool isEditCampaignName,
            Visibility cancelEditVisibility, string campaignButtonContent, string templateId)
        {
            var objReblog = TumblrView.Blogs.Reblog.GetSingeltonObjectReblog();
            if (campaignButtonContent == ConstantVariable.CreateCampaign())
                objReblog.CampaignName =
                    $"{SocialNetworks.Tumblr} {campaignDetails.SubModule} [{DateTime.Now.ToString(CultureInfo.InvariantCulture)}]";
            objReblog.IsEditCampaignName = isEditCampaignName;
            objReblog.CancelEditVisibility = cancelEditVisibility;
            objReblog.TemplateId = templateId;
            objReblog.CampaignName = campaignDetails.CampaignName;
            objReblog.CampaignButtonContent = campaignButtonContent;
            objReblog.SelectedAccountCount = campaignDetails.SelectedAccountList.Count +
                                             $" {"LangKeyAccountSelected".FromResourceDictionary()}";
            objReblog.ReblogFooter.list_SelectedAccounts = campaignDetails.SelectedAccountList;
            objReblog.ObjViewModel.ReblogModel =
                JsonConvert.DeserializeObject<ReblogModel>(templateDetails.ActivitySettings);

            objReblog.MainGrid.DataContext = objReblog.ObjViewModel;

            TabSwitcher.ChangeTabIndex(3, 0);
        }
    }
}