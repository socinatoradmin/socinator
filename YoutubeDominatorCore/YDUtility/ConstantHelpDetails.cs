using DominatorHouseCore.Utility;

namespace YoutubeDominatorCore.YDUtility
{
    public class ConstantHelpDetails
    {
        public static string ContactSupportLink { get; set; } = ConstantVariable.ContactSupportLink;

        public static string ReportVideoTutorialsLink { get; set; } = "";
        public static string ReportVideoKnowledgeBaseLink { get; set; } = "";

        #region LikeModule

        public static string LikeVideoTutorialsLink { get; set; } = "https://www.youtube.com/watch?v=NB4NPs9ZaTk&t=42s";

        public static string LikeKnowledgeBaseLink { get; set; } =
            "https://help.socinator.com/support/solutions/articles/42000034802-youtube-auto-like";

        #endregion

        #region DislikeModule

        public static string DislikeVideoTutorialsLink { get; set; } = "https://www.youtube.com/watch?v=DJLDgPkbMRY";

        public static string DislikeKnowledgeBaseLink { get; set; } =
            "https://help.socinator.com/support/solutions/articles/42000034803-youtube-auto-dislike";

        #endregion

        #region CommentModule

        public static string CommentVideoTutorialsLink { get; set; } = "https://www.youtube.com/watch?v=lzLYEAVVn9c";

        public static string CommentKnowledgeBaseLink { get; set; } =
            "https://help.socinator.com/support/solutions/articles/42000034804-youtube-auto-comment";

        #endregion

        #region LikeCommentModule

        public static string LikeCommentVideoTutorialsLink { get; set; } =
            "https://www.youtube.com/watch?v=OUd7svPb6To";

        public static string LikeCommentKnowledgeBaseLink { get; set; } =
            "https://help.socinator.com/support/solutions/articles/42000034805-youtube-auto-like-comments";

        #endregion

        #region SubscribeModule

        public static string SubscribeVideoTutorialsLink { get; set; } = "https://www.youtube.com/watch?v=aXLbQt4L3BI";

        public static string SubscribeKnowledgeBaseLink { get; set; } =
            "https://help.socinator.com/support/solutions/articles/42000034806-youtube-auto-subscribe";

        #endregion

        #region UnsubscribeModule

        public static string UnsubscribeVideoTutorialsLink { get; set; } =
            "https://www.youtube.com/watch?v=GqCi2FxLxtg";

        public static string UnsubscribeKnowledgeBaseLink { get; set; } =
            "https://help.socinator.com/support/solutions/articles/42000034808-youtube-auto-unsubscribe";

        #endregion

        #region ScrapVideosModule

        public static string ScrapVideosVideoTutorialsLink { get; set; } =
            "https://www.youtube.com/watch?v=bEJPdCOcja8";

        public static string ScrapVideosKnowledgeBaseLink { get; set; } =
            "https://help.socinator.com/support/solutions/articles/42000034811-youtube-auto-post-scraper";

        #endregion

        #region ScrapChannelsModule

        public static string ScrapChannelsVideoTutorialsLink { get; set; } =
            "https://www.youtube.com/watch?v=Ev4h1_Axxpo";

        public static string ScrapChannelsKnowledgeBaseLink { get; set; } =
            "https://help.socinator.com/support/solutions/articles/42000034812-youtube-auto-channel-scraper";

        #endregion

        #region ViewIncreaserModule

        public static string ViewIncreaserVideoTutorialsLink { get; set; } =
            "https://www.youtube.com/watch?v=vFPi7kmcGpU";

        public static string ViewIncreaserKnowledgeBaseLink { get; set; } =
            "https://help.socinator.com/support/solutions/articles/42000034813-youtube-auto-view-increaser";

        #endregion
    }
}