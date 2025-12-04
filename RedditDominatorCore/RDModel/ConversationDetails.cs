using System;
using System.Collections.Generic;

namespace RedditDominatorCore.RDModel
{
    public class ConversationDetails
    {
        public string RoomID { get; set; } = string.Empty;

        public int UnreadMessageCount { get; set; } = 0;
        public string RoomCreatorID { get; set; } = string.Empty;
        public List<MessageInfo> Messages { get; set; } = new List<MessageInfo>();
        public string username { get; set; } = string.Empty;
        public string ProfileUrl { get; set; } = string.Empty;
        public RedditUser redditUser { get; set; } = new RedditUser();
        public string PaginationToken { get; set; } = string.Empty;

    }
    public class MessageInfo
    {
        public string EventID { get; set; } = string.Empty;
        public string SenderID { get; set; } = string.Empty;
        public DateTime MessageTimeStamp { get; set; }
        public string Message { get; set; } = string.Empty;
        public bool IsPending { get; set; } = false;
    }
}
