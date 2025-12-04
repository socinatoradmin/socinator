using DominatorHouseCore.Interfaces;

namespace QuoraDominatorCore.Response
{
    public class ReportAnswerResponseHandler : QuoraResponseHandler
    {
        public ReportAnswerResponseHandler(IResponseParameter response) : base(response)
        {
            if(response == null || string.IsNullOrEmpty(response.Response))
            {
                Success = false;
                return;
            }
            Success = response.Response.Contains("\"viewerHasReported\": true");
        }
    }
}