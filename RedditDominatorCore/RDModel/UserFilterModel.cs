using DominatorHouseCore.Utility;
using ProtoBuf;
using RedditDominatorCore.Interface;

namespace RedditDominatorCore.RDModel
{
    [ProtoContract]
    public class UserFilterModel : BindableBase, IUserFilter
    {
        private RangeUtilities _commentKarmaCounts = new RangeUtilities(10, 20);
        private bool _filterAlreadyFollowed;

        private bool _filterCommentKarmaCounts;
        private bool _filterEmployees;

        private bool _filterGoldUsers;
        private bool _filterModerators;
        private bool _filterNsfwUsers;

        [ProtoMember(1)]
        public bool FilterEmployees
        {
            get => _filterEmployees;
            set
            {
                if (value == _filterEmployees) return;
                SetProperty(ref _filterEmployees, value);
            }
        }

        [ProtoMember(2)]
        public bool FilterGoldUsers
        {
            get => _filterGoldUsers;
            set
            {
                if (value == _filterGoldUsers) return;
                SetProperty(ref _filterGoldUsers, value);
            }
        }

        [ProtoMember(3)]
        public bool FilterModerators
        {
            get => _filterModerators;
            set
            {
                if (value == _filterModerators) return;
                SetProperty(ref _filterModerators, value);
            }
        }

        [ProtoMember(4)]
        public bool FilterNsfwUsers
        {
            get => _filterNsfwUsers;
            set
            {
                if (value == _filterNsfwUsers) return;
                SetProperty(ref _filterNsfwUsers, value);
            }
        }

        [ProtoMember(5)]
        public bool FilterAlreadyFollowed
        {
            get => _filterAlreadyFollowed;
            set
            {
                if (value == _filterAlreadyFollowed) return;
                SetProperty(ref _filterAlreadyFollowed, value);
            }
        }

        [ProtoMember(6)]
        public bool FilterCommentKarmaCounts
        {
            get => _filterCommentKarmaCounts;
            set
            {
                if (value == _filterCommentKarmaCounts) return;
                SetProperty(ref _filterCommentKarmaCounts, value);
            }
        }

        [ProtoMember(7)]
        public RangeUtilities CommentKarmaCounts
        {
            get => _commentKarmaCounts;
            set
            {
                if (value == _commentKarmaCounts) return;
                SetProperty(ref _commentKarmaCounts, value);
            }
        }
    }
}