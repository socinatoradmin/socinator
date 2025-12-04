#region

using DominatorHouseCore.Enums;
using SQLite;

#endregion

namespace DominatorHouseCore.DatabaseHandler.FdTables.Accounts
{
    public class FeedInfo
    {
        [PrimaryKey]
        [AutoIncrement]
        [Indexed]
        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 1)]
        public int Id { get; set; }

        /// <summary>
        /// </summary>
        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 3)]
        public string PostId { get; set; }

        /// <summary>
        /// </summary>

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 4)]
        public MediaType MediaType { get; set; }

        /// <summary>
        /// </summary>

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 5)]
        public string PostTitle { get; set; }

        /// <summary>
        /// </summary>
        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 6)]
        public string PostDescription { get; set; }

        /// <summary>
        ///     Like Count On The Post
        /// </summary>

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 7)]
        public int LikeCount { get; set; }

        /// <summary>
        ///     Comment Count On The Post
        /// </summary>

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 8)]
        public int CommentCount { get; set; }


        /// <summary>
        /// </summary>
        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 9)]
        public int ShareCount { get; set; }


        /// <summary>
        ///     Time when the Post had been Posted in TimeStamp
        /// </summary>
        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 10)]
        public long PostedTimeStamp { get; set; }


        /// <summary>
        /// </summary>
        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 2)]
        public string DetailedPostInfo { get; set; }
    }
}