#region

using System.ComponentModel.DataAnnotations.Schema;
using DominatorHouseCore.DatabaseHandler.Common;

#endregion

namespace DominatorHouseCore.DatabaseHandler.TtdTables.Campaigns
{
    public class UnfollowedUsers : Entity
    {
        [Column(Order = 2)] public string AccountUsername { get; set; }

        [Column(Order = 3)] public string AccountUserId { get; set; }

        [Column(Order = 4)] public string UnfollowedUsername { get; set; }

        [Column(Order = 5)] public string UnfollowedUserId { get; set; }

        [Column(Order = 6)] public FollowType FollowType { get; set; }

        [Column(Order = 7)] public int Date { get; set; }

        [Column(Order = 8)] public string Status { get; set; }
    }
}