using System.Collections.ObjectModel;
using DominatorHouseCore.Utility;
using PinDominatorCore.Interface;
using PinDominatorCore.Settings;
using ProtoBuf;

namespace PinDominatorCore.PDModel
{
    [ProtoContract]
    public class PostFilterModel : BindableBase, IPostFilter
    {
        private ObservableCollection<string> _acceptedPostCaptionList = new ObservableCollection<string>();


        private string _captionBlacklists;
        private string _captionWhitelist;


        private RangeUtilities _commentsCountRange = new RangeUtilities(10, 20);


        private bool _filterAcceptedPostCaptionList;


        private bool _filterComments;


        private bool _filterPostAge;


        private bool _filterRestrictedPostCaptionList;


        private bool _filterTried;


        private bool _ignoreAdPost;


        private bool _ignoreTopPost;

        private bool _isPostedBefore;

        private bool _isPostedInLast;

        private RangeUtilities _likesCountRange = new RangeUtilities(10, 20);


        private int _maxPostAge;


        private int _minimumPostCaptionChars = 10;


        private PostCategory _postCategory = new PostCategory();

        private int _postedBeforeDays;

        private int _postedInLastDays;


        private ObservableCollection<string> _restrictedPostCaptionList = new ObservableCollection<string>();


        private RangeUtilities _triedCountRange = new RangeUtilities(10, 20);

        [ProtoMember(11)]
        public bool IgnoreAdPost
        {
            get => _ignoreAdPost;
            set
            {
                if (value == _ignoreAdPost)
                    return;
                SetProperty(ref _ignoreAdPost, value);
            }
        }

        [ProtoMember(15)]
        public string CaptionBlacklists
        {
            get => _captionBlacklists;
            set
            {
                if (value == _captionBlacklists)
                    return;
                SetProperty(ref _captionBlacklists, value);
            }
        }

        [ProtoMember(16)]
        public string CaptionWhitelist
        {
            get => _captionWhitelist;
            set
            {
                if (value == _captionWhitelist)
                    return;
                SetProperty(ref _captionWhitelist, value);
            }
        }

        public int PostedBeforeDays
        {
            get => _postedBeforeDays;
            set
            {
                if (value == _postedBeforeDays)
                    return;
                SetProperty(ref _postedBeforeDays, value);
            }
        }

        public int PostedInLastDays
        {
            get => _postedInLastDays;
            set
            {
                if (value == _postedInLastDays)
                    return;
                SetProperty(ref _postedInLastDays, value);
            }
        }

        public bool IsPostedBefore
        {
            get => _isPostedBefore;
            set
            {
                if (value)
                    IsPostedInLast = false;
                if (value == _isPostedBefore)
                    return;
                SetProperty(ref _isPostedBefore, value);
            }
        }

        public bool IsPostedInLast
        {
            get => _isPostedInLast;
            set
            {
                if (value)
                    IsPostedBefore = false;
                if (value == _isPostedInLast)
                    return;
                SetProperty(ref _isPostedInLast, value);
            }
        }

        [ProtoMember(1)]
        public int MinimumPostCaptionChars
        {
            get => _minimumPostCaptionChars;
            set
            {
                if (value == _minimumPostCaptionChars)
                    return;
                SetProperty(ref _minimumPostCaptionChars, value);
            }
        }

        [ProtoMember(2)]
        public bool FilterRestrictedPostCaptionList
        {
            get => _filterRestrictedPostCaptionList;
            set
            {
                if (value == _filterRestrictedPostCaptionList)
                    return;
                SetProperty(ref _filterRestrictedPostCaptionList, value);
            }
        }

        [ProtoMember(3)]
        public ObservableCollection<string> RestrictedPostCaptionList
        {
            get => _restrictedPostCaptionList;
            set
            {
                if (value == _restrictedPostCaptionList)
                    return;
                SetProperty(ref _restrictedPostCaptionList, value);
            }
        }

        [ProtoMember(4)]
        public bool FilterComments
        {
            get => _filterComments;
            set
            {
                if (value == _filterComments)
                    return;
                SetProperty(ref _filterComments, value);
            }
        }

        [ProtoMember(5)]
        public RangeUtilities CommentsCountRange
        {
            get => _commentsCountRange;
            set
            {
                if (value == _commentsCountRange)
                    return;
                SetProperty(ref _commentsCountRange, value);
            }
        }

        [ProtoMember(6)]
        public bool FilterTried
        {
            get => _filterTried;
            set
            {
                if (value == _filterTried)
                    return;
                SetProperty(ref _filterTried, value);
            }
        }

        [ProtoMember(7)]
        public RangeUtilities TriedCountRange
        {
            get => _triedCountRange;
            set
            {
                if (value == _triedCountRange)
                    return;
                SetProperty(ref _triedCountRange, value);
            }
        }

        [ProtoMember(8)]
        public bool FilterPostAge
        {
            get => _filterPostAge;
            set
            {
                if (value == _filterPostAge)
                    return;
                SetProperty(ref _filterPostAge, value);
            }
        }

        [ProtoMember(9)]
        public int MaxPostAge
        {
            get => _maxPostAge;
            set
            {
                if (value == _maxPostAge)
                    return;
                SetProperty(ref _maxPostAge, value);
            }
        }

        [ProtoMember(10)]
        public PostCategory PostCategory
        {
            get => _postCategory;
            set
            {
                if (value == _postCategory)
                    return;
                SetProperty(ref _postCategory, value);
            }
        }

        [ProtoMember(12)]
        public bool IgnoreTopPost
        {
            get => _ignoreTopPost;
            set
            {
                if (value == _ignoreTopPost)
                    return;
                SetProperty(ref _ignoreTopPost, value);
            }
        }

        [ProtoMember(13)]
        public bool FilterAcceptedPostCaptionList
        {
            get => _filterAcceptedPostCaptionList;
            set
            {
                if (value == _filterAcceptedPostCaptionList)
                    return;
                SetProperty(ref _filterAcceptedPostCaptionList, value);
            }
        }

        [ProtoMember(14)]
        public ObservableCollection<string> AcceptedPostCaptionList
        {
            get => _acceptedPostCaptionList;
            set
            {
                if (value == _acceptedPostCaptionList)
                    return;
                SetProperty(ref _acceptedPostCaptionList, value);
            }
        }

        [ProtoMember(16)]
        public RangeUtilities LikesCountRange
        {
            get => _likesCountRange;
            set
            {
                if (value == _likesCountRange)
                    return;
                SetProperty(ref _likesCountRange, value);
            }
        }
    }
}