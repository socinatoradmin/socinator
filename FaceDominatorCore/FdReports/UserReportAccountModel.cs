using System;

namespace FaceDominatorCore.FdReports
{
    public class UserReportAccountModel
    {
        public int Id { get; set; }

        public string QueryType { get; set; }

        public string QueryValue { get; set; }

        public string ActivityType { get; set; }

        public string UserId { get; set; }

        public string UserProfileUrl { get; set; }

        public string UserName { get; set; }

        public string Message { get; set; }

        public string UploadedMediaPath { get; set; }

        public string DetailedUserInfo { get; set; }

        public DateTime InteractionTimeStamp { get; set; }

        public string ModuleName { get; set; }

        public bool IsPublishedPostOnTimeline { get; set; }

        public string PostDescription { get; set; }

        public string PublishedPostUrl { get; set; }
    }

    public class SendFriendAccountModel
    {
        public int Id { get; set; }

        public string QueryType { get; set; }

        public string QueryValue { get; set; }

        public string ActivityType { get; set; }

        public string UserId { get; set; }

        public string UserProfileUrl { get; set; }

        public string UserName { get; set; }

        public DateTime InteractionDateTime { get; set; }

    }


    public class PageInviterReportAccountModel
    {
        public int Id { get; set; }



        public string ActivityType
        { get; set; }


        public string UserProfileUrl
        { get; set; }


        public string UserName
        { get; set; }


        public string PageName { get; set; } = string.Empty;

        public string PageUrl { get; set; } = string.Empty;

        public string IsInvitedInMessanger { get; set; } = string.Empty;


        public string IsInvitedWithNote { get; set; } = string.Empty;

        public string Note { get; set; } = string.Empty;

        public DateTime InteractionTimeStamp { get; set; }



    }



    public class GroupInviterReportAccountModel
    {
        public int Id { get; set; }



        public string ActivityType
        { get; set; }


        public string UserProfileUrl
        { get; set; }


        public string UserName
        { get; set; }

        public string GroupName { get; set; } = string.Empty;

        public string GroupUrl { get; set; } = string.Empty;


        public string IsInvitedWithNote { get; set; } = string.Empty;

        public string Note { get; set; } = string.Empty;

        public DateTime InteractionTimeStamp { get; set; }

    }
}
