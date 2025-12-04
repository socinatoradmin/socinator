#region

using System;
using SQLite;

#endregion

namespace DominatorHouseCore.DatabaseHandler.QdTables.Accounts
{
    public class InteractedAnswers
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
        public string AnswersUrl { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 7)]
        public string AnsweredUserName { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 8)]
        public string Accountusername { get; set; }
    }
}