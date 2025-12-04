#region

using System.Windows;

#endregion

namespace DominatorHouseCore.Interfaces
{
    public interface IHeaderControl
    {
        // To specify the campaign name
        string CampaignName { get; set; }

        //To specify the campaign name is possible to edit the campaign
        bool IsEditCampaignName { get; set; }

        // To specify the cancel edit button is visible in header section
        Visibility CancelEditVisibility { get; set; }
        string TemplateId { get; set; }
    }
}