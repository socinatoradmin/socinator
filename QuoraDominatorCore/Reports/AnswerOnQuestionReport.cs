using System;

namespace QuoraDominatorCore.Reports
{
    public class AnswerOnQuestionReport
    {
        public int Id { get; set; }
        public string AccountName { get; set; }

        public string Query { get; set; }
        public string QueryType { get; set; }

        public DateTime Date { get; set; }

        public string Username { get; set; }
        public string AnswerUrl { get; set; }
    }
}