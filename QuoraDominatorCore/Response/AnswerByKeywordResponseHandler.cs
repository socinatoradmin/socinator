using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using DominatorHouseCore;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Utility;
using QuoraDominatorCore.QdUtility;

namespace QuoraDominatorCore.Response
{
    public class AnswerByKeywordResponseHandler : QuoraResponseHandler
    {
        public List<string> AnswerList = new List<string>();


        public string AnswerUrl;

        public AnswerByKeywordResponseHandler(IResponseParameter response,bool IsBrowser=true) : base(response)
        {
            try
            {
                if (IsBrowser)
                {
                    var decodedResponse = QdUtilities.GetDecodedResponse(response.Response);
                    if (RespJ == null)
                    {
                        if (decodedResponse.Contains("facebookNetworkShareUrl"))
                            AnswerUrl = Utilities.GetBetween(decodedResponse, "facebookNetworkShareUrl\":", ",");
                        var nodes = HtmlDocument.DocumentNode.SelectNodes("//a[@class='question_link']");
                        if (nodes != null && nodes.Count > 0)
                            nodes.ForEach(node =>
                            {
                                AnswerUrl = QdConstants.HomePageUrl +Utilities.GetBetween(node.OuterHtml.Replace("\"", "'"), "href='", "'");
                                AnswerList.Add(AnswerUrl);
                            });
                    }
                    else
                    {
                        var s = jsonHandler.GetJTokenValue(RespJ, "value");
                        AnswerUrl = QdConstants.HomePageUrl +Utilities.GetBetween(s, "AnswerPermalinkClickthrough", ">").Split('\'')[2];
                        AnswerUrl = Regex.Unescape(Regex.Replace(AnswerUrl, "\\\\([^u])", "\\\\$1")).Replace("\\", "");
                    }
                }
                else
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