using DominatorHouseCore;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using TumblrDominatorCore.Models;
using TumblrDominatorCore.TmblrUtility;

namespace TumblrDominatorCore.TumblrResponseHandler
{
    public class SearchforFollowingsorFollowersResponse : ResponseHandler
    {
        public SearchforFollowingsorFollowersResponse()
        {
            Success = true;
        }

        public List<TumblrUser> LstTumblrUser = new List<TumblrUser>();

        public IResponseParameter SearchResponse = null;
        public string NextPageUrl { get; set; }
        public int TotalFollowings { get; set; }
        public SearchforFollowingsorFollowersResponse(IResponseParameter responeParameter) : base(responeParameter)
        {
            SearchResponse = responeParameter;
            try
            {
                if (responeParameter.Response.Contains("\"status\":200,\"msg\":\"OK\"") ||
                     responeParameter.Response.Contains("\"queryKey\":[\"following\"]")
                    || responeParameter.Response.Contains("\"queryKey\":[\"user-info\",true]"))

                {
                    Success = true;
                    JsonHandler handler = new JsonHandler("{}");
                    try
                    {
                        if (!responeParameter.Response.IsValidJson() && responeParameter.Response.StartsWith("<!DOCTYPE html>"))
                        {
                            var requireData1 = TumblrUtility.GetJsonFromPageResponse(responeParameter.Response)?.Trim()?.TrimEnd(';');
                            handler = new JsonHandler(requireData1);
                        }
                        else if (!responeParameter.Response.IsValidJson())
                            responeParameter.Response = TumblrUtility.GetDecodedResponseOfJson(responeParameter.Response);
                        handler = new JsonHandler(responeParameter.Response);
                    }
                    catch (Exception)
                    {
                    }
                    var followings = handler.GetJToken("response", "blogs");
                    if (!followings.Any()) followings = handler.GetJToken("response", "users");
                    if (!followings.Any()) followings = handler.GetJToken("queries", "queries", 1, "state", "data", "pages", 0, "objects");
                    if (!followings.Any()) followings = handler.GetJToken("Followers", "objects");
                    var totalfollowings = handler.GetElementValue("response", "total_blogs");
                    if (string.IsNullOrEmpty(totalfollowings)) totalfollowings = handler.GetElementValue("response", "totalBlogs");
                    if (string.IsNullOrEmpty(totalfollowings))
                        totalfollowings = handler.GetElementValue("queries", "queries", 1, "state", "data", "pages", 0, "totalBlogs");
                    if (string.IsNullOrEmpty(totalfollowings))
                        totalfollowings = handler.GetElementValue("Followers", "totalUsers");
                    if (string.IsNullOrEmpty(totalfollowings))
                        totalfollowings = handler.GetElementValue("response", "totalUsers");
                    TotalFollowings = !string.IsNullOrEmpty(totalfollowings) ? Convert.ToInt32(totalfollowings) : 0;
                    foreach (var user in followings)
                    {
                        var jsonUser = new JsonHandler(user);
                        var profileUrl = JsonSearcher.FindStringValueByKey(user, "blog_view_url");
                        if (string.IsNullOrEmpty(profileUrl)) profileUrl = JsonSearcher.FindStringValueByKey(user, "blogViewUrl");
                        if (string.IsNullOrEmpty(profileUrl)) profileUrl = jsonUser.GetElementValue("resources", 0, "blogViewUrl");
                        var canFollow = JsonSearcher.FindStringValueByKey(user, "can_be_followed");
                        if (string.IsNullOrEmpty(canFollow)) canFollow = JsonSearcher.FindStringValueByKey(user, "canBeFollowed");
                        if (string.IsNullOrEmpty(canFollow)) canFollow = jsonUser.GetElementValue("resources", 0, "canBeFollowed");
                        var canMessage = JsonSearcher.FindStringValueByKey(user, "can_message");
                        if (string.IsNullOrEmpty(canMessage)) canMessage = JsonSearcher.FindStringValueByKey(user, "canMessage");
                        if (string.IsNullOrEmpty(canMessage)) canMessage = jsonUser.GetElementValue("resources", 0, "canMessage");
                        var isFollowed = JsonSearcher.FindStringValueByKey(user, "followed");
                        if (string.IsNullOrEmpty(isFollowed)) isFollowed = jsonUser.GetElementValue("resources", 0, "followed");
                        if (string.IsNullOrEmpty(isFollowed)) isFollowed = JsonSearcher.FindStringValueByKey(user, "following");
                        var userName = JsonSearcher.FindStringValueByKey(user, "name");
                        if (string.IsNullOrEmpty(userName)) userName = jsonUser.GetElementValue("resources", 0, "name");
                        var pageUrl = jsonUser.GetElementValue("blog", "url");
                        if (string.IsNullOrEmpty(pageUrl)) pageUrl = jsonUser.GetElementValue("resources", 0, "url");
                        var uuid = JsonSearcher.FindStringValueByKey(user, "uuid");
                        if (string.IsNullOrEmpty(uuid)) uuid = jsonUser.GetElementValue("resources", 0, "uuid");
                        var tumblrUser = new TumblrUser
                        {
                            Username = userName,
                            PageUrl = pageUrl,
                            ProfilePicUrl = profileUrl,
                            CanFollow = canFollow.ToLower().Contains("true") ? true : false,
                            CanMessage = canMessage.ToLower().Contains("true") ? true : false,
                            Uuid = uuid,
                            IsFollowed = isFollowed.ToLower().Contains("true") ? true : false
                        };
                        LstTumblrUser.Add(tumblrUser);
                    }
                    NextPageUrl = handler.GetElementValue("response", "_links", "next", "href");
                    if (string.IsNullOrEmpty(NextPageUrl)) NextPageUrl = handler.GetElementValue("response", "links", "next", "href");
                    if (string.IsNullOrEmpty(NextPageUrl))
                        NextPageUrl = handler.GetElementValue("queries", "queries", 1, "state", "data", "pages", 0, "nextLink", "href");
                    if (!string.IsNullOrEmpty(NextPageUrl))
                        NextPageUrl = "https://www.tumblr.com/api" + NextPageUrl;
                }
            }

            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }
    }
}
