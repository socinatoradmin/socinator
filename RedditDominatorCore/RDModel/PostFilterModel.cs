using DominatorHouseCore.Utility;
using ProtoBuf;
using RedditDominatorCore.Interface;

namespace RedditDominatorCore.RDModel
{
    [ProtoContract]
    public class PostFilterModel : BindableBase, IPostFilter
    {
        private RangeUtilities _commentsCount = new RangeUtilities(10, 20);


        private RangeUtilities _crossPostsCount = new RangeUtilities(10, 20);

        private bool _filterArchived;

        private bool _filterBlankPost;
        private bool _filterCommentsCount;

        private bool _filterCrossPostsCount;
        private bool _filterGoldCounts;

        private bool _filterHidden;
        private bool _filterLocked;

        private bool _filterOriginalContent;
        private bool _filterPinned;

        private bool _filterRoadblock;
        private bool _filterScore;
        private bool _filterSponsored;

        private bool _filterViewCount;


        private RangeUtilities _goldCounts = new RangeUtilities(10, 20);


        private RangeUtilities _score = new RangeUtilities(10, 20);


        private RangeUtilities _viewCount = new RangeUtilities(10, 20);

        [ProtoMember(1)]
        public bool FilterSponsored
        {
            get => _filterSponsored;
            set
            {
                if (value == _filterSponsored) return;
                SetProperty(ref _filterSponsored, value);
            }
        }

        [ProtoMember(2)]
        public bool FilterPinned
        {
            get => _filterPinned;
            set
            {
                if (value == _filterPinned) return;
                SetProperty(ref _filterPinned, value);
            }
        }

        [ProtoMember(3)]
        public bool FilterLocked
        {
            get => _filterLocked;
            set
            {
                if (value == _filterLocked) return;
                SetProperty(ref _filterLocked, value);
            }
        }

        [ProtoMember(4)]
        public bool FilterArchived
        {
            get => _filterArchived;
            set
            {
                if (value == _filterArchived) return;
                SetProperty(ref _filterArchived, value);
            }
        }

        [ProtoMember(5)]
        public bool FilterHidden
        {
            get => _filterHidden;
            set
            {
                if (value == _filterHidden) return;
                SetProperty(ref _filterHidden, value);
            }
        }

        [ProtoMember(6)]
        public bool FilterRoadblock
        {
            get => _filterRoadblock;
            set
            {
                if (value == _filterRoadblock) return;
                SetProperty(ref _filterRoadblock, value);
            }
        }

        [ProtoMember(7)]
        public bool FilterBlankPost
        {
            get => _filterBlankPost;
            set
            {
                if (value == _filterBlankPost) return;
                SetProperty(ref _filterBlankPost, value);
            }
        }

        [ProtoMember(8)]
        public bool FilterOriginalContent
        {
            get => _filterOriginalContent;
            set
            {
                if (value == _filterOriginalContent) return;
                SetProperty(ref _filterOriginalContent, value);
            }
        }

        [ProtoMember(9)]
        public bool FilterGoldCounts
        {
            get => _filterGoldCounts;
            set
            {
                if (value == _filterGoldCounts) return;
                SetProperty(ref _filterGoldCounts, value);
            }
        }

        [ProtoMember(10)]
        public RangeUtilities GoldCounts
        {
            get => _goldCounts;
            set
            {
                if (value == _goldCounts) return;
                SetProperty(ref _goldCounts, value);
            }
        }

        [ProtoMember(11)]
        public bool FilterScore
        {
            get => _filterScore;
            set
            {
                if (value == _filterScore) return;
                SetProperty(ref _filterScore, value);
            }
        }

        [ProtoMember(12)]
        public RangeUtilities Score
        {
            get => _score;
            set
            {
                if (value == _score) return;
                SetProperty(ref _score, value);
            }
        }

        [ProtoMember(13)]
        public bool FilterCrossPostsCount
        {
            get => _filterCrossPostsCount;
            set
            {
                if (value == _filterCrossPostsCount) return;
                SetProperty(ref _filterCrossPostsCount, value);
            }
        }

        [ProtoMember(14)]
        public RangeUtilities CrossPostsCount
        {
            get => _crossPostsCount;
            set
            {
                if (value == _crossPostsCount) return;
                SetProperty(ref _crossPostsCount, value);
            }
        }

        [ProtoMember(15)]
        public bool FilterViewCount
        {
            get => _filterViewCount;
            set
            {
                if (value == _filterViewCount) return;
                SetProperty(ref _filterViewCount, value);
            }
        }

        [ProtoMember(16)]
        public RangeUtilities ViewCount
        {
            get => _viewCount;
            set
            {
                if (value == _viewCount) return;
                SetProperty(ref _viewCount, value);
            }
        }

        [ProtoMember(15)]
        public bool FilterCommentsCount
        {
            get => _filterCommentsCount;
            set
            {
                if (value == _filterCommentsCount) return;
                SetProperty(ref _filterCommentsCount, value);
            }
        }

        [ProtoMember(16)]
        public RangeUtilities CommentsCount
        {
            get => _commentsCount;
            set
            {
                if (value == _commentsCount) return;
                SetProperty(ref _commentsCount, value);
            }
        }
        private string _CustomFlairText;

        public string CustomFlairText
        {
            get => _CustomFlairText;
            set
            {
                if (value == _CustomFlairText)
                    return;
                SetProperty(ref _CustomFlairText, value);
            }
        }
        private bool _isCheckedCustomFlair;

        public bool IsCheckedCustomFlair
        {
            get => _isCheckedCustomFlair;
            set => SetProperty(ref _isCheckedCustomFlair, value);
        }
        private bool _isCheckedFlairFilter;

        public bool IsCheckedFlairFilter
        {
            get => _isCheckedFlairFilter;
            set => SetProperty(ref _isCheckedFlairFilter, value);
        }
        private bool _isCheckedSelectFlair;

        public bool IsCheckedSelectFlair
        {
            get => _isCheckedSelectFlair;
            set => SetProperty(ref _isCheckedSelectFlair, value);
        }

        #region FlairFilter Options

        private bool _isCheckedGeneralDiscourse;

        public bool IsCheckedGeneralDiscourse
        {
            get => _isCheckedGeneralDiscourse;
            set => SetProperty(ref _isCheckedGeneralDiscourse, value);
        }
        private bool _isCheckedHighlights;

        public bool IsCheckedHighlights
        {
            get => _isCheckedHighlights;
            set => SetProperty(ref _isCheckedHighlights, value);
        }
        private bool _isCheckedDiscussion;

        public bool IsCheckedDiscussion
        {
            get => _isCheckedDiscussion;
            set => SetProperty(ref _isCheckedDiscussion, value);
        }
        private bool _isCheckedDiscuss;

        public bool IsCheckedDiscuss
        {
            get => _isCheckedDiscuss;
            set => SetProperty(ref _isCheckedDiscuss, value);
        }
        private bool _isCheckedPhoto;

        public bool IsCheckedPhoto
        {
            get => _isCheckedPhoto;
            set => SetProperty(ref _isCheckedPhoto, value);
        }
        private bool _isCheckedWedding;

        public bool IsCheckedWedding
        {
            get => _isCheckedWedding;
            set => SetProperty(ref _isCheckedWedding, value);
        }
        private bool _isCheckedImage;

        public bool IsCheckedImage
        {
            get => _isCheckedImage;
            set => SetProperty(ref _isCheckedImage, value);
        }
        private bool _isCheckedBlastFromPast;

        public bool IsCheckedBlastFromPast
        {
            get => _isCheckedBlastFromPast;
            set => SetProperty(ref _isCheckedBlastFromPast, value);
        }
        private bool _isCheckedStats;

        public bool IsCheckedStats
        {
            get => _isCheckedStats;
            set => SetProperty(ref _isCheckedStats, value);
        }
        private bool _isCheckedImUnoriginal;

        public bool IsCheckedImUnoriginal
        {
            get => _isCheckedImUnoriginal;
            set => SetProperty(ref _isCheckedImUnoriginal, value);
        }
        private bool _isCheckedVideo;

        public bool IsCheckedVideo
        {
            get => _isCheckedVideo;
            set => SetProperty(ref _isCheckedVideo, value);
        }
        #endregion
    }
}