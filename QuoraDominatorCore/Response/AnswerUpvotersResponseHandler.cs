using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using DominatorHouseCore;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Utility;
using HtmlAgilityPack;
using QuoraDominatorCore.QdUtility;

namespace QuoraDominatorCore.Response
{
    public class AnswerUpvotersResponseHandler : QuoraResponseHandler
    {
        public List<string> AnswerUpvoters = new List<string>();
        public int PaginationCount { get; set; }
        public bool HasMoreResult { get; set; }
        public AnswerUpvotersResponseHandler(IResponseParameter response,bool IsBrowser=true) : base(response)
        {
            try
            {
                if (IsBrowser)
                {
                    if(RespJ is null)
                    {
                        var Nodes = HtmlParseUtility.GetListNodeFromPartialTagNamecontains(response.Response, "div", "role", "listitem");
                        if (Nodes != null && Nodes.Count > 0)
                            Nodes.ForEach(upvoter =>
                            {
                                var url = Utilities.GetBetween(upvoter.OuterHtml, "href=\"", "\"");
                                if (!string.IsNullOrEmpty(url) && url.Contains("/profile"))
                                    AnswerUpvoters.Add(url.Replace($"{QdConstants.HomePageUrl}/profile/", ""));
                            });
                    }
                    else
                    {
                        var upvoters = Regex.Split(RespJ["value"].ToString(), "class='user'");
                        foreach (var upvoter in upvoters.Skip(1))
                        {
                            var url = QdConstants.HomePageUrl + Utilities.GetBetween(upvoter, "href='", "'");
                            if(!string.IsNullOrEmpty(url))
                                AnswerUpvoters.Add(url.Replace($"{QdConstants.HomePageUrl}/profile/", ""));
                        }
                    }
                }
                else
                {
                    //Get Details From HTTP Response.
                    var jsonObject = jsonHandler.ParseJsonToJObject(response.Response);
                    var Upvoters = jsonHandler.GetJArrayElement(jsonHandler.GetJTokenValue(jsonObject, "data","node", "upvotersConnection","edges"));
                    if (Upvoters != null && Upvoters.Count > 0)
                        Upvoters.ForEach(upvoter =>
                        {
                            var PublicIdentifier = jsonHandler.GetJTokenValue(upvoter, "node", "profileUrl")?.Replace("/profile/", "");
                            if (!string.IsNullOrEmpty(PublicIdentifier))
                                AnswerUpvoters.Add(PublicIdentifier);
                        });
                    int.TryParse(jsonHandler.GetJTokenValue(jsonObject, "data", "node", "upvotersConnection", "pageInfo", "endCursor"),out int paginationCount);
                    PaginationCount = paginationCount;
                    bool.TryParse(jsonHandler.GetJTokenValue(jsonObject, "data", "node", "upvotersConnection", "pageInfo", "hasNextPage"), out bool hasNextPage);
                    HasMoreResult = hasNextPage;
                }

            }catch(Exception ex) { ex.DebugLog(); }
        }
    }
}