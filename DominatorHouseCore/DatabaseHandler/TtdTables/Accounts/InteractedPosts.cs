#region

using System;
using System.ComponentModel.DataAnnotations.Schema;
using DominatorHouseCore.DatabaseHandler.Common;
using DominatorHouseCore.Enums;

#endregion

namespace DominatorHouseCore.DatabaseHandler.TtdTables.Accounts
{
    public class InteractedPosts : Entity, IActivityTypeEntity
    {
        /// <summary>
        ///     Contains QueryType For Interaction
        /// </summary>
        [Column(Order = 2)]
        public string QueryType { get; set; }

        /// <summary>
        ///     Contains QueryValue For Interaction
        /// </summary>
        [Column(Order = 3)]
        public string QueryValue { get; set; }

        /// <summary>
        ///     Describes Activity
        /// </summary>
        [Column(Order = 4)]
        public string ActivityType { get; set; }

        [Column(Order = 5)] public string AwemeId { get; set; }

        [Column(Order = 6)] public string Description { get; set; }

        [Column(Order = 7)] public string CreateTime { get; set; }

        [Column(Order = 8)] public string Username { get; set; }

        [Column(Order = 9)] public string UserId { get; set; }

        [Column(Order = 10)] public string VideoUrl { get; set; }

        [Column(Order = 11)] public string Duration { get; set; }

        [Column(Order = 12)] public string CommentCount { get; set; }

        // ReSharper disable once UnusedMember.Global
        [Column(Order = 13)] public string DiggCount { get; set; }

        [Column(Order = 14)] public string DownloadCount { get; set; }

        [Column(Order = 15)] public string PlayCount { get; set; }

        [Column(Order = 16)] public string ShareCount { get; set; }

        [Column(Order = 17)] public int InteractionDate { get; set; }

        [Column(Order = 18)] public string Status { get; set; }

        [Column(Order = 19)] public string Comment { get; set; }

        [Column(Order = 20)] public string AccountUsername { get; set; }

        public ActivityType GetActivityType()
        {
            return (ActivityType) Enum.Parse(typeof(ActivityType), ActivityType);
        }
    }
}