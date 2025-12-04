using DominatorHouseCore.Interfaces;

namespace QuoraDominatorCore.Response
{
    public class PostUpvoteResponseHandler : QuoraResponseHandler
    {
        public PostUpvoteResponseHandler(IResponseParameter response, bool isBrowser=false) : base(response)
        {
        }
    }
}