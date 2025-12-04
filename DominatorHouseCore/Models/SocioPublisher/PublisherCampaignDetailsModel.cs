#region

using System;

#endregion

namespace DominatorHouseCore.Models.SocioPublisher
{
    public class PublisherCampaignDetailsModel
    {
        public int CampaignId { get; set; }

        public string CampaignName { get; set; }

        public string Status { get; set; }

        public int DestinationCount { get; set; }

        public int DraftCount { get; set; }

        public int PendingCount { get; set; }

        public int PublishedCount { get; set; }

        public DateTime CreatedDate { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }
    }
}