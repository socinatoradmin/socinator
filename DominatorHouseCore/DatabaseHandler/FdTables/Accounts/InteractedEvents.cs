#region

using System;
using SQLite;

#endregion

namespace DominatorHouseCore.DatabaseHandler.FdTables.Accounts
{
    public class InteractedEvents
    {
        [PrimaryKey]
        [AutoIncrement]
        [Indexed]
        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 1)]
        public int Id { get; set; }


        /// <summary>
        ///     Contains QueryType For Interaction
        /// </summary>
        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 2)]
        public string EventGUid { get; set; }

        /// <summary>
        ///     Contains QueryType For Interaction
        /// </summary>
        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 3)]
        public string QueryType { get; set; }

        /// <summary>
        ///     Contains QueryValue For Interaction
        /// </summary>
        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 4)]
        public string QueryValue { get; set; }

        /// <summary>
        ///     Describes Activity
        /// </summary>
        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 5)]
        public string ActivityType { get; set; }

        /// <summary>
        ///     Contains Name of the Event being interacted
        /// </summary>
        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 6)]
        public string EventName { get; set; }

        /// <summary>
        ///     Contains Url of the Event being interacted
        /// </summary>
        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 7)]
        public string EventUrl { get; set; }

        /// <summary>
        ///     Contains EventId in the Event being interacted
        /// </summary>
        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 8)]
        public string EventId { get; set; }

        /// <summary>
        ///     Describes EventType of the Event being interacted
        /// </summary>
        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 9)]
        public string EventType { get; set; }

        /// <summary>
        ///     Describes EventDescripion of Event
        /// </summary>
        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 10)]
        public string EventDescrption { get; set; }

        /// <summary>
        ///     Describes EventStartDate of Event
        /// </summary>
        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 11)]
        public DateTime EventStartDate { get; set; }

        /// <summary>
        ///     Describes Event Going to be end on of Event
        /// </summary>
        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 12)]
        public DateTime EventEndDate { get; set; }

        /// <summary>
        ///     Describes Event Location
        /// </summary>
        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 13)]
        public string EventLocation { get; set; }

        /// <summary>
        ///     Describes IsGuestCanInviteFriends return bool
        /// </summary>
        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 14)]
        public bool? IsGuestCanInviteFriends { get; set; }

        /// <summary>
        ///     Describes IsReportedPost return bool
        /// </summary>
        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 15)]
        public bool? IsReportedPost { get; set; }

        /// <summary>
        ///     Describes ShowGuestList return bool
        /// </summary>
        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 16)]
        public bool? IsShowGuestList { get; set; }

        /// <summary>
        ///     Describes AnyOneCanPostForAllPost return bool
        /// </summary>
        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 17)]
        public bool? IsAnyOneCanPostForAllPost { get; set; }

        /// <summary>
        ///     Describes IsPostMustApproved return bool
        /// </summary>
        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 18)]
        public bool? IsPostMustApproved { get; set; }

        /// <summary>
        ///     Describes IsQuesOnMessanger return bool
        /// </summary>
        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 19)]
        public bool? IsQuesOnMessanger { get; set; }

        /// <summary>
        ///     TimeStamp when interacted with the Event
        /// </summary>
        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 20)]
        public int InteractionTimeStamp { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 21)]
        public DateTime InteractionDateTime { get; set; }
    }
}