#region

using System;
using System.ComponentModel.DataAnnotations.Schema;
using DominatorHouseCore.DatabaseHandler.Common;
using DominatorHouseCore.Enums;

#endregion

namespace DominatorHouseCore.DatabaseHandler.GplusTables.Accounts
{
    public class InteractedPosts : Entity, IActivityTypeEntity
    {
        [Column(Order = 2)] public string Query { get; set; }

        [Column(Order = 3)] public string QueryType { get; set; }

        [Column(Order = 4)] public string ActivityType { get; set; }

        [Column(Order = 5)] public int InteractionDate { get; set; }

        [Column(Order = 6)] public MediaType MediaType { get; set; }

        [Column(Order = 7)] public string PostId { get; set; }

        [Column(Order = 8)] public int UploadedDate { get; set; }

        // ReSharper disable once UnusedMember.Global
        [Column(Order = 9)] public string PostOwnerId { get; set; }

        [Column(Order = 10)] public string PostOwnerName { get; set; }


        [Column(Order = 11)] public string Caption { get; set; }

        [Column(Order = 12)] public int LikeCount { get; set; }

        [Column(Order = 13)] public int CommentCount { get; set; }

        [Column(Order = 14)] public int ShareCount { get; set; }

        [Column(Order = 15)] public string PostUrl { get; set; }

        [Column(Order = 16)] public string Comment { get; set; }

        [Column(Order = 17)] public int IsLiked { get; set; }

        [Column(Order = 18)] public string CommentId { get; set; }


        public ActivityType GetActivityType()
        {
            return (ActivityType) Enum.Parse(typeof(ActivityType), ActivityType);
        }
    }
}