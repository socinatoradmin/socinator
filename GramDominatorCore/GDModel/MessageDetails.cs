
using System.Collections.Generic;
using DominatorHouseCore.Enums;
using GramDominatorCore.GDEnums;

namespace GramDominatorCore.GDModel
{
    public class MessageDetails
    {
        public string ItemId { get; set; }

        public DirectMessageType MessageType { get; set; }

        public string Timestamp { get; set; }

        public string UserId { get; set; }

        public string ClientContext { get; set; }

        public string MessageText { get; set; }

        public MediaMessageDetails MediaMessageDetails { get; set; }

        public LinkMessageDetails LinkMessageDetails { get; set; }

        public LikeMessageDetails LikeMessageDetails { get; set; }
    }


    #region Media Message related Classes

    public class MediaMessageDetails
    {
        public List<MediaCandidate> LstMediaCandidates { get; set; }

        public int OriginalWidth { get; set; }

        public int OriginalHeight { get; set; }

        public MediaType MediaType { get; set; }
    }

    public class MediaCandidate
    {
        public int Width { get; set; }

        public int Height { get; set; }

        public string ImageUrl { get; set; }
    }

    #endregion


    #region Link Message related Classes

    public class LinkMessageDetails
    {
        public string MessageText { get; set; }

        public LinkMessageContext LinkMessageContext { get; set; }
    }

    public class LinkMessageContext
    {
        public string LinkUrl { get; set; }

        public string LinkTitle { get; set; }

        public string LinkSummary { get; set; }

        public string LinkImageUrl { get; set; }
    }

    #endregion


    #region Like Message related class

    public class LikeMessageDetails
    {
        public bool IsLiked { get; set; }
        public string Like { get; set; } = "❤️";
    }

    #endregion
}
