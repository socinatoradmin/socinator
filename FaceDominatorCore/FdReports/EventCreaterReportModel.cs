using System;

namespace FaceDominatorCore.FdReports
{
    public class EventCreaterReportModel
    {
        public int Id { get; set; }

        public string QueryType { get; set; }

        public string QueryValue { get; set; }

        public string AccountEmail { get; set; }

        public string ActivityType { get; set; }

        public string EventType { get; set; }

        public string EventId { get; set; }

        public string EventName { get; set; }

        public string EventLocation { get; set; }

        public string EventUrl { get; set; }

        public DateTime EventStartDate { get; set; }

        public DateTime EventEndDate { get; set; }

        public DateTime InteractionTimeStamp { get; set; }

        public DateTime RequestedDate { get; set; }

    }
}
