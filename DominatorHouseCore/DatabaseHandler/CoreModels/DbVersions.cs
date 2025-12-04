#region

using System;
using SQLite;

#endregion

namespace DominatorHouseCore.DatabaseHandler.CoreModels
{
    public class DbVersions
    {
        [PrimaryKey]
        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 1)]
        [Indexed]
        public int Id { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 2)]
        public string Description { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 3)]
        public DateTime MIgrationDate { get; set; }
    }
}