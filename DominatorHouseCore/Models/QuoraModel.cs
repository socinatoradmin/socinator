#region

using DominatorHouseCore.Utility;
using ProtoBuf;

#endregion

namespace DominatorHouseCore.Models
{
    [ProtoContract]
    public class QuoraModel : BindableBase
    {
        private bool _isEnableFollowDifferentUserChecked;

        [ProtoMember(1)]
        public bool IsEnableFollowDifferentUserChecked
        {
            get => _isEnableFollowDifferentUserChecked;
            set
            {
                if (value == _isEnableFollowDifferentUserChecked)
                    return;
                SetProperty(ref _isEnableFollowDifferentUserChecked, value);
            }
        }
    }
    [ProtoContract]
    public class RedditOtherConfigModel : BindableBase
    {
        public bool _isEnableFeedActivity;
        [ProtoMember(1)]
        public bool IsEnableFeedActivity
        {
            get => _isEnableFeedActivity;
            set
            {
                if(_isEnableFeedActivity == value) return;
                SetProperty(ref _isEnableFeedActivity, value);
            }
        }
        public int _MaxThreadCount;
        [ProtoMember(2)]
        public int MaxThreadCount
        {
            get => _MaxThreadCount;
            set=>SetProperty(ref _MaxThreadCount, value);
        }

    }
    [ProtoContract]
    public class LinkedInModel : BindableBase
    {
        private bool _isEnableExportingHTMLOfDifferentConnections;

        [ProtoMember(1)]
        public bool IsEnableExportingHTMLOfDifferentConnections
        {
            get => _isEnableExportingHTMLOfDifferentConnections;
            set
            {
                if (value == _isEnableExportingHTMLOfDifferentConnections)
                    return;
                SetProperty(ref _isEnableExportingHTMLOfDifferentConnections, value);
            }
        }

        private bool _isFilterDuplicateMessageByCheckingConversationsHistory;

        [ProtoMember(2)]
        public bool IsFilterDuplicateMessageByCheckingConversationsHistory
        {
            get => _isFilterDuplicateMessageByCheckingConversationsHistory;
            set
            {
                if (value == _isFilterDuplicateMessageByCheckingConversationsHistory)
                    return;
                SetProperty(ref _isFilterDuplicateMessageByCheckingConversationsHistory, value);
            }
        }

        private bool _IsEnableSendConnectionRequestToDifferentUsers;

        [ProtoMember(3)]
        public bool IsEnableSendConnectionRequestToDifferentUsers
        {
            get => _IsEnableSendConnectionRequestToDifferentUsers;
            set
            {
                if (value == _IsEnableSendConnectionRequestToDifferentUsers)
                    return;
                SetProperty(ref _IsEnableSendConnectionRequestToDifferentUsers, value);
            }
        }
    }

    [ProtoContract]
    public class InstagramUserModel : BindableBase
    {
        private bool _isEnableScrapeDiffrentUserChecked;

        [ProtoMember(1)]
        public bool IsEnableScrapeDiffrentUserChecked
        {
            get => _isEnableScrapeDiffrentUserChecked;
            set
            {
                if (value == _isEnableScrapeDiffrentUserChecked)
                    return;
                SetProperty(ref _isEnableScrapeDiffrentUserChecked, value);
            }
        }
    }
}