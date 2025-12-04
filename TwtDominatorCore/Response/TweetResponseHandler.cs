using DominatorHouseCore.Interfaces;
using TwtDominatorCore.TDUtility;

namespace TwtDominatorCore.Response
{
    public class TweetResponseHandler : TdResponseHandler
    {
        public TweetResponseHandler(IResponseParameter response) : base(response)
        {
            if (!Success)
                return;
            Success = !string.IsNullOrEmpty(TweetId = TdUtility.GetTweetOrUserId(response.Response, tweetTrue: true));
        }

        public string TweetId { get; set; }
    }
}