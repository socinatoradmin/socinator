using System;
using System.Collections.Generic;
using DominatorHouseCore;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Utility;
using QuoraDominatorCore.QdUtility;

namespace QuoraDominatorCore.Response
{
    public class AnswerByTopicResponseHandler : QuoraResponseHandler
    {
        public List<AnswerDetails> AnswerList = new List<AnswerDetails>();
        public bool HasMoreResults { get; set; }
        public string MultifeedAfter { get; set; }
        public string PageData { get; set; }
        public AnswerByTopicResponseHandler(IResponseParameter response,bool IsBrowser=true) : base(response)
        {
            try
            {
                if (IsBrowser)
                {
                    if (response.Response.Contains("permaUrl\":"))
                    {
                        var AnswerUrl = Utilities.GetBetween(response.Response, "permaUrl\":", ",").Replace("\"", "");
                        AnswerUrl = QdConstants.HomePageUrl + AnswerUrl;
                        AnswerList.Add(new AnswerDetails { AnswerUrl = AnswerUrl });
                    }
                    else
                    {
                        if (RespJ == null)
                        {
                            var ans = HtmlUtility.GetListNodesFromClassName(response.Response, "answer_timestamp qu-cursor--pointer qu-hover--textDecoration--underline", "a");
                            foreach (var ansUrl in ans)
                            {
                                var AnswerUrl = Utilities.GetBetween(ansUrl.OuterHtml, "href=\"", "\"");
                                if (AnswerUrl.StartsWith("https://www.quora.com/") || (AnswerUrl.Contains(".quora.com/") && AnswerUrl.Contains("answer")))
                                    AnswerList.Add(new AnswerDetails { AnswerUrl = AnswerUrl });
                            }
                        }
                        else
                        {
                            var s = RespJ["value"].ToString();
                            var AnswerUrl = QdConstants.HomePageUrl +Utilities.GetBetween(s, "AnswerPermalinkClickthrough", ">").Split('\'')[2];
                            AnswerList.Add(new AnswerDetails { AnswerUrl = AnswerUrl });
                        }
                    }
                }
                else
                {
                    var jsonObject = jsonHandler.ParseJsonToJObject(response.Response);
                    var Answers = jsonHandler.GetJArrayElement(jsonHandler.GetJTokenValue(jsonObject, "data", "multifeedObject", "multifeedConnection","edges"));
                    if (Answers != null && Answers.Count > 0)
                        Answers.ForEach(answer =>
                        {
                            var answerUrl = jsonHandler.GetJTokenValue(answer, "node", "stories", 0, "question", "url");
                            if(!string.IsNullOrEmpty(answerUrl))
                                AnswerList.Add(new AnswerDetails {AnswerUrl=$"{QdConstants.HomePageUrl}{answerUrl}"});
                        });
                    PageData = jsonHandler.GetJTokenValue(jsonObject, "data", "multifeedObject", "pageData");
                    MultifeedAfter = jsonHandler.GetJTokenValue(jsonObject, "data", "multifeedObject", "multifeedConnection", "pageInfo", "endCursor");
                    bool.TryParse(jsonHandler.GetJTokenValue(jsonObject, "data", "multifeedObject", "multifeedConnection", "pageInfo", "hasNextPage"), out bool hasNextPage);
                    HasMoreResults = hasNextPage;
                }
                
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }
    }
}