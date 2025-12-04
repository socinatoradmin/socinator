using System;
using System.Collections.Generic;
using DominatorHouseCore;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Utility;
using QuoraDominatorCore.QdUtility;

namespace QuoraDominatorCore.Response
{
    public class UserAnswerResponseHandler : QuoraResponseHandler
    {
        public List<AnswerDetails> AnswerList = new List<AnswerDetails>();
        public bool HasMoreResult { get; set; } = false;
        public int EndCursor { get; set; } = 0;
        public UserAnswerResponseHandler(IResponseParameter response,bool IsBrowser=true) : base(response)
        {
            try
            {
                if (IsBrowser)
                {
                    //if (RespJ is null)
                    //{
                    //    var decodedResponse = QdUtilities.GetDecodedResponse(response.Response);
                    //    var document = new HtmlDocument();
                    //    document.LoadHtml(decodedResponse);
                    //    var userAnswers = document.DocumentNode.SelectNodes("//a[@class=\"q-box Link___StyledBox-t2xg9c-0 dFkjrQ answer_timestamp qu-cursor--pointer qu-hover--textDecoration--underline\"]").ToArray();
                    //    if (userAnswers != null && userAnswers.Length > 0)
                    //        userAnswers.ForEach(answerNode => {
                    //            var ansLink = Utilities.GetBetween(answerNode.OuterHtml, "href=\"", "\"").Replace("\"", "").Replace(" ", "-");
                    //            AnswerList.Add(new AnswerDetails { AnswerUrl = ansLink });
                    //        });
                    //}
                    //else
                    //{
                    //    var answerLinks = Regex.Split(RespJ["value"].ToString(), "AnswerPermalinkClickthrough");
                    //    if (answerLinks != null && answerLinks.Length > 0)
                    //        answerLinks.ForEach(link => {
                    //            if (!link.Contains("DOCTYPE"))
                    //            {
                    //                var answerLink = QdConstants.HomePageUrl + Utilities.GetBetween(link, "href='", "'");
                    //                answerLink = Regex.Unescape(Regex.Replace(answerLink, "\\\\([^u])", "\\\\$1")).Replace("\\", "");
                    //                AnswerList.Add(new AnswerDetails { AnswerUrl = answerLink });
                    //            }
                    //        });
                    //}
                    var jsonResponse=QdConstants.GetJsonForAllTypePosts(response.Response.Replace("\\\"", "\"").Replace("\\\"", "\""), "user");
                    response.Response = jsonResponse;
                }
                var jsonObject = jsonHandler.ParseJsonToJObject(response.Response);
                var Answers = jsonHandler.GetJArrayElement(jsonHandler.GetJTokenValue(jsonObject, "data", "user", "recentPublicAndPinnedAnswersConnection", "edges"));
                if (Answers != null && Answers.Count > 0)
                    Answers.ForEach(answer => {
                        var url = jsonHandler.GetJTokenValue(answer, "node", "url");
                        if (!string.IsNullOrEmpty(url))
                        {
                            url = url.Contains("https://") ? url : $"{QdConstants.HomePageUrl}{url}";
                            AnswerList.Add(new AnswerDetails { AnswerID = jsonHandler.GetJTokenValue(answer, "node", "aid"), AnswerUrl = url });
                        }
                    });
                var PageInfo = jsonHandler.GetJTokenOfJToken(jsonObject, "data", "user", "recentPublicAndPinnedAnswersConnection", "pageInfo");
                int.TryParse(jsonHandler.GetJTokenValue(PageInfo, "endCursor"), out int endCursor);
                EndCursor = endCursor;
                bool.TryParse(jsonHandler.GetJTokenValue(PageInfo, "hasNextPage"), out bool hasMoreResult);
                HasMoreResult = hasMoreResult;
                
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }
    }
}