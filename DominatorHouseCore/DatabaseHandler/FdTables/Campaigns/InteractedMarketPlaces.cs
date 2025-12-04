#region

using System;
using SQLite;

#endregion

namespace DominatorHouseCore.DatabaseHandler.FdTables.Campaigns
{
    public class InteractedMarketPlaces
    {
        [PrimaryKey]
        [AutoIncrement]
        [Indexed]
        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 1)]
        public int Id { get; set; }


        /// <summary>
        ///     EmailId of the Account from which Interaction has been done
        /// </summary>
        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 2)]
        public string AccountEmail { get; set; }

        /// <summary>
        /// </summary>
        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 3)]
        public string QueryType { get; set; }

        /// <summary>
        ///     Contains QueryValue For Interaction
        /// </summary>
        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 4)]
        public string QueryValue { get; set; }

        /// <summary>
        /// </summary>
        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 5)]
        public string ActivityType { get; set; }

        /// <summary>
        /// </summary>
        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 6)]
        public string CommentId { get; set; }

        /// <summary>
        /// </summary>
        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 7)]
        public string CommentUrl { get; set; }

        /// <summary>
        /// </summary>
        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 8)]
        public string CommenterId { get; set; }

        /// <summary>
        /// </summary>
        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 9)]
        public string CommentText { get; set; }

        /// <summary>
        /// </summary>
        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 10)]
        // ReSharper disable once UnusedMember.Global
        // need to keep it to support existing data model
        public string CommetLikeCount { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 11)]
        // ReSharper disable once UnusedMember.Global
        // need to keep it to support existing data model
        public string CommentTimeWithDate { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 12)]
        public string HasLikedByUser { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 13)]
        public string CommentPostId { get; set; }

        /// <summary>
        /// </summary>
        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 14)]
        public int InteractionTimeStamp { get; set; }


        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 15)]
        public DateTime InteractionDateTime { get; set; }


        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 16)]
        // ReSharper disable once UnusedMember.Global
        // need to keep it to support existing data model
        public string LikeAsPageId { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 17)]
        public string Mentions { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 18)]
        // ReSharper disable once UnusedMember.Global
        // need to keep it to support existing data model
        public string ReplyForComment { get; set; }
    }
}