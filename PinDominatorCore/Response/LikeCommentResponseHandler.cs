using DominatorHouseCore.Interfaces;
using PinDominatorCore.PDUtility;

namespace PinDominatorCore.Response
{
    public class LikeCommentResponseHandler : PdResponseHandler
    {
        public LikeCommentResponseHandler(IResponseParameter response) : base(response)
        {
            if (string.IsNullOrEmpty(response.Response))
            {
                Success = false;
                return;
            }

            var jsonHand = new DominatorHouseCore.Utility.JsonHandler(response.Response);
            var error = jsonHand.GetJToken("resource_response", "error");
            if (error.HasValues)
                Success = false;
        }
    }
}