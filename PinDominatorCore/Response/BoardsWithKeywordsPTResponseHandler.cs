using System;
using System.Collections.Generic;
using System.ComponentModel;
using DominatorHouseCore;
using DominatorHouseCore.Interfaces;
using PinDominatorCore.PDModel;
using PinDominatorCore.PDUtility;

namespace PinDominatorCore.Response
{
    [Localizable(false)]
    public class BoardsWithKeywordsPtResponseHandler : PdResponseHandler
    {
        public BoardsWithKeywordsPtResponseHandler(IResponseParameter response) : base(response)
        {
            if (string.IsNullOrEmpty(response.Response))
            {
                Success = false;
                return;
            }
            var jsonHand = new DominatorHouseCore.Utility.JsonHandler(response.Response);
            var jToken = jsonHand.GetJToken("initialReduxState", "boards");
            var oldUi = false;
            if(jToken == null || !jToken.HasValues)
            {
                jToken = jsonHand.GetJToken("resource_response", "data", "results");
                oldUi = true;
            }
            foreach (var node in jToken)
                try
                {
                    var pinsCount = 0;
                    var token = oldUi?node : node?.First;
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
                        PinsCount = pinsCount
                    };
                    BoardsList.Add(board);
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