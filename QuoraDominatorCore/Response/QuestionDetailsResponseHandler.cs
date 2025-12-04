using System;
using System.Linq;
using System.Text.RegularExpressions;
using DominatorHouseCore;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Utility;

namespace QuoraDominatorCore.Response
{
    public class QuestionDetailsResponseHandler : QuoraResponseHandler
    {
        public int AnswerCount;
        public int CommentCount;
        public int FollowCount;
        public int LastAskDayCount;
        public bool QuestionLocked;
        public int AskedInSpacesCount;

        public string
            QuestionUrl; //https://www.quora.com/If-there-was-a-prime-membership-on-Quora-what-would-be-the-benefits

        public int ViewCount; //884
        public string Qid;
        public QuestionDetailsResponseHandler(IResponseParameter response) : base(response)
        {
            try
            {
                //ui_button_count_inner
                if (HtmlDocument != null)
                
                try
                    {
                        var QuestionUrlSplit = Regex.Split(response.Response, "facebookSharePermalink");
                        
                        for(int index=1;index<QuestionUrlSplit.Length;index++)
                        {
                            var question = QuestionUrlSplit[index];
                            question = Utilities.GetBetween(question.Split(',')?.FirstOrDefault()+"***", "\"", "***").Replace(":\\\"","").Replace("\\\"","");
                            if (question.Contains("target_type=question"))
                            {
                                QuestionUrl = question;
                                break;
                            }
                        }
                        Qid=Utilities.GetBetween(response.Response, "qid\\\":", ",");
                        var match = Regex.Match(response.Response, "\\\\\\\"followerCount\\\\\\\":{1}(.*),\\\\\\\"viewerCantAnswer\\\\\\\":")?.Value;
                        var Node = Regex.Split(match, ",")?.LastOrDefault(x=>x.Contains("followerCount"));
                        int.TryParse(Utilities.GetBetween(Node+"**", "\\\"followerCount\\\":", "**"), out FollowCount);
                        int.TryParse(Utilities.GetBetween(response.Response, "\\\"answerCount\\\":", ","),out AnswerCount);
                        int.TryParse(Utilities.GetBetween(response.Response, "\\\"numViews\\\":", ","),out ViewCount);
                        match = Regex.Match(response.Response, "\\\\\\\"numDisplayComments\\\\\\\":{1}(.*),\\\\\\\"viewerCommentDraft\\\\\\\":\\{\\\\\"{1}")?.Value;
                        Node = Regex.Split(match, ",").LastOrDefault(x=>x.Contains("numDisplayComments"));
                        int.TryParse(Utilities.GetBetween(Node+"**", "\\\"numDisplayComments\\\":", "**"), out CommentCount);
                        AskedInSpacesCount = response.Response.Contains("\">Asked in") ? int.Parse(Utilities.GetBetween(response.Response, "\">Asked in ", " Spaces")) : 0;
                    }
                    catch (Exception)
                    {
                    }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }
    }
}