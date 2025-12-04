using System;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Utility;
using GramDominatorCore.GDLibrary.Response;
using Newtonsoft.Json.Linq;

namespace GramDominatorCore.Response
{
    public class UserFriendshipResponse : IGResponseHandler
    {
        public UserFriendshipResponse(IResponseParameter response,string userId = "") : base(response)
        {
            if (!Success)
                return;
            if(response.Response.Contains("DOCTYPE html"))
            {
                Success = true;
                return;
            }
            if (response.Response.Contains("{\"data\":{\"user\""))
            {
                var jObject = handler.ParseJsonToJObject(response.Response);
                var token = !string.IsNullOrEmpty(response?.Response) && response.Response.Contains("{\"data\":{\"user\":{\"friendship_status\"") 
                    ? handler.GetJTokenOfJToken(jObject,"data","user", "friendship_status") :handler.GetJTokenOfJToken(jObject,"data","user");
                if(!string.IsNullOrEmpty(response?.Response) && response.Response.Contains("{\"data\":{\"user\":{\"friendship_status\""))
                {
                    bool.TryParse(handler.GetJTokenValue(token, "following"), out bool IsFollowing);
                    Following = IsFollowing;
                    bool.TryParse(handler.GetJTokenValue(token, "followed_by"), out bool IsFollowBack);
                    FollowedBack = IsFollowBack;
                    bool.TryParse(handler.GetJTokenValue(token, "incoming_request"), out bool IsIncomingRequest);
                    IncomingRequest = IsIncomingRequest;
                    bool.TryParse(handler.GetJTokenValue(token, "outgoing_request"), out bool IsOutgoingRequest);
                    OutgoingRequest = IsOutgoingRequest;
                    bool.TryParse(handler.GetJTokenValue(jObject,"data","user", "is_private"), out bool Isprivate);
                    IsPrivate = Isprivate;
                }
                else
                {
                    bool.TryParse(handler.GetJTokenValue(token, "followed_by_viewer"), out bool IsFollowing);
                    Following = IsFollowing;
                    bool.TryParse(handler.GetJTokenValue(token, "followed_by_viewer"), out bool IsFollowBack);
                    FollowedBack = IsFollowBack;
                    bool.TryParse(handler.GetJTokenValue(token, "has_requested_viewer"), out bool IsIncomingRequest);
                    IncomingRequest = IsIncomingRequest;
                    bool.TryParse(handler.GetJTokenValue(token, "requested_by_viewer"), out bool IsOutgoingRequest);
                    OutgoingRequest = IsOutgoingRequest;
                    bool.TryParse(handler.GetJTokenValue(token, "is_verified"), out bool Isprivate);
                    IsPrivate = Isprivate;
                }
                return;
            }
            try
            {
                var jObject = handler.ParseJsonToJObject(response.Response);
                var token = handler.GetJTokenOfJToken(jObject, "friendship_statuses", userId);
                bool.TryParse(handler.GetJTokenValue(token, "following"), out bool IsFollowing);
                Following = IsFollowing;
                bool.TryParse(handler.GetJTokenValue(token, "incoming_request"), out bool IsIncomingRequest);
                IncomingRequest = IsIncomingRequest;
                bool.TryParse(handler.GetJTokenValue(token, "outgoing_request"), out bool IsOutgoingRequest);
                OutgoingRequest = IsOutgoingRequest;
                bool.TryParse(handler.GetJTokenValue(token, "is_private"), out bool Isprivate);
                IsPrivate = Isprivate;
            }
            catch (Exception)
            {

            }
        }

        public bool Following { get; set; }

        public bool FollowedBack { get; set; }

        public bool IncomingRequest { get; set; }

        public bool OutgoingRequest { get; set; }

        public bool Blocking { get; set; }

        public bool IsPrivate { get; set; }
    }
}
