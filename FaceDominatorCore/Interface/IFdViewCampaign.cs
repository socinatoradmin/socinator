using DominatorHouseCore.Models;
using System.Windows;

namespace FaceDominatorCore.Interface
{
    public interface IFdViewCampaign
    {
        void ManageCampaign(TemplateModel templateDetails,
            CampaignDetails campaignDetails,
            bool isEditCampaignName,
            Visibility cancelEditVisibility,
            string campaignButtonContent,
            string templateId);
    }
}