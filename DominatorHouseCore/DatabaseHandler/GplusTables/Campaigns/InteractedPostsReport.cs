#region

using DominatorHouseCore.Enums;
using SQLite;

#endregion

namespace DominatorHouseCore.DatabaseHandler.GplusTables.Campaigns
{
    public class InteractedPostsReport
    {
        [PrimaryKey]
        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 1)]
        [Indexed]
        [AutoIncrement]
        public int Id { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 2)]
        public string Query { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 3)]
        public string QueryType { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 4)]
        public string ActivityType { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 5)]
        public int InteractionDate { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 6)]
        public MediaType MediaType { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 7)]
        public string PostId { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 8)]
        public int UploadedDate { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 9)]
        // ReSharper disable once UnusedMember.Global
        public string PostOwnerId { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 10)]
        public string PostOwnerName { get; set; }


        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 11)]
        public string Caption { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 12)]
        public int LikeCount { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 13)]
        public int CommentCount { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 14)]
        public int ShareCount { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 15)]
        public string PostUrl { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 16)]
        public string Comment { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 17)]
        public string InteractedUserId { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 18)]
        public int IsLiked { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 19)]
        public string CommentId { get; set; }
    }
}