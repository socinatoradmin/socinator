/*
using FaceDominatorCore.FDResponse.BaseResponse;
using System.Collections.Generic;
using DominatorHouseCore.Interfaces;
using FaceDominatorCore.FDLibrary.FdClassLibrary;
using FaceDominatorCore.FDLibrary.FdFunctions;

namespace FaceDominatorCore.FDResponse.GroupsResponse
{
    public class GroupDateDetailsResponseHandler : FdResponseHandler
    {
        public GroupDateDetailsResponseHandler(IResponseParameter responseParameter) 
            : base(responseParameter)
        {
            var decodedResponse = FdFunctions.GetDecodedResponse(responseParameter.Response);
            HtmlAgilityPack.HtmlDocument htmlDocument = new HtmlAgilityPack.HtmlDocument();
            htmlDocument.LoadHtml(decodedResponse);
            
        }
        public List<GroupDetails> ListGroupDetails = new List<GroupDetails>();
    }
}
*/
