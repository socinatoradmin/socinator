#region

using System.ComponentModel;

#endregion

namespace DominatorHouseCore.Enums.PdQuery
{
    public enum PDPinQueries
    {
        [Description("LangKeyKeywords")] Keywords = 1,
        [Description("LangKeyCustomUsers")] Customusers = 2,
        [Description("LangKeyCustomBoard")] CustomBoard = 3,
        [Description("LangKeyCustomPin")] CustomPin = 4,

        [Description("LangKeySocinatorPublisherCampaign")]
        SocinatorPublisherCampaign = 5,
        [Description("LangKeyOwnFollowers")] OwnFollowers = 6,
        [Description("LangKeyOwnFollowings")] OwnFollowings = 7,
        [Description("LangKeyNewsfeed")] Newsfeed = 8
    }
}