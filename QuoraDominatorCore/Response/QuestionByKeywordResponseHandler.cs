using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using DominatorHouseCore;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Utility;
using QuoraDominatorCore.QdUtility;

namespace QuoraDominatorCore.Response
{
    public class QuestionByKeywordResponseHandler : QuoraResponseHandler
    {
        public List<QuestionDetails> QuestionList { get; set; } = new List<QuestionDetails>();
        public List<AnswerDetails> AnswerList { get; set; } = new List<AnswerDetails>();
        public int PaginationCount = 0;
        public bool HasMoreResults=false;
        public QuestionByKeywordResponseHandler(IResponseParameter response,bool IsBrowser=true) : base(response)
        {
            try
            {
                if (IsBrowser)
                    response.Response = QdConstants.GetJsonForAllTypePosts(response.Response.Replace("\\\"", "\"").Replace("\\\"", "\""), "searchConnection");
                var jsonObject = jsonHandler.ParseJsonToJObject(response.Response);
                var Questions = jsonHandler.GetJArrayElement(jsonHandler.GetJTokenValue(jsonObject, "data", "searchConnection", "edges"));
                int.TryParse(jsonHandler.GetJTokenValue(jsonObject, "data", "searchConnection", "pageInfo", "endCursor"), out int endCursor);
                bool.TryParse(jsonHandler.GetJTokenValue(jsonObject, "data", "searchConnection", "pageInfo", "hasNextPage"), out bool hasNextPage);
                HasMoreResults = hasNextPage;
                PaginationCount = endCursor;
                if (Questions != null && Questions.HasValues)
                    Questions.ForEach(Question =>
                    {
                        var questionUrl = jsonHandler.GetJTokenValue(Question, "node", "question", "url");
                        questionUrl = questionUrl.Contains(".quora.com") ? questionUrl : $"{QdConstants.HomePageUrl}{questionUrl}";
                        bool.TryParse(jsonHandler.GetJTokenValue(Question, "node", "question", "viewerHasDownvoted"), out bool ViewerDownvoted);
                        QuestionList.Add(new QuestionDetails
                        {
                            QuestionUrl = questionUrl,
                            FollowCount = jsonHandler.GetJTokenValue(Question, "node", "question", "followerCount"),
                            AnswerCount = jsonHandler.GetJTokenValue(Question, "node", "question", "answerCount"),
                            QuestionId = jsonHandler.GetJTokenValue(Question, "node", "question", "qid"),
                            ViewerHasDownvoted = ViewerDownvoted
                        });
                    });
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }
        public List<AnswerDetails> GetAnswersLink(string response,bool IsBrowser=true)
        {
            try
            {
                AnswerList.Clear();
                if (IsBrowser)
                {
                    var decodedResponse = QdUtilities.GetDecodedResponse(response);
                    var questions= HtmlUtility.GetListNodesFromClassName(response, "qu-hover--textDecoration--underline answer_timestamp", "a");
                    if (questions != null && questions.Count > 0)
                        questions.ForEach(question =>
                        {
                            string ansLink = Utilities.GetBetween(question.OuterHtml.Replace("\"", "'"), "href='", "'");
                            if (!string.IsNullOrEmpty(ansLink) && ansLink.Contains("answer"))
                            {
                                ansLink = Regex.Unescape(Regex.Replace(ansLink, "\\\\([^u])", "\\\\$1")).Replace("\\", "");
                                AnswerList.Add(new AnswerDetails { AnswerUrl = ansLink });
                            }
                        });
                }
                else
                {
                    if(!string.IsNullOrEmpty(response) && response.Contains("--qgqlmpb"))
                    {
                        var Answers = Regex.Split(response, "Content-Type: application/json;");
                        if(Answers!=null && Answers.Length > 0)
                        {
                            foreach(var answer in Answers)
                            {
                                if (Answers.FirstOrDefault() == answer || Answers.LastOrDefault() == answer)
                                    continue;
                                var jsonObject = jsonHandler.ParseJsonToJObject(answer?.Replace("\r\n", "")?.Replace("--qgqlmpb", ""));
                                var answers =jsonHandler.GetJArrayElement(jsonHandler.GetJTokenValue(jsonObject, "data", "node", "answers", "edges"));
                                if (answers != null && answers.HasValues)
                                    answers.ForEach(ans =>
                                    {
                                        var AnswerUrl = jsonHandler.GetJTokenValue(ans, "node", "answer", "url");
                                        if (!string.IsNullOrEmpty(AnswerUrl))
                                            AnswerList.Add(new AnswerDetails { AnswerUrl = $"{QdConstants.HomePageUrl}{AnswerUrl}",AnswerID=jsonHandler.GetJTokenValue(ans,"node","answer","aid") });
                                    });
                                else
                                {
                                    jsonObject= jsonHandler.ParseJsonToJObject(answer?.Replace("\r\n", "")?.Replace("--qgqlmpb", ""));
                                    var AnswerUrl = jsonHandler.GetJTokenValue(jsonObject, "data","node", "answer","url");
                                    if (!string.IsNullOrEmpty(AnswerUrl))
                                        AnswerList.Add(new AnswerDetails { AnswerUrl = $"{QdConstants.HomePageUrl}{AnswerUrl}", AnswerID = jsonHandler.GetJTokenValue(jsonObject,"data", "node", "answer", "aid") });
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
            return AnswerList;
        }
    }
    public class QuestionDetails
    {
        public string AnswerCount { get; set; }
        public string FollowCount { get; set; }
        public string QuestionUrl { get; set; }
        public string QuestionId { get; set; }
        public bool ViewerHasDownvoted { get; set; }
    }
    public class AnswerDetails
    {
        public string AnswerUrl { get; set; }
        public string AnswerID { get; set; }
    }
}