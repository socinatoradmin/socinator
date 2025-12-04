#region

using SQLite;

#endregion

namespace DominatorHouseCore.DatabaseHandler.LdTables.Account
{
    public class Pages
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
        public string PageName { get; set; }

        /// <summary>
        ///     Contains Url Of the Group
        /// </summary>
        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 3)]
        //[Unique]
        public string PageUrl { get; set; }

        /// <summary>
        ///     Contains Profile Picture Url Of the Connection
        /// </summary>
        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 4)]
        public string TotalMembers { get; set; }

        /// <summary>
        ///     Describe Connection Type If FirstDegree,SecondDegree Or ThirdPlusDegree
        /// </summary>
        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 5)]
        // ReSharper disable once UnusedMember.Global
        // need to keep it to support existing data model
        public string UniversalPageName { get; set; }

        /// <summary>
        ///     Contains Interaction Time Stamp
        /// </summary>
        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 6)]
        public int InteractionTimeStamp { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 7)]
        public string PageId { get; set; }
    }
}