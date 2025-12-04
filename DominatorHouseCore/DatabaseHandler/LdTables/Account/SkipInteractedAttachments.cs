#region

using System;
using SQLite;

#endregion

namespace DominatorHouseCore.DatabaseHandler.LdTables.Account
{
    public class SkipInteractedAttachments
    {
        [PrimaryKey]
        [AutoIncrement]
        [Indexed]
        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 1)]
        public int Id { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 2)]
        public string PublicIdentifier { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 3)]
        public DateTime InteractionDatetime { get; set; }
    }
}