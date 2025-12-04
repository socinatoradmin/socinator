using SQLite;
using System;

namespace DominatorHouseCore.DatabaseHandler.QdTables.Campaigns
{
    public class InteractedTopic
    {
        [PrimaryKey]
        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 1)]
        [Indexed]
        [AutoIncrement]
        public int Id { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 2)]
        public string QueryType { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 3)]
        public string QueryValue { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 4)]
        public string ActivityType { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 5)]
        public DateTime InteractionDateTime { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 6)]
        public string Accountusername { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 7)]
        public string TopicId { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 8)]
        public string TopicUrl { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 9)]
        public bool IsSensitive { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 10)]
        public bool IsFollowing { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 11)]
        public int FollowerCount { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 12)]
        public string TopicName { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 13)]
        public string TopicProfilePicUrl { get; set; }
    }
}
