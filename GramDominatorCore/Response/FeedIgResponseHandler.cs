using DominatorHouseCore.Interfaces;
using GramDominatorCore.GDLibrary.Response;
using GramDominatorCore.GDModel;
using GramDominatorCore.GDUtility;
using System.Collections.Generic;
using System.ComponentModel;

namespace GramDominatorCore.Response
{
    [Localizable(false)]
    public class FeedIgResponseHandler : IGResponseHandler
    {
        public FeedIgResponseHandler(IResponseParameter response)
            : base(response)
        {
            if (!Success)
                return;
            if(response.Response.Contains("<!DOCTYPE html>"))
            {
                Success = true;
                return;
            }
            bool.TryParse(handler.GetJTokenValue(RespJ, "more_available"), out bool moreAvailable);
            MoreAvailable = moreAvailable;
            MaxId = handler.GetJTokenValue(RespJ, "next_max_id");
            Success = true;
            var rankedItems = handler.GetJArrayElement(handler.GetJTokenValue(RespJ, "ranked_items"));
            if (rankedItems != null && rankedItems.HasValues)
                RankedItems.AddRange(rankedItems.GetImageData());
            var ItemsData = handler.GetJArrayElement(handler.GetJTokenValue(RespJ, "items"));
            if (ItemsData != null && !ItemsData.HasValues)
                Items.AddRange(ItemsData.GetImageData());
        }

        public List<InstagramPost> Items { get; set; } = new List<InstagramPost>();

        public string MaxId { get; set; }

        public bool MoreAvailable { get; }

        public List<InstagramPost> RankedItems { get; set; } = new List<InstagramPost>();
    }
}
