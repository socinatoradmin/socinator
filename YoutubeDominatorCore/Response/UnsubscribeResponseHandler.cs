using DominatorHouseCore.Interfaces;

namespace YoutubeDominatorCore.Response
{
    public class UnsubscribeResponseHandler : YdResponseHandler
    {
        public UnsubscribeResponseHandler(bool success)
        {
            Success = success;
        }

        public UnsubscribeResponseHandler(IResponseParameter response)
        {
            Success = response?.Response.Contains("\"code\":\"SUCCESS\"") ?? false;
        }
    }
}