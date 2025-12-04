#region

using DominatorHouseCore.Utility;
using ProtoBuf;

#endregion

namespace DominatorHouseCore.Models.Publisher.CampaignsAdvanceSetting
{
    [ProtoContract]
    public class RedditModel : BindableBase
    {
        private bool _isMarkAsNsfw;

        [ProtoMember(1)]
        public bool IsMarkAsNsfw
        {
            get => _isMarkAsNsfw;
            set
            {
                if (_isMarkAsNsfw == value)
                    return;
                SetProperty(ref _isMarkAsNsfw, value);
            }
        }


        private bool _isMarkAsOriginalContent;

        [ProtoMember(2)]
        public bool IsMarkAsOriginalContent
        {
            get => _isMarkAsOriginalContent;
            set
            {
                if (_isMarkAsOriginalContent == value)
                    return;

                SetProperty(ref _isMarkAsOriginalContent, value);
            }
        }


        private bool _isMarkAsSpoiler;

        [ProtoMember(3)]
        public bool IsMarkAsSpoiler
        {
            get => _isMarkAsSpoiler;
            set
            {
                if (_isMarkAsSpoiler == value)
                    return;

                SetProperty(ref _isMarkAsSpoiler, value);
            }
        }


        private bool _isDisableSendingReplies;

        [ProtoMember(4)]
        public bool IsDisplaySendingReplies
        {
            get => _isDisableSendingReplies;
            set
            {
                if (_isDisableSendingReplies == value)
                    return;

                SetProperty(ref _isDisableSendingReplies, value);
            }
        }


        [ProtoMember(5)] public string CampaignId { get; set; }


        private bool _isNsfwSubOptions;
        private bool _isOriginalContentSubOptions;
        private bool _isSpoilerSubOptions;
        private bool _isDisableSendingSubOptions;

        [ProtoMember(6)]
        public bool IsNsfwSubOptions
        {
            get => _isNsfwSubOptions;
            set
            {
                if (_isNsfwSubOptions == value)
                    return;


                SetProperty(ref _isNsfwSubOptions, value);
            }
        }

        [ProtoMember(7)]
        public bool IsOriginalContentSubOptions
        {
            get => _isOriginalContentSubOptions;
            set
            {
                if (_isOriginalContentSubOptions == value)
                    return;


                SetProperty(ref _isOriginalContentSubOptions, value);
            }
        }

        [ProtoMember(8)]
        public bool IsSpoilerSubOptions
        {
            get => _isSpoilerSubOptions;
            set
            {
                if (_isSpoilerSubOptions == value)
                    return;


                SetProperty(ref _isSpoilerSubOptions, value);
            }
        }


        [ProtoMember(9)]
        public bool IsDisableSendingSubOptions
        {
            get => _isDisableSendingSubOptions;
            set
            {
                if (_isDisableSendingSubOptions == value)
                    return;


                SetProperty(ref _isDisableSendingSubOptions, value);
            }
        }

        public RedditModel Clone()
        {
            return (RedditModel) MemberwiseClone();
        }
    }
}