using System;

namespace QuoraDominatorCore.Reports
{
    public class AnswersReport
    {
        public int Id { get; set; }
        public string QueryType { get; set; }
        public string QueryValue { get; set; }
        public string ActivityType { get; set; }
        public DateTime InteractionDateTime { get; set; }
        public string AnswersUrl { get; set; }
        public string AnsweredUserName { get; set; }
        public string AccountUsername { get; set; }
    }
}