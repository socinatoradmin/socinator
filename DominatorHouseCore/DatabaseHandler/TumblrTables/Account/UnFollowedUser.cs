#region

using SQLite;

#endregion

namespace DominatorHouseCore.DatabaseHandler.TumblrTables.Account
{
    public class UnFollowedUser
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
        public string QueryType { get; set; }

        /// <summary>
        ///     Contains QueryValue For Interaction
        /// </summary>
        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 3)]
        public string QueryValue { get; set; }


        /// <summary>
        ///     Describes Activity
        /// </summary>
        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 4)]
        public string ActivityType { get; set; }


        /// <summary>
        ///     Contains TimeStamp when interacted with the User
        /// </summary>
        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 5)]
        public int InteractionTimeStamp { get; set; }


        /// <summary>
        ///     Contains whom we are unfollowing
        /// </summary>
        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 6)]
        public string InteractedUsername { get; set; }


        /// <summary>
        ///     Contains whom we are unfollowing
        /// </summary>
        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 7)]
        public string UserName { get; set; }


        /// <summary>
        ///     Contais the TemplateId
        /// </summary>
        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 8)]
        public string TemplateId { get; set; }
    }
}