using DominatorHouseCore.Interfaces;

namespace QuoraDominatorCore.Response
{
    public class UpvoteResponseHandler : QuoraResponseHandler
    {
        public string ErrorMessage=string.Empty;
        public UpvoteResponseHandler(IResponseParameter response) : base(response)
        {
            try
            {
                if (!Success)
                    return;
                if (response.Response.Contains("answerUpvoteAdd") || response.Response.Contains("{\"voteChange\": {\"status\": \"success\""))
                    Success = true;
                else if (response.Response.Contains("already_performed"))
                {
                    Success = false;
                    ErrorMessage = "Already Performed Activity";
                }
                else
                    Success = false;
            }
            catch (System.Exception)
            {
            }
        }
    }
}