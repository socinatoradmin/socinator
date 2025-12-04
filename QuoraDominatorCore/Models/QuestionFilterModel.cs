using DominatorHouseCore.Utility;
using QuoraDominatorCore.Interface;

namespace QuoraDominatorCore.Models
{
    public class QuestionFilterModel : BindableBase, IQuestionFilter
    {
        private RangeUtilities _answersCount = new RangeUtilities(0, 1000);

        private RangeUtilities _commentsCount = new RangeUtilities(0, 1000);
        private bool _filterAnswersCount;

        private bool _filterCommentsCount;

        private bool _filterAskedInSpacesCount;


        private bool _filterLockedStatus;

        private bool _filterPublicFollowersCount;

        private bool _filterQuestionFollowers;

        private bool _filterViewsCount;

        private RangeUtilities _askedInSpacesCount = new RangeUtilities(0, 1000);


        private RangeUtilities _publicFollowersCount = new RangeUtilities(0, 1000);

        private RangeUtilities _questionFollowersCount = new RangeUtilities(0, 1000);

        private bool _saveCloseButtonVisible;

        private RangeUtilities _viewsCount = new RangeUtilities(0, 1000);

        public bool FilterPublicFollowersCount
        {
            get => _filterPublicFollowersCount;
            set
            {
                if (value == _filterPublicFollowersCount) return;
                SetProperty(ref _filterPublicFollowersCount, value);
            }
        }

        public bool SaveCloseButtonVisible
        {
            get => _saveCloseButtonVisible;
            set
            {
                if (value == _saveCloseButtonVisible) return;
                SetProperty(ref _saveCloseButtonVisible, value);
            }
        }

        public bool FilterAnswersCount
        {
            get => _filterAnswersCount;
            set
            {
                if (_filterAnswersCount == value) return;
                SetProperty(ref _filterAnswersCount, value);
            }
        }

        public RangeUtilities AnswersCount
        {
            get => _answersCount;
            set
            {
                if (value == _answersCount) return;
                SetProperty(ref _answersCount, value);
            }
        }

        public bool FilterQuestionFollowers
        {
            get => _filterQuestionFollowers;
            set
            {
                if (value == _filterQuestionFollowers) return;
                SetProperty(ref _filterQuestionFollowers, value);
            }
        }

        public RangeUtilities QuestionFollowersCount
        {
            get => _questionFollowersCount;
            set
            {
                if (value == _questionFollowersCount) return;
                SetProperty(ref _questionFollowersCount, value);
            }
        }

        public bool FilterCommentsCount
        {
            get => _filterCommentsCount;
            set
            {
                if (value == _filterCommentsCount) return;
                SetProperty(ref _filterCommentsCount, value);
            }
        }

        public RangeUtilities CommentsCount
        {
            get => _commentsCount;
            set
            {
                if (value == _commentsCount) return;
                SetProperty(ref _commentsCount, value);
            }
        }

        public RangeUtilities PublicFollowersCount
        {
            get => _publicFollowersCount;
            set
            {
                if (value == _publicFollowersCount) return;
                SetProperty(ref _publicFollowersCount, value);
            }
        }

        public bool FilterViewsCount
        {
            get => _filterViewsCount;
            set
            {
                if (value == _filterViewsCount) return;
                SetProperty(ref _filterViewsCount, value);
            }
        }

        public RangeUtilities ViewsCount
        {
            get => _viewsCount;
            set
            {
                if (value == _viewsCount) return;
                SetProperty(ref _viewsCount, value);
            }
        }

        public bool FilterAskedInSpacesCount
        {
            get => _filterAskedInSpacesCount;
            set
            {
                if (value == _filterAskedInSpacesCount) return;
                SetProperty(ref _filterAskedInSpacesCount, value);
            }
        }

        public RangeUtilities AskedInSpacesCount
        {
            get => _askedInSpacesCount;
            set
            {
                if (value == _askedInSpacesCount) return;
                SetProperty(ref _askedInSpacesCount, value);
            }
        }

        public bool FilterLockedStatus
        {
            get => _filterLockedStatus;
            set
            {
                if (value == _filterLockedStatus) return;
                SetProperty(ref _filterLockedStatus, value);
            }
        }
    }
}