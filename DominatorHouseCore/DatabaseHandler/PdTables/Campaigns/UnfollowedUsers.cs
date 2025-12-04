#region

using DominatorHouseCore.Enums;
using SQLite;

#endregion

namespace DominatorHouseCore.DatabaseHandler.PdTables.Campaigns
{
    public class UnfollowedUsers
    {
        [PrimaryKey]
        [AutoIncrement]
        [Indexed]
        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 1)]
        public int Id { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 2)]
        public string FilterArgument { get; set; }


        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 3)]
        // ReSharper disable once UnusedMember.Global
        // need to keep it to support existing data model
        public int FilterTypeSql { get; set; }


        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 4)]
        public int FollowedBack { get; set; }


        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 5)]
        // ReSharper disable once UnusedMember.Global
        // need to keep it to support existing data model
        public int FollowedBackDate { get; set; }


        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 6)]
        public int InteractionDate { get; set; }


        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 7)]
        public ActivityType OperationType { get; set; }


        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 8)]
        public string Username { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 9)]
        public string UserId { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 10)]
        public string FullName { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 11)]
        public string SinAccId { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 12)]
        public string SinAccUsername { get; set; }
    }
}