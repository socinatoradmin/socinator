#region

using System.ComponentModel;

#endregion

namespace DominatorHouseCore.Enums.RdQuery
{
    public enum UserQuery
    {
        [Description("LangKeyKeywords")] Keywords = 1,

        [Description("LangKeyCustomUsersList")]
        CustomUsers = 2,

        [Description("LangKeyUsersWhoCommentedOnPosts")]
        UsersWhoCommentedOnPost = 3
    }
}