#region

using System.ComponentModel;

#endregion

namespace DominatorHouseCore.Enums.QdQuery
{
    public enum UserQueryParameters
    {
        [Description("LangKeyKeywords")] Keywords,

        [Description("LangKeySomeonesFollowers")]
        SomeonesFollowers,

        [Description("LangKeySomeonesFollowings")]
        SomeonesFollowings,

        [Description("LangKeyCustomUsersList")]
        CustomUsers,
        [Description("LangKeyEngagedUsers")] EngagedUsers,

        //[Description("QDlangOwnFollowers")]
        //OwnFollowers,
        //[Description("LangKeyTopicFollowers")]
        //TopicFollowers,
        [Description("LangKeyAnswerUpvoters")] AnswerUpvoters
    }
}