using System.Collections.Generic;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Utility;
using Newtonsoft.Json.Linq;
using PinDominatorCore.PDModel;
using PinDominatorCore.PDUtility;
using System.Linq;

namespace PinDominatorCore.Response
{
    public class NewsFeedPinsResponseHandler : PdResponseHandler
    {
        public NewsFeedPinsResponseHandler(IResponseParameter response) : base(response)
        {
            if (string.IsNullOrEmpty(response.Response))
            {
                Success = false;
                return;
            }

            var jsonHand = new DominatorHouseCore.Utility.JsonHandler(response.Response);
            if (jsonHand.GetJToken("resource_response", "error").HasValues)
            {
                Issue = new PinterestIssue
                {
                    Message = jsonHand.GetElementValue("resource_response", "error", "message")
                };
                Success = false;
                return;
            }

            BookMark = jsonHand.GetElementValue("resource_response", "bookmark");
            if (string.IsNullOrEmpty(BookMark))
                BookMark = Utilities.GetBetween(response.Response, PdConstants.BookMark, "\"]");
            HasMoreResults = !BookMark.Contains("-end-");
            var data = jsonHand.GetJToken("resource_response", "data");

            if (!data.HasValues)
                data = jsonHand.GetJToken("pins");
            foreach (var pin in data)
            {
                JToken jsonToken;
                if (pin.Count() == 1)
                    jsonToken = pin.First;
                else
                    jsonToken = pin;
                var objPinterestPin = new PinterestPin
                {
                    PinId = jsonHand.GetJTokenValue(jsonToken, "id"),
                    PinName = jsonHand.GetJTokenValue(jsonToken, "grid_title"),
                    BoardName = jsonHand.GetJTokenValue(jsonToken, "board", "name"),
                    BoardUrl = jsonHand.GetJTokenValue(jsonToken, "board", "url"),
                    PinWebUrl = jsonHand.GetJTokenValue(jsonToken, "link"),
                    MediaType = jsonHand.GetJToken(jsonToken, "link").HasValues ? MediaType.Video : MediaType.Image,
                    PublishDate = jsonHand.GetJTokenValue(jsonToken, "rich_summary", "created_at"),
                    MediaString = jsonHand.GetJTokenValue(jsonToken, "images", "736x", "url"),
                    User = { Username = jsonHand.GetJTokenValue(jsonToken, "pinner", "username") },
                    Description = jsonHand.GetJTokenValue(jsonToken, "description")
                };
                PinList.Add(objPinterestPin);
            }
        }

        public bool HasMoreResults { get; set; }
        public string BookMark { get; set; }
        public List<PinterestPin> PinList { get; } = new List<PinterestPin>();
    }
}