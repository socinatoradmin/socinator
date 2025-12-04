#region

using System;
using SQLite;

#endregion

namespace DominatorHouseCore.DatabaseHandler.PdTables.Accounts
{
    public class DailyStatitics
    {
        [PrimaryKey]
        [AutoIncrement]
        [Indexed]
        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 1)]
        // Row ID
        public int Id { get; set; }

        //Date when statistics are entered in Unix Timestamp
        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 2)]
        public DateTime Date { get; set; }

        //Followers count of the DB owner when the statistics has got updated
        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 3)]
        public int Followers { get; set; }

        //Followings count of the DB owner when the statistics has got updated
        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 4)]
        public int Followings { get; set; }

        //Pins count of the DB owner when the statistics has got updated
        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 5)]
        public int Pins { get; set; }

        //Boards count of the DB owner when the statistics has got updated
        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 6)]
        public int Boards { get; set; }
    }
}