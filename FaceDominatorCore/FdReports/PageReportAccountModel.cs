using DominatorHouseCore.DatabaseHandler.FdTables.Accounts;
using System;

namespace FaceDominatorCore.FdReports
{
    public class PageReportAccountModel
    {
        public int Id { get; set; }


        public string QueryType
        { get; set; }


        public string QueryValue { get; set; }


        public string ActivityType { get; set; }


        public string PageId { get; set; }



        public string PageName { get; set; }


        public string PageUrl { get; set; }


        public string TotalLikers { get; set; }


        public string PageType { get; set; }

        public PageMemberShip MembershipStatus { get; set; }


        public DateTime InteractionTimeStamp { get; set; }

        public string UploadedMediaPath { get; set; }

        public string Message { get; set; }


        /*
                public String DetailedPageInfo { get; set; }
        */
    }
}
