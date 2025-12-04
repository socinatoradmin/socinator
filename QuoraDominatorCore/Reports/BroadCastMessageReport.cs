using System;

namespace QuoraDominatorCore.Reports
{
    public class BroadCastMessageReport
    {
        public int Id { get; set; }

        public string QueryType { get; set; }

        public string QueryValue { get; set; }

        public string Account { get; set; }

        public DateTime InteractionDateTime { get; set; }

        public string Message { get; set; }

        public string UserName { get; set; }
    }
}