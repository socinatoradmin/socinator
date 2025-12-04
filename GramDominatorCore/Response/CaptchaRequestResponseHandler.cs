using DominatorHouseCore.Interfaces;
using GramDominatorCore.GDLibrary.Response;

namespace GramDominatorCore.Response
{
    public class CaptchaRequestResponseHandler : IGResponseHandler
    {
        public CaptchaRequestResponseHandler(IResponseParameter response)
            : base(response)
        {
            if (!Success)
                return;
            challenge_context = handler.GetJTokenValue(RespJ, "challenge", "challenge_context");
            challengeType = handler.GetJTokenValue(RespJ, "challenge", "challengeType");
            siteKey = handler.GetJTokenValue(RespJ, "fields", "sitekey");
        }
        public string challengeType { get; set; }
        public string Error { get; set; }
        public string siteKey { get; set; }
        public string challenge_context { get; set; }
    }
}
