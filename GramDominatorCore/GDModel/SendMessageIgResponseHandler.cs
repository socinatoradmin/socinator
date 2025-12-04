using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Utility;
using GramDominatorCore.GDLibrary.Response;
using GramDominatorCore.Utility;
using System;

namespace GramDominatorCore.GDModel
{
    public class SendMessageIgResponseHandler : IGResponseHandler
    {
        public string ErrorMessage {  get; set; }
        public SendMessageIgResponseHandler(IResponseParameter response,string Error_Message="") : base(response)
        {
            var obj = handler.ParseJsonToJObject(response?.Response);
            ThreadId = handler.GetJTokenValue(obj, "payload", "thread_id");
            if (!Success || !string.IsNullOrEmpty(Error_Message))
            {
                ErrorMessage = Error_Message;
                Success = false;
                if(!Success)
                    return;
            }
            if(response.Response.StartsWith("<!DOCTYPE"))
            {
                var res = HtmlParseUtility.GetInnerTextFromTagName(response.Response, "div", "class", "x6prxxf x1fc57z9 x1yc453h x126k92a x14ctfv");
                if (response.Response.Contains("<title>IGD message send error icon</title>"))
                    Success = false;
                if(string.IsNullOrEmpty(res))
                    Success = true;

                return;
            }
            if (!Success && (response != null && !string.IsNullOrEmpty(response?.Response) && response.Response.Contains("\"status\":\"ok\"")))
                Success = true;
        }
        public string ThreadId = string.Empty;
    }
}
