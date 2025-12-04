#region

using System;
using SQLite;

#endregion

namespace DominatorHouseCore.DatabaseHandler.RdTables.Campaigns
{
    public class InteractedPost
    {
        [PrimaryKey]
        [AutoIncrement]
        [Indexed]
        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 1)]
        public int Id { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 2)]
        public string InteracteduserId { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 3)]
        public string Query { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 4)]
        public string QueryType { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 5)]
        public string CommentsCount { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 6)]
        public string ActivityType { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 7)]
        public string Caption { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 8)]
        public bool IsCrosspostable { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 9)]
        public bool IsStickied { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 10)]
        public bool Saved { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 11)]
        public int NumComments { get; set; }

        //public object upvoteRatio { get; set; }
        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 12)]
        public bool IsPinned { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 13)]
        public string InteractedUserName { get; set; }

        //public Media media { get; set; }
        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 14)]
        public int NumCrossposts { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 15)]
        public bool IsSponsored { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 16)]
        public bool IsLocked { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 17)]
        public int Score { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 18)]
        public bool IsArchived { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 19)]
        public bool Hidden { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 20)]
        public string Preview { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 21)]
        public bool IsRoadblock { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 22)]
        public bool SendReplies { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 23)]
        public int GoldCount { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 24)]
        public bool IsSpoiler { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 25)]
        public int VoteState { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 26)]
        public bool IsNsfw { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 27)]
        public bool IsMediaOnly { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 28)]
        public string PostId { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 29)]
        public bool IsBlank { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 30)]
        public int ViewCount { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 31)]
        public string Permalink { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 32)]
        public long Created { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 33)]
        public string Title { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 34)]
        public bool IsOriginalContent { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 35)]
        public int InteractionTimeStamp { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 36)]
        public DateTime InteractionDateTime { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 37)]
        public string SinAccId { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 38)]
        public string SinAccUsername { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 39)]
        public string Status { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 40)]
        public string CommentText { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 41)]
        public string CommentId { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 42)]
        public string OldComment { get; set; }
    }
}