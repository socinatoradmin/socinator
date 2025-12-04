#region

using System.ComponentModel.DataAnnotations.Schema;
using DominatorHouseCore.DatabaseHandler.Common;
using DominatorHouseCore.Enums;

#endregion

namespace DominatorHouseCore.DatabaseHandler.GdTables.Accounts
{
    public class UserConversation : Entity
    {
        [Column(Order = 2)] public string SenderName { get; set; }

        [Column(Order = 3)] public string SenderId { get; set; }

        [Column(Order = 4)] public string ThreadId { get; set; }

        [Column(Order = 5)] public string Date { get; set; }

        [Column(Order = 6)] public ActivityType ActivityType { get; set; }

        [Column(Order = 7)] public string ConversationType { get; set; }
    }
}