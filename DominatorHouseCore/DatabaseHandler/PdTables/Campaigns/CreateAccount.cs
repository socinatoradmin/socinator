#region

using SQLite;

#endregion

namespace DominatorHouseCore.DatabaseHandler.PdTables.Campaigns
{
    public class CreateAccount
    {
        [PrimaryKey]
        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 1)]
        [Indexed]
        [AutoIncrement]
        public int Id { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 2)]
        public string Email { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 3)]
        public string Password { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 4)]
        public string Age { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 5)]
        public string Gender { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 6)]
        public int InteractionDate { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 7)]
        public string ActivityType { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 8)]
        public string Status { get; set; }
    }
}