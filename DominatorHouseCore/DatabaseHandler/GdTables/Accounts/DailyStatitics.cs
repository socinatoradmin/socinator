#region

using System;
using SQLite;

#endregion

namespace DominatorHouseCore.DatabaseHandler.GdTables.Accounts
{
    public class DailyStatitics
    {
        [PrimaryKey]
        [AutoIncrement]
        [Indexed]
        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 1)]
        public int Id { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 2)]
        public DateTime Date { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 3)]
        public int Followers { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 4)]
        public int Followings { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 5)]
        public int Uploads { get; set; }
    }
}