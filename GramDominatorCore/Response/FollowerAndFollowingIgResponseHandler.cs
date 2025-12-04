using DominatorHouseCore.Interfaces;
using GramDominatorCore.GDModel;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace GramDominatorCore.Response
{
    [Localizable(false)]
    public class FollowerAndFollowingIgResponseHandler : GDLibrary.Response.IGResponseHandler
    {
        public bool HasMoreresult { get; set; }
        public List<string> CloseFriends { get; set; }=new List<string>();
        public int SkippedCount {  get; set; }
        public FollowerAndFollowingIgResponseHandler(IResponseParameter response,bool hasMoreResult=true,bool IsCloseFriend=false,string Maxid="",List<string> closeFriends=null,bool IsWeb=false)
            : base(response)
        {
            CloseFriends = closeFriends;
            if(CloseFriends != null && CloseFriends.Count > 0)
                SkippedCount = CloseFriends.Count;
            HasMoreresult = hasMoreResult;
            MaxId = Maxid;
            if (!Success)
                return;
            if (IsWeb)
            {
                var jobj = handler.ParseJsonToJObject(response?.Response);
                var pageInfo = handler.GetJTokenOfJToken(jobj, "data", "user", "edge_follow");
                if(pageInfo is null || !pageInfo.HasValues)
                    pageInfo = handler.GetJTokenOfJToken(jobj, "data", "user", "edge_followed_by");
                MaxId = handler.GetJTokenValue(pageInfo, "page_info", "end_cursor");
                bool.TryParse(handler.GetJTokenValue(pageInfo, "page_info", "has_next_page"), out bool hasNextPage);
                HasMoreresult = hasNextPage;
                var Users = handler.GetJArrayElement(handler.GetJTokenValue(pageInfo, "edges"));
                if (Users != null && Users.HasValues)
                {
                    foreach (var jtoken in Users)
                    {
                        var instagramUser = new InstagramUser(handler.GetJTokenValue(jtoken, "node", "id"),
                            handler.GetJTokenValue(jtoken, "node", "username"))
                        {
                            ProfilePicUrl = handler.GetJTokenValue(jtoken, "node", "profile_pic_url"),
                            FullName = handler.GetJTokenValue(jtoken, "node", "full_name")
                        };
                        bool.TryParse(handler.GetJTokenValue(jtoken, "node", "is_verified"), out bool isVerified);
                        instagramUser.IsVerified = isVerified;
                        bool.TryParse(handler.GetJTokenValue(jtoken, "node", "followed_by_viewer"), out bool followed_by_viewer);
                        bool.TryParse(handler.GetJTokenValue(jtoken, "node", "requested_by_viewer"), out bool requested_by_viewer);
                        instagramUser.IsFollowing = followed_by_viewer || requested_by_viewer;
                        UsersList.Add(instagramUser);
                    }
                }
                return;
            }
            List<string> lstData = null;
            try
            {
                lstData = JsonConvert.DeserializeObject<List<string>>(response.Response);
            }
            catch (Exception)
            {
            }
            
            if(lstData!=null)
            {
                foreach(var item in lstData)
                {
                    GetFollowerandFollowingUsers(item,IsCloseFriend);
                }
                return;
            }
            var users = handler.GetJArrayElement(handler.GetJTokenValue(RespJ, "users"));
            if(users != null && users.HasValues)
            {
                foreach (var jtoken in users)
                {
                    bool.TryParse(handler.GetJTokenValue(jtoken, "has_anonymous_profile_picture"), out bool hasAnonymous);
                    var instagramUser = new InstagramUser(handler.GetJTokenValue(jtoken,"pk"),
                        handler.GetJTokenValue(jtoken, "username"))
                    {
                        HasAnonymousProfilePicture = hasAnonymous,
                        ProfilePicUrl = handler.GetJTokenValue(jtoken, "profile_pic_url"),
                        FullName = handler.GetJTokenValue(jtoken, "full_name")
                    };
                    bool.TryParse(handler.GetJTokenValue(jtoken, "is_verified"),out bool isVerified);
                    instagramUser.IsVerified = isVerified;
                    bool.TryParse(handler.GetJTokenValue(jtoken, "is_private"), out bool isPrivate);
                    instagramUser.IsPrivate = isPrivate;
                    UsersList.Add(instagramUser);
                }
            }
            MaxId = handler.GetJTokenValue(RespJ, "next_max_id");
            HasMoreResults = !string.IsNullOrEmpty(MaxId);
        }

        private void GetFollowerandFollowingUsers(object item,bool CloseFriend=false)
        {
            var Tokens = handler.ParseJsonToJObject(item.ToString());
            var Users = handler.GetJArrayElement(handler.GetJTokenValue(Tokens, "users"));
            if(Users != null && Users.HasValues)
            {
                foreach (var jtoken in Users)
                {
                    bool.TryParse(handler.GetJTokenValue(jtoken, "has_anonymous_profile_picture"), out bool hasAnonymous);
                    var instagramUser = new InstagramUser(handler.GetJTokenValue(jtoken, "pk"),
                        handler.GetJTokenValue(jtoken, "username"))
                    {
                        HasAnonymousProfilePicture = hasAnonymous,
                        ProfilePicUrl = handler.GetJTokenValue(jtoken, "profile_pic_url"),
                        FullName = handler.GetJTokenValue(jtoken, "full_name")
                    };
                    bool.TryParse(handler.GetJTokenValue(jtoken, "is_verified"), out bool isVerified);
                    instagramUser.IsVerified = isVerified;
                    bool.TryParse(handler.GetJTokenValue(jtoken, "is_private"), out bool isPrivate);
                    instagramUser.IsPrivate = isPrivate;
                    bool.TryParse(handler.GetJTokenValue(jtoken, "friendship_status", "is_bestie"), out bool closeFriend);
                    instagramUser.IsBestie = closeFriend;
                    if (CloseFriend && instagramUser.IsBestie ||(CloseFriends!=null && CloseFriends.Any(z => z == instagramUser.Username)))
                        continue;
                    if (!UsersList.Any(x=>x.Username == instagramUser.Username))
                    {
                        instagramUser.IsOldProfile = true;
                        UsersList.Add(instagramUser);
                    }
                }
            }
            else
            {
                Users = handler.GetJArrayElement(handler.GetJTokenValue(Tokens, "data", "xdt_api__v1__friendships__followers__connection", "edges"));
                Users = Users == null || !Users.HasValues ? handler.GetJArrayElement(handler.GetJTokenValue(Tokens, "data", "xdt_api__v1__friendships__following__connection", "edges")) : Users;
                if(Users!=null && Users.HasValues)
                {
                    foreach (var jtoken in Users)
                    {
                        var token = handler.GetJTokenOfJToken(jtoken, "node");
                        bool.TryParse(handler.GetJTokenValue(token, "has_anonymous_profile_picture"), out bool hasAnonymous);
                        var instagramUser = new InstagramUser(handler.GetJTokenValue(token, "pk"),
                            handler.GetJTokenValue(token, "username"))
                        {
                            HasAnonymousProfilePicture = hasAnonymous,
                            ProfilePicUrl = handler.GetJTokenValue(token, "profile_pic_url"),
                            FullName = handler.GetJTokenValue(token, "full_name")
                        };
                        bool.TryParse(handler.GetJTokenValue(token, "is_verified"), out bool isVerified);
                        instagramUser.IsVerified = isVerified;
                        bool.TryParse(handler.GetJTokenValue(token, "is_private"), out bool isPrivate);
                        instagramUser.IsPrivate = isPrivate;
                        bool.TryParse(handler.GetJTokenValue(token, "friendship_status", "is_bestie"), out bool closeFriend);
                        instagramUser.IsBestie = closeFriend;
                        if (CloseFriend && instagramUser.IsBestie || (CloseFriends != null && CloseFriends.Any(z => z == instagramUser.Username)))
                            continue;
                        if (!UsersList.Any(x => x.Username == instagramUser.Username))
                        {
                            instagramUser.IsOldProfile = false;
                            UsersList.Add(instagramUser);
                        }
                    }
                }
            }
        }

        public bool HasMoreResults { get; }

        public string MaxId { get; set; }

        public List<InstagramUser> UsersList { get; set; } = new List<InstagramUser>();
    }
}
