#region

using System;
using SQLite;

#endregion

namespace DominatorHouseCore.DatabaseHandler.DHTables
{
    public class BlackListUser
    {
        [PrimaryKey]
        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 1)]
        [Indexed]
        [AutoIncrement]
        public int Id { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 2)]
        public string UserId { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 3)]
        public string UserName { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 4)]
        public DateTime AddedDateTime { get; set; }
    }
}