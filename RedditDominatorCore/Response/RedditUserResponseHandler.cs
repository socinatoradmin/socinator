using CefSharp;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Utility;
using Newtonsoft.Json.Linq;
using RedditDominatorCore.RDLibrary;
using RedditDominatorCore.RDModel;
using RedditDominatorCore.RDUtility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedditDominatorCore.Response
{
    public class RedditUserResponseHandler: RdResponseHandler
    {
        public readonly List<RedditUser> LstRedditUser = new List<RedditUser>();
        private readonly JsonHandler _jsonHand;
        public RedditUserResponseHandler(IResponseParameter response, bool isPaginatedData,
            PaginationParameter paginationParameter = null) : base(response)
        {
            var notRequiredData = string.Empty;
            Response = response.Response;
            if (isPaginatedData && paginationParameter==null)
            {
                PaginationParameter = new PaginationParameter();
                return;
            }
            if (!Success) return;
            try
            {
               var nodes = HtmlUtility.GetListOfNodesFromTagName(Response, "search-telemetry-tracker", "view-events", "search/view/people");
                if(nodes != null)
                {
                    foreach (var node in nodes)
                    {
                        string PostData = node.GetAttributeValue("data-faceplate-tracking-context", null);
                        PostData = PostData.Replace("&quot;", "\"");
                        _jsonHand = new JsonHandler(PostData);
                        AddToList();
                    }
                    PaginationParameter = new PaginationParameter();
                    //PaginationParameter.LastPaginationId = Utilities.GetBetween(Response, "peopleCursor=", "\"");
                    //if (PaginationParameter != null && string.IsNullOrEmpty(PaginationParameter.LastPaginationId))
                    //    PaginationParameter.LastPaginationId = Utilities.GetBetween(Response, "cursor=", "\"");

                    //if (PaginationParameter != null && string.IsNullOrEmpty(PaginationParameter.LastPaginationId))
                    //{
                    //    var paginationRes = HtmlUtility.GetListOfNodesFromTagName(Response, "faceplate-partial", "slot", "load-after");
                    //    if (paginationRes != null)
                    //    {
                    //        foreach (var pagination in paginationRes)
                    //        {
                    //            var Id = pagination.GetAttributeValue("src", string.Empty);
                    //            if (!string.IsNullOrEmpty(Id))
                    //                PaginationParameter.LastPaginationId = Id;
                    //        }
                    //    }
                    //}
                    //if (PaginationParameter != null && string.IsNullOrEmpty(PaginationParameter.LastPaginationId))
                    //    PaginationParameter.LastPaginationId = Utilities.GetBetween(Response, "people-cursor=", "\"");
                }
            }
            catch (Exception ex)
            { 
            }
        }

        public PaginationParameter PaginationParameter { get; }

        public string BookMark { get; set; }
        public bool HasMoreResults { get; set; }
        private void AddToList()
        {
            try
            {
                var jToken = _jsonHand.GetJToken("profile");
                var reddituser = new RedditUser();
                reddituser.Username = _jsonHand.GetJTokenValue(jToken, "name");
                reddituser.Id = _jsonHand.GetJTokenValue(jToken, "id");
                reddituser.Url = $"{RdConstants.GetRedditHomePageAPI}/user/{reddituser.Username}"; 
                if(!LstRedditUser.Any(user=>user.Id==reddituser.Id) && !string.IsNullOrEmpty(reddituser.Username)
                    && !string.IsNullOrEmpty(reddituser.Id))
                   LstRedditUser.Add(reddituser);
            }
            catch (Exception)
            {
                // ignored
            }
        }
    }
}
