#region

using System.ComponentModel;

#endregion

namespace DominatorHouseCore.Models.SocioPublisher
{
    public enum PublisherCampaignStatus
    {
        [Description("Campaign Active")] Active = 0,

        [Description("Campaign Paused")] Paused = 1,

        [Description("Campaign Completed")] Completed = 2
    }
}