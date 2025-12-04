using DominatorHouseCore.Interfaces;
using TwtDominatorCore.TDUtility;

namespace TwtDominatorCore.Response
{
    public class CommentResponseHandler : TdResponseHandler
    {
        public CommentResponseHandler(IResponseParameter response) : base(response)
        {
            if (!Success)
                return;

            Success = !string.IsNullOrEmpty(TweetId = TdUtility.GetTweetOrUserId(response.Response, "id"));
        }

        public string TweetId { get; set; }
    }
}