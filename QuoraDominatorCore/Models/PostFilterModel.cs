using System.Collections.ObjectModel;
using DominatorHouseCore.Utility;
using ProtoBuf;
using QuoraDominatorCore.Interface;
using QuoraDominatorCore.Settings;

namespace QuoraDominatorCore.Models
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


        private bool _filterLikes;


        private bool _filterPostAge;


        private bool _filterRestrictedPostCaptionList;


        private bool _ignoreAdPost;


        private bool _ignoreTopPost;


        private RangeUtilities _likesCountRange = new RangeUtilities(10, 20);


        private int _maxPostAge;


        private int _minimumPostCaptionChars = 10;


        private PostCategory _postCategory = new PostCategory();


        private ObservableCollection<string> _restrictedPostCaptionList = new ObservableCollection<string>();

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

        [ProtoMember(7)]
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

        [ProtoMember(6)]
        public bool FilterLikes
        {
            get => _filterLikes;
            set
            {
                if (value == _filterLikes)
                    return;
                SetProperty(ref _filterLikes, value);
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
    }
}