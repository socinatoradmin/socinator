using DominatorHouseCore.Interfaces;

namespace TwtDominatorCore.Response
{
    // for private user accounts
    public class AcceptPendingReqResponseHandler : TdResponseHandler
    {
        public AcceptPendingReqResponseHandler(IResponseParameter response) : base(response)
        {
            // if success 
            // check response not empty, must contains user_id
            Success = Success && response != null && !string.IsNullOrEmpty(response.Response) &&
                      response.Response.Contains("\"user_id\":");
        }
    }
}