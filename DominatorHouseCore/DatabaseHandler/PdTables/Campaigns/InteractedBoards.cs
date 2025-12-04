#region

using DominatorHouseCore.Enums;
using SQLite;

#endregion

namespace DominatorHouseCore.DatabaseHandler.PdTables.Campaigns
{
    public class InteractedBoards
    {
        [PrimaryKey]
        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 1)]
        [Indexed]
        [AutoIncrement]
        public int Id { get; set; }

        //ID of the Board
        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 2)]
        public string BoardId { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 3)]
        public string BoardName { get; set; }

        //Description of the Board
        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 4)]
        public string BoardDescription { get; set; }

        //Pin Count Of The Board
        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 5)]
        public int PinCount { get; set; }

        //Follower Count Of The Board
        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 6)]
        public int FollowerCount { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 7)]
        public string Username { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 8)]
        public string UserId { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 9)]
        public string Query { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 10)]
        public string QueryType { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 11)]
        public int InteractionDate { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 12)]
        public ActivityType OperationType { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 13)]
        public string SinAccId { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 14)]
        public string SinAccUsername { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 15)]
        public string Category { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 16)]
        public string BoardUrl { get; set; }
        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 17)]
        public string BoardSection { get; set; }
    }
}