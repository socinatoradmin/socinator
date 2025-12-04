#region

using SQLite;

#endregion

namespace DominatorHouseCore.DatabaseHandler.DHTables
{
    public class InstaAccountBackup
    {
        [PrimaryKey]
        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 1)]
        [Indexed]
        [AutoIncrement]
        public int Id { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 2)]
        public string AccountName { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 3)]
        public string DeviceId { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 4)]
        public string UserAgent { get; set; }
    }
}