using System;

namespace QuoraDominatorCore.Reports.AccountConfigReport
{
    public class InteractedMessages
    {
        public int Id { get; set; }

        public string QueryType { get; set; }

        public string QueryValue { get; set; }

        public DateTime InteractionDateTime { get; set; }
        public string Message { get; set; }

        public string MessagedUserName { get; set; }
    }
}