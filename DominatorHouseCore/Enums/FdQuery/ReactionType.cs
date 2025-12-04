#region

using System.ComponentModel;

#endregion

namespace DominatorHouseCore.Enums.FdQuery
{
    public enum ReactionType
    {
        [Description("Like")] Like = 1,

        [Description("Love")] Love = 2,

        [Description("Care")] Care = 3,

        [Description("Wow")] Wow = 4,

        [Description("Haha")] Haha = 5,

        [Description("Sad")] Sad = 8,

        [Description("Angry")] Angry = 9,

        [Description("Unlike")] Unlike = 0

        //        [Description(" ")]
        //        NotLiked = 0,
    }

    public enum BrowserReactionType
    {
        [Description("Like")] Like = 0,

        [Description("Love")] Love = 1,

        [Description("Care")] Care = 2,

        [Description("Haha")] Haha = 3,

        [Description("Wow")] Wow = 4,

        [Description("Sad")] Sad = 5,

        // ReSharper disable once UnusedMember.Global
        [Description("Angry")] Angry = 6,

        [Description("Share")] Share = 7,

        [Description("Comment")] Comment = 8

        //        [Description(" ")]
        //        NotLiked = 0,
    }
}