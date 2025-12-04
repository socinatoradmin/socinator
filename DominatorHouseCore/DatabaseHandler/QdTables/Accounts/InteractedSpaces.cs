using SQLite;
using System;

namespace DominatorHouseCore.DatabaseHandler.QdTables.Accounts
{
    public class InteractedSpaces
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
        public string SpaceUrl { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 7)]
        public string SpaceName { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 8)]
        public string Accountusername { get; set; }
        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 9)]
        public string SpaceDescription { get; set; }
        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 10)]
        public string SpaceId { get; set; }
        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 11)]
        public int ContributorsCount { get; set; }
        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 12)]
        public int SubscriberCount { get; set; }
        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 13)]
        public int FollowerCount { get; set; }
        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 14)]
        public int LastWeekPostCount { get; set; }
        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 15)]
        public bool IsFollowing { get; set; }
        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 16)]
        public bool IsSubscriber { get; set; }
        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 17)]
        public bool IsContributor { get; set; }
        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 18)]
        public bool IsAdmin { get; set; }
    }
}
