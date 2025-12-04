using DominatorHouseCore.Interfaces;
using GramDominatorCore.GDLibrary.Response;

namespace GramDominatorCore.Response
{
    public class LocationIdIgReponseHandler : IGResponseHandler
    {
        public LocationIdIgReponseHandler(IResponseParameter response) : base(response)
        {
            if (!Success)
                return;
            if (response.Response.Contains("\"items\": [], \"status\": \"ok\""))
            {

            }
            else
            {
                var tokenVenues = handler.ParseJsonToJObject(handler.GetJTokenValue(RespJ, "location"));
                Latitude = handler.GetJTokenValue(tokenVenues,"lat");
                Longitude = handler.GetJTokenValue(tokenVenues,"lng");
                ExternalSource = handler.GetJTokenValue(tokenVenues, "external_source");
                FacebookPlaceId = handler.GetJTokenValue(tokenVenues, "facebook_places_id");
                Name = handler.GetJTokenValue(tokenVenues, "name");
                Address = handler.GetJTokenValue(tokenVenues, "address");
            }
        }
        public string Latitude { get; set; }

        public string Longitude { get; set; }

        public string ExternalSource { get; set; }

        public string FacebookPlaceId { get; set; }

        public string Name { get; set; }

        public string Address { get; set; }
    }
   
}
