using DominatorHouseCore.Interfaces;
using TwtDominatorCore.TDUtility;

namespace TwtDominatorCore.Response
{
    public class MuteResponseHandler : TdResponseHandler
    {
        public MuteResponseHandler(IResponseParameter response) : base(response)
        {
            if (!Success)
                return;
            Success = !string.IsNullOrEmpty(TdUtility.GetTweetOrUserId(response.Response, "user", "id_str"));
        }
    }
}