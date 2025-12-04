using DominatorHouseCore.Enums;
using System;

namespace GramDominatorCore.Report
{
    public class SendMessageToNewFollowerReportDetails
    {
        public int Id { get; set; }

        public string AccountUsername { get; set; }

        public ActivityType ActivityType { get; set; } = ActivityType.SendMessageToFollower;

        public string MessageReceiverUsername { get; set; }

        public string MessageReceiverUserId { get; set; }

        public string Message { get; set; }

        public DateTime Date { get; set; }
    }
}
