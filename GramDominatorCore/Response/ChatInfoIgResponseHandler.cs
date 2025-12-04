using DominatorHouseCore;
using DominatorHouseCore.Interfaces;
using GramDominatorCore.GDLibrary.Response;
using GramDominatorCore.GDModel;
using GramDominatorCore.GDUtility;
using System;
using System.Collections.Generic;

namespace GramDominatorCore.Response
{
    public class ChatInfoIgResponseHandler : IGResponseHandler
    {
        public ChatInfoIgResponseHandler(IResponseParameter response) : base(response)
        {
            if (!Success)
                return;

            var jObject = handler.ParseJsonToJObject(RespJ.ToString());
            var inboxJToken = handler.GetJTokenOfJToken(jObject, "inbox");
            foreach (var threads in handler.GetJArrayElement(handler.GetJTokenValue(inboxJToken, "threads")))
            {
                try
                {
                    MessageThreads messageThreads = new MessageThreads
                    {
                        IsCanonical = Convert.ToBoolean(threads["canonical"]),
                        HasNewer = Convert.ToBoolean(threads["has_newer"]),
                        HasOlder = Convert.ToBoolean(threads["has_older"]),
                        ValuedRequest = Convert.ToBoolean(threads["valued_request"]),
                        IsvcMuted = Convert.ToBoolean(threads["vc_muted"]),
                        IsPending = Convert.ToBoolean(threads["pending"]),
                        IsMuted = Convert.ToBoolean(threads["muted"]),
                        IsNamed = Convert.ToBoolean(threads["named"]),
                        ExpiringMediaReceiveCount = Convert.ToInt32(threads["expiring_media_receive_count"]),
                        ExpiringMediaSendCount = Convert.ToInt32(threads["expiring_media_send_count"]),
                        ReshareReceiveCount = Convert.ToInt32(threads["reshare_receive_count"]),
                        ReshareSendCount = Convert.ToInt32(threads["reshare_send_count"]),
                        LastActivityAt = threads["last_activity_at"].ToString(),
                        NewestCursor = threads["newest_cursor"].ToString(),
                        OldestCursor = threads["oldest_cursor"].ToString(),
                        PendingScore = threads["pending_score"].ToString(),
                        ThreadId = threads["thread_id"].ToString(),
                        ThreadV2Id = threads["thread_v2_id"].ToString(),
                        ThreadTitle = threads["thread_title"].ToString(),
                        ViewerId = threads["viewer_id"].ToString(),
                        Inviter = threads["inviter"].GetInstagramUser(),
                        LastPermanentMessageDetails = threads["last_permanent_item"].GetMessageDetails(),
                        MessageItems = new List<MessageDetails>() { threads["items"][0].GetMessageDetails() },
                        MessagedUsers = new List<InstagramUser>() { threads["users"][0].GetInstagramUser() }
                    };
                    LstMessageThreads.Add(messageThreads);
                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                }
            }
        }

        public List<MessageThreads> LstMessageThreads { get; set; } = new List<MessageThreads>();
    }
}
