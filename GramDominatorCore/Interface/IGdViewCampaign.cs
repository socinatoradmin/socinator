using System.Windows;
using DominatorHouseCore.Models;

namespace GramDominatorCore.Interface
{
    public interface IGdViewCampaign
    {
        void ManageCampaign(TemplateModel templateDetails,
            CampaignDetails campaignDetails,
            bool isEditCampaignName,
            Visibility cancelEditVisibility,
            string campaignButtonContent,
            string templateId);
    }
}
