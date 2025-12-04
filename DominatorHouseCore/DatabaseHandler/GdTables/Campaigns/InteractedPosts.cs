#region

using DominatorHouseCore.Enums;
using SQLite;

#endregion

namespace DominatorHouseCore.DatabaseHandler.GdTables.Campaigns
{
    public class InteractedPosts
    {
        [PrimaryKey]
        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 1)]
        [Indexed]
        [AutoIncrement]
        public int Id { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 2)]
        public int InteractionDate { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 3)]
        public MediaType MediaType { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 4)]
        public ActivityType ActivityType { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 5)]
        public string PkOwner { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 6)]
        public int TakenAt { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 7)]
        public string UsernameOwner { get; set; }


        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 8)]
        public string Username { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 9)]
        public string Comment { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 10)]
        public string OriginalMediaCode { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 11)]
        public string OriginalMediaOwner { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 12)]
        public string QueryType { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 13)]
        public string QueryValue { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 14)]
        public string CommentId { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 15)]
        public string Permalink { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 16)]
        public string Status { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 17)]
        public string TotalLike { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 18)]
        public string TotalComment { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 19)]
        public string PostLocation { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 20)]
        public string CommentOwnerName { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 21)]
        public string CommentOwnerId { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 22)]
        public string PostUrl { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 23)]
        public string UsersTag { get; set; }
    }
}