using System;
using DominatorHouseCore.DatabaseHandler.LdTables.Account;
using DominatorHouseCore.Enums;

namespace LinkedDominatorCore.LDModel.ReportModel
{
    public class InteractedPostsReportModel
    {
        public int Id { get; set; }


        public string AccountEmail { get; set; }


        public string QueryType { get; set; }


        public string QueryValue { get; set; }


        public string ActivityType { get; set; }

        public MediaType MediaType { get; set; }


        public string PostLink { get; set; }


        public string PostTitle { get; set; }


        public string PostDescription { get; set; }

        public string MyComment { get; set; }


        public string LikeCount { get; set; }


        public string CommentCount { get; set; }

        public string ShareCount { get; set; }


        public string PostOwnerFullName { get; set; }


        public string PostOwnerProfileUrl { get; set; }

        public ConnectionType ConnectionType { get; set; }

        public DateTime PostedDateTime { get; set; }

        public DateTime InteractionDateTime { get; set; }
    }
}