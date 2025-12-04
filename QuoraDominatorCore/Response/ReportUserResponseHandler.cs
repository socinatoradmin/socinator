using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Utility;

namespace QuoraDominatorCore.Response
{
    public class ReportUserResponseHandler : QuoraResponseHandler
    {
        public string Status;
        public ReportUserResponseHandler(IResponseParameter response) : base(response)
        {
            if(response == null || string.IsNullOrEmpty(response.Response))
            {
                Success=false;
                return;
            }
            Success = response.Response.Contains("\"viewerHasReported\": true");
            Status = Utilities.GetBetween(response.Response, "status\": \"", "\"");
        }
    }
}