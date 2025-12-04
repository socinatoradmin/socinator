#region

using SQLite;

#endregion

namespace DominatorHouseCore.DatabaseHandler.YdTables.Accounts
{
    public class OwnChannels
    {
        [PrimaryKey]
        [AutoIncrement]
        [Indexed]
        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 1)]
        public int Id { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 2)]
        public string ChannelName { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 3)]
        public string SubscribersCount { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 4)]
        public string VideosCount { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 5)]
        public string PageId { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 6)]
        public bool IsSelected { get; set; }
    }
}