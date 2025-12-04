using SQLite;
using System;

namespace DominatorHouseCore.DatabaseHandler.GdTables.Campaigns
{
    public class MakeCloseFriendCampaign
    {
        [PrimaryKey]
        [AutoIncrement]
        [Indexed]
        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 1)]
        public int Id { get; set; }
        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 2)]
        public string AccountUserName { get; set; }
        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 3)]
        public string ActivityType { get; set; }
        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 4)]
        public string UserName { get; set; }
        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 5)]
        public bool IsCloseFriend { get; set; }
        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 6)]
        public DateTime InteractedDate { get; set; }
    }
}
