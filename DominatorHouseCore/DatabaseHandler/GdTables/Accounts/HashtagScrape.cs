#region

using DominatorHouseCore.Enums;
using SQLite;

#endregion

namespace DominatorHouseCore.DatabaseHandler.GdTables.Accounts
{
    public class HashtagScrape
    {
        [PrimaryKey]
        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 1)]
        [Indexed]
        [AutoIncrement]
        public int Id { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 2)]
        public string AccountUsername { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 3)]
        public ActivityType ActivityType { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 4)]
        public string Keyword { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 5)]
        public string HashtagName { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 6)]
        public string HashtagId { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 7)]
        public string MediaCount { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 8)]
        public int Date { get; set; }
    }
}