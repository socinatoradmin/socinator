#region

using System;
using SQLite;

#endregion

namespace DominatorHouseCore.DatabaseHandler.LdTables.Account
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
        ///     Connections count of the DB owner when the statistics has got updated
        /// </summary>

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 3)]
        public int Connections { get; set; }

        /// <summary>
        ///     LinkedinGroups count of the DB owner when the statistics has got updated
        /// </summary>

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 4)]
        public int LinkedinGroups { get; set; }

        /// <summary>
        ///     Posts count of the DB owner when the statistics has got updated
        /// </summary>

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 5)]
        public int Posts { get; set; }

        /// <summary>
        ///     Likes count of the DB owner when the statistics has got updated
        /// </summary>

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 6)]
        public int Likes { get; set; }

        /// <summary>
        ///     Comments count of the DB owner when the statistics has got updated
        /// </summary>

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 7)]
        public int Comments { get; set; }
    }
}