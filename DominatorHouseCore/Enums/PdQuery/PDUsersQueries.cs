#region

using System.ComponentModel;

#endregion

namespace DominatorHouseCore.Enums.PdQuery
{
    public enum PDUsersQueries
    {
        [Description("LangKeyKeywords")] Keywords = 1,

        [Description("LangKeySomeonesFollowers")]
        SomeonesFollowers = 2,

        [Description("LangKeySomeonesFollowings")]
        SomeonesFollowings = 3,

        [Description("LangKeyFollowersOfSomeonesFollowings")]
        FollowersOfSomeonesFollowings = 4,

        [Description("LangKeyFollowersOfSomeonesFollowers")]
        FollowersOfSomeonesFollowers = 5,
        [Description("LangKeyCustomUsers")] Customusers = 6,
        //[Description("LangKeyBoardFollowers")] BoardFollowers = 7,

        [Description("LangKeyUsersWhoTriedPins")]
        UsersWhoTriedPins = 8,
        [Description("LangKeyCustomBoard")] CustomBoard = 9,

        [Description("LangKeyBoardsbyKeywords")]
        BoardsbyKeywords = 10
    }
}