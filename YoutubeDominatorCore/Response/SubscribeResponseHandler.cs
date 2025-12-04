using DominatorHouseCore.Interfaces;

namespace YoutubeDominatorCore.Response
{
    public class SubscribeResponseHandler : YdResponseHandler
    {
        public SubscribeResponseHandler(bool isSucces)
        {
            Success = isSucces;
        }

        public SubscribeResponseHandler(IResponseParameter response)
        {
            if (response?.Response != null)
                Success = response.Response.Contains("\"subscribed\":true") ||
                          response.Response.Contains("\"code\":\"SUCCESS\"");
            else
                Success = false;
        }
    }
}