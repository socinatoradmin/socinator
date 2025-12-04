using DominatorHouseCore.Interfaces;
using FaceDominatorCore.FDLibrary.FdFunctions;
using FaceDominatorCore.FDResponse.BaseResponse;

namespace FaceDominatorCore.FDResponse.FriendsResponse
{
    public class AcceptFriendRequestResponseHandler : FdResponseHandler
    {
        public bool IsAcceptedRequest { get; set; }

        public AcceptFriendRequestResponseHandler(IResponseParameter responseParameter)
            : base(responseParameter)
        {
            if (responseParameter.HasError || responseParameter.Response == null)
                return;

            var decodedResponse = FdFunctions.GetDecodedResponse(responseParameter.Response);

            IsAcceptedRequest = decodedResponse.Contains("success\":true,");

        }
    }
}
