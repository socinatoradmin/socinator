#region

using System;
using SQLite;

#endregion

namespace DominatorHouseCore.DatabaseHandler.TumblrTables.Account
{
    public class OwnBlogs
    {
        [PrimaryKey]
        [AutoIncrement]
        [Indexed]
        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 1)]
        public int Id { get; set; }


        /// <summary>
        /// </summary>
        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 2)]
        //[Unique]
        public string BlogKey { get; set; }


        /// <summary>
        /// </summary>
        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 3)]
        public string BlogUrl { get; set; }

        /// <summary>
        /// </summary>
        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 4)]
        public string BlogName { get; set; }

        /// <summary>
        /// </summary>
        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 5)]
        public int Postcount { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 6)]
        public DateTime InteractionDate { get; set; }
    }
}