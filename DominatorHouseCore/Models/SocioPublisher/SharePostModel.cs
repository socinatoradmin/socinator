#region

using DominatorHouseCore.Utility;
using ProtoBuf;
using System;

#endregion

namespace DominatorHouseCore.Models.SocioPublisher
{
    [Serializable]
    [ProtoContract]
    public class SharePostModel : BindableBase
    {
        private bool _isShareCustomPostList;

        [ProtoMember(1)]
        public bool IsShareCustomPostList
        {
            get => _isShareCustomPostList;
            set
            {
                if (_isShareCustomPostList == value)
                    return;
                _isShareCustomPostList = value;
                OnPropertyChanged(nameof(IsShareCustomPostList));
            }
        }

        private string _shareAddCustomPostList = string.Empty;

        [ProtoMember(2)]
        public string ShareAddCustomPostList
        {
            get => _shareAddCustomPostList;
            set
            {
                if (_shareAddCustomPostList == value)
                    return;
                _shareAddCustomPostList = value;
                OnPropertyChanged(nameof(ShareAddCustomPostList));
            }
        }

        private bool _isShareFdPagePost;

        [ProtoMember(3)]
        public bool IsShareFdPagePost
        {
            get => _isShareFdPagePost;
            set
            {
                if (_isShareFdPagePost == value)
                    return;
                _isShareFdPagePost = value;
                OnPropertyChanged(nameof(IsShareFdPagePost));
            }
        }

        private string _addFdPageUrl = string.Empty;

        [ProtoMember(4)]
        public string AddFdPageUrl
        {
            get => _addFdPageUrl;
            set
            {
                if (_addFdPageUrl == value)
                    return;
                _addFdPageUrl = value;
                OnPropertyChanged(nameof(AddFdPageUrl));
            }
        }

        private bool _isNewPagePost;

        [ProtoMember(5)]
        public bool IsNewPagePost
        {
            get => _isNewPagePost;
            set
            {
                if (_isNewPagePost == value)
                    return;
                _isNewPagePost = value;
                OnPropertyChanged(nameof(IsNewPagePost));
            }
        }

        private int _postPerMinute;

        [ProtoMember(6)]
        public int PostPerMinute
        {
            get => _postPerMinute;
            set
            {
                if (_postPerMinute == value)
                    return;
                _postPerMinute = value;
                OnPropertyChanged(nameof(PostPerMinute));
            }
        }

        private bool _isExtractMinimumPost;

        [ProtoMember(7)]
        public bool IsExtractMinimumPost
        {
            get => _isExtractMinimumPost;
            set
            {
                if (_isExtractMinimumPost == value)
                    return;
                _isExtractMinimumPost = value;
                OnPropertyChanged(nameof(IsExtractMinimumPost));
            }
        }

        private int _maxPost;

        [ProtoMember(8)]
        public int MaxPost
        {
            get => _maxPost;
            set
            {
                if (_maxPost == value)
                    return;
                _maxPost = value;
                OnPropertyChanged(nameof(MaxPost));
            }
        }

        private bool _isKeywords;

        [ProtoMember(9)]
        public bool IsKeywords
        {
            get => _isKeywords;
            set
            {
                if (_isKeywords == value)
                    return;
                _isKeywords = value;
                OnPropertyChanged(nameof(IsKeywords));
            }
        }

        private string _addKeywords = string.Empty;

        [ProtoMember(10)]
        public string AddKeywords
        {
            get => _addKeywords;
            set
            {
                if (_addKeywords == value)
                    return;
                _addKeywords = value;
                OnPropertyChanged(nameof(AddKeywords));
            }
        }

        private bool _isMinimumDays;

        [ProtoMember(11)]
        public bool IsMinimumDays
        {
            get => _isMinimumDays;
            set
            {
                if (_isMinimumDays == value)
                    return;
                _isMinimumDays = value;
                OnPropertyChanged(nameof(IsMinimumDays));
            }
        }

        private int _minimumDays;

        [ProtoMember(12)]
        public int MinimumDays
        {
            get => _minimumDays;
            set
            {
                if (_minimumDays == value)
                    return;
                _minimumDays = value;
                OnPropertyChanged(nameof(MinimumDays));
            }
        }

        private bool _isPostBetween;

        [ProtoMember(13)]
        public bool IsPostBetween
        {
            get => _isPostBetween;
            set
            {
                if (_isPostBetween == value)
                    return;
                _isPostBetween = value;
                OnPropertyChanged(nameof(IsPostBetween));
            }
        }

        private RangeUtilities _postBetween = new RangeUtilities();

        [ProtoMember(14)]
        public RangeUtilities PostBetween
        {
            get => _postBetween;
            set
            {
                if (_postBetween == value)
                    return;
                _postBetween = value;
                OnPropertyChanged(nameof(PostBetween));
            }
        }

        private int _scrapeCount = 1;

        [ProtoMember(15)]
        public int ScrapeCount
        {
            get => _scrapeCount;
            set
            {
                if (_scrapeCount == value)
                    return;
                SetProperty(ref _scrapeCount, value);
            }
        }


        private int _startScrapeOnXminute = 30;

        [ProtoMember(16)]
        public int StartScrapeOnXminute
        {
            get => _startScrapeOnXminute;
            set
            {
                if (_startScrapeOnXminute == value)
                    return;
                SetProperty(ref _startScrapeOnXminute, value);
            }
        }
    }
}