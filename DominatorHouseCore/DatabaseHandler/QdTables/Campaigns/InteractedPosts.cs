#region

using System;
using DominatorHouseCore.Enums;
using SQLite;

#endregion

namespace DominatorHouseCore.DatabaseHandler.QdTables.Campaigns
{
    public class InteractedPosts
    {
        [PrimaryKey]
        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 1)]
        [Indexed]
        [AutoIncrement]
        public int Id { get; set; }

        /// <summary>
        ///     UserName of the Account from which Interaction is done
        /// </summary>

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 2)]
        public string SinAccUsername { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 3)]
        public string QueryType { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 4)]
        public string QueryValue { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 5)]
        public string ActivityType { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 6)]
        public int InteractionTimeStamp { get; set; }

        /// <summary>
        ///     Id of the quora
        /// </summary>

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 7)]
        // ReSharper disable once UnusedMember.Global
        // need to keep it to support existing data model
        public string QuoraId { get; set; }

        /// <summary>
        ///     UserId of the quora Owner
        /// </summary>

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 8)]
        public string UserId { get; set; }

        /// <summary>
        ///     UserName of the quora owner
        /// </summary>

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 9)]
        public string Username { get; set; }

        /// <summary>
        ///     Image/Video url of quora
        /// </summary>

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 10)]
        public string MediaId { get; set; }

        /// <summary>
        ///     Message/Description of the quora
        /// </summary>

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 11)]
        public string Message { get; set; }

        /// <summary>
        ///     Like Count Of The quora
        /// </summary>

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 12)]
        public int LikeCount { get; set; }

        /// <summary>
        ///     Comment Count Of The quora
        /// </summary>

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 13)]
        public int CommentCount { get; set; }

        /// <summary>
        ///     Image or Video or Text
        /// </summary>

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 14)]
        public MediaType MediaType { get; set; }

        /// <summary>
        ///     If Interaction Type is Comment Interaction
        /// </summary>

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 15)]
        public string CommentedText { get; set; }

        /// <summary>
        ///     1(true) if following quora Owner
        /// </summary>

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 16)]
        public int FollowStatus { get; set; }

        /// <summary>
        ///     1(true) quora Owner follows back
        /// </summary>

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 17)]
        public int FollowBackStatus { get; set; }

        /// <summary>
        ///     Describes wheather the activity is done in Activity process or after activity process
        /// </summary>
        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 18)]
        public string ProcessType { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 19)]
        public DateTime InteractionDate { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 20)]
        public string CommentId { get; set; }
        public DateTime PostCreationTime { get; set; }
        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 21)] public string PostUrl { get; set; }
        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 22)] public int ShareCount { get; set; }
        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 23)] public int ViewsCount { get; set; }
        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 24)] public string PostOwnerName { get; set; }
        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 25)] public int PostOwnerFollowerCount { get; set; }
    }
}