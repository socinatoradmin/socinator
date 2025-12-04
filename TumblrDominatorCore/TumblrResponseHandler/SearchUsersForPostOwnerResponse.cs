using DominatorHouseCore;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Utility;
using System;
using System.Net;
using System.Text.RegularExpressions;
using TumblrDominatorCore.Models;

namespace TumblrDominatorCore.TumblrResponseHandler
{
    public class SearchUsersForPostOwnerResponse : ResponseHandler
    {
        public SearchUsersForPostOwnerResponse()
        {
            Success = true;
        }
        public IResponseParameter PostOwnerResponse = null;
        public TumblrUser user = new TumblrUser();
        public SearchUsersForPostOwnerResponse(IResponseParameter responeParameter) : base(responeParameter)
        {
            PostOwnerResponse = responeParameter;
            if (responeParameter == null)
                return;
            try
            {
                var response = WebUtility.HtmlDecode(Response.Response);

                if (response.Contains("{\"meta\":{\"status\":200,\"msg\":\"OK\"}"))
                {
                    Success = true;
                    JsonHandler handler = null;
                    if (!response.IsValidJson())
                    {
                        var decodedResponse = Regex.Replace(response, "}}csrf :(.*)?", "}}");
                        handler = new JsonHandler(decodedResponse);
                    }
                    else
                    {
                        handler = new JsonHandler(response);
                    }
                    NextPageUrl = handler.GetElementValue("response", "posts", "links", "next", "href");
                    if (!string.IsNullOrEmpty(NextPageUrl))
                        NextPageUrl = "https://www.tumblr.com" + NextPageUrl;
                    var data = handler.GetJToken("response", "blog");
                    foreach (var item in data)
                    {

                        user.Username = handler.GetElementValue("response", "blog", "name");
                        user.PageUrl = handler.GetElementValue("response", "blog", "url");
                        user.Uuid = handler.GetElementValue("response", "blog", "uuid");
                        user.IsFollowed = handler.GetElementValue("response", "blog", "followed").Contains("True") ? true : false;
                        user.CanFollow = handler.GetElementValue("response", "blog", "canBeFollowed").Contains("True") ? true : false;
                        user.CanMessage = handler.GetElementValue("response", "blog", "canMessage").Contains("True") ? true : false;
                    }
                }
            }
            catch (Exception ex)
            {
                IsPagination = false;
                ex.DebugLog();
            }
        }

        public string NextPageUrl { get; set; }
        public bool IsPagination { get; set; } = true;
    }
}