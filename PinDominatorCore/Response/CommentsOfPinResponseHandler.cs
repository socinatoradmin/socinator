using System;
using System.Collections.Generic;
using DominatorHouseCore;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Utility;
using PinDominatorCore.PDModel;
using PinDominatorCore.PDUtility;

namespace PinDominatorCore.Response
{
    public class CommentsOfPinResponseHandler : PdResponseHandler
    {
        public List<CommentDetails> LstComments = new List<CommentDetails>();

        public CommentsOfPinResponseHandler(IResponseParameter response) : base(response)
        {
            if (string.IsNullOrEmpty(response.Response))
            {
                Success = false;
                return;
            }

            var jsonHand = new DominatorHouseCore.Utility.JsonHandler(response.Response);
            var dataToken = jsonHand.GetJToken("resource_response", "data");
            var errorToken = jsonHand.GetJToken("resource_response", "error");

            if (errorToken.HasValues)
            {
                Success = false;
                Issue = new PinterestIssue
                {
                    Message = jsonHand.GetJTokenValue(errorToken, "message")
                };
            }
            else
            {
                foreach (var token in dataToken)
                    try
                    {
                        LstComments.Add(new CommentDetails
                        {
                            CommentId= jsonHand.GetJTokenValue(token, "id"),
                            Comment = jsonHand.GetJTokenValue(token, "text"),
                            Commentor = new PinterestUser
                            {
                                FullName = jsonHand.GetJTokenValue(token,"user", "full_name"),
                                UserId = jsonHand.GetJTokenValue(token,"user","id"),
                                ProfilePicUrl = jsonHand.GetJTokenValue(token,"user", "image_medium_url"),
                                Username = jsonHand.GetJTokenValue(token,"user","username")
                            }
                        });
                    }
                    catch (Exception ex)
                    {
                        ex.DebugLog();
                    }

                BookMark = Utilities.GetBetween(response.Response, PdConstants.BookMark, "\"");
                if (BookMark.Contains("-end-"))
                    HasMoreResults = false;
                else
                    HasMoreResults = true;
            }
        }

        public bool HasMoreResults { get; set; }
        public string BookMark { get; set; }
    }
}