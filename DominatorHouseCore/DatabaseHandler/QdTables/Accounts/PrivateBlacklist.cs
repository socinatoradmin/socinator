#region

using SQLite;

#endregion

namespace DominatorHouseCore.DatabaseHandler.QdTables.Accounts
{
    public class PrivateBlacklist
    {
        [PrimaryKey]
        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 1)]
        [Indexed]
        [AutoIncrement]
        public int Id { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 2)]
        public string UserName { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 3)]
        public string UserId { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 4)]
        public int InteractionTimeStamp { get; set; }
    }
}