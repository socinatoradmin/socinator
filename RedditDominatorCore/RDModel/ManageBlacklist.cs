using DominatorHouseCore.Utility;
using ProtoBuf;

namespace RedditDominatorCore.RDModel
{
    public class ManageBlacklist
    {
        public interface IManageBlackWhiteListModel
        {
            bool IsSkipWhiteListUsers { get; set; }

            bool IsSkipPrivateWhiteList { get; set; }

            bool IsSkipGroupWhiteList { get; set; }

            bool IsAddToBlackListOnceUnfollowed { get; set; }

            bool IsAddToPrivateBlackList { get; set; }

            bool IsAddToGroupBlackList { get; set; }
        }

        public interface ISkipBlacklist
        {
            bool IsSkipBlackListUsers { get; set; }

            bool IsSkipPrivateBlackListUser { get; set; }

            bool IsSkipGroupBlackListUsers { get; set; }
        }

        [ProtoContract]
        public class ManageBlackWhiteListModel : BindableBase, IManageBlackWhiteListModel
        {
            #region BlackList and Whitelist

            private bool _isSkipWhiteListUsers;

            [ProtoMember(1)]
            public bool IsSkipWhiteListUsers
            {
                get => _isSkipWhiteListUsers;
                set => SetProperty(ref _isSkipWhiteListUsers, value);
            }

            private bool _isSkipPrivateWhiteList;

            [ProtoMember(2)]
            public bool IsSkipPrivateWhiteList
            {
                get => _isSkipPrivateWhiteList;
                set => SetProperty(ref _isSkipPrivateWhiteList, value);
            }

            private bool _isSkipGroupWhiteList;

            [ProtoMember(2)]
            public bool IsSkipGroupWhiteList
            {
                get => _isSkipGroupWhiteList;
                set => SetProperty(ref _isSkipGroupWhiteList, value);
            }

            private bool _isAddToBlackListOnceUnfollowed;

            [ProtoMember(4)]
            public bool IsAddToBlackListOnceUnfollowed
            {
                get => _isAddToBlackListOnceUnfollowed;
                set => SetProperty(ref _isAddToBlackListOnceUnfollowed, value);
            }

            private bool _isAddToPrivateBlackList;

            [ProtoMember(5)]
            public bool IsAddToPrivateBlackList
            {
                get => _isAddToPrivateBlackList;
                set => SetProperty(ref _isAddToPrivateBlackList, value);
            }

            private bool _isAddToGroupBlackList;

            [ProtoMember(6)]
            public bool IsAddToGroupBlackList
            {
                get => _isAddToGroupBlackList;
                set => SetProperty(ref _isAddToGroupBlackList, value);
            }

            #endregion
        }

        [ProtoContract]
        public class SkipBlacklist : BindableBase, ISkipBlacklist
        {
            #region Skip Blacklist

            private bool _isSkipBlackListUsers;

            [ProtoMember(1)]
            public bool IsSkipBlackListUsers
            {
                get => _isSkipBlackListUsers;
                set => SetProperty(ref _isSkipBlackListUsers, value);
            }

            private bool _isSkipPrivateBlackListUser;

            [ProtoMember(2)]
            public bool IsSkipPrivateBlackListUser
            {
                get => _isSkipPrivateBlackListUser;
                set => SetProperty(ref _isSkipPrivateBlackListUser, value);
            }

            private bool _isSkipGroupBlackListUsers;

            [ProtoMember(3)]
            public bool IsSkipGroupBlackListUsers
            {
                get => _isSkipGroupBlackListUsers;
                set => SetProperty(ref _isSkipGroupBlackListUsers, value);
            }

            #endregion
        }
    }
}