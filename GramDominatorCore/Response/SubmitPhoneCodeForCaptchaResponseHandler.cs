using DominatorHouseCore.Interfaces;
using GramDominatorCore.GDLibrary.Response;

namespace GramDominatorCore.Response
{
    public class SubmitPhoneCodeForCaptchaResponseHandler : IGResponseHandler
    {
        public SubmitPhoneCodeForCaptchaResponseHandler(IResponseParameter response)
            : base(response)
        {
            if (!response.Response.ToString().Contains("instagram://checkpoint/dismiss"))
                return;
            type = RespJ["type"].ToString();
            Status = RespJ["status"].ToString();
            //Location = RespJ["location"].ToString();
        }
        public string type { get; set; }
        public string Status { get; set; }
        public string Location { get; set; }
    }
}
