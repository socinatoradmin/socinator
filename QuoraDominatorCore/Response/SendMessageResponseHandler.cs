using DominatorHouseCore.Interfaces;

namespace QuoraDominatorCore.Response
{
    public class SendMessageResponseHandler : QuoraResponseHandler
    {
        public SendMessageResponseHandler(IResponseParameter response) : base(response)
        {
            if (string.IsNullOrEmpty(response.Response))
            {
                Success = false;
                return;
            }
            else if (response.Response.Contains("\"status\": \"success\""))
                Success = true;
            else
                Success = false;
        }
    }
}