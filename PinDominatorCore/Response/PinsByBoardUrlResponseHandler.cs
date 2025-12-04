using DominatorHouseCore;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Interfaces;
using PinDominatorCore.PDModel;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PinDominatorCore.Response
{
    public class PinsByBoardUrlResponseHandler : PdResponseHandler
    {
        public List<PinterestPin> LstBoardPin { get; } = new List<PinterestPin>();
        public bool HasMoreResults { get; set; }
        public string BookMark { get; set; }

        public PinsByBoardUrlResponseHandler(IResponseParameter response) : base(response)
        {
            try
            {
                if (string.IsNullOrEmpty(response.Response))
                {
                    Success = false;
                    return;
                }

                var jsonHand = new DominatorHouseCore.Utility.JsonHandler(response.Response);
                var firstOne = jsonHand.GetJToken("resource_data_cache").Count() > 0 ? jsonHand.GetJToken("resource_data_cache")?.First() : jsonHand.GetJToken("resource_response");
                var jsonToken = jsonHand.GetJTokenOfJToken(firstOne, "data");

                foreach (var token in jsonToken)
                    if (token.Count() > 25)
                        try
                        {
                            long.TryParse(jsonHand.GetJTokenValue(token, "id"),out long Id);
                            if ( Id> 0)
                            {
                                var objPinterestPin = new PinterestPin
                                {
                                    PinId = Id.ToString(),
                                    BoardName = jsonHand.GetJTokenValue(token, "board", "name"),
                                    BoardUrl = jsonHand.GetJTokenValue(token, "board", "url"),
                                    MediaType = jsonHand.GetJTokenOfJToken(token, "videos").HasValues
                                    ? MediaType.Video
                                    : MediaType.Image,
                                    MediaString = jsonHand.GetJTokenValue(token, "images", "736x", "url"),
                                    PinName = jsonHand.GetJTokenValue(token, "grid_title"),
                                    PinWebUrl = jsonHand.GetJTokenValue(token, "link"),
                                    User = new PinterestUser
                                    {
                                        Username = jsonHand.GetJTokenValue(token, "pinner", "username"),
                                        UserId = jsonHand.GetJTokenValue(token, "pinner", "id")
                                    },
                                    Description = jsonHand.GetJTokenValue(token, "description"),
                                    PublishDate = jsonHand.GetJTokenValue(token, "created_at"),
                                    AggregatedPinId = jsonHand.GetJTokenValue(token, "aggregated_pin_data", "id")
                                };
                                LstBoardPin.Add(objPinterestPin);
                            }
                        }
                        catch (Exception ex)
                        {
                            ex.DebugLog();
                        }

                BookMark = jsonHand.GetJToken("resource", "options", "bookmarks")?.First()?.ToString();
                HasMoreResults = BookMark != null && !BookMark.Contains("-end-");
            }
            catch (Exception e)
            {
                e.DebugLog();
            }
        }

    }
}