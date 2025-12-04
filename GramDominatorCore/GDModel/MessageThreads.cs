using System.Collections.Generic;

namespace GramDominatorCore.GDModel
{
    public class MessageThreads
    {
        public bool IsCanonical { get; set; }

        public bool HasNewer { get; set; }

        public bool HasOlder { get; set; }

        public bool ValuedRequest { get; set; }

        public bool IsvcMuted { get; set; }

        public bool IsPending { get; set; }

        public bool IsMuted { get; set; }

        public bool IsNamed { get; set; }

        public int ExpiringMediaReceiveCount { get; set; }

        public int ExpiringMediaSendCount { get; set; }

        public int ReshareReceiveCount { get; set; }

        public int ReshareSendCount { get; set; }

        public string LastActivityAt { get; set; }

        public string NewestCursor { get; set; }

        public string OldestCursor { get; set; }

        public string PendingScore { get; set; }

        public string ThreadId { get; set; }

        public string ThreadV2Id { get; set; }

        public string ThreadTitle { get; set; }

        public string ViewerId { get; set; }

        public InstagramUser Inviter { get; set; }

        public MessageDetails LastPermanentMessageDetails { get; set; }

        public List<MessageDetails> MessageItems { get; set; } = new List<MessageDetails>();

        public List<InstagramUser> MessagedUsers { get; set; } = new List<InstagramUser>();
    }
}
