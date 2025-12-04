using DominatorHouseCore.Interfaces;
using TwtDominatorCore.TDUtility;

namespace TwtDominatorCore.Response
{
    public class UnlikeResponseHandler : TdResponseHandler
    {
        public UnlikeResponseHandler(IResponseParameter response) : base(response)
        {
            if (!Success)
                return;
            Success = !string.IsNullOrEmpty(TdUtility.GetTweetOrUserId(response.Response));
        }
    }
}