using DominatorHouseCore.Interfaces;
using TwtDominatorCore.TDUtility;

namespace TwtDominatorCore.Response
{
    public class LikeResponseHandler : TdResponseHandler
    {
        public LikeResponseHandler(IResponseParameter response) : base(response)
        {
            if (!Success)
                return;
            Success = !string.IsNullOrEmpty(TdUtility.GetTweetOrUserId(response.Response));
        }
    }
}