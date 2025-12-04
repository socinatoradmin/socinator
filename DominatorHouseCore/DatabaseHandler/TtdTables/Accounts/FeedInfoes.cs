#region

using SQLite;

#endregion

namespace DominatorHouseCore.DatabaseHandler.TtdTables.Accounts
{
    public class FeedInfoes
    {
        [PrimaryKey]
        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 1)]
        [Indexed]
        [AutoIncrement]
        public int Id { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 2)]
        public string AwemeId { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 3)]
        public string VideoUrl { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 4)]
        public string Caption { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 5)]
        public int CreateTime { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 6)]
        public string OwnerId { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 7)]
        public string OwnerUsername { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 8)]
        public string OwnerNickName { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 9)]
        public string OwnerProfilePic { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 10)]
        public string CommentCount { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 11)]
        public string DownloadCount { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 12)]
        public string PlayCount { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 13)]
        public string ShareCount { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 14)]
        public string ForwardCount { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 15)]
        public string Duration { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 16)]
        public int UpdateTime { get; set; }
    }
}