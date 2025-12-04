#region

using SQLite;

#endregion

namespace DominatorHouseCore.DatabaseHandler.TtdTables.Campaigns
{
    public class InteractedHashTags
    {
        [PrimaryKey]
        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 1)]
        [Indexed]
        [AutoIncrement]
        public int Id { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 2)]
        public string AccountUsername { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 3)]
        public string ActivityType { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 4)]
        public string Keyword { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 5)]
        public string HashTagId { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 6)]
        public string HashTagName { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 7)]
        public string Description { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 8)]
        public string UserCount { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 9)]
        public string ViewCount { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 10)]
        public string OwnerUsername { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 11)]
        public string OwnerUserId { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 12)]
        public int Date { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 13)]
        public string Status { get; set; }
    }
}