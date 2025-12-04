#region

using System.ComponentModel;

#endregion

namespace DominatorHouseCore.Enums.RdQuery
{
    public enum CommunityQuery
    {
        [Description("LangKeyKeywords")] Keywords = 1,
        [Description("LangKeyCustomurl")] CustomUrl = 2
    }
}