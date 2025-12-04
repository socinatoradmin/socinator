using System;
using System.Collections.Generic;
using System.Linq;
using DominatorHouseCore;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Models;
using Newtonsoft.Json.Linq;
using PinDominatorCore.PDModel;
using PinDominatorCore.PDUtility;
using DominatorHouseCore.Utility;

namespace PinDominatorCore.Response
{
    public class SearchAllPinResponseHandler : PdResponseHandler
    {
        public List<PinterestPin> LstPin { get; } = new List<PinterestPin>();
        public bool HasMoreResults { get; set; }
        public string BookMark { get; set; }

        public SearchAllPinResponseHandler(IResponseParameter response) : base(response)
        {
            if (string.IsNullOrEmpty(response.Response))
            {
                Success = false;
                return;
            }

            var jsonHand = new DominatorHouseCore.Utility.JsonHandler(response.Response);
            JToken jsonToken = null;

            try
            {
                var dataToken = jsonHand.GetJToken("resource_data_cache");
                dataToken = dataToken.HasValues ? jsonHand.GetJToken(dataToken?.First(), "data", "results") : dataToken;
                if (dataToken.Count() <= 0)
                    dataToken = jsonHand.GetJToken("props", "initialReduxState", "pins");
                if (dataToken.Count() <= 0)
                    dataToken = jsonHand.GetJToken("initialReduxState", "pins");
                jsonToken = dataToken.HasValues ? dataToken : jsonHand.GetJToken("pins").HasValues ?
                    jsonHand.GetJToken("pins") : jsonHand.GetJToken("resource_response", "data", "results");

                var bookMark = jsonHand.GetElementValue("resource_data_cache", 0, "resource", "options", "bookmarks", 0);
                if (string.IsNullOrEmpty(bookMark))
                {
                    bookMark = jsonHand.GetElementValue("resource_response", "bookmark");
                }
                if (string.IsNullOrEmpty(bookMark))
                {
                    bookMark=Utilities.GetBetween(response.Response, "nextBookmark\":\"", "\"}}");
                }
                BookMark = bookMark;
                HasMoreResults = !bookMark.Contains("end");
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            if (jsonToken != null && jsonToken.Count()>0)
                foreach (JToken token in jsonToken)
                    try
                    {
                        JToken jToken = null;
                        jToken = token?.First()?.Count() >= 30 ? jToken = token?.First() : token;
                        if (token.Count() >= 30 || token?.First()?.Count() >= 30)
                        {
                            var isPromoted = jsonHand.GetJTokenValue(jToken, "is_promoted");
                            if (string.IsNullOrEmpty(isPromoted))
                                isPromoted = "True";
                            var objPinterestPin = new PinterestPin
                            {
                                PinId = jsonHand.GetJTokenValue(jToken, "id"),
                                BoardName = jsonHand.GetJTokenValue(jToken, "board", "name"),
                                BoardUrl = jsonHand.GetJTokenValue(jToken, "board", "url"),
                                MediaType = string.IsNullOrEmpty(jsonHand.GetJTokenValue(jToken, "videos"))
                                    ? MediaType.Image
                                    : MediaType.Video,
                                MediaString = jsonHand.GetJTokenValue(jToken, "images", "736x", "url"),
                                PinName = jsonHand.GetJTokenValue(jToken, "grid_title"),
                                PinWebUrl = jsonHand.GetJTokenValue(jToken, "link"),
                                User = new PinterestUser()
                                {
                                    Username = jsonHand.GetJTokenValue(jToken, "pinner", "username"),
                                    UserId = jsonHand.GetJTokenValue(jToken, "pinner", "id")
                                },
                                Description = jsonHand.GetJTokenValue(jToken, "description"),
                                PublishDate = jsonHand.GetJTokenValue(jToken, "created_at"),
                                AggregatedPinId=jsonHand.GetJTokenValue(jToken, "aggregated_pin_data","id")
                            };
                            if (!isPromoted.Equals("True"))
                                LstPin.Add(objPinterestPin);
                        }
                    }
                    catch (Exception ex)
                    {
                        ex.DebugLog();
                    }

            if (LstPin.Count > 0)
                Success = true;
        }
    }
}