#region

using System;
using SQLite;

#endregion

namespace DominatorHouseCore.DatabaseHandler.RdTables.Campaigns
{
    public class InteractedUsers
    {
        [PrimaryKey]
        [AutoIncrement]
        [Indexed]
        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 1)]
        public int Id { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 2)]
        public string QueryType { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 3)]
        public string QueryValue { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 4)]
        public string ActivityType { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 5)]
        public int InteractionTimeStamp { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 6)]
        public string InteractedUsername { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 7)]
        public int Date { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 8)]
        public string InteractedUserId { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 9)]
        public int UpdatedTime { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 10)]
        public string AccountIcon { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 11)]
        public int CommentKarma { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 12)]
        public DateTime Created { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 13)]
        public string DisplayName { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 14)]
        public string DisplayNamePrefixed { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 15)]
        public string DisplayText { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 16)]
        public bool HasUserProfile { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 17)]
        public bool IsEmployee { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 18)]
        public bool IsFollowing { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 19)]
        public bool IsGold { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 20)]
        public bool IsMod { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 21)]
        public bool IsNsfw { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 22)]
        public bool PrefShowSnoovatar { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 23)]
        public int PostKarma { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 24)]
        public string Url { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 25)]
        public DateTime InteractionDateTime { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 26)]
        public string SinAccId { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 27)]
        public string SinAccUsername { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 28)]
        public string Message { get; set; }
    }
}