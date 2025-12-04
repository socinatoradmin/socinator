using DominatorHouseCore;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Utility;
using GramDominatorCore.GDLibrary.Response;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

namespace GramDominatorCore.Response
{
    public class FriendshipsResponse : IGResponseHandler
    {
        public string ErrorMessage {  get; set; }
        public List<string> SuggestedUsers { get; set; }=new List<string>();
        public FriendshipsResponse()
        {  }
        public FriendshipsResponse(IResponseParameter response,string FriendShipResponse="",string SuggestedUsersResponse="") : base(response)
        {
            try
            {
                if (!Success)
                {
                    if (!string.IsNullOrEmpty(response?.Response))
                    {
                        ErrorMessage = response.Response;
                        return;
                    }
                    else
                        response.Response = FriendShipResponse;
                }
                if (response.Response.Contains("<!DOCTYPE html>"))
                {
                    Success = true;
                    try
                    {
                        Following = HtmlParseUtility.GetInnerTextFromTagName(response.Response, "button", "class", "_acan _acap _acat _aj1-").Contains("Following");
                    }
                    catch (Exception)
                    {

                    }
                    return;
                }
                if (response.Response.Contains("{\"status\":\"ok\"}") || response.Response.Contains("{\"friendship_status\":{\"following\""))
                {
                    var jObject = handler.ParseJsonToJObject(response.Response);
                    var user = handler.GetJTokenOfJToken(jObject, "data", "user");
                    if (user != null && user.HasValues)
                        jObject = user as JObject;
                    bool.TryParse(handler.GetJTokenValue(jObject, "friendship_status", "following"), out bool isFollowing);
                    Following = isFollowing;
                    bool.TryParse(handler.GetJTokenValue(jObject, "friendship_status", "blocking"), out bool blocking);
                    Blocking = blocking;
                    bool.TryParse(handler.GetJTokenValue(jObject, "friendship_status", "is_private"), out bool is_private);
                    IsPrivate = is_private;
                    bool.TryParse(handler.GetJTokenValue(jObject, "friendship_status", "outgoing_request"), out bool outgoing_request);
                    OutgoingRequest = outgoing_request;
                    bool.TryParse(handler.GetJTokenValue(jObject, "friendship_status", "incoming_request"), out bool incoming_request);
                    IncomingRequest = incoming_request;
                    bool.TryParse(handler.GetJTokenValue(jObject, "friendship_status", "followed_by"), out bool followed_by);
                    FollowedBack = followed_by;
                    Success = true;
                    GetSuggestedUsers(SuggestedUsersResponse);
                    return;
                }
                if (response.Response.Contains("{\"data\":{\"user\":{\"biography\"") || response.Response.Contains("{\"data\":{\"user\":{\"ai_agent_type\""))
                {
                    var jObject = handler.ParseJsonToJObject(response.Response);
                    bool.TryParse(handler.GetJTokenValue(jObject, "data", "user", "followed_by_viewer"), out bool followedByViewer);
                    Following = followedByViewer;
                    bool.TryParse(handler.GetJTokenValue(jObject, "data", "user", "follows_viewer"), out bool follows_viewer);
                    FollowedBack = follows_viewer;
                    bool.TryParse(handler.GetJTokenValue(jObject, "data", "user", "has_requested_viewer"), out bool has_requested_viewer);
                    IncomingRequest = has_requested_viewer;
                    bool.TryParse(handler.GetJTokenValue(jObject, "data", "user", "requested_by_viewer"), out bool requested_by_viewer);
                    OutgoingRequest = requested_by_viewer;
                    bool.TryParse(handler.GetJTokenValue(jObject, "data", "user", "blocked_by_viewer"), out bool blocked_by_viewer);
                    Blocking = blocked_by_viewer;
                    bool.TryParse(handler.GetJTokenValue(jObject, "data", "user", "is_private"), out bool is_private);
                    IsPrivate = is_private;
                    GetSuggestedUsers(SuggestedUsersResponse);
                    return;
                }
                if (!string.IsNullOrEmpty(response?.Response) && response.Response.Contains("{\"data\":{\"xdt_create_friendship\""))
                {
                    var jObject = handler.ParseJsonToJObject(response.Response);
                    bool.TryParse(handler.GetJTokenValue(jObject, "data", "xdt_create_friendship", "friendship_status", "following"), out bool following);
                    Following = following;
                    bool.TryParse(handler.GetJTokenValue(jObject, "data", "xdt_create_friendship", "friendship_status", "followed_by"), out bool followed_by);
                    FollowedBack = followed_by;
                    bool.TryParse(handler.GetJTokenValue(jObject, "data", "xdt_create_friendship", "friendship_status", "outgoing_request"), out bool outgoing_request);
                    OutgoingRequest = outgoing_request;
                    jObject = handler.ParseJsonToJObject(FriendShipResponse);
                    if (jObject != null)
                    {
                        var user = handler.GetJTokenOfJToken(jObject, "data", "user");
                        if (user != null)
                            jObject = user as JObject;
                        bool.TryParse(handler.GetJTokenValue(jObject, "is_private"),out bool isPrivate);
                        IsPrivate = isPrivate;
                        bool.TryParse(handler.GetJTokenValue(jObject, "friendship_status", "incoming_request"), out bool has_requested_viewer);
                        IncomingRequest = has_requested_viewer;
                        bool.TryParse(handler.GetJTokenValue(jObject, "friendship_status", "blocking"), out bool blocked_by_viewer);
                        Blocking = blocked_by_viewer;
                    }
                    GetSuggestedUsers(SuggestedUsersResponse);
                    return;
                }
                if (!string.IsNullOrEmpty(response?.Response) && response.Response.Contains("\"xdt_block_many\""))
                {
                    Blocking = !string.IsNullOrEmpty(response?.Response) && response.Response.Contains("\"xdt_block_many\"")
                        && response.Response.Contains("\"status\":\"ok\"");
                }
                else
                {
                    var jObject = handler.ParseJsonToJObject(response.Response);
                    var RespJ = JObject.Parse(response.Response);
                    bool.TryParse(handler.GetJTokenValue(jObject,"friendship_status", "following"), out bool following);
                    Following = following;
                    bool.TryParse(handler.GetJTokenValue(jObject, "friendship_status", "followed_by"), out bool followed_by);
                    FollowedBack = followed_by;
                    bool.TryParse(handler.GetJTokenValue(jObject, "friendship_status", "incoming_request"), out bool incoming_request);
                    IncomingRequest = incoming_request;
                    bool.TryParse(handler.GetJTokenValue(jObject, "friendship_status", "outgoing_request"), out bool outgoing_request);
                    OutgoingRequest = outgoing_request;
                    bool.TryParse(handler.GetJTokenValue(jObject, "friendship_status", "blocking"), out bool blocking);
                    Blocking = blocking;
                    bool.TryParse(handler.GetJTokenValue(jObject, "friendship_status", "is_private"), out bool is_private);
                    IsPrivate = is_private;
                    GetSuggestedUsers(SuggestedUsersResponse);
                }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        private void GetSuggestedUsers(string suggestedUsersResponse)
        {
            try
            {
                if (string.IsNullOrEmpty(suggestedUsersResponse)) return;
                var jObject = handler.ParseJsonToJObject(suggestedUsersResponse);
                var users = handler.GetJTokenOfJToken(jObject, "payload", "payloads");
                if(users != null && users.HasValues)
                {
                    foreach (var user in users)
                    {
                        var name = Utilities.GetBetween(user?.Path?.ToString(), "[\'/", "/\']");
                        if (!string.IsNullOrEmpty(name))
                            SuggestedUsers.Add(name);
                    }
                }
            }
            catch { }
        }

        public bool Following { get; set; }

        public bool FollowedBack { get; set; }

        public bool IncomingRequest { get; set; }

        public bool OutgoingRequest { get; set; }

        public bool Blocking { get; set; }

        public bool IsPrivate { get; set; }

        public bool IsNotClicked { get; set; }
    }
}
