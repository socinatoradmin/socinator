using DominatorHouseCore;
using DominatorHouseCore.Interfaces;
using QuoraDominatorCore.QdUtility;
using System;
using System.Linq;

namespace QuoraDominatorCore.Response
{
    public class PostQuestionResponseHandler : QuoraResponseHandler
    {
        public string QuestionUrl { get; set; } = string.Empty;
        public bool AlreadyAsked { get; set; } = false;
        public PostQuestionResponseHandler(IResponseParameter response) : base(response)
        {
            try
            {
                var jsonObject = jsonHandler.ParseJsonToJObject(response.Response);
                var DuplicateQuestion = jsonHandler.GetJArrayElement(jsonHandler.GetJTokenValue(jsonObject, "data", "questionCreate", "duplicateSuggestions"));
                var ExistingQuestion = jsonHandler.GetJArrayElement(jsonHandler.GetJTokenValue(jsonObject, "data", "questionCreate", "existingQuestion"));
                if (DuplicateQuestion != null && DuplicateQuestion.HasValues ||ExistingQuestion!=null && ExistingQuestion.HasValues)
                {
                    var QuestionDetails =DuplicateQuestion!=null? DuplicateQuestion.FirstOrDefault():jsonHandler.GetJTokenOfJToken(jsonObject, "data", "questionCreate", "existingQuestion");
                    QuestionUrl = $"{QdConstants.HomePageUrl}{jsonHandler.GetJTokenValue(QuestionDetails, "url")}";
                    AlreadyAsked = true;
                }
                else
                {
                    var Url = jsonHandler.GetJTokenValue(jsonObject, "data", "question", "url");
                    Url = string.IsNullOrEmpty(Url) ?jsonHandler.GetJTokenValue(jsonObject, "data", "questionCreate", "question","url") : Url;
                    //Url from Create PostResponse.
                    Url = string.IsNullOrEmpty(Url) ?jsonHandler.GetJTokenValue(jsonObject, "data", "postAdd", "post","url") : Url;
                    QuestionUrl = $"{QdConstants.HomePageUrl}{Url}";
                }
            }
            catch (Exception ex)
            { ex.DebugLog(); }
        }
    }
}