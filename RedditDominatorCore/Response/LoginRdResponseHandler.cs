using DominatorHouseCore;
using DominatorHouseCore.Interfaces;
using Newtonsoft.Json.Linq;
using System;

namespace RedditDominatorCore.Response
{
    public class LoginRdResponseHandler : RdResponseHandler
    {
        public LoginRdResponseHandler(IResponseParameter response) : base(response)
        {
            try
            {
                if (Response != null)
                {
                    if (response.Response.Contains("{\"dest\": \"https://www.reddit.com/\"}")) return;

                    var jObject = JObject.Parse(Response);
                    var loginError = jObject["explanation"]?.ToString();
                    if (!Success || HasError || response.Response == null)
                    {
                        HasError = true;
                        Error = loginError?.Replace("[]", "");
                        return;
                    }

                    if (string.IsNullOrEmpty(loginError?.Replace("[]", ""))) return;
                    Error = jObject["explanation"]?.ToString();
                    HasError = true;
                }
                else
                {
                    HasError = true;
                }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        public string Error { get; set; } = string.Empty;
    }
}