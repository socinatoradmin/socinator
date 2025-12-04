using DominatorHouseCore.Utility;
using ProtoBuf;
using QuoraDominatorCore.Interface;

namespace QuoraDominatorCore.Models
{
    [ProtoContract]
    public class AnswerFilterModel : BindableBase, IAnswerFilter
    {
        private RangeUtilities _answeredInLastDays = new RangeUtilities(10, 20);


        private bool _filterAnsweredDays;
        private bool _filterFollowingsCount;

        private bool _filterUpvoteCounts;

        private bool _filterViewsCount;
        private bool _saveCloseButtonVisible;

        private RangeUtilities _upvoteCounts = new RangeUtilities(0, 1000);

        private RangeUtilities _viewsCount = new RangeUtilities(0, 1000);

        [ProtoMember(7)]
        public bool FilterFollowingsCount
        {
            get => _filterFollowingsCount;
            set
            {
                if (value == _filterFollowingsCount) return;
                SetProperty(ref _filterFollowingsCount, value);
            }
        }

        [ProtoMember(8)]
        public bool SaveCloseButtonVisible
        {
            get => _saveCloseButtonVisible;
            set
            {
                if (value == _saveCloseButtonVisible) return;
                SetProperty(ref _saveCloseButtonVisible, value);
            }
        }

        [ProtoMember(1)]
        public bool FilterViewsCount
        {
            get => _filterViewsCount;
            set
            {
                if (value == _filterViewsCount) return;
                SetProperty(ref _filterViewsCount, value);
            }
        }

        [ProtoMember(2)]
        public RangeUtilities ViewsCount
        {
            get => _viewsCount;
            set
            {
                if (value == _viewsCount) return;
                SetProperty(ref _viewsCount, value);
            }
        }

        [ProtoMember(3)]
        public bool FilterUpvoteCounts
        {
            get => _filterUpvoteCounts;
            set
            {
                if (value == _filterUpvoteCounts) return;
                SetProperty(ref _filterUpvoteCounts, value);
            }
        }

        [ProtoMember(4)]
        public RangeUtilities UpvoteCount
        {
            get => _upvoteCounts;
            set
            {
                if (value == _upvoteCounts) return;
                SetProperty(ref _upvoteCounts, value);
            }
        }

        [ProtoMember(5)]
        public bool FilterAnsweredDays
        {
            get => _filterAnsweredDays;
            set
            {
                if (value == _filterAnsweredDays) return;
                SetProperty(ref _filterAnsweredDays, value);
            }
        }

        [ProtoMember(6)]
        public RangeUtilities AnsweredInLastDays
        {
            get => _answeredInLastDays;
            set
            {
                if (value == _answeredInLastDays) return;
                SetProperty(ref _answeredInLastDays, value);
            }
        }
    }
}