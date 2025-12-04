#region

using SQLite;

#endregion

namespace DominatorHouseCore.DatabaseHandler.RdTables.Accounts
{
    public class OwnCommunities
    {
        [PrimaryKey]
        [AutoIncrement]
        [Indexed]
        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 1)]
        public int Id { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 2)]
        public string WhitelistStatus { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 3)]
        public bool IsNsfw { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 4)]
        public int Subscribers { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 5)]
        public string PrimaryColor { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 6)]
        //[Unique]
        public string CommunityId { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 7)]
        public bool IsQuarantined { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 8)]
        public string Name { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 9)]
        public string Title { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 10)]
        public string Url { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 11)]
        public string DisplayText { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 12)]
        public string Type { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 13)]
        public string CommunityIcon { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 14)]
        public bool IsOwn { get; set; }
    }
}