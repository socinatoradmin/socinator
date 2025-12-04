#region

using System.ComponentModel.DataAnnotations.Schema;
using DominatorHouseCore.DatabaseHandler.Common;
using DominatorHouseCore.Enums;

#endregion

namespace DominatorHouseCore.DatabaseHandler.PdTables.Accounts
{
    public class UnfollowedUsers : Entity
    {
        [Column(Order = 2)] public string FilterArgument { get; set; }


        [Column(Order = 3)]
        // ReSharper disable once UnusedMember.Global
        // need to keep it to support existing data model
        public int FilterTypeSql { get; set; }


        [Column(Order = 4)] public int FollowedBack { get; set; }


        [Column(Order = 5)]
        // ReSharper disable once UnusedMember.Global
        // need to keep it to support existing data model
        public int FollowedBackDate { get; set; }


        [Column(Order = 6)] public int InteractionDate { get; set; }


        [Column(Order = 7)] public ActivityType OperationType { get; set; }


        [Column(Order = 8)] public string Username { get; set; }

        [Column(Order = 9)] public string UserId { get; set; }

        [Column(Order = 10)] public string FullName { get; set; }
    }
}