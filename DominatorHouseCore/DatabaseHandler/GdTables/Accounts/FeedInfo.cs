#region

using DominatorHouseCore.Enums;
using SQLite;

#endregion

namespace DominatorHouseCore.DatabaseHandler.GdTables.Accounts
{
    public class FeedInfoes
    {
        [PrimaryKey]
        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 1)]
        [Indexed]
        [AutoIncrement]
        public int Id { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 2)]
        public string Caption { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 3)]
        public int CommentCount { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 4)]
        public bool CommentsDisabled { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 5)]
        public string Preview { get; set; }


        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 6)]
        public int TakenAt { get; set; }


        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 7)]
        public double VideoDuration { get; set; }


        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 8)]
        public int ViewCount { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 9)]
        public string MediaId { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 10)]
        public MediaType MediaType { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 11)]
        public string MediaCode { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 12)]
        public string PostedBy { get; set; }
    }
}