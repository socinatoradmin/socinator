using DominatorHouseCore.Interfaces;
using RedditDominatorCore.RDLibrary;
using RedditDominatorCore.RDUtility;

namespace RedditDominatorCore.Response
{
    public class RdResponseHandler
    {
        public readonly JsonJArrayHandler jsonHandler = JsonJArrayHandler.GetInstance;
        public readonly string HomePage = RdConstants.NewRedditHomePageAPI;
        public RdResponseHandler(IResponseParameter response)
        {
            Success = true;
            Response = response.Response;
            HasError = HasError;
        }

        public bool Success { get; set; }
        public string Response { get; set; }
        public bool HasError { get; set; }
    }
}