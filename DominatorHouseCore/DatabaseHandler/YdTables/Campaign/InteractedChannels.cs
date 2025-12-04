#region

using SQLite;

#endregion

namespace DominatorHouseCore.DatabaseHandler.YdTables.Campaign
{
    public class InteractedChannels
    {
        [PrimaryKey]
        [AutoIncrement]
        [Indexed]
        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 1)]
        public int Id { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 2)]
        public string AccountUsername { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 3)]
        public string QueryType { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 4)]
        public string QueryValue { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 5)]
        public string ActivityType { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 6)]
        public string InteractedChannelName { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 7)]
        public string InteractedChannelId { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 8)]
        public int InteractionTimeStamp { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 9)]
        public string SubscriberCount { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 10)]
        public bool IsSubscribed { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 11)]
        public string ViewsCount { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 12)]
        public string ChannelProfilePic { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 13)]
        public string ChannelLocation { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 14)]
        public string VideosCount { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 15)]
        public string ChannelDescription { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 16)]
        public string ChannelJoinedDate { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 17)]
        public string ExternalLinks { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 18)]
        public string ChannelUrl { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 19)]
        public string MyChannelId { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 20)]
        public string MyChannelPageId { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 21)]
        public string InteractedChannelUsername { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 22)]
        public string InteractedCommentUrl { get; set; }
    }
}