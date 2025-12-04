using DominatorHouseCore.Interfaces;
using TwtDominatorCore.TDUtility;

namespace TwtDominatorCore.Response
{
    public class RetweetResponseHandler : TdResponseHandler
    {
        public string TweetId {  get; set; }
        public RetweetResponseHandler(IResponseParameter response) : base(response)
        {
            if (!Success)
                return;
            Success = !string.IsNullOrEmpty(TweetId = TdUtility.GetTweetOrUserId(response.Response));
        }
    }
}