#region

using System;
using System.ComponentModel.DataAnnotations.Schema;
using DominatorHouseCore.DatabaseHandler.Common;
using DominatorHouseCore.Enums;

#endregion

namespace DominatorHouseCore.DatabaseHandler.PdTables.Accounts
{
    public class InteractedPosts : Entity, IActivityTypeEntity
    {
        //ID of the Pin
        [Column(Order = 2)] public string PinId { get; set; }

        //ID/Path of the media file
        [Column(Order = 3)] public string MediaString { get; set; }

        /// <summary>
        ///     Message/Description of the Pin
        /// </summary>

        [Column(Order = 4)]
        public string PinDescription { get; set; }

        //Like Count Of The Pin
        [Column(Order = 5)] public int TryCount { get; set; }

        //Comment Count Of The Pin
        [Column(Order = 6)] public int CommentCount { get; set; }


        //Time when the Pin has been posted in TimeStamp
        [Column(Order = 7)]
        // ReSharper disable once UnusedMember.Global
        // need to keep it to support existing data model
        public double PinnedTimeStamp { get; set; }

        //Duration of the video Pins
        [Column(Order = 8)] public double VideoDuration { get; set; }

        //ID of the Board
        [Column(Order = 9)] public string SourceBoard { get; set; }

        //Web url of the Pin
        [Column(Order = 10)] public string PinWebUrl { get; set; }

        // Board Name in which the Pin belongs to
        [Column(Order = 11)] public string SourceBoardName { get; set; }

        [Column(Order = 12)] public int InteractionDate { get; set; }

        //Type of the Media(Image/Video)
        [Column(Order = 13)] public MediaType MediaType { get; set; }

        //Type of Operation performed(follow/comment...etc)
        [Column(Order = 14)] public string OperationType { get; set; }

        //User id of the User in which the Pin belongs to
        [Column(Order = 15)] public string UserId { get; set; }

        //Username of the User in which the Pin belongs to
        [Column(Order = 16)] public string Username { get; set; }

        [Column(Order = 17)] public string Query { get; set; }

        [Column(Order = 18)] public string QueryType { get; set; }

        [Column(Order = 19)] public string CommentId { get; set; }
        [Column(Order = 20)] public string BoardLabel { get; set; }
        [Column(Order = 21)] public string DestinationBoard { get; set; }
        [Column(Order = 22)] public string Comment { get; set; }
        [Column(Order = 23)] public string PublishedDate { get; set; }
        [Column(Order = 24)] public string PinTitle { get; set; }
        [Column(Order = 25)] public string GeneratedPinId { get; set; }

        public ActivityType GetActivityType()
        {
            return (ActivityType) Enum.Parse(typeof(ActivityType), OperationType);
        }
    }
}