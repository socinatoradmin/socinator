using DominatorHouseCore;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Utility;
using System;
using System.Collections.Generic;
using System.Net;
using TumblrDominatorCore.Models;
using TumblrDominatorCore.TmblrUtility;

namespace TumblrDominatorCore.TumblrResponseHandler
{
    public class MainPageResponseHandler : ResponseHandler
    {
        protected new readonly IResponseParameter Response;

        public MainPageResponseHandler()
        { }
        public MainPageResponseHandler(IResponseParameter responseParameter) : base(responseParameter)
        {
            try
            {
                if (string.IsNullOrEmpty(responseParameter.Response))
                {
                    Success = false;
                    return;
                }
                Response = base.Response;
                if (responseParameter.Response.Contains("{\"meta\":{\"status\":200,\"msg\":\"OK\""))
                {
                    try
                    {
                        JsonHandler handler;
                        try
                        {
                            if (!responseParameter.Response.IsValidJson())
                            {
                                var decodedResponse = TumblrUtility.GetDecodedResponseOfJson(responseParameter.Response);
                                handler = new JsonHandler(decodedResponse);
                            }
                            else
                                handler = new JsonHandler(responseParameter.Response);
                        }
                        catch (Exception)
                        {
                            var requireData1 = Utilities.GetBetween(WebUtility.HtmlDecode(responseParameter.Response), "window['___INITIAL_STATE___'] =", "</script>")?.Trim()?.TrimEnd(';');
                            handler = new JsonHandler(requireData1);
                        }
                        TumblrFormKey = "";
                        var userId = handler.GetElementValue("response", "user", "name");
                        if (string.IsNullOrEmpty(userId)) userId = handler.GetElementValue("queries", "queries", 0, "state", "data", "user", "name");
                        if (!string.IsNullOrEmpty(userId)) UserContextId = userId;


                        LoginJsonResponse = new LoginJsonResponse();
                        try
                        {
                            LoginJsonResponse.Context = new Context();
                            LoginJsonResponse.Context.userinfo = new Userinfo();
                            var friendCount = handler.GetElementValue("response", "user", "following");
                            if (string.IsNullOrEmpty(friendCount)) friendCount = handler.GetElementValue("queries", "queries", 0, "state", "data", "user", "following");
                            LoginJsonResponse.Context.userinfo.friend_count =
                                string.IsNullOrEmpty(friendCount) ? 0 : int.Parse(friendCount);
                            LoginJsonResponse.Context.userinfo.channels = new List<Channel>();
                            var jsonToken = handler.GetJToken("response", "user", "blogs");
                            if (!jsonToken.HasValues) jsonToken = handler.GetJToken("queries", "queries", 0, "state", "data", "user", "blogs");
                            foreach (var item in jsonToken)
                            {
                                var handler1 = new JsonHandler(item);
                                var channel = new Channel();
                                channel.name = handler1.GetElementValue("name");
                                channel.directory_safe_title = handler1.GetElementValue("title");
                                channel.blog_url = handler1.GetElementValue("url");
                                var followerCount = handler1.GetElementValue("followers");
                                channel.follower_count =
                                    string.IsNullOrEmpty(followerCount) ? 0 : int.Parse(followerCount);
                                channel.mention_key = handler1.GetElementValue("mentionKey");
                                var isPrivateGroup = handler1.GetElementValue("isPrivateChannel");
                                channel.is_private_group_channel = isPrivateGroup.Contains("True") ? true : false;
                                var postCount = handler1.GetElementValue("posts");
                                channel.post_count = string.IsNullOrEmpty(postCount) ? 0 : int.Parse(postCount);
                                channel.blog_url = handler1.GetElementValue("url");
                                LoginJsonResponse.Context.userinfo.channels.Add(channel);
                            }
                        }
                        catch (Exception ex)
                        {
                            ex.DebugLog();
                        }
                    }
                    catch (Exception)
                    {
                        // ignored
                    }

                    Success = true;
                }
                else
                {
                    Success = true;
                }
            }
            catch (Exception e)
            {
                e.DebugLog();
            }
        }


        public LoginJsonResponse LoginJsonResponse { get; set; }

        public string TumblrFormKey { get; set; }

        public string UserContextId { get; set; }
    }
}