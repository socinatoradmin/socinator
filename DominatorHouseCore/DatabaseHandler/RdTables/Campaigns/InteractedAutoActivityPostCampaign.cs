using SQLite;
using System;

namespace DominatorHouseCore.DatabaseHandler.RdTables.Campaigns
{
    public class InteractedAutoActivityPostCampaign
    {
        [PrimaryKey]
        [AutoIncrement]
        [Indexed]
        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 1)]
        public int Id { get; set; }
        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 2)]
        public string PostId { get; set; }
        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 3)]
        public string PostUrl { get; set; }
        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 4)]
        public string ActivityType { get; set; }
        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 5)]
        public string UserName { get; set; }
        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 6)]
        public bool IsFollowing { get; set; }
        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 7)]
        public bool IsJoined { get; set; }
        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 8)]
        public bool IsUpvoted { get; set; }
        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 9)]
        public bool IsDownvoted { get; set; }
        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 10)]
        public DateTime InteractedDate { get; set; }
        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 11)]
        public string ProfileUrl { get; set; }
        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 12)]
        public DateTime Created { get; set; }
        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 13)]
        public string CommunityUrl { get; set; }
    }
}
