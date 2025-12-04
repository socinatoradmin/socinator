using DominatorHouseCore;
using DominatorHouseCore.Interfaces;
using System;
using System.Text.RegularExpressions;

namespace TumblrDominatorCore.TumblrResponseHandler
{
    public class MessageUserResponse : ResponseHandler
    {
        public IResponseParameter messageResponse = null;
        public MessageUserResponse()
        { }
        public MessageUserResponse(IResponseParameter responeParameter) : base(responeParameter)
        {
            try
            {
                if (string.IsNullOrEmpty(responeParameter.Response)) return;
                messageResponse = responeParameter;
                if (!string.IsNullOrEmpty(responeParameter.Response) &&
                    responeParameter.Response.Contains("\"msg\":\"OK\"")) Success = true;

                if (!string.IsNullOrEmpty(responeParameter.Response) && responeParameter.Response.Contains("\"detail\""))
                    ErrorMessage = Regex.Matches(responeParameter.Response, "\"detail\":\"(.*)\"")[0].Groups[1].Value;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        public string ErrorMessage { get; set; }
    }
}