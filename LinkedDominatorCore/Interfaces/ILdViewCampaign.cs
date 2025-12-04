using System.Windows;
using DominatorHouseCore.Models;

namespace LinkedDominatorCore.Interfaces
{
    public interface ILdViewCampaign
    {
        void ManageCampaign(TemplateModel templateDetails,
            CampaignDetails campaignDetails,
            bool isEditCampaignName,
            Visibility cancelEditVisibility,
            string campaignButtonContent,
            string templateId);
    }
}