using System;
using System.Collections.Generic;
using System.Linq;
using DominatorHouseCore;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Interfaces;
using PinDominatorCore.PDModel;
using Newtonsoft.Json.Linq;
using PinDominatorCore.PDUtility;

namespace PinDominatorCore.Response
{
    public class PinsFromSpecificUserResponseHandler : PdResponseHandler
    {
        public List<PinterestPin> LstUserPin { get; } = new List<PinterestPin>();
        public bool HasMoreResults { get; set; }
        public string BookMark { get; set; }

        public PinsFromSpecificUserResponseHandler(IResponseParameter response) :
            base(response)
        {
            if (string.IsNullOrEmpty(response.Response))
            {
                Success = false;
                return;
            }

            var jsonHand = new DominatorHouseCore.Utility.JsonHandler(response.Response);
            var jsonDataToken = jsonHand.GetJToken("resource_response", "data");
            var jsonErrorToken = jsonHand.GetJToken("resource_response", "error");

            if (!jsonDataToken.HasValues && jsonErrorToken.HasValues)
            {
                Success = false;
                HasMoreResults = false;
                Issue = new PinterestIssue
                {
                    Message = jsonHand.GetJTokenValue(jsonErrorToken, "message")
                };
                return;
            }

            BookMark = jsonHand.GetJToken("resource_response", "bookmark")?.ToString();
            HasMoreResults = !BookMark.Contains("-end-");
            foreach (var token in jsonDataToken)
                try
                {
                    JToken jsonToken;
                    if (token.Count() == 1)
                        jsonToken = token.First;
                    else
                        jsonToken = token;
                    if (jsonToken.Count() >= 39)
                    {
                        var tryCount = 0;
                        int.TryParse(jsonHand.GetJTokenValue(jsonToken, "aggregated_pin_data", "did_it_data", "user_count"), out tryCount);
                        var objPinterestPin = new PinterestPin
                        {
                            PinId = jsonHand.GetJTokenValue(jsonToken, "id"),
                            Description = jsonHand.GetJTokenValue(jsonToken, "description"),
                            MediaString = jsonHand.GetJTokenValue(jsonToken, "images", "736x", "url"),
                            PinName = jsonHand.GetJTokenValue(jsonToken, "grid_title"),
                            MediaType = jsonHand.GetJTokenValue(jsonToken, "is_video") == "False" ? MediaType.Image : MediaType.Video,
                            PinWebUrl = jsonHand.GetJTokenValue(jsonToken, "link"),
                            BoardName = jsonHand.GetJTokenValue(jsonToken, "board", "name"),
                            User =
                            {
                                Username = jsonHand.GetJTokenValue(jsonToken, "pinner", "username"),
                                UserId = jsonHand.GetJTokenValue(jsonToken, "pinner", "id")
                            },
                            BoardUrl = jsonHand.GetJTokenValue(jsonToken, "board", "url"),
                            NoOfTried = tryCount,
                            PublishDate= jsonHand.GetJTokenValue(jsonToken, "created_at"),
                            AggregatedPinId = jsonHand.GetJTokenValue(jsonToken, "aggregated_pin_data", "id")
                        };

                        if (!string.IsNullOrEmpty(jsonHand.GetJTokenValue(jsonToken, "rich_summary")))
                            objPinterestPin.PinName = jsonHand.GetJTokenValue(jsonToken, "rich_summary", "display_name");

                        LstUserPin.Add(objPinterestPin);
                    }
                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                }
        }

    }
}