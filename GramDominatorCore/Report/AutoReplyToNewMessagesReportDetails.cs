using System;
using DominatorHouseCore.Enums;

namespace GramDominatorCore.Report
{
    public class AutoReplyToNewMessagesReportDetails
    {
        public int Id { get; set; }

        public string AccountUsername { get; set; }

        public ActivityType ActivityType { get;} = ActivityType.AutoReplyToNewMessage;

        public string MessageReceiverUsername { get; set; }

        public string MessageReceiverUserId { get; set; }

        public string Message { get; set; }

        public DateTime Date { get; set; }
    }
}
