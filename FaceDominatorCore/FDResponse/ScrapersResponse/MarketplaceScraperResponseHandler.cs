using DominatorHouseCore.Interfaces;
using FaceDominatorCore.FDLibrary.FdClassLibrary;
using FaceDominatorCore.FDResponse.BaseResponse;
using System.Collections.Generic;

namespace FaceDominatorCore.FDResponse.ScrapersResponse
{
    public class MarketplaceScraperResponseHandler : FdResponseHandler
    {
        public List<MarketplaceModel> ListMarketplaceDetails { get; set; } = new List<MarketplaceModel>();

        public string SessionCookies { get; set; } = string.Empty;

        public MarketplaceScraperResponseHandler(IResponseParameter responseParameter)
            : base(responseParameter)
        {
        }
    }
}
