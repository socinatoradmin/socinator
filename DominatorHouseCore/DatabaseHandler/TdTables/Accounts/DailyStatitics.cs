#region

using System;
using SQLite;

#endregion

namespace DominatorHouseCore.DatabaseHandler.TdTables.Accounts
{
    public class DailyStatitics
    {
        [PrimaryKey]
        [AutoIncrement]
        [Indexed]
        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 1)]
        public int Id { get; set; }

        /// <summary>
        ///     Date when statistics are entered in Unix Timestamp
        /// </summary>

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 2)]
        public DateTime Date { get; set; }

        /// <summary>
        ///     Followers count of the DB owner when the statistics has got updated
        /// </summary>

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 3)]
        public int Followers { get; set; }

        /// <summary>
        ///     Followings count of the DB owner when the statistics has got updated
        /// </summary>

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 4)]
        public int Followings { get; set; }

        /// <summary>
        ///     Tweets count of the DB owner when the statistics has got updated
        /// </summary>

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 5)]
        public int Tweets { get; set; }

        /// <summary>
        ///     Likes count of the DB owner when the statistics has got updated
        /// </summary>

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 6)]
        public int Likes { get; set; }
    }
}