using System;
using System.Collections.Generic;
using System.Linq;
using DominatorHouseCore;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Utility;
using HtmlAgilityPack;
using QuoraDominatorCore.QdUtility;

namespace QuoraDominatorCore.Response
{
    public class CommentOnQuestionResponseHandler : QuoraResponseHandler
    {
        public List<string> CommentedUser = new List<string>();
        public int PaginationCount { get; set; }
        public bool HasMoreResult { get; set; }
        public CommentOnQuestionResponseHandler(IResponseParameter response,bool IsBrowser=true) : base(response)
        {
            try
            {
                if (IsBrowser)
                {
                    var objHtmlDocument = new HtmlDocument();
                    objHtmlDocument.LoadHtml(response.Response);
                    var targetClassName = "q-box qu-borderAll qu-borderRadius--small qu-bg--raised";
                    var specificSpanClass = "q-text qu-dynamicFontSize--small qu-bold qu-color--gray_dark qu-passColorToLinks";

                    var commentors = objHtmlDocument
                        .DocumentNode
                        .SelectNodes($"//*[@class='{targetClassName}']//span[@class='{specificSpanClass}']").ToArray();
                        
                    //var commentors = objHtmlDocument.DocumentNode.SelectNodes("//span[@class='q-text qu-dynamicFontSize--small qu-bold qu-color--gray_dark qu-passColorToLinks']").ToArray();
                    if (commentors != null && commentors.Length > 0)
                        commentors.ForEach(commentor =>
                        {
                            var username = Utilities.GetBetween(commentor.OuterHtml, "href=\"", "\"").Replace($"{QdConstants.HomePageUrl}/profile/", "");
                            if(!string.IsNullOrEmpty(username) && !CommentedUser.Contains(username))
                                CommentedUser.Add(username);
                        });
                }
                else
                {
                    var jsonObject = jsonHandler.ParseJsonToJObject(response.Response);
                    var Commentors = jsonHandler.GetJArrayElement(jsonHandler.GetJTokenValue(jsonObject, "data","node", "allCommentsConnection", "edges"));
                    if (Commentors != null && Commentors.Count > 0)
                        Commentors.ForEach(commentor =>
                        {
                            var PublicIdentifier = jsonHandler.GetJTokenValue(commentor, "node", "user", "profileUrl")?.Replace("/profile/", "");
                            if (!string.IsNullOrEmpty(PublicIdentifier))
                                CommentedUser.Add(PublicIdentifier);
                        });
                    int.TryParse(jsonHandler.GetJTokenValue(jsonObject, "data", "node", "allCommentsConnection", "pageInfo", "endCursor"), out int endCursor);
                    PaginationCount = endCursor;
                    bool.TryParse(jsonHandler.GetJTokenValue(jsonObject, "data", "node", "allCommentsConnection", "pageInfo", "hasNextPage"), out bool hasNextPage);
                    HasMoreResult = hasNextPage;
                }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }
    }
}