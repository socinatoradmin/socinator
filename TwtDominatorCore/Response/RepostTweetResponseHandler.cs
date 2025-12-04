using DominatorHouseCore.Interfaces;
using TwtDominatorCore.TDUtility;

namespace TwtDominatorCore.Response
{
    public class RepostTweetResponseHandler: TdResponseHandler
    {
        public string TweetId { get; set; }
        public RepostTweetResponseHandler(IResponseParameter response) : base(response)
        {
            if (!Success)
                return;
            Success = !string.IsNullOrEmpty(TweetId = TdUtility.GetTweetOrUserId(response.Response));
        }
    }
}
