#region

using System.ComponentModel;

#endregion

namespace DominatorHouseCore.Enums.LdQuery
{
    public enum LDMessengerQueryParameters
    {
        [Description("LangKeyKeyword")] Keyword,
        [Description("LangKeyNotification")] Notification,
        // ReSharper disable once UnusedMember.Global
        [Description("LangKeyJoinedGroupUrl")] JoinedGroupUrl
    }
}