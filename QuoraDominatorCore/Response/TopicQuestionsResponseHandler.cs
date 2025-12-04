using DominatorHouseCore;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Utility;
using System;
using System.Collections.Generic;
using System.Linq;

namespace QuoraDominatorCore.Response
{
    public class TopicQuestionsResponseHandler : QuoraResponseHandler
    {
        public List<QuestionDetails> QuestionsList = new List<QuestionDetails>();
        public TopicQuestionsResponseHandler(IResponseParameter response,bool IsBrowser=true) : base(response)
        {
            try
            {
                if (IsBrowser)
                {
                    var Nodes = HtmlParseUtility.GetListNodesFromClassName(response.Response, "qu-hover--textDecoration--underline answer_timestamp");
                    if (Nodes != null && Nodes.Count > 0)
                        Nodes.ForEach(node =>
                        {
                            var url = node.GetAttributeValue("href", string.Empty);
                            if (!string.IsNullOrEmpty(url) && url.Contains("-"))
                            {
                                var index = url.IndexOf("/answer/");
                                url = index >= 0 ? url.Substring(0, index) : url;
                                QuestionsList.Add(new QuestionDetails { QuestionUrl = url });
                            }
                        });
                }
                else
                {
                    var jsonObject = jsonHandler.ParseJsonToJObject(response.Response);
                    var Questions = jsonHandler.GetJArrayElement(jsonHandler.GetJTokenValue(jsonObject, "data", "multifeedObject", "multifeedConnection", "edges"));
                    if (Questions != null && Questions.Count > 0)
                        Questions.ForEach(Question =>
                        {
                            var Details = jsonHandler.GetJTokenOfJToken(Question, "node", "bundleConnection", "edges", 0, "node", "answer", "question");
                            var questionUrl = jsonHandler.GetJTokenValue(Details, "url");
                            if (!string.IsNullOrEmpty(questionUrl))
                                QuestionsList.Add(new QuestionDetails { QuestionUrl = questionUrl.StartsWith("https://") ? questionUrl : $"https://www.quora.com{questionUrl}" });
                        });
                }
                QuestionsList.Distinct();
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }
    }
}
