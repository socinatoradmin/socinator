using DominatorHouseCore.Enums;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Models.SocioPublisher;
using System.Collections.Generic;

namespace PinDominatorCore.PDModel
{
    public class PinterestPin : IPost
    {
        public string PinId { get; set; } = string.Empty;
        public string PinName { get; set; } = string.Empty;
        public string BoardName { get; set; } = string.Empty;
        public string BoardUrl { get; set; } = string.Empty;
        public PinterestUser User { get; set; } = new PinterestUser();
        public int CommentCount { get; set; }
        public int NoOfTried { get; set; }
        public string Description { get; set; } = string.Empty;
        public string PinWebUrl { get; set; } = string.Empty;
        public string MediaString { get; set; } = string.Empty;
        public MediaType MediaType { get; set; }
        public string PublishDate { get; set; } = string.Empty;
        public string BoardUrlToRepin { get; set; } = string.Empty;
        public string Section { get; set; } = string.Empty;
        public string Id { get; set; } = string.Empty;

        public string Caption { get; set; } = string.Empty;

        public string Code { get; set; }
        public string AggregatedPinId { get; set; }
        public List<SectionDetails> LstSection { get; set; }
        public override int GetHashCode()
        {
            return 0;
        }
    }
}