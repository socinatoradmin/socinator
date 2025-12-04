using System;
using System.Collections.Generic;
using DominatorHouseCore.Interfaces;
using PinDominatorCore.PDEnums;
using PinDominatorCore.PDModel;
using PinDominatorCore.PDUtility;
using DominatorHouseCore;
using DominatorHouseCore.Utility;

namespace PinDominatorCore.Response
{
    public class BoardInvitationsResponseHandler : PdResponseHandler
    {
        public BoardInvitationsResponseHandler(IResponseParameter response, bool isBoardInvite,
            BoardInvitationsResponseHandler boardResponse = null) : base(response)
        {
            var jsonHand = new JsonHandler(response.Response);

            var dataToken = jsonHand.GetJToken("resource_response", "data");

            if (boardResponse != null && boardResponse.Success)
                BoardsList.AddRange(boardResponse.BoardsList);

            string sender = "sender";
            string recipient = "recipient";

            if(isBoardInvite)
            {
                sender = "invited_by_user";
                recipient = "invited_user";
            }

            foreach (var boardToken in dataToken)
                try
                {
                    var board = new PinterestBoard();
                    var conversation = jsonHand.GetJToken(boardToken, "conversation");

                    if (!conversation.HasValues)
                    {
                        board.PinterestUserSender = new PinterestUser();
                        board.PinterestUserSender.Gender =
                            jsonHand.GetJTokenValue(boardToken, sender, "gender") == "female"
                                ? PinterestGender.Female
                                : jsonHand.GetJTokenValue(boardToken, sender, "gender") == "male"
                                    ? PinterestGender.Male
                                    : PinterestGender.Unknown;
                        board.PinterestUserSender.Username = jsonHand.GetJTokenValue(boardToken, sender, "username");
                        board.PinterestUserSender.FirstName = jsonHand.GetJTokenValue(boardToken, sender, "first_name");
                        board.PinterestUserSender.LastName = jsonHand.GetJTokenValue(boardToken, sender, "last_name");
                        board.PinterestUserSender.FullName = jsonHand.GetJTokenValue(boardToken, sender, "full_name");

                        board.PinterestUserSender.ImageSmallUrl =
                            jsonHand.GetJTokenValue(boardToken, sender, "image_small_url");
                        board.PinterestUserSender.ImageMediumUrl =
                            jsonHand.GetJTokenValue(boardToken, sender, "image_medium_url");
                        board.PinterestUserSender.ImageLargeUrl =
                            jsonHand.GetJTokenValue(boardToken, sender, "image_large_url");
                        board.PinterestUserSender.ImageXlargeUrl =
                            jsonHand.GetJTokenValue(boardToken, sender, "image_xlarge_url");
                        board.PinterestUserSender.UserId = jsonHand.GetJTokenValue(boardToken, sender, "id");

                        board.PinterestUserRecipient = new PinterestUser();
                        board.PinterestUserRecipient.Gender =
                            jsonHand.GetJTokenValue(boardToken, recipient, "gender") == "female" ? PinterestGender.Female :
                            jsonHand.GetJTokenValue(boardToken, recipient, "gender") == "male" ? PinterestGender.Male :
                            PinterestGender.Unknown;
                        board.PinterestUserRecipient.Username =
                            jsonHand.GetJTokenValue(boardToken, recipient, "username");
                        board.PinterestUserRecipient.FirstName =
                            jsonHand.GetJTokenValue(boardToken, recipient, "first_name");
                        board.PinterestUserRecipient.LastName =
                            jsonHand.GetJTokenValue(boardToken, recipient, "last_name");
                        board.PinterestUserRecipient.FullName =
                            jsonHand.GetJTokenValue(boardToken, recipient, "full_name");
                        board.PinterestUserRecipient.ImageSmallUrl =
                            jsonHand.GetJTokenValue(boardToken, recipient, "image_small_url");
                        board.PinterestUserRecipient.ImageMediumUrl =
                            jsonHand.GetJTokenValue(boardToken, recipient, "image_medium_url");
                        board.PinterestUserRecipient.ImageLargeUrl =
                            jsonHand.GetJTokenValue(boardToken, recipient, "image_large_url");
                        board.PinterestUserRecipient.ImageXlargeUrl =
                            jsonHand.GetJTokenValue(boardToken, recipient, "image_xlarge_url");
                        board.PinterestUserRecipient.UserId = jsonHand.GetJTokenValue(boardToken, recipient, "id");

                        var boardString = jsonHand.GetJTokenValue(boardToken, "board");
                        if (!string.IsNullOrEmpty(boardString))
                        {
                            board.BoardName = jsonHand.GetJTokenValue(boardToken, "board", "name");
                            board.BoardUrl = jsonHand.GetJTokenValue(boardToken, "board", "url");
                            board.Id = jsonHand.GetJTokenValue(boardToken, "board", "id");
                            board.BoardCreatedAt = jsonHand.GetJTokenValue(boardToken, "board", "created_at");
                            board.CreatedAt = jsonHand.GetJTokenValue(boardToken, "created_at");
                            board.BoardOrderModifiedAt =
                                jsonHand.GetJTokenValue(boardToken, "board", "board_order_modified_at");
                            board.CollaboratedByMe = jsonHand.GetJTokenValue(boardToken, "board", "collaborated_by_me");
                            board.IsCollaborative =
                                jsonHand.GetJTokenValue(boardToken, "board", "is_collaborative") == "True";
                            board.FollowedByMe = jsonHand.GetJTokenValue(boardToken, "board", "followed_by_me");
                            board.ImageThumbnailUrl = jsonHand.GetJTokenValue(boardToken, "board", "image_thumbnail_url");
                            board.ContactRequestId = jsonHand.GetJTokenValue(boardToken, "id");
                            board.IsFollowed = jsonHand.GetJTokenValue(boardToken, "board", "followed_by_me") == "True";

                            // if boardString is not empty than only it should add
                            BoardsList.Add(board);
                        }
                    }
                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                }
        }

        public bool HasMoreResults { get; set; }

        public string BookMark { get; set; }

        public List<PinterestBoard> BoardsList { get; } = new List<PinterestBoard>();
    }
}