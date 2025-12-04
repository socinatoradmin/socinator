using DominatorHouseCore;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Utility;
using GramDominatorCore.GDLibrary.Response;
using GramDominatorCore.GDModel;
using GramDominatorCore.GDUtility;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace GramDominatorCore.Response
{
    public class FeedIgResponseHandlerAlternate : IGResponseHandler
    {
        public FeedIgResponseHandlerAlternate(IResponseParameter response) : base(response)
        {
            if (!Success)
                return;
            if(Success && response.Response.Contains("<!DOCTYPE html>"))
            {
                if (response.Response.Contains("end_cursor"))
                    MaxId = Utilities.GetBetween(response.Response, "end_cursor\":\"", "\"");
                Success = true;
                return;
            }
            try
            {
                var obj = handler.ParseJsonToJObject(response?.Response);
                var Data = handler.GetJTokenOfJToken(obj, "data", "xdt_location_get_web_info_tab");
                var pageInfo = handler.GetJTokenOfJToken(Data, "page_info");
                MaxId = handler.GetJTokenValue(pageInfo, "end_cursor");
                bool.TryParse(handler.GetJTokenValue(pageInfo, "has_next_page"), out bool hasMore);
                MoreAvailable = hasMore;
                var LocationsData = handler.GetJArrayElement(handler.GetJTokenValue(Data, "edges"));
                if(LocationsData != null && LocationsData.HasValues)
                {
                    Sections.AddRange(LocationsData.GetImageData(true,true));
                }
            }
            catch (Exception ex)
            {

                ex.DebugLog();
            }
        }
        public string MaxId { get; }

        public bool MoreAvailable { get; }

        public List<InstagramPost> Sections { get; set; } = new List<InstagramPost>();

        public string NextPage { get; }
    }
}
