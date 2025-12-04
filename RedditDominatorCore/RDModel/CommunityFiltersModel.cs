using DominatorHouseCore.Utility;
using ProtoBuf;

namespace RedditDominatorCore.RDModel
{
    [ProtoContract]
    public class CommunityFiltersModel : BindableBase, ICommunityFilter
    {
        private bool _filterallOriginalContentCommmunities;


        private bool _filteremojisEnabledCommmunities;
        private bool _filterisQuarantinedCommmunities;

        private bool _filterNsfwCommmunities;


        private bool _filteroriginalContentTagEnabledCommmunities;


        private bool _filtershowMediaCommmunities;
        private bool _filterNotshowMediaCommmunities;
        private bool _filterMediaCommunities;

        private bool _filterSubscribersCounts;


        private bool _filteruserIsSubscriberCommmunities;

        private bool _filterwlsCounts;


        private RangeUtilities _subscribersCounts = new RangeUtilities(10, 20);


        private RangeUtilities _wlsCounts = new RangeUtilities(10, 20);

        [ProtoMember(1)]
        public bool FilterNsfwCommmunities
        {
            get => _filterNsfwCommmunities;
            set
            {
                if (value == _filterNsfwCommmunities) return;
                SetProperty(ref _filterNsfwCommmunities, value);
            }
        }

        [ProtoMember(2)]
        public bool FilterSubscribersCounts
        {
            get => _filterSubscribersCounts;
            set
            {
                if (value == _filterSubscribersCounts) return;
                SetProperty(ref _filterSubscribersCounts, value);
            }
        }

        [ProtoMember(3)]
        public RangeUtilities SubscribersCounts
        {
            get => _subscribersCounts;
            set
            {
                if (value == _subscribersCounts) return;
                SetProperty(ref _subscribersCounts, value);
            }
        }

        [ProtoMember(4)]
        public bool FilterisQuarantinedCommmunities
        {
            get => _filterisQuarantinedCommmunities;
            set
            {
                if (value == _filterisQuarantinedCommmunities) return;
                SetProperty(ref _filterisQuarantinedCommmunities, value);
            }
        }

        [ProtoMember(5)]
        public bool FilterwlsCounts
        {
            get => _filterwlsCounts;
            set
            {
                if (value == _filterwlsCounts) return;
                SetProperty(ref _filterwlsCounts, value);
            }
        }

        [ProtoMember(6)]
        public RangeUtilities WlsCounts
        {
            get => _wlsCounts;
            set
            {
                if (value == _wlsCounts) return;
                SetProperty(ref _wlsCounts, value);
            }
        }

        [ProtoMember(7)]
        public bool FilteruserIsSubscriberCommmunities
        {
            get => _filteruserIsSubscriberCommmunities;
            set
            {
                if (value == _filteruserIsSubscriberCommmunities) return;
                SetProperty(ref _filteruserIsSubscriberCommmunities, value);
            }
        }

        [ProtoMember(8)]
        public bool FiltershowMediaCommmunities
        {
            get => _filtershowMediaCommmunities;
            set
            {
                if (value == _filtershowMediaCommmunities) return;
                SetProperty(ref _filtershowMediaCommmunities, value);
            }
        }

        [ProtoMember(9)]
        public bool FilteremojisEnabledCommmunities
        {
            get => _filteremojisEnabledCommmunities;
            set
            {
                if (value == _filteremojisEnabledCommmunities) return;
                SetProperty(ref _filteremojisEnabledCommmunities, value);
            }
        }

        [ProtoMember(10)]
        public bool FilteroriginalContentTagEnabledCommmunities
        {
            get => _filteroriginalContentTagEnabledCommmunities;
            set
            {
                if (value == _filteroriginalContentTagEnabledCommmunities) return;
                SetProperty(ref _filteroriginalContentTagEnabledCommmunities, value);
            }
        }

        [ProtoMember(11)]
        public bool FilterallOriginalContentCommmunities
        {
            get => _filterallOriginalContentCommmunities;
            set
            {
                if (value == _filterallOriginalContentCommmunities) return;
                SetProperty(ref _filterallOriginalContentCommmunities, value);
            }
        }
        [ProtoMember(12)]
        public bool FilterNotshowMediaCommmunities
        {
            get => _filterNotshowMediaCommmunities;
            set
            {
                if (value == _filterNotshowMediaCommmunities) return;
                SetProperty(ref _filterNotshowMediaCommmunities, value);
            }
        }

        public bool FilterMediaCommmunities
        {
            get => _filterMediaCommunities;
            set
            {
                if (value == _filterMediaCommunities) return;
                SetProperty(ref _filterMediaCommunities, value);
            }
        }
    }
}