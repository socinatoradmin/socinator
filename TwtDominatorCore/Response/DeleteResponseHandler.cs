using DominatorHouseCore.Interfaces;

namespace TwtDominatorCore.Response
{
    public class DeleteResponseHandler : TdResponseHandler
    {
        public DeleteResponseHandler(IResponseParameter response) : base(response)
        {
            // if success 
            // check response not empty, must contains user_id
            if (!Success)
                return;
            Success = response != null && response.Response.Contains("\"id_str\":") ||
                      !string.IsNullOrEmpty(response.Response) && response.Response.Contains("created_at") ||response.Response.Contains("\"delete_tweet\":{\"tweet_results\"");
        }
    }
}