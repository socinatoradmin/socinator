using System;
using System.Collections.Generic;
using System.Linq;
using DominatorHouseCore;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using GramDominatorCore.GDLibrary.Response;
using GramDominatorCore.GDModel;
using Newtonsoft.Json.Linq;

namespace GramDominatorCore.Response
{
    public class V2InboxResponse : IGResponseHandler
    {
        private readonly bool _isScrapPendingRequests;
        public int ScrollCount { get; set; }
        public V2InboxResponse(IResponseParameter response, bool isScrapPendingRequests = false,bool IsAutoReplyToNewMessage=false,List<string> PendingUsers = null,int scrollCount=0) : base(response)
        {
            ScrollCount = scrollCount;
            _isScrapPendingRequests = isScrapPendingRequests;
            if (!Success)
                return;

            if (response.Response.StartsWith("{\"name\":null,\"step\":"))
            {
                GetMessageDetails(response, isScrapPendingRequests, IsAutoReplyToNewMessage, PendingUsers);
                return;
            }


            #region Get most_recent_inviter ThreadId
            try
            {
                int.TryParse(handler.GetJTokenValue(RespJ, "pending_requests_total"), out int recent);
                if (recent > 0)
                {
                    HasPendingRequests = true;
                    ThreadIdOfMostRecentUser = handler.GetJTokenValue(RespJ, "inbox", "threads", 0, "thread_id");
                }
            }
            catch (Exception)
            {
                // ignored
            }

            #endregion

            #region Get OldestCursor details
            try
            {
                //CursorId = handler.GetJTokenValue(RespJ, "inbox", "threads", 0, "oldest_cursor");
                CursorId = handler.GetJTokenValue(RespJ, "inbox", "next_cursor", "cursor_thread_v2_id");
                CursorId = string.IsNullOrEmpty(CursorId) ? handler.GetJTokenValue(RespJ, "inbox", "oldest_cursor"):CursorId;
                HasMore = !string.IsNullOrEmpty(CursorId);
            }
            catch (Exception)
            {
                CursorId = "";
            }
            //oldest_cursor
            #endregion

            #region Last messages details
            var tokens = handler.GetJArrayElement(handler.GetJTokenValue(RespJ, "inbox", "threads"));
            foreach (var jtoken in tokens)
            {
                var Sendtype = handler.GetJTokenValue(jtoken, "last_permanent_item", "send_attribution");
                var IsTaggedUser = !string.IsNullOrEmpty(Sendtype) && Sendtype.ToLower().Contains("tagged");
                try
                {
                    if (true)
                    {
                        var user = new InstagramUser();
                        var userToken = handler.GetJTokenOfJToken(jtoken, "users", 0);
                        var username = handler.GetJTokenValue(userToken, "username");
                        user.Username = username;
                        user.FullName = handler.GetJTokenValue(userToken, "full_name");
                        bool.TryParse(handler.GetJTokenValue(userToken, "has_anonymous_profile_picture"), out bool anonymous);
                        user.HasAnonymousProfilePicture = anonymous;
                        bool.TryParse(handler.GetJTokenValue(userToken, "is_private"), out bool IsPrivate);
                        user.IsPrivate = IsPrivate;
                        bool.TryParse(handler.GetJTokenValue(userToken, "is_verified"), out bool is_verified);
                        user.IsVerified = is_verified;
                        bool.TryParse(handler.GetJTokenValue(userToken, "friendship_status", "blocking"), out bool blocking);
                        user.IsBlocking = blocking;
                        var threadId = handler.GetJTokenValue(userToken, "thread_id");
                        threadId = string.IsNullOrEmpty(threadId) ? handler.GetJTokenValue(userToken, "interop_messaging_user_fbid") : threadId;
                        user.UserDetails.ThreadId = threadId;
                        user.UserDetails.ChatID = handler.GetJTokenValue(jtoken, "thread_id");
                        var userimageUrl = handler.GetJTokenValue(userToken, "profile_pic_url");
                        var pk = handler.GetJTokenValue(userToken, "pk");
                        user.ProfilePicUrl = userimageUrl;
                        user.Pk = user.UserId = pk;
                        SenderDetails senderDetails = new SenderDetails
                        {
                            SenderName = username,
                            SenderId = pk
                        };
                        var messageItemType = handler.GetJTokenValue(jtoken, "items", 0, "item_type");
                        if (messageItemType == "text")
                            senderDetails.LastMesseges = handler.GetJTokenValue(jtoken, "items", 0, "text");
                        else if (messageItemType == "link")
                        {
                            senderDetails.LastMesseges = handler.GetJTokenValue(jtoken, "items", 0, "link", "text");
                        }
                        else if (messageItemType.Contains("story_share"))
                        {
                            senderDetails.LastMesseges = handler.GetJTokenValue(jtoken, "items", 0, "story_share", "message");
                        }
                        senderDetails.LastMessageOwnerId = handler.GetJTokenValue(jtoken, "items", 0, "user_id");
                        senderDetails.LastMessegedate = handler.GetJTokenValue(jtoken, "items", 0, "timestamp");
                        senderDetails.ThreadId = threadId;
                        senderDetails.SenderImage = userimageUrl;
                        if (!user.IsBlocking && !LstInviterDetails.Any(x => x.Username == user.Username))
                            LstInviterDetails.Add(user);
                        if (!LstSenderDetails.Any(y => y?.SenderName == senderDetails?.SenderName))
                            LstSenderDetails.Add(senderDetails);
                    }
                }
                catch (Exception)
                {
                    // ignored
                }

                #region Inviter Details
                try
                {
                    if (true)
                    {
                        var jtokenInviter = handler.GetJTokenOfJToken(jtoken, "inviter");
                        if (jtokenInviter != null && jtokenInviter.HasValues)
                        {
                            var token = jtokenInviter;
                            InstagramUser instagramUser = new InstagramUser();
                            if (token != null)
                            {
                                bool.TryParse(handler.GetJTokenValue(token, "has_anonymous_profile_picture"), out bool hasProfile);
                                instagramUser = new InstagramUser(handler.GetJTokenValue(token, "pk"),
                                   handler.GetJTokenValue(token, "username"))
                                {
                                    HasAnonymousProfilePicture = hasProfile,
                                    UserId = handler.GetJTokenValue(token, "pk"),
                                    ProfilePicUrl = handler.GetJTokenValue(token, "profile_pic_url"),
                                    FullName = handler.GetJTokenValue(token, "full_name")
                                };
                                bool.TryParse(handler.GetJTokenValue(token, "is_verified"), out bool is_verified);
                                instagramUser.IsVerified = is_verified;
                                bool.TryParse(handler.GetJTokenValue(token, "is_private"), out bool is_private);
                                instagramUser.IsPrivate = is_private;
                                bool.TryParse(handler.GetJTokenValue(token, "friendship_status", "blocking"), out bool blocking);
                                instagramUser.IsBlocking = blocking;
                                if (!LstInviterDetails.Any(K => K?.Username == instagramUser?.Username))
                                    LstInviterDetails.Add(instagramUser);
                            }
                        }
                    }
                }
                catch (Exception)
                {

                }
                #endregion
            }
            #endregion


        }

        private void GetMessageDetails(IResponseParameter response, bool IsPendingUsers = false, bool IsAutoReplyToNewMessage = false, List<string> PendingUsers = null)
        {
            var jsonHandler = new JsonHandler(response.Response);
            try
            {
                #region Get CursorId
                var cursorToken = jsonHandler.GetElementValue("step", 1, 1, 3);
                if (cursorToken.Contains("executeFirstBlockForSyncTransaction"))
                    CursorId = jsonHandler.GetElementValue("step", 1, 1, 3, 5);
                #endregion

                #region Get Messages.
                var messageToken = jsonHandler.GetJToken("step", 1, 2, 2);

                foreach (JToken token in messageToken)
                {
                    if (token.ToString().Contains("deleteThenInsertThread"))
                    {
                        try
                        {
                            string message = jsonHandler.GetJTokenValue(token, 1, 4);
                            if (IsAutoReplyToNewMessage && !IsPendingUsers && message.StartsWith("You:"))
                                continue;
                            string username = jsonHandler.GetJTokenValue(token, 1, 38);
                            username = username.Substring(0, username.IndexOf(" u00b7"));
                            string threadId = jsonHandler.GetJTokenValue(token, 1, 9, 1);
                            string timestamp = jsonHandler.GetJTokenValue(token, 1, 2, 1);
                            string senderImage = jsonHandler.GetJTokenValue(token, 1, 6);
                            SenderDetails sender = new SenderDetails()
                            {
                                SenderName = username,
                                LastMesseges = message,
                                ThreadId = threadId,
                                SenderImage = senderImage,
                                LastMessegedate = timestamp
                            };
                            LstSenderDetails.Add(sender);
                        }
                        catch (Exception ex)
                        {
                            ex.DebugLog();
                        }
                    }
                    else
                        continue;
                }
                if (PendingUsers != null && PendingUsers.Count > 0 && IsPendingUsers)
                {
                    HasPendingRequests = true;
                    var allusers = LstSenderDetails;
                    allusers.RemoveAll(x => !PendingUsers.Any(y => y == x.SenderName));
                    LstSenderDetails = allusers;
                }
                #endregion
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

        }


        public bool HasPendingRequests { get; set; }

        public string ThreadIdOfMostRecentUser { get; set; }

        public List<InstagramUser> LstInviterDetails { get; set; } = new List<InstagramUser>();

       // public List<InstagramUser> LstPendingMessagesUsers { get; set; } = new List<InstagramUser>();

        public List<SenderDetails> LstSenderDetails { get; set; } = new List<SenderDetails>();

        public string CursorId { get; set; }
        public bool HasMore { get; set; }
    }
}