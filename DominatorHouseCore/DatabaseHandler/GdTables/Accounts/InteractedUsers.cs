#region

using System.ComponentModel.DataAnnotations.Schema;
using DominatorHouseCore.DatabaseHandler.Common;

#endregion

namespace DominatorHouseCore.DatabaseHandler.GdTables.Accounts
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

        [Column(Order = 6)] public int Date { get; set; }

        [Column(Order = 7)] public string ActivityType { get; set; }

        [Column(Order = 8)] public string Username { get; set; }

        [Column(Order = 9)] public string InteractedUsername { get; set; }

        [Column(Order = 10)] public string DirectMessage { get; set; }

        [Column(Order = 11)] public string InteractedUserId { get; set; }


        [Column(Order = 12)] public int Time { get; set; }


        [Column(Order = 13)] public bool IsPrivate { get; set; }


        [Column(Order = 14)] public bool IsBusiness { get; set; }


        [Column(Order = 15)] public bool IsVerified { get; set; }


        [Column(Order = 16)] public bool? IsProfilePicAvailable { get; set; }


        [Column(Order = 17)] public string ProfilePicUrl { get; set; }
        [Column(Order = 18)] public string Status { get; set; }

        [Column(Order = 19)] public string RequiredData { get; set; }
        [Column(Order = 20)] public string TaggedUser { get; set; }

        [Column(Order = 21)] public string Gender { get; set; }
    }
}