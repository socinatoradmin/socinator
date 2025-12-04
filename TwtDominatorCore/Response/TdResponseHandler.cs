using System;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Utility;
using Newtonsoft.Json.Linq;
using TwtDominatorCore.TDEnums;
using TwtDominatorCore.TDModels;
using TwtDominatorCore.TDUtility;
namespace TwtDominatorCore.Response
{
    public class TdResponseHandler
    {
        private readonly int errorCode;
        public TwitterIssue Issue;
        public readonly JsonJArrayHandler handler = JsonJArrayHandler.GetInstance;
        public string Domain=>TdConstants.Domain;
        public TdResponseHandler()
        {
        }

        public TdResponseHandler(IResponseParameter response)
        {
            var errorMessage = string.Empty;

            if (response.HasError)
                try
                {
                    Success = false;
                    Issue = new TwitterIssue
                    {
                        Message = response.Exception.Message,
                        Error = TwitterError.FailedRequest
                    };
                    return;
                }
                catch (Exception)
                {
                }

            if (response.Response.IsValidJson())
                try
                {
                    if (response.Response.Contains("errors") && !response.Response.Contains("{\"globalObjects\":{"))
                    {
                        var obj = handler.ParseJsonToJObject(response?.Response);
                        int.TryParse(handler.GetJTokenValue(obj, "errors",0, "code"), out errorCode);
                        errorMessage = handler.GetJTokenValue(obj, "errors", 0, "message");
                    }

                    //else if (response.Response.Contains("message"))
                    //    errorMessage = Newtonsoft.Json.Linq.JObject.Parse(response.Response)["message"].ToString();
                    if (!string.IsNullOrEmpty(errorMessage))
                    {
                        Success = false;
                        Issue = new TwitterIssue
                        {
                            Error = (TwitterError) Enum.ToObject(typeof(TwitterError), errorCode),
                            Message = errorMessage
                        };
                        return;
                    }
                }
                catch (Exception)
                {
                    Issue = new TwitterIssue
                    {
                        Message = "Failed request"
                    };
                    return;
                }

            if (!string.IsNullOrEmpty(response.Response))
                Success = true;
            else
                Issue = new TwitterIssue {Message = "Empty Response"};
        }

        public bool Success { get; set; }
    }
}