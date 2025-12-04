using DominatorHouseCore.Interfaces;
using FaceDominatorCore.FDLibrary.FdFunctions;
using FaceDominatorCore.FDResponse.BaseResponse;

namespace FaceDominatorCore.FDResponse.FriendsResponse
{
    public class CancelIncomingRequestResponseHandler : FdResponseHandler
    {
        public bool IsCancelledRequest { get; set; }

        public CancelIncomingRequestResponseHandler(IResponseParameter responseParameter)
            : base(responseParameter)
        {
            if (responseParameter.HasError || responseParameter.Response == null)
                return;

            var decodedResponse = FdFunctions.GetDecodedResponse(responseParameter.Response);

            IsCancelledRequest = decodedResponse.Contains("\"success\":true");
        }
    }
}
