#region

using DominatorHouseCore.Enums;
using SQLite;

#endregion

namespace DominatorHouseCore.DatabaseHandler.PdTables.Campaigns
{
    public class InteractedPosts
    {
        [PrimaryKey]
        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 1)]
        [Indexed]
        [AutoIncrement]
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

        //Like Count Of The Tweet
        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 5)]
        public int TryCount { get; set; }

        //Comment Count Of The Tweet
        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 6)]
        public int CommentCount { get; set; }


        //Time when the tweet has been posted in TimeStamp
        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 7)]
        // ReSharper disable once UnusedMember.Global
        // need to keep it to support existing data model
        public double PinnedTimeStamp { get; set; }

        //Duration of the video tweets
        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 8)]
        public double VideoDuration { get; set; }

        //View Count of the video tweets
        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 9)]
        public int ViewCount { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 10)]
        public string PinWebUrl { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 11)]
        public string SourceBoardName { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 12)]
        public int InteractionDate { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 13)]
        public MediaType MediaType { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 14)]
        public string OperationType { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 15)]
        public string UserId { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 16)]
        public string Username { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 17)]
        public string Query { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 18)]
        public string QueryType { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 19)]
        public string SourceBoard { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 20)]
        public string SinAccId { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 21)]
        public string SinAccUsername { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 22)]
        public string CommentId { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 23)]
        public string BoardLabel { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 24)]
        public string DestinationBoard { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 25)]
        public string Comment { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 26)]
        public string PublishedDate { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 27)]
        public string PinTitle { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 28)]
        public string GeneratedPinId { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 29)]
        public string Status { get; set; }
    }
}