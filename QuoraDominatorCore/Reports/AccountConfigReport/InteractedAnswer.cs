using System;

namespace QuoraDominatorCore.Reports.AccountConfigReport
{
    public class InteractedAnswer
    {
        public int Id { get; set; }

        public string QueryValue { get; set; }

        public string QueryType { get; set; }

        public DateTime Date { get; set; }

        public string AnswerUrl { get; set; }

        public string Answered { get; set; }
    }
}