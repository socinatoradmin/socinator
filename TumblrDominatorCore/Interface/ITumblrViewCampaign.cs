using DominatorHouseCore.Models;
using System.Windows;

namespace TumblrDominatorCore.Interface
{
    public interface ITumblrViewCampaign
    {
        void ManageCampaign(TemplateModel templateDetails,
            CampaignDetails campaignDetails,
            bool isEditCampaignName,
            Visibility cancelEditVisibility,
            string campaignButtonContent,
            string templateId);
    }
}