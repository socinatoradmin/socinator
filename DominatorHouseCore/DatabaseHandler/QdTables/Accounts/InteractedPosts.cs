#region

using System;
using System.ComponentModel.DataAnnotations.Schema;
using DominatorHouseCore.DatabaseHandler.Common;
using DominatorHouseCore.Enums;

#endregion

namespace DominatorHouseCore.DatabaseHandler.QdTables.Accounts
{
    public class InteractedPosts : Entity, IActivityTypeEntity
    {
        [Column(Order = 2)] public DateTime InteractionDate { get; set; }

        [Column(Order = 3)] public MediaType MediaType { get; set; }

        [Column(Order = 4)] public ActivityType ActivityType { get; set; }

        [Column(Order = 5)]
        // ReSharper disable once UnusedMember.Global
        // need to keep it to support existing data model
        public string PkOwner { get; set; }

        [Column(Order = 6)] public int TakenAt { get; set; }

        [Column(Order = 7)]
        // ReSharper disable once UnusedMember.Global
        // need to keep it to support existing data model
        public string UsernameOwner { get; set; }

        [Column(Order = 8)] public string Username { get; set; }

        [Column(Order = 9)] public string Comment { get; set; }

        [Column(Order = 10)] public string CommentId { get; set; }
        [Column(Order = 11)] public string PostUrl { get; set; }
        [Column(Order = 12)] public string QueryValue { get; set; }
        [Column(Order = 13)] public int CommentCount { get; set; }
        [Column(Order = 14)] public int ShareCount { get; set; }
        [Column(Order = 15)] public int UpvoteCount { get; set; }
        [Column(Order = 16)] public int ViewsCount { get; set; }
        [Column(Order = 17)] public string PostOwnerName { get; set; }
        [Column(Order = 18)] public int PostOwnerFollowerCount { get; set; }
        public int InteractionDateTimeStamp { get; set; }
        public string AccountName { get; set; }

        public ActivityType GetActivityType()
        {
            return ActivityType;
        }
    }
}