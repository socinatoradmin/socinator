using SQLite;

namespace DominatorHouseCore.DatabaseHandler.Common
{
    public class AccountsSessionsTable
    {
        [PrimaryKey]
        [AutoIncrement]
        [Indexed]
        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 1)]
        public int Id { get; set; }
        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 2)]
        public string AccountId { get; set; }
        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 3)]
        public string CurrentSessionId { get; set; }
        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 4)]
        public string LastUpdated { get; set; }
        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 5)]
        public string CurrentSession { get; set; }
    }
}
