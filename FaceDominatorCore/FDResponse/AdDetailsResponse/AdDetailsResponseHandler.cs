using DominatorHouseCore.Interfaces;
using FaceDominatorCore.FDLibrary.FdClassLibrary;
using FaceDominatorCore.FDResponse.BaseResponse;
using FaceDominatorCore.Interface;

namespace FaceDominatorCore.FDResponse.AdDetailsResponse
{

    public class AdDetailsResponseHandler : FdResponseHandler, IResponseHandler
    {
        public bool HasMoreResults { get; set; }

        public string EntityId { get; set; }

        public string PageletData { get; set; }

        public bool Status { get; set; }

        public FdScraperResponseParameters ObjFdScraperResponseParameters { get; set; } = new FdScraperResponseParameters();

        public AdDetailsResponseHandler(IResponseParameter responseParameter)
            : base(responseParameter)
        {

            if (responseParameter.HasError || responseParameter.Response == null)
                return;

            ObjFdScraperResponseParameters.FacebookAdsDetails = new FacebookAdsDetails();

            ObjFdScraperResponseParameters.FacebookAdsDetails.AdId = "3070956";

            ObjFdScraperResponseParameters.FacebookAdsDetails.Id = "2832624913474598";

            ObjFdScraperResponseParameters.FacebookAdsDetails.PostUrl = $"{FdConstants.FbHomeUrl}2832624913474598";
        }
    }
}
