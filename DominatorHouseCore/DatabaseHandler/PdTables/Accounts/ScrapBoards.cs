#region

using SQLite;

#endregion

namespace DominatorHouseCore.DatabaseHandler.PdTables.Accounts
{
    public class ScrapBoards
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
        public int InteractionTime { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 12)]
        public string OperationType { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 13)]
        public string BoardUrl { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 14)]
        public string PinterestUserSender { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 15)]
        // ReSharper disable once UnusedMember.Global
        // need to keep it to support existing data model
        public string PinterestUserRecipient { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 16)]
        public bool IsFollowed { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 17)]
        // ReSharper disable once UnusedMember.Global
        // need to keep it to support existing data model
        public string BoardCreatedAt { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 18)]
        public string CreatedAt { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 19)]
        // ReSharper disable once UnusedMember.Global
        // need to keep it to support existing data model
        public string BoardOrderModifiedAt { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 20)]
        // ReSharper disable once UnusedMember.Global
        // need to keep it to support existing data model
        public string CollaboratedByMe { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 21)]
        public bool? IsCollaborative { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 22)]
        // ReSharper disable once UnusedMember.Global
        // need to keep it to support existing data model
        public string FollowedByMe { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 23)]
        // ReSharper disable once UnusedMember.Global
        // need to keep it to support existing data model
        public string ImageThumbnailUrl { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 24)]
        // ReSharper disable once UnusedMember.Global
        // need to keep it to support existing data model
        public string ContactRequestId { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 25)]
        public bool? Filtered { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 26)]
        public bool? FullDetailsScraped { get; set; }
    }
}