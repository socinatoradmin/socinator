#region

using System;
using SQLite;

#endregion

namespace DominatorHouseCore.DatabaseHandler.QdTables.Accounts
{
    public class InteractedMessage
    {
        [PrimaryKey]
        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 1)]
        [Indexed]
        [AutoIncrement]
        public int Id { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 2)]
        public string SinAccUsername { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 3)]
        public string QueryType { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 4)]
        public string QueryValue { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 5)]
        public string ActivityType { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 6)]
        public int InteractionTimeStamp { get; set; }


        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 9)]
        public string Username { get; set; }


        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 11)]
        public string Message { get; set; }


        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 17)]
        public int FollowBackStatus { get; set; }


        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 19)]
        public DateTime InteractionDate { get; set; }
    }
}