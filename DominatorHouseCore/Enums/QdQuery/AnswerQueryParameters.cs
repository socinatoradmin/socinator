#region

using System.ComponentModel;

#endregion

namespace DominatorHouseCore.Enums.QdQuery
{
    public enum AnswerQueryParameters
    {
        [Description("LangKeyCustomURLS")] CustomUrl,
        [Description("LangKeyKeywords")] Keywords,
        [Description("LangKeyCustomUsers")] CustomUser,
        [Description("LangKeyTopicList")] TopicList,
        //[Description("LangKeyCustomTopic")] CustomTopicUrl
    }
}