using DominatorHouseCore.Interfaces;
using GramDominatorCore.GDLibrary.Response;

namespace GramDominatorCore.Response
{
    public class SubmitPhoneNumberAfterCaptchaResponseHandler : IGResponseHandler
    {
        public SubmitPhoneNumberAfterCaptchaResponseHandler(IResponseParameter response)
            : base(response)
        {
            if (!response.Response.ToString().Contains("VerifySMSCodeFormForSMSCaptcha"))
                return;
            challenge_context = RespJ["challenge_context"].ToString();
            challengeType = RespJ["challengeType"].ToString();
            phoneNumber = RespJ["fields"]["phone_number"].ToString();
        }
        public string challengeType { get; set; }
        public string challenge_context { get; set; }
        public string phoneNumber { get; set; }
    }
}
