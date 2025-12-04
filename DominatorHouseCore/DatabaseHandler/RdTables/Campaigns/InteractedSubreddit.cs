#region

using System;
using SQLite;

#endregion

namespace DominatorHouseCore.DatabaseHandler.RdTables.Campaigns
{
    public class InteractedSubreddit
    {
        [PrimaryKey]
        [AutoIncrement]
        [Indexed]
        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 1)]
        public int Id { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 2)]
        public string whitelistStatus { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 3)]
        public bool isNSFW { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 4)]
        public int subscribers { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 5)]
        public string primaryColor { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 6)]
        public string SubscribeId { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 7)]
        public bool isQuarantined { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 8)]
        public string name { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 9)]
        public string title { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 10)]
        public string url { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 11)]
        public int wls { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 12)]
        public string displayText { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 13)]
        public string type { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 14)]
        public string communityIcon { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 15)]

        public string publicDescription { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 16)]
        public bool userIsSubscriber { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 17)]
        public string accountsActive { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 18)]
        public string advertiserCategory { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 19)]
        public bool showMedia { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 20)]
        public string usingNewModmail { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 21)]
        public bool emojisEnabled { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 22)]
        public bool originalContentTagEnabled { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 23)]
        public bool allOriginalContent { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 24)]
        public string QueryType { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 25)]
        public string QueryValue { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 26)]
        public string ActivityType { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 27)]
        public int InteractionTimeStamp { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 28)]
        public DateTime InteractionDateTime { get; set; }


        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 29)]
        public string SinAccId { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 30)]
        public string SinAccUsername { get; set; }
    }
}