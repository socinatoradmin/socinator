using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Utility;
using FaceDominatorCore.FDLibrary.FdFunctions;
using FaceDominatorCore.FDResponse.BaseResponse;

namespace FaceDominatorCore.FDResponse.FriendsResponse
{
    public class SendRequestResponseHandler : FdResponseHandler
    {
        public string RequestStatus = string.Empty;

        public SendRequestResponseHandler(IResponseParameter responseParameter)
            : base(responseParameter)
        {

            if (responseParameter.HasError || responseParameter.Response == null)
                return;

            var decodedResponse = FdFunctions.GetDecodedResponse(responseParameter.Response);

            if (decodedResponse.ToLower().Contains("Confirmation Required".ToLower()))
                RequestStatus = "confirmation required";
            else if (decodedResponse.ToLower().Contains("success\":true".ToLower()) || decodedResponse.ToLower().Contains("OUTGOING_REQUEST".ToLower()))
                RequestStatus = "success";
            else if (decodedResponse.ToLower().Contains("{\"text\":\"Following\"}".ToLower()))
                RequestStatus = "successfollowing";
            else if (decodedResponse.ToLower().Contains("Already Sent Request".ToLower()))
                RequestStatus = "Already sent request to user";
            else if (decodedResponse.Contains("error\":1407036"))
                RequestStatus = "Request blocked by facebook to user";
            else if (decodedResponse.Contains("error\":1407036"))
                RequestStatus = "Can't send request to user";
            else if (decodedResponse.Contains("code\":1407036"))
                RequestStatus = "Can't send request to user";
            else if (decodedResponse.Contains("You can't send yourself a friend request"))
                RequestStatus = "You can't send yourself a friend request";
            else if (decodedResponse.Contains("Can't send request"))
                RequestStatus = "Can't send request to user";
            else
                RequestStatus = Utilities.GetBetween(decodedResponse, "errorDescription\":\"", "\"");
        }
    }
}
