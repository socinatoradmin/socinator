using DominatorHouseCore.Interfaces;
using System;

namespace FaceDominatorCore.FDLibrary.FdClassLibrary
{
    public class GroupDetails : IGroup
    {
        public string GroupId { get; set; }

        public string GroupName { get; set; }

        public string GroupUrl { get; set; }

        public string GroupMemberCount { get; set; }

        public string GroupDescription { get; set; }

        public string GroupJoinStatus { get; set; }

        public string GroupType { get; set; }

        public DateTime JoinDate { get; set; }


    }
}
