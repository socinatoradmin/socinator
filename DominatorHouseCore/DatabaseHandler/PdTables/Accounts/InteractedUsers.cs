#region

using System.ComponentModel.DataAnnotations.Schema;
using DominatorHouseCore.DatabaseHandler.Common;

#endregion

namespace DominatorHouseCore.DatabaseHandler.PdTables.Accounts
{
    public class InteractedUsers : Entity
    {
        [Column(Order = 2)] public string Query { get; set; }

        [Column(Order = 3)] public string QueryType { get; set; }

        [Column(Order = 4)] public int FollowedBack { get; set; }

        [Column(Order = 5)]
        // ReSharper disable once UnusedMember.Global
        // need to keep it to support existing data model
        public int FollowedBackDate { get; set; }

        [Column(Order = 6)] public int InteractionTime { get; set; }

        [Column(Order = 7)] public string ActivityType { get; set; }

        [Column(Order = 8)] public string Username { get; set; }

        [Column(Order = 9)] public string InteractedUsername { get; set; }

        [Column(Order = 10)] public int Date { get; set; }

        [Column(Order = 11)] public string InteractedUserId { get; set; }


        [Column(Order = 12)] public int UpdatedTime { get; set; }

        [Column(Order = 13)] public int FollowersCount { get; set; }


        [Column(Order = 14)] public int FollowingsCount { get; set; }

        [Column(Order = 15)] public int PinsCount { get; set; }

        [Column(Order = 16)] public int TriesCount { get; set; }

        [Column(Order = 17)] public string FullName { get; set; }

        [Column(Order = 18)] public bool? HasAnonymousProfilePicture { get; set; }


        [Column(Order = 19)] public bool IsVerified { get; set; }

        [Column(Order = 20)] public string ProfilePicUrl { get; set; }

        [Column(Order = 21)] public string Website { get; set; }

        [Column(Order = 22)] public string Bio { get; set; }

        [Column(Order = 23)] public string BoardDescription { get; set; }

        [Column(Order = 24)] public string BoardUrl { get; set; }

        [Column(Order = 25)] public string BoardName { get; set; }

        [Column(Order = 26)] public string Type { get; set; }

        [Column(Order = 27)] public string DirectMessage { get; set; }
        [Column(Order = 28)] public bool? Filtered { get; set; }

        [Column(Order = 29)] public bool? FullDetailsScraped { get; set; }

        [Column(Order = 30)] public bool IsFollowedByMe { get; set; }
    }
}