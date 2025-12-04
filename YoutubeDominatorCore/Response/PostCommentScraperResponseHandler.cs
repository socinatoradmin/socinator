using DominatorHouseCore;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Utility;
using System;
using System.Collections.Generic;
using YoutubeDominatorCore.YDUtility;
using YoutubeDominatorCore.YoutubeModels;
using static DominatorHouseCore.DatabaseHandler.YdTables.Accounts.InteractedPosts;

namespace YoutubeDominatorCore.Response
{
    public class PostCommentScraperResponseHandler : YdResponseHandler
    {
        public PostCommentScraperResponseHandler()
        {
        }

        public PostCommentScraperResponseHandler(IResponseParameter response, bool getCommentsList,
            bool getNecessaryElements)
        {
            try
            {
                //Utilities.UpdateTestResponseDataFile(response.Response, YdStatic.MyCoreLocation() + @".UnitTests\TestData\PostCommentsScraperResponse.json");

                var jsonHand = new JsonHandler(response.Response);

                if (getCommentsList)
                    GetCommentsList(jsonHand);

                CommentsCount = jsonHand.GetElementValue("response", "continuationContents", "itemSectionContinuation",
                    "header", "commentsHeaderRenderer", "countText", "simpleText").RemoveAllExceptWholeNumb();
                if (string.IsNullOrEmpty(CommentsCount))
                    CommentsCount = jsonHand.GetElementValue("response", "contents", "itemSectionContinuation",
                        "header", "commentsHeaderRenderer", "countText", "simpleText").RemoveAllExceptWholeNumb();

                if (getNecessaryElements)
                    GetNecessaryElements(jsonHand);

                Success = true;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        public List<YoutubePostCommentModel> ListOfCommentsInfo { get; set; }
        public PostDataElements PostDataElements { get; set; } = new PostDataElements();
        public string CommentsCount { get; set; }
        public bool HasMoreResults { get; set; }

        private void GetCommentsList(JsonHandler jsonHand)
        {
            var jsonToken =
                jsonHand.GetJToken("response", "continuationContents", "itemSectionContinuation", "contents");
            ListOfCommentsInfo = new List<YoutubePostCommentModel>();

            foreach (var token in jsonToken)
            {
                var jToken = jsonHand.GetJTokenOfJToken(token, "commentThreadRenderer", "comment", "commentRenderer");

                var youtubePostCommentModel = new YoutubePostCommentModel
                {
                    CommentText = jsonHand.GetJTokenValue(jToken, "contentText", "simpleText"),
                    CommentActionParam = jsonHand.GetJTokenValue(jToken, "actionButtons",
                        "commentActionButtonsRenderer", "likeButton", "toggleButtonRenderer", "defaultServiceEndpoint",
                        "performCommentActionEndpoint", "action"),
                    CommentId = jsonHand.GetJTokenValue(jToken, "commentId"),
                    CommentTime = jsonHand.GetJTokenValue(jToken, "publishedTimeText", "runs", 0, "text"),
                    CommenterChannelName = jsonHand.GetJTokenValue(jToken, "authorText", "simpleText"),
                    CommenterChannelId =
                        jsonHand.GetJTokenValue(jToken, "authorEndpoint", "browseEndpoint", "browseId"),
                    CommentLikesCount = jsonHand.GetJTokenValue(jToken, "likeCount"),
                    TrackingParams = jsonHand.GetJTokenValue(jToken, "trackingParams"),
                    CreateReplyParams = jsonHand.GetJTokenValue(jToken, "actionButtons", "commentActionButtonsRenderer",
                        "replyButton", "buttonRenderer", "navigationEndpoint", "createCommentReplyDialogEndpoint",
                        "dialog", "commentReplyDialogRenderer", "replyButton", "buttonRenderer", "serviceEndpoint",
                        "createCommentReplyEndpoint", "createReplyParams")

                    //createReplyParams
                };

                if (string.IsNullOrEmpty(youtubePostCommentModel.CommentText))
                {
                    var textJToken = jsonHand.GetJTokenOfJToken(jToken, "contentText", "runs");

                    foreach (var eachToken in textJToken)
                        youtubePostCommentModel.CommentText += jsonHand.GetJTokenValue(eachToken, "text");
                }

                var vote = jsonHand.GetJTokenValue(jToken, "voteStatus");
                youtubePostCommentModel.CommentLikeStatus =
                    vote.Equals("like", StringComparison.CurrentCultureIgnoreCase) ? LikeStatus.Like :
                    vote.Equals("dislike", StringComparison.CurrentCultureIgnoreCase) ? LikeStatus.Dislike :
                    LikeStatus.Indifferent;

                ListOfCommentsInfo.Add(youtubePostCommentModel);
            }
        }

        private void GetNecessaryElements(JsonHandler jsonHand)
        {
            PostDataElements.ContinuationToken = jsonHand.GetElementValue("response", "continuationContents",
                "itemSectionContinuation", "continuations", 0, "nextContinuationData", "continuation");
            HasMoreResults = !string.IsNullOrEmpty(PostDataElements.ContinuationToken);

            PostDataElements.TrackingParams = jsonHand.GetElementValue("response", "continuationContents",
                "itemSectionContinuation", "header", "commentsHeaderRenderer", "createRenderer",
                "commentSimpleboxRenderer", "submitButton", "buttonRenderer", "trackingParams");

            if (string.IsNullOrEmpty(PostDataElements.TrackingParams))
                PostDataElements.TrackingParams = jsonHand.GetElementValue("response", "continuationContents",
                    "itemSectionContinuation", "header", "commentsHeaderRenderer", "createRenderer",
                    "commentSimpleboxRenderer", "trackingParams");
            if (string.IsNullOrEmpty(PostDataElements.TrackingParams))
                PostDataElements.TrackingParams =
                    jsonHand.GetElementValue("response", "contents", "itemSectionRenderer", "trackingParams");
            if (string.IsNullOrEmpty(PostDataElements.TrackingParams))
                PostDataElements.TrackingParams =
                    jsonHand.GetElementValue("response", "responseContext", "trackingParams");

            PostDataElements.CreateCommentParams = jsonHand.GetElementValue("response", "continuationContents",
                "itemSectionContinuation", "header", "commentsHeaderRenderer", "createRenderer",
                "commentSimpleboxRenderer", "submitButton", "buttonRenderer", "serviceEndpoint",
                "createCommentEndpoint", "createCommentParams");
            if (string.IsNullOrEmpty(PostDataElements.CreateCommentParams))
                PostDataElements.CreateCommentParams = jsonHand.GetElementValue("response", "contents",
                    "itemSectionRenderer", "header", "commentsHeaderRenderer", "createRenderer",
                    "commentSimpleboxRenderer", "submitButton", "buttonRenderer", "serviceEndpoint",
                    "createCommentEndpoint", "createCommentParams");

            if (string.IsNullOrEmpty(PostDataElements.CreateCommentParams))
                PostDataElements.CreateCommentParams = jsonHand.GetElementValue("response", "continuationContents",
                    "itemSectionContinuation", "header", "commentsHeaderRenderer", "createRenderer",
                    "commentSimpleboxRenderer", "submitButton", "buttonRenderer", "serviceEndpoint",
                    "channelCreationServiceEndpoint", "zeroStepChannelCreationParams", "zeroStepCreateCommentParams",
                    "createCommentParams");
        }
    }
}