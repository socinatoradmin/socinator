#region

using SQLite;

#endregion

namespace DominatorHouseCore.DatabaseHandler.LdTables.Account
{
    public class Groups
    {
        [PrimaryKey]
        [AutoIncrement]
        [Indexed]
        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 1)]
        public int Id { get; set; }

        /// <summary>
        ///     Contains Name Of the Group
        /// </summary>
        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 2)]
        public string GroupName { get; set; }

        /// <summary>
        ///     Contains Url Of the Group
        /// </summary>
        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 3)]
        //[Unique]
        public string GroupUrl { get; set; }

        /// <summary>
        ///     Contains Profile Picture Url Of the Connection
        /// </summary>
        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 4)]
        public string TotalMembers { get; set; }

        /// <summary>
        ///     Describe Connection Type If FirstDegree,SecondDegree Or ThirdPlusDegree
        /// </summary>
        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 5)]
        public string CommunityType { get; set; }

        /// <summary>
        ///     Contains Connected TimeStamp with this Account
        /// </summary>
        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 6)]
        public string MembershipStatus { get; set; }

        /// <summary>
        ///     Contains Interaction Time Stamp
        /// </summary>
        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 7)]
        public int InteractionTimeStamp { get; set; }
    }
}