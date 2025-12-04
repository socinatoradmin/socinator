using System.ComponentModel;

namespace DominatorHouseCore.Enums.QdQuery
{
    public enum PostQueryParameters
    {
        [Description("LangKeyCustomURLS")] CustomUrl,
        [Description("LangKeyKeywords")] Keywords,
        [Description("LangKeyCustomUsers")] CustomUsers
    }
}
