using System.Windows;
using DominatorHouseCore.Models;

namespace TwtDominatorCore.Interface
{
    public interface ITDViewCampaign
    {
        void ManageCampaign(TemplateModel templateDetails,
            CampaignDetails campaignDetails,
            bool isEditCampaignName,
            Visibility cancelEditVisibility,
            string campaignButtonContent,
            string templateId);
    }
}