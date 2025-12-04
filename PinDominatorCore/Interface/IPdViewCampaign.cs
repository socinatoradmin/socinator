using System.Windows;
using DominatorHouseCore.Models;

namespace PinDominatorCore.Interface
{
    public interface IPdViewCampaign
    {
        void ManageCampaign(TemplateModel templateDetails,
            CampaignDetails campaignDetails,
            bool isEditCampaignName,
            Visibility cancelEditVisibility,
            string campaignButtonContent,
            string templateId);
    }
}