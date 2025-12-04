#region

using System.ComponentModel.DataAnnotations.Schema;
using DominatorHouseCore.DatabaseHandler.Common;

#endregion

namespace DominatorHouseCore.DatabaseHandler.TtdTables.Accounts
{
    public class InteractedUsers : Entity
    {
        [Column(Order = 2)] public string Query { get; set; }

        [Column(Order = 3)] public string QueryType { get; set; }

        [Column(Order = 4)] public int FollowedBack { get; set; }

        // ReSharper disable once UnusedMember.Global
        [Column(Order = 5)] public int FollowedBackDate { get; set; }

        [Column(Order = 6)] public int Date { get; set; }

        [Column(Order = 7)] public string ActivityType { get; set; }

        [Column(Order = 8)] public string AccountUsername { get; set; }

        [Column(Order = 9)] public string InteractedUsername { get; set; }

        [Column(Order = 10)] public string InteractedUserId { get; set; }

        [Column(Order = 11)] public string RequiredData { get; set; }

        [Column(Order = 12)] public int Gender { get; set; }

        [Column(Order = 13)] public string FullName { get; set; }

        [Column(Order = 14)] public string ProfilePicUrl { get; set; }

        [Column(Order = 15)] public bool IsVerified { get; set; }

        [Column(Order = 16)] public bool IsBlocked { get; set; }

        [Column(Order = 17)] public bool IsBlocking { get; set; }

        [Column(Order = 18)] public string Birthday { get; set; }

        [Column(Order = 19)] public string Signature { get; set; }

        [Column(Order = 20)] public string FollowingsCount { get; set; }

        [Column(Order = 21)] public string FollowersCount { get; set; }

        [Column(Order = 22)] public string FeedsCount { get; set; }

        [Column(Order = 23)] public string Country { get; set; }

        [Column(Order = 24)] public string Status { get; set; }
    }
}