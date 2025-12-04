using DominatorHouseCore.Enums;
using System;

namespace TumblrDominatorUI.Report
{
    public class AccountReportEngage
    {
        public int Id { get; set; }

        public string Query { get; set; }

        public MediaType MediaType { get; set; }

        public string QueryType { get; set; }

        public string PostOwner { get; set; }

        public string ContentId { get; set; }
        public string PostUrl { get; set; }

        public DateTime Date { get; set; }
    }
}