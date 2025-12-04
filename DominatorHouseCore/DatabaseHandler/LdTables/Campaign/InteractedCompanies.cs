#region

using SQLite;

#endregion

namespace DominatorHouseCore.DatabaseHandler.LdTables.Campaign
{
    public class InteractedCompanies
    {
        [PrimaryKey]
        [AutoIncrement]
        [Indexed]
        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 1)]
        public int Id { get; set; }

        /// <summary>
        ///     EmailId of the Account from which Interaction has been done
        /// </summary>

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 2)]
        public string AccountEmail { get; set; }

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
        ///     Contains Name of the Company being interacted
        /// </summary>
        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 6)]
        public string CompanyName { get; set; }

        /// <summary>
        ///     Contains Id of the Company being interacted
        /// </summary>

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 7)]
        public string CompanyUrl { get; set; }

        /// <summary>
        ///     Contains TotalEmployees in the Company being interacted
        /// </summary>
        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 8)]
        public string TotalEmployees { get; set; }

        /// <summary>
        ///     Describes Industry of the Company being interacted
        /// </summary>
        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 9)]
        public string Industry { get; set; }

        /// <summary>
        ///     Describes Follow Status For this Account in the Company being interacted
        /// </summary>
        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 10)]
        public string IsFollowed { get; set; }

        /// <summary>
        ///     Contains DetailedInfo Regarding Interacted Company In Jason String Form
        /// </summary>
        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 11)]
        public string DetailedInfo { get; set; }

        /// <summary>
        ///     TimeStamp when interacted with the Company
        /// </summary>
        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 12)]
        public int InteractionTimeStamp { get; set; }
    }
}