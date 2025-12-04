#region

using System;
using SQLite;

#endregion

namespace DominatorHouseCore.DatabaseHandler.FdTables.Accounts
{
    public class OwnGroups
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
        public string GroupId { get; set; }


        /// <summary>
        /// </summary>
        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 3)]
        public string GroupUrl { get; set; }

        /// <summary>
        /// </summary>
        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 4)]
        public string GroupName { get; set; }

        /// <summary>
        /// </summary>
        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 5)]
        public string GroupType { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 6)]
        public DateTime InteractionDate { get; set; }
    }
}