using DominatorHouseCore;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Utility;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Net;
using System.Web;
using TumblrDominatorCore.Models;
using TumblrDominatorCore.TmblrUtility;

namespace TumblrDominatorCore.TumblrResponseHandler
{
    public class UserInfoResponeHandler : ResponseHandler
    {
        public UserInfoResponeHandler(IResponseParameter responseParameter) : base(responseParameter)
        {
            try
            {
                if (string.IsNullOrEmpty(responseParameter.Response))
                {
                    Success = false;
                    return;
                }
                Response = base.Response;
                if (responseParameter.Response.Contains("icon_user_settings") || responseParameter.Response.Contains("\"dashboardTimeline\":{\"meta\":{\"status\":200,\"msg\":\"OK\"}"))
                {
                    try
                    {

                        var info = HtmlParseUtility.GetAttributeValueFromId(Response.Response, "bootloader",
                            "data-bootstrap");

                        var jsonInfo = HttpUtility.HtmlDecode(info);
                        var loginJsonResponse = new LoginJsonResponse();
                        try
                        {
                            JObject jObject = new JObject();
                            if (!string.IsNullOrEmpty(jsonInfo))
                            {
                                jObject = JObject.Parse(jsonInfo);
                            }
                            if (!Object.Equals(jObject, null) && jObject.HasValues)
                                loginJsonResponse.Context =
                                    JsonConvert.DeserializeObject<Context>(jObject["Context"].ToString());
                            TumblrUser.FollowingCount = loginJsonResponse?.Context?.userinfo?.friend_count ?? 0;
                            TumblrUser.FollowersCount =
                            loginJsonResponse?.Context?.userinfo?.channels[0]?.follower_count ?? 0;
                            TumblrUser.PostsCount = loginJsonResponse?.Context?.userinfo?.channels[0]?.post_count ?? 0;
                        }
                        catch (Exception)
                        { }
                        var requireData1 = Utilities.GetBetween(WebUtility.HtmlDecode(responseParameter.Response), "window['___INITIAL_STATE___'] =", "</script>")?.Trim()?.TrimEnd(';');
                        requireData1 = string.IsNullOrEmpty(requireData1) ? Utilities.GetBetween(WebUtility.HtmlDecode(responseParameter.Response), "id=\"___INITIAL_STATE___\">", "</script>")?.Trim()?.TrimEnd(';') : requireData1;
                        JsonHandler handler;
                        if (!requireData1.IsValidJson())
                        {
                            var validJsonResponse = TumblrUtility.GetDecodedResponseOfJson(requireData1);
                            handler = new JsonHandler(validJsonResponse);
                        }
                        else
                            handler = new JsonHandler(requireData1);
                        var userdata = handler.GetJToken("queries", "queries", 0, "state", "data", "user");
                        var hand1 = new JsonHandler(userdata);
                        if (TumblrUser.FollowingCount == 0)
                        {
                            TumblrUser.FollowingCount = Convert.ToInt32(hand1.GetElementValue("following"));
                        }
                        if (TumblrUser.FollowersCount == 0)
                        {
                            TumblrUser.FollowersCount = Convert.ToInt32(hand1.GetElementValue("blogs", 0, "followers"));
                        }
                        if (TumblrUser.PostsCount == 0)
                        {
                            TumblrUser.PostsCount = Convert.ToInt32(hand1.GetElementValue("blogs", 0, "posts"));
                        }
                        TumblrUser.UserId = hand1.GetElementValue("name");
                        TumblrUser.Uuid = hand1.GetElementValue("blogs", 0, "uuid");
                        TumblrUser.UserUuid = hand1.GetElementValue("userUuid");

                    }
                    catch (Exception ex)
                    {
                        ex.DebugLog();
                    }

                    Success = true;
                }
                else
                {
                    Success = false;
                }
            }
            catch (Exception e)
            {
                e.DebugLog();
            }
        }
        public new IResponseParameter Response { get; set; }
        public TumblrUser TumblrUser { get; set; } = new TumblrUser();
    }
}