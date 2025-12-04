#region

using System.ComponentModel.DataAnnotations.Schema;
using DominatorHouseCore.DatabaseHandler.Common;
using DominatorHouseCore.Enums;

#endregion

namespace DominatorHouseCore.DatabaseHandler.GdTables.Accounts
{
    public class InteractedPosts : Entity, IActivityTypeEntity
    {
        [Column(Order = 2)] public int InteractionDate { get; set; }

        [Column(Order = 3)] public MediaType MediaType { get; set; }

        [Column(Order = 4)] public ActivityType ActivityType { get; set; }

        [Column(Order = 5)] public string PkOwner { get; set; }

        [Column(Order = 6)] public int TakenAt { get; set; }

        [Column(Order = 7)] public string UsernameOwner { get; set; }


        [Column(Order = 8)] public string Username { get; set; }

        [Column(Order = 9)] public string Comment { get; set; }

        [Column(Order = 10)] public string OriginalMediaCode { get; set; }

        [Column(Order = 11)] public string OriginalMediaOwner { get; set; }
        [Column(Order = 12)] public string QueryType { get; set; }

        [Column(Order = 13)] public string QueryValue { get; set; }

        [Column(Order = 14)] public string CommentId { get; set; }

        [Column(Order = 15)] public string Status { get; set; }
        [Column(Order = 16)] public string TotalLike { get; set; }

        [Column(Order = 17)] public string TotalComment { get; set; }

        [Column(Order = 18)] public string PostLocation { get; set; }
        [Column(Order = 19)] public string CommentOwnerName { get; set; }

        [Column(Order = 20)] public string CommentOwnerId { get; set; }
        [Column(Order = 21)] public string PostUrl { get; set; }

        public ActivityType GetActivityType()
        {
            return ActivityType;
        }
    }
}