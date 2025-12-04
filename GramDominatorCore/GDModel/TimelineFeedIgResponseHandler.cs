//using System;
//using System.Collections.Generic;
//using System.ComponentModel;
//using System.Linq;
//using DominatorHouseCore.Interfaces;
//using GramDominatorCore.GDUtility;
//using Newtonsoft.Json.Linq;

//namespace GramDominatorCore.GDModel
//{
//    [Localizable(false)]
//    public class TimelineFeedIgResponseHandler : GDLibrary.Response.IGResponseHandler
//    {
//        public TimelineFeedIgResponseHandler(IResponseParameter response)
//            : base(response)
//        {
//            if (!Success)
//                return;
//            MoreAvailable = Convert.ToBoolean(RespJ["more_available"].ToString());
//            JToken jtoken = RespJ["next_max_id"];
//            MaxId = jtoken?.ToString();
//            Items.AddRange(JArray.FromObject(RespJ["feed_items"].Select(x => x["media_or_ad"])).GetImageData());
//        }

//        public List<InstagramPost> Items { get; } = new List<InstagramPost>();

//        public string MaxId { get; }

//        public bool MoreAvailable { get; }
//    }
//}
