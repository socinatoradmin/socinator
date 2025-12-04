using DominatorHouseCore.Models;
using System.Windows;

namespace YoutubeDominatorCore.Interface
{
    public interface IYdViewCampaign
    {
        void ManageCampaign(TemplateModel templateDetails,
            CampaignDetails campaignDetails,
            bool isEditCampaignName,
            Visibility cancelEditVisibility,
            string campaignButtonContent,
            string templateId);
    }
}