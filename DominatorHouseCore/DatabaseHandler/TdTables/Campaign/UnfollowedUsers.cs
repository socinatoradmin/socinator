#region

using System;
using System.ComponentModel.DataAnnotations.Schema;
using DominatorHouseCore.DatabaseHandler.Common;

#endregion

namespace DominatorHouseCore.DatabaseHandler.TdTables.Campaign
{
    public class UnfollowedUsers : Entity
    {
        [Column(Order = 2)] public string SinAccUsername { get; set; }


        [Column(Order = 3)] public string UnfollowSource { get; set; }


        [Column(Order = 4)] public string SourceType { get; set; }

        [Column(Order = 5)] public int SourceFilter { get; set; }


        [Column(Order = 6)] public string Username { get; set; }

        [Column(Order = 7)] public string UserId { get; set; }

        [Column(Order = 8)] public int FollowBackStatus { get; set; }


        [Column(Order = 9)]
        // need to keep it to support existing data model
        // ReSharper disable once UnusedMember.Global
        public int FollowedBackDate { get; set; }

        [Column(Order = 10)] public int InteractionTimeStamp { get; set; }

        [Column(Order = 11)] public DateTime InteractionDate { get; set; }

        [Column(Order = 12)] public string ProcessType { get; set; }
    }
}