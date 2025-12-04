using System;
using System.Collections.Generic;
using DominatorHouseCore;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Utility;
using PinDominatorCore.PDModel;

namespace PinDominatorCore.Response
{
    public class BoardsOfUserResponseHandler : PdResponseHandler
    {
        public bool HasMoreResults { get; set; }
        public string BookMark { get; set; }
        public List<PinterestBoard> BoardsList { get; } = new List<PinterestBoard>();

        public BoardsOfUserResponseHandler(IResponseParameter response) : base(response)
        {
            try
            {
                if (string.IsNullOrEmpty(response.Response))
                {
                    Success = false;
                    return;
                }

                var jsonHand = new JsonHandler(response.Response);
                var jToken = jsonHand.GetJToken("resource_response", "data");

                foreach (var token in jToken)
                {
                    var followersCount = 0;
                    var pinsCount = 0;
                    int.TryParse(jsonHand.GetJTokenValue(token, "follower_count"), out followersCount);
                    int.TryParse(jsonHand.GetJTokenValue(token, "pin_count"), out pinsCount);
                    var board = new PinterestBoard
                    {
                        BoardDescription = jsonHand.GetJTokenValue(token, "description"),
                        BoardName = jsonHand.GetJTokenValue(token, "name"),
                        BoardUrl = jsonHand.GetJTokenValue(token, "url"),
                        Id = jsonHand.GetJTokenValue(token, "id"),
                        IsFollowed = jsonHand.GetJTokenValue(token, "followed_by_me") == "True",
                        UserId = jsonHand.GetJTokenValue(token, "owner", "id"),
                        UserName = jsonHand.GetJTokenValue(token, "owner", "username"),
                        FollowersCount = followersCount,
                        PinsCount = pinsCount
                    };
                    if(!string.IsNullOrEmpty(board.BoardName) && !string.IsNullOrEmpty(board.BoardUrl))
                        BoardsList.Add(board);
                }

                BookMark = jsonHand.GetJToken("resource_response", "bookmark")?.ToString()?.Replace("[]","");
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }
    }
}