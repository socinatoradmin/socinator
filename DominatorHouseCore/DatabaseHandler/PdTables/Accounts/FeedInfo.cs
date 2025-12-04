#region

using SQLite;

#endregion

namespace DominatorHouseCore.DatabaseHandler.PdTables.Accounts
{
    public class FeedInfoes
    {
        [PrimaryKey]
        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 1)]
        [Indexed]
        [AutoIncrement]


        //Row ID
        public int Id { get; set; }

        //ID of the tweet
        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 2)]
        public string PinId { get; set; }

        //ID/Path of the media file
        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 3)]
        public string MediaString { get; set; }

        /// <summary>
        ///     Message/Description of the tweet
        /// </summary>

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 4)]
        public string PinDescription { get; set; }

        //Try Count Of The Tweet
        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 5)]
        public int TryCount { get; set; }


        //Comment Count Of The Tweet
        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 6)]
        public int CommentCount { get; set; }


        //Time when the Pin has been posted in TimeStamp
        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 7)]
        // ReSharper disable once UnusedMember.Global
        // need to keep it to support existing data model
        public double PinnedTimeStamp { get; set; }

        //Duration of the video Pins
        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 8)]
        public double VideoDuration { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 9)]
        public string BoardId { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 10)]
        // ReSharper disable once UnusedMember.Global
        // need to keep it to support existing data model
        public string PinWebUrl { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 11)]
        public string BoardName { get; set; }
    }
}