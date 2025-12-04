using DominatorHouseCore.Models.SocioPublisher;
using System.Collections.Generic;

namespace FaceDominatorCore.FDLibrary.FdClassLibrary
{
    public class PublisherParameter
    {
        public string ComposerId { get; set; }

        public Dictionary<string, string> MediaDictionary { get; set; }

        public string PageId { get; set; }

        public string Message { get; set; }

        public string WaterfallId { get; set; }

        public string TargetType { get; set; }

        public string WebPrivacy { get; set; }

        public string Currency { get; set; }

        public string LocationId { get; set; }

        public string[] Tags { get; set; }

        public string[] Mentions { get; set; }

        public PublisherPostlistModel PostDetails { get; set; }

    }
}
