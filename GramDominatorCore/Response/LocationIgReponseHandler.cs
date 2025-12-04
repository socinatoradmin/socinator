using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Models;
using GramDominatorCore.GDLibrary.Response;
using GramDominatorCore.GDModel;
using System.Linq;

namespace GramDominatorCore.Response
{
    public class LocationIgReponseHandler : IGResponseHandler
    {
        public Location location { get; set; }=new Location();
        public LocationIgReponseHandler(IResponseParameter response,string LocationName="") : base(response)
        {
            if (!Success)
                return;
            {
                var obj = handler.ParseJsonToJObject(response?.Response);
                var locations = handler.GetJArrayElement(handler.GetJTokenValue(obj, "venues"));
                if(locations != null && locations.HasValues)
                {
                    var targetLocation = locations?.FirstOrDefault(x => handler.GetJTokenValue(x, "name") == LocationName)
                        ?? locations?.FirstOrDefault();
                    if(targetLocation != null)
                    {
                        location.Name = handler.GetJTokenValue(targetLocation, "name");
                        location.Address = handler.GetJTokenValue(targetLocation, "address");
                        location.Id = handler.GetJTokenValue(targetLocation, "external_id");
                        double.TryParse(handler.GetJTokenValue(targetLocation, "lat"), out double lat);
                        location.Lat = lat;
                        double.TryParse(handler.GetJTokenValue(targetLocation, "lng"), out double lng);
                        location.Lng = lng;
                    }
                }
                var tokenVenues = handler.ParseJsonToJObject(handler.GetJTokenValue(RespJ, "venues",0));
                Latitude = handler.GetJTokenValue(tokenVenues, "lat");
                Longitude = handler.GetJTokenValue(tokenVenues, "lng");
                ExternalSource = handler.GetJTokenValue(tokenVenues, "external_id_source");
                FacebookPlaceId = handler.GetJTokenValue(tokenVenues, "external_id");
            }
            
        }

        public string Latitude { get; set; }

        public string Longitude { get; set; }

        public string ExternalSource { get; set; }

        public string FacebookPlaceId { get; set; }
    }
}
