using DominatorHouseCore.Interfaces;

namespace QuoraDominatorCore.Response
{
    public class DownvoteQuestionResponseHandler : QuoraResponseHandler
    {
        public DownvoteQuestionResponseHandler(IResponseParameter response) : base(response)
        {
            if (string.IsNullOrEmpty(response.Response))
            {
                Success = false;
                return;
            }
            else if (response.Response.Contains("\"viewerHasDownvoted\": true"))
                Success = true;
        }
    }
}