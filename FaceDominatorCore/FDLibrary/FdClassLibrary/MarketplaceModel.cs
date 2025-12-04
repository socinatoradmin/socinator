using System.Collections.Generic;

namespace FaceDominatorCore.FDLibrary.FdClassLibrary
{
    public class MarketplaceModel
    {
        public string ProductTitle { get; set; } = string.Empty;

        public string ProductDescription { get; set; } = string.Empty;

        public LocationDetails LocationDetails { get; set; }

        public string Category { get; set; } = string.Empty;

        public string CategoryUrl { get; set; } = string.Empty;

        public string ProductPrice { get; set; }

        public List<string> ImageUrls { get; set; }

        public string PostId { get; set; }

        public string UserId { get; set; }

        public string UserName { get; set; }

    }
}
