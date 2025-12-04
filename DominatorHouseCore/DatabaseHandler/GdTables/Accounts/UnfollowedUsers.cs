#region

using System.ComponentModel.DataAnnotations.Schema;
using DominatorHouseCore.DatabaseHandler.Common;

#endregion

namespace DominatorHouseCore.DatabaseHandler.GdTables.Accounts
{
    public class UnfollowedUsers : Entity
    {
        [Column(Order = 2)] public string AccountUsername { get; set; }

        [Column(Order = 3)] public string FilterArgument { get; set; }


        [Column(Order = 4)]
        // ReSharper disable once UnusedMember.Global
        // need to keep it to support existing data model
        public int FilterTypeSql { get; set; }


        [Column(Order = 5)] public int FollowedBack { get; set; }


        [Column(Order = 6)]
        // ReSharper disable once UnusedMember.Global
        // need to keep it to support existing data model
        public int FollowedBackDate { get; set; }


        [Column(Order = 7)] public int InteractionDate { get; set; }


        [Column(Order = 8)] public FollowType FollowType { get; set; }


        [Column(Order = 9)] public string UnfollowedUsername { get; set; }
    }
}