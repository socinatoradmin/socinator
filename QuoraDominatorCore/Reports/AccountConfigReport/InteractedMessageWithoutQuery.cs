using System;

namespace QuoraDominatorCore.Reports.AccountConfigReport
{
    public class InteractedMessageWithoutQuery
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string Message { get; set; }
        public DateTime InteractionDateTime { get; set; }
    }
}