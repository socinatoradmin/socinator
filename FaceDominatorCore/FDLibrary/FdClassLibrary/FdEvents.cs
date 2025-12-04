using DominatorHouseCore.Interfaces;
using System;

namespace FaceDominatorCore.FDLibrary.FdClassLibrary
{
    public class FdEvents : IEvent
    {
        public string Id { get; set; }
        public string EventId { get; set; }

        public string EventName { get; set; }

        public string OwnerId { get; set; }

        public string Note { get; set; }

        public bool IsInvitedInMessanger { get; set; }

        public string EventType { get; set; }

        public string MediaPath { get; set; }

        public string EventLocation { get; set; }

        public string EventDescription { get; set; }
        public DateTime EventStartDate { get; set; }
        public DateTime EventEndDate { get; set; }
        public bool? IsGuestCanInviteFriends { get; set; }
        public bool? IsShowGuestList { get; set; }
        public bool? IsAnyOneCanPostForAllPost { get; set; }
        public bool? IsPostMustApproved { get; set; }
        public bool? IsQuesOnMessanger { get; set; }
    }
}
