using System;
using DominatorHouseCore;
using DominatorHouseCore.Interfaces;
using QuoraDominatorCore.QdUtility;

namespace QuoraDominatorCore.Response
{
    public class AnswerOnQuestionResponseHandler : QuoraResponseHandler
    {
        public string AnswerUrls = string.Empty;
        private readonly JsonJArrayHandler handler = JsonJArrayHandler.GetInstance;
        public string Message = string.Empty;
        public AnswerOnQuestionResponseHandler(IResponseParameter response) : base(response)
        {
            try
            {
                var json = handler.ParseJsonToJObject(response.Response);
                bool.TryParse(handler.GetJTokenValue(json, "data", "answerSubmit", "success"), out bool isSuccess);
                Success = isSuccess;
                var url = handler.GetJTokenValue(json, "data", "answerSubmit", "redirectUrl");
                if(!string.IsNullOrEmpty(url))
                    AnswerUrls = $"{QdConstants.HomePageUrl}{url}";
                Message = handler.GetJTokenValue(json, "data", "answerSubmit", "msg");
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }
    }
}