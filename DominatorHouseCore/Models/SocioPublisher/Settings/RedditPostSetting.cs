#region

using System;
using DominatorHouseCore.Interfaces.SocioPublisher;
using DominatorHouseCore.Utility;
using ProtoBuf;

#endregion

namespace DominatorHouseCore.Models.SocioPublisher.Settings
{
    [Serializable]
    [ProtoContract]
    public class RedditPostSetting : BindableBase, IRedditSettings
    {
        private bool _isNsfw;
        private bool _isOriginalContent;
        private bool _isSpoiler;
        private bool _isDisableSendingReplies;

        [ProtoMember(1)]
        public bool IsNsfw
        {
            get => _isNsfw;
            set
            {
                if (_isNsfw == value)
                    return;


                SetProperty(ref _isNsfw, value);
            }
        }

        [ProtoMember(2)]
        public bool IsOriginalContent
        {
            get => _isOriginalContent;
            set
            {
                if (_isOriginalContent == value)
                    return;


                SetProperty(ref _isOriginalContent, value);
            }
        }

        [ProtoMember(3)]
        public bool IsSpoiler
        {
            get => _isSpoiler;
            set
            {
                if (_isSpoiler == value)
                    return;


                SetProperty(ref _isSpoiler, value);
            }
        }

        [ProtoMember(4)]
        public bool IsDisableSendingReplies
        {
            get => _isDisableSendingReplies;
            set
            {
                if (_isDisableSendingReplies == value)
                    return;


                SetProperty(ref _isDisableSendingReplies, value);
            }
        }
    }
}