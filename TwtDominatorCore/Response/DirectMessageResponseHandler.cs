using DominatorHouseCore.Interfaces;
using TwtDominatorCore.TDModels;

namespace TwtDominatorCore.Response
{
    public class DirectMessageResponseHandler : TdResponseHandler
    {
        public DirectMessageResponseHandler(IResponseParameter response) : base(response)
        {
            // here Success means we get some response
            // if not get response simply return false
            // if response.Response not contains this message means user must not suspended 
            // otherwise send error message to get error
            if (!Success || !response.Response.Contains("Sorry! We did something wrong"))
                return;

            Issue = new TwitterIssue
            {
                Message = "Sorry!We did something wrong"
            };
            Success = false;
        }
    }
}