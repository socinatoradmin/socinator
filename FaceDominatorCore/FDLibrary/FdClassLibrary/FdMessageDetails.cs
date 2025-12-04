using FaceDominatorCore.FDEnums;
using System;

namespace FaceDominatorCore.FDLibrary.FdClassLibrary
{
    public class FdMessageDetails
    {
        public string Message { get; set; } = string.Empty;

        public string MessageSenderId { get; set; } = string.Empty;

        public string MessageSenderName { get; set; } = string.Empty;

        public string MessageReceiverName { get; set; } = string.Empty;

        public string MessageReceiverId { get; set; } = string.Empty;

        public DateTime InteractionDate { get; set; } = new DateTime();

        public string OtherUserGender { get; set; }

        public string OtherUserFbid { get; set; } = string.Empty;

        /*
                public bool HasContainImageOrSticker { get; set; }
        */

        /*
                public bool CanReply { get; set; }
        */

        /*
                public string MessageImageUrl { get; set; }
        */

        public MessageType MesageType { get; set; }


        public int UnreadCount { get; set; }

        public string ProfileUrl { get; set; }
        public bool IsOldUi { get; internal set; }
    }
}
