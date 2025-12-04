#region

using System;
using System.ComponentModel.DataAnnotations.Schema;
using DominatorHouseCore.DatabaseHandler.Common;
using DominatorHouseCore.Enums;

#endregion

namespace DominatorHouseCore.DatabaseHandler.RdTables.Accounts
{
    public class InteractedPost : Entity, IActivityTypeEntity
    {
        [Column(Order = 2)] public string InteracteduserId { get; set; }

        [Column(Order = 3)] public string Query { get; set; }

        [Column(Order = 4)] public string QueryType { get; set; }

        [Column(Order = 5)] public string CommentsCount { get; set; }

        [Column(Order = 6)] public string ActivityType { get; set; }

        [Column(Order = 7)] public string Caption { get; set; }
        [Column(Order = 8)] public bool IsCrosspostable { get; set; }
        [Column(Order = 9)] public bool IsStickied { get; set; }

        [Column(Order = 10)] public bool Saved { get; set; }

        [Column(Order = 11)] public int NumComments { get; set; }

        //public object upvoteRatio { get; set; }
        [Column(Order = 12)] public bool IsPinned { get; set; }

        [Column(Order = 13)] public string InteractedUserName { get; set; }

        //public Media media { get; set; }
        [Column(Order = 14)] public int NumCrossposts { get; set; }
        [Column(Order = 15)] public bool IsSponsored { get; set; }

        [Column(Order = 16)] public bool IsLocked { get; set; }
        [Column(Order = 17)] public int Score { get; set; }
        [Column(Order = 18)] public bool IsArchived { get; set; }
        [Column(Order = 19)] public bool Hidden { get; set; }
        [Column(Order = 20)] public string Preview { get; set; }

        [Column(Order = 21)] public bool IsRoadblock { get; set; }

        [Column(Order = 22)] public bool SendReplies { get; set; }
        [Column(Order = 23)] public int GoldCount { get; set; }
        [Column(Order = 24)] public bool IsSpoiler { get; set; }
        [Column(Order = 25)] public int VoteState { get; set; }
        [Column(Order = 26)] public bool IsNsfw { get; set; }
        [Column(Order = 27)] public bool IsMediaOnly { get; set; }
        [Column(Order = 28)] public string PostId { get; set; }
        [Column(Order = 29)] public bool IsBlank { get; set; }
        [Column(Order = 30)] public int ViewCount { get; set; }
        [Column(Order = 31)] public string Permalink { get; set; }
        [Column(Order = 32)] public long Created { get; set; }
        [Column(Order = 33)] public string Title { get; set; }
        [Column(Order = 34)] public bool IsOriginalContent { get; set; }

        [Column(Order = 35)] public int InteractionTimeStamp { get; set; }

        [Column(Order = 36)] public DateTime InteractionDateTime { get; set; }

        [Column(Order = 37)] public string CommentText { get; set; }

        [Column(Order = 38)] public string CommentId { get; set; }
        [Column(Order = 39)] public string OldComment { get; set; }

        public ActivityType GetActivityType()
        {
            return (ActivityType) Enum.Parse(typeof(ActivityType), ActivityType);
        }
    }
}