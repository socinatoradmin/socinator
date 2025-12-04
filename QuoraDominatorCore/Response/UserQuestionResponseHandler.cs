using System;
using System.Collections.Generic;
using DominatorHouseCore;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Utility;
using HtmlAgilityPack;
using QuoraDominatorCore.QdUtility;

namespace QuoraDominatorCore.Response
{
    public class UserQuestionResponseHandler : QuoraResponseHandler
    {
        public List<string> QuestionUrl { get; set; } = new List<string>();
        private readonly JsonJArrayHandler handler = JsonJArrayHandler.GetInstance;
        public int PaginationCount = 0;
        public bool HasMoreResults = false;
        public UserQuestionResponseHandler(IResponseParameter response,bool IsBrowser=true) : base(response)
        {
            try
            {
                if (IsBrowser)
                {
                    var htmlDoc = new HtmlDocument();
                    var decodedResponse = QdUtilities.GetDecodedResponse(response.Response);
                    htmlDoc.LoadHtml(decodedResponse);
                    var userQuestionNodes = HtmlUtility.GetListNodesFromClassName(response.Response, "puppeteer_test_link qu-display--block qu-cursor--pointer qu-hover--textDecoration--underline", "a");
                    if (userQuestionNodes != null && userQuestionNodes.Count > 0)
                        userQuestionNodes.ForEach(question =>
                        {
                            var questionUrl = Utilities.GetBetween(question.OuterHtml, "href=\"", "\"");
                            if (!string.IsNullOrEmpty(questionUrl) && !questionUrl.Contains("/q/"))
                                QuestionUrl.Add(questionUrl);
                        });
                }
                else
                {
                    var jsonObject = handler.ParseJsonToJObject(response.Response);
                    int.TryParse(handler.GetJTokenValue(jsonObject, "data", "user", "recentPublicQuestionsConnection", "pageInfo", "endCursor"), out int endCursor);
                    PaginationCount = endCursor;
                    bool.TryParse(handler.GetJTokenValue(jsonObject, "data", "user", "recentPublicQuestionsConnection", "pageInfo", "hasNextPage"), out bool hasMoreResults);
                    HasMoreResults = hasMoreResults;
                    var QuestionsList = handler.GetJArrayElement(handler.GetJTokenValue(jsonObject, "data", "user", "recentPublicQuestionsConnection", "edges"));
                    if (QuestionsList != null && QuestionsList.Count > 0)
                        QuestionsList.ForEach(Question =>
                        {
                            var questionUrl = handler.GetJTokenValue(Question, "node", "url");
                            if (!string.IsNullOrEmpty(questionUrl) && !questionUrl.Contains("/q/"))
                                QuestionUrl.Add(questionUrl.Contains("https://") ?questionUrl:$"https://www.quora.com{questionUrl}");
                        });
                }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }
    }
}