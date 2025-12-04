using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Utility;

namespace YoutubeDominatorCore.Response
{
    public abstract class YdResponseHandler
    {
        protected readonly IResponseParameter Response;
        public readonly JsonHandler handler = JsonHandler.GetInstance;

        protected YdResponseHandler()
        {
        }

        protected YdResponseHandler(IResponseParameter response)
        {
            if (string.IsNullOrWhiteSpace(response.Response))
                IsEmptyResponse = true;
            else if (response.Response.Contains("('captcha-form')"))
                CaptchaFound = true;
        }

        public bool Success { get; set; }
        public bool IsEmptyResponse { get; set; }

        public bool CaptchaFound { get; set; }

        public override string ToString()
        {
            if (!string.IsNullOrEmpty(Response.Response))
                return Response.Response;
            return string.Empty;
        }
    }
}