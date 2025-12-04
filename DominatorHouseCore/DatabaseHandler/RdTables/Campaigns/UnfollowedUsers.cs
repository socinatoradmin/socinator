#region

using SQLite;

#endregion

namespace DominatorHouseCore.DatabaseHandler.RdTables.Campaigns
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
        // need to keep it to support existing data model
        // ReSharper disable once UnusedMember.Global
        public int FollowedBackDate { get; set; }


        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 6)]
        public int InteractionDate { get; set; }


        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 7)]
        public int OperationType { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 8)]
        public string Username { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 9)]
        public string UserId { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 10)]
        public string FullName { get; set; }

        public string SinAccUsername { get; set; }
    }
}