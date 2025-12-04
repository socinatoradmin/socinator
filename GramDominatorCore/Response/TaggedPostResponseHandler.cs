using DominatorHouseCore.Interfaces;
using GramDominatorCore.GDLibrary.Response;
using GramDominatorCore.GDModel;
using GramDominatorCore.GDUtility;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

namespace GramDominatorCore.Response
{
    public class TaggedPostResponseHandler : IGResponseHandler
    {
        public TaggedPostResponseHandler(IResponseParameter response) : base(response)
        {
            if (!Success)
                return;
            if (Success && response.Response.Contains("<!DOCTYPE html>"))
            {    
                MaxId = "";
                return;
            }

            MoreAvailable = Convert.ToBoolean(RespJ["more_available"].ToString());
            JToken jtoken = RespJ["next_max_id"];
            MaxId = jtoken?.ToString();
            Success = true;            
            if (RespJ["items"] == null)
                return;
            Items.AddRange(((JArray)RespJ["items"]).GetImageData());

        }
        public List<InstagramPost> Items { get; set; } = new List<InstagramPost>();

        public bool MoreAvailable { get; }

        public string MaxId { get; set; }

    }
}
