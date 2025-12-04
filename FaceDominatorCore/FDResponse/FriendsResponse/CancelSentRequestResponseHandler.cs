using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Utility;
using FaceDominatorCore.FDLibrary.FdFunctions;
using FaceDominatorCore.FDResponse.BaseResponse;

namespace FaceDominatorCore.FDResponse.FriendsResponse
{
    public class CancelSentRequestResponseHandler : FdResponseHandler
    {
        public bool IsCancelledRequest { get; set; }

        public string Error { get; set; } = string.Empty;

        public CancelSentRequestResponseHandler(IResponseParameter responseParameter)
            : base(responseParameter)
        {
            if (responseParameter.HasError || responseParameter.Response == null)
                return;

            var decodedResponse = FdFunctions.GetDecodedResponse(responseParameter.Response);

            if (decodedResponse.Contains("You are not currently friends"))
                Error = "LangKeyNotAFriend".FromResourceDictionary();

            else if (!decodedResponse.Contains("Confirmation Required"))
                IsCancelledRequest = true;
        }
    }
}
