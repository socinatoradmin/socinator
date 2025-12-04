using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Utility;

namespace PinDominatorCore.Response
{
    public class SwitchProfileRespHandler : PdResponseHandler
    {
        public SwitchProfileRespHandler(IResponseParameter response) : base(response)
        {
            try
            {
                if (string.IsNullOrEmpty(response.Response))
                {
                    Success = false;
                    return;
                }

                var jsonHand = new JsonHandler(response.Response);
                var status = jsonHand.GetJToken("resource_response", "status")?.ToString();

                if (status != null && status.Equals("success"))
                    Success = true;
            }
            catch
            {
                // ignored
            }
        }
    }
}
