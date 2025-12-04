#region

using System;
using DominatorHouseCore.Enums;
using ProtoBuf;

#endregion

namespace DominatorHouseCore.Models
{
    [ProtoContract]
    public class PublisherAccountDetails
    {
        /// <summary>
        ///     Category is used to specify whether details belongs to groups / pages / boards
        /// </summary>
        [ProtoMember(1)]
        public string Category { get; set; }

        /// <summary>
        ///     Specify the group name / page name / boards
        /// </summary>
        [ProtoMember(2)]
        public string Name { get; set; }

        /// <summary>
        ///     Specify the member count
        /// </summary>
        [ProtoMember(3)]
        public int MemberCount { get; set; }

        /// <summary>
        ///     Specify the added date
        /// </summary>
        [ProtoMember(4)]
        public DateTime AddedDate { get; set; }

        /// <summary>
        ///     Specify for accessing particular category whether need admin verfication required
        /// </summary>
        [ProtoMember(5)]
        public bool IsAdminVerficationRequired { get; set; }

        /// <summary>
        ///     Account name which category belongs
        /// </summary>
        [ProtoMember(6)]
        public string Accountname { get; set; }

        /// <summary>
        ///     Account Id which category belongs
        /// </summary>
        [ProtoMember(7)]
        public string AccountId { get; set; }

        /// <summary>
        ///     Account Network which category belongs
        /// </summary>
        [ProtoMember(8)]
        public SocialNetworks AccountNetwork { get; set; }
    }
}