#region

using System.ComponentModel;

#endregion

namespace DominatorHouseCore.Enums.QdQuery
{
    public enum QuestionQueryParameters
    {
        [Description("LangKeyCustomURLS")] CustomUrl,
        [Description("LangKeyKeywords")] Keywords,

        [Description("LangKeyCustomUsersList")]
        CustomUser,
        [Description("LangKeyTopicList")] TopicFaqs,
        //[Description("LangKeyCustomTopic")] CustomTopicUrl,
    }
}