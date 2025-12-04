using DominatorHouseCore;
using DominatorHouseCore.DatabaseHandler.YdTables.Accounts;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Utility;
using System;
using System.Linq;
using YoutubeDominatorCore.YDUtility;
using YoutubeDominatorCore.YoutubeModels;

namespace YoutubeDominatorCore.Response
{
    internal class CommentInfoFromBrowser : YdResponseHandler
    {
        public YoutubePostCommentModel CommentModel = new YoutubePostCommentModel();

        public CommentInfoFromBrowser(IResponseParameter responsePara)
        {
            try
            {
                var response = responsePara.Response;

                var channelIdentity = Utilities.GetBetween(response,
                    "class=\"yt-simple-endpoint style-scope ytd-comment-view-model\" href=\"", "\"");
                CommentModel.CommenterChannelId = Utilities.GetBetween($"{channelIdentity}/", "/user/", "/");
                if (string.IsNullOrEmpty(CommentModel.CommenterChannelId))
                    CommentModel.CommenterChannelId = Utilities.GetBetween($"{channelIdentity}/", "/channel/", "/");
                CommentModel.CommenterChannelId = string.IsNullOrEmpty(CommentModel.CommenterChannelId) ? channelIdentity?.Replace("/", "") : CommentModel.CommenterChannelId;
                //
                var authorDetails = Utilities.GetBetween(response, "<a id=\"author-text\"", "</a>");
                CommentModel.CommenterChannelName = Utilities.GetBetween(authorDetails,
                    "<span class=\"channel-owner style-scope ytd-comment-view-model style-scope ytd-comment-view-model\">", "</span>").Trim();
                if (string.IsNullOrEmpty(CommentModel.CommenterChannelName))
                    CommentModel.CommenterChannelName = Utilities.GetBetween(authorDetails,
                    "<span class=\" style-scope ytd-comment-renderer style-scope ytd-comment-renderer\">", "</span>").Trim();
                if (string.IsNullOrEmpty(CommentModel.CommenterChannelName))
                    CommentModel.CommenterChannelName = Utilities.GetBetween(authorDetails,
                    "<yt-formatted-string respect-lang-dir=\"\" class=\" style-scope ytd-comment-renderer style-scope ytd-comment-renderer\">", "</yt-formatted-string>").Trim();
                if (string.IsNullOrEmpty(CommentModel.CommenterChannelName))
                    CommentModel.CommenterChannelName = Utilities.GetBetween(authorDetails,
                    "href=\"/", "\"").Trim();
                var commentIdAndTime = Utilities.GetBetween(response,
                    "<a class=\"yt-simple-endpoint style-scope yt-formatted-string\"", "</a>");
                CommentModel.CommentId = Utilities.GetBetween(string.IsNullOrEmpty(commentIdAndTime) ? response : commentIdAndTime, ";lc=", "\"");
                CommentModel.CommentTime = Utilities.GetBetween($"{commentIdAndTime}</a>", "\">", "</a>").Trim();
                CommentModel.CommentTime = string.IsNullOrEmpty(CommentModel.CommentTime) ? HtmlParseUtility.GetListInnerTextFromPartialTagNamecontains(response, "a", "class", "yt-simple-endpoint style-scope ytd-comment-view-model")?.LastOrDefault()?.Replace("\n", "")?.Replace("\t", "")?.Trim() : CommentModel.CommentTime;
                var likeCountDetails = Utilities.GetBetween(response, "<span id=\"vote-count-middle\"", "</span>");
                CommentModel.CommentLikesCount =
                    Utilities.GetBetween($"{likeCountDetails}</span>", "\">", "</span>").Trim();
                CommentModel.CommentLikesCount = string.IsNullOrEmpty(CommentModel.CommentLikesCount) ? "N/A" : CommentModel.CommentLikesCount;
                var likeVoteDetails = Utilities.GetBetween(response, "<ytd-toggle-button-renderer id=\"like-button\"",
                    "<button id=\"button\"");
                CommentModel.CommentLikeStatus =
                    likeVoteDetails.Contains("style-scope ytd-comment-action-buttons-renderer style-default-active size-default")
                        ? InteractedPosts.LikeStatus.Like
                        : InteractedPosts.LikeStatus.Indifferent;

                if (CommentModel.CommentLikeStatus == InteractedPosts.LikeStatus.Indifferent)
                {
                    var disLikeVoteDetails = Utilities.GetBetween(response,
                        "<ytd-toggle-button-renderer id=\"dislike-button\"", "<button id=\"button\"");
                    CommentModel.CommentLikeStatus =
                        disLikeVoteDetails.Contains(
                            "style-scope ytd-comment-action-buttons-renderer style-default-active size-default")
                            ? InteractedPosts.LikeStatus.Dislike
                            : InteractedPosts.LikeStatus.Indifferent;
                }

                var commentText = Utilities.GetBetween(response,
                    "<yt-attributed-string id=\"content-text\" slot=\"content\" user-input=\"\" class=\"style-scope ytd-comment-view-model\">", "</yt-attributed-string>");
                CommentModel.CommentText = YdStatic.GetTextRemoveHtmlTags($"{commentText}</div>".Trim());

                Success = true;

                // Testing  
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }
    }
}