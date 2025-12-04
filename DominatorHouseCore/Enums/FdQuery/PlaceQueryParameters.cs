#region

using System.ComponentModel;

#endregion

namespace DominatorHouseCore.Enums.FdQuery
{
    public enum PlaceQueryParameters
    {
        [Description("LangKeyKeywords")] Keywords = 1,
        [Description("LangKeyGraphSearchUrl")] GraphSearchUrl = 2,
        [Description("LangKeyCustomPageUrlS")] CustomPageList = 3
    }
}