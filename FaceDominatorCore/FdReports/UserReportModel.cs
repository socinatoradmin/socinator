using System;

namespace FaceDominatorCore.FdReports
{
    public class UserReportModel
    {


        public int Id { get; set; }


        public string AccountEmail { get; set; } = string.Empty;


        public string QueryType { get; set; } = string.Empty;


        public string QueryValue { get; set; } = string.Empty;



        public string ActivityType
        { get; set; } = string.Empty;


        public string UserId
        { get; set; } = string.Empty;


        public string UserProfileUrl
        { get; set; } = string.Empty;


        public string UserName
        { get; set; } = string.Empty;

        public string DetailedUserInfo
        { get; set; } = string.Empty;

        /*
                public DateTime ConnectionDate { get; set; }
        */

        public DateTime InteractionTimeStamp { get; set; }


        public string Message
        { get; set; } = string.Empty;


        public string UploadedMediaPath
        { get; set; } = string.Empty;
        public string PublishedPostUrl { get; set; }
        public bool IsPublishedPostOnTimeline { get; set; }
        public string PostDescription { get; set; }
        public string Gender { get; set; }
        public string University { get; set; }
        public string Workplace { get; set; }
        public string CurrentCity { get; set; }
        public string HomeTown { get; set; }
        public string BirthDate { get; set; }
        public string ContactNo { get; set; }
        public string ProfilePic { get; set; }
    }



    public class PageInviterReportModel
    {


        public int Id { get; set; }


        public string AccountEmail { get; set; } = string.Empty;




        public string ActivityType
        { get; set; } = string.Empty;


        public string UserId
        { get; set; } = string.Empty;


        public string UserProfileUrl
        { get; set; } = string.Empty;


        public string UserName
        { get; set; } = string.Empty;

        public string DetailedUserInfo
        { get; set; } = string.Empty;

        /*
                public DateTime ConnectionDate { get; set; }
        */

        public DateTime InteractionTimeStamp { get; set; }


        public string PageName { get; set; } = string.Empty;

        public string PageUrl { get; set; } = string.Empty;


        public string IsInvitedInMessanger { get; set; } = string.Empty;


        public string IsInvitedWithNote { get; set; } = string.Empty;

        public string Note { get; set; } = string.Empty;
    }


    public class GroupInviterReportModel
    {


        public int Id { get; set; }


        public string AccountEmail { get; set; } = string.Empty;




        public string ActivityType
        { get; set; } = string.Empty;


        public string UserId
        { get; set; } = string.Empty;


        public string UserProfileUrl
        { get; set; } = string.Empty;


        public string UserName
        { get; set; } = string.Empty;

        public string DetailedUserInfo
        { get; set; } = string.Empty;

        /*
                public DateTime ConnectionDate { get; set; }
        */

        public DateTime InteractionTimeStamp { get; set; }


        public string GroupName { get; set; } = string.Empty;

        public string GroupUrl { get; set; } = string.Empty;


        public string IsInvitedWithNote { get; set; } = string.Empty;

        public string Note { get; set; } = string.Empty;
    }
}
