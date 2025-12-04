#region

using System;
using System.ComponentModel.DataAnnotations.Schema;
using DominatorHouseCore.DatabaseHandler.Common;
using DominatorHouseCore.Enums;

#endregion

namespace DominatorHouseCore.DatabaseHandler.RdTables.Accounts
{
    public class InteractedUsers : Entity, IActivityTypeEntity
    {
        [Column(Order = 2)] public string QueryType { get; set; }
        [Column(Order = 3)] public string QueryValue { get; set; }

        [Column(Order = 4)] public string ActivityType { get; set; }

        [Column(Order = 5)] public int InteractionTimeStamp { get; set; }

        [Column(Order = 6)] public string InteractedUsername { get; set; }

        [Column(Order = 7)] public int Date { get; set; }

        [Column(Order = 8)] public string InteractedUserId { get; set; }

        [Column(Order = 9)] public int UpdatedTime { get; set; }
        [Column(Order = 10)] public string AccountIcon { get; set; }
        [Column(Order = 11)] public int CommentKarma { get; set; }
        [Column(Order = 12)] public DateTime Created { get; set; }
        [Column(Order = 13)] public string DisplayName { get; set; }
        [Column(Order = 14)] public string DisplayNamePrefixed { get; set; }
        [Column(Order = 15)] public string DisplayText { get; set; }
        [Column(Order = 16)] public bool HasUserProfile { get; set; }
        [Column(Order = 17)] public bool IsEmployee { get; set; }
        [Column(Order = 18)] public bool IsFollowing { get; set; }
        [Column(Order = 19)] public bool IsGold { get; set; }
        [Column(Order = 20)] public bool IsMod { get; set; }
        [Column(Order = 21)] public bool IsNsfw { get; set; }
        [Column(Order = 22)] public bool PrefShowSnoovatar { get; set; }
        [Column(Order = 23)] public int PostKarma { get; set; }
        [Column(Order = 24)] public string Url { get; set; }

        [Column(Order = 25)] public DateTime InteractionDateTime { get; set; }

        [Column(Order = 26)] public string Message { get; set; }

        public ActivityType GetActivityType()
        {
            return (ActivityType)Enum.Parse(typeof(ActivityType), ActivityType);
        }
    }
}