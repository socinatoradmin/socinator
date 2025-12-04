using DominatorHouseCore.Enums;
using System;

namespace FaceDominatorCore.FdReports
{
    public class PostReportModel
    {
        public int Id { get; set; }

        public string AccountEmail { get; set; }

        public string QueryType
        { get; set; }


        public string QueryValue { get; set; }


        public string ActivityType { get; set; }


        public MediaType MediaType { get; set; }


        public string PostId { get; set; }

        public string PostUrl { get; set; }

        public string PostTitle { get; set; }


        public string PostDescription { get; set; }


        public string Likes { get; set; }


        public string Comments { get; set; }



        public string Shares { get; set; }



        public DateTime InteractionTimeStamp { get; set; }

        public string LikeType { get; set; }



        public DateTime PostedDateTime { get; set; } = new DateTime();

        public string Title { get; set; } = string.Empty;

        public string MediaUrl { get; set; } = string.Empty;

        public string OtherMediaUrl { get; set; } = string.Empty;

        public string PostedBy { get; set; } = string.Empty;

        public string CommentText { get; set; } = string.Empty;


        public string SubDescription { get; set; } = string.Empty;

        public string Caption { get; set; } = string.Empty;

        public string NavigationUrl { get; set; } = string.Empty;

        public string OwnerId { get; set; } = string.Empty;

        public bool IsReactedAsPage { get; set; }

        public string ReactedAsPageUrl { get; set; } = string.Empty;

        public string DownLoadFolderPath { get; set; } = string.Empty;


        public string Mentions { get; set; } = string.Empty;
        public string InvitedTo { get; set; }
        public string InvitedToUserName { get; set; }
    }
}
