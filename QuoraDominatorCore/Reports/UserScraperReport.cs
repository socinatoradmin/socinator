using System;

namespace QuoraDominatorCore.Reports
{
    public class UserScraperReport
    {
        public int Id { get; set; }
        public string AccountName { get; set; }

        public string Query { get; set; }
        public string QueryType { get; set; }

        public DateTime Date { get; set; }

        public string Username { get; set; }
        public int FollowerCount {  get; set; }
        public int FollowingCount {  get; set; }
        public int AnswerCount {  get; set; }
        public int QuestionCount {  get; set; }
        public int PostCount {  get; set; }
    }
}