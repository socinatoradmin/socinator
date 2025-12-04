/*
using DominatorHouseCore.Interfaces;
using FaceDominatorCore.FDLibrary.FdFunctions;
using FaceDominatorCore.FDResponse.BaseResponse;
using HtmlAgilityPack;

namespace FaceDominatorCore.FDResponse.FriendsResponse
{

    public class LocationResponseHandler : FdResponseHandler
    {
        public string Location { get; set; } = string.Empty;

        public LocationResponseHandler(IResponseParameter responseParameter)
            : base(responseParameter)
        {
            var decodedResponse = FdFunctions.GetDecodedResponse(responseParameter.Response);

            HtmlDocument objDocument = new HtmlDocument();

            objDocument.LoadHtml(decodedResponse);

            HtmlNodeCollection objHtmlNodeCollection = objDocument.DocumentNode.SelectNodes("//div[starts-with(@data-overviewsection, 'places')]");

            if (objHtmlNodeCollection != null)
            {
                var locationDetails = objHtmlNodeCollection[0].InnerHtml;

                objDocument.LoadHtml(locationDetails);

                objHtmlNodeCollection = objDocument.DocumentNode.SelectNodes("//a[starts-with(@class, 'profileLink')]");

                if (objHtmlNodeCollection != null)
                {
                    Location = objHtmlNodeCollection[0].InnerHtml;
                }
            }
        }
    }
}
*/
