using DominatorHouseCore.Interfaces;
using GramDominatorCore.GDLibrary.Response;

namespace GramDominatorCore.Response
{
    public class CommonIgResponseHandler : IGResponseHandler
    {
        public CommonIgResponseHandler(IResponseParameter response) : base(response)
        {
            if (string.IsNullOrEmpty(response?.Response) || !response.Response.Contains("\"status\":\"ok\""))
                Success = false;
        }
        public bool IsStoryAvailable { get; set; } = true;
    }
}