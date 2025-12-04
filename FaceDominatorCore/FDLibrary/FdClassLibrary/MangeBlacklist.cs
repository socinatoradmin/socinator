using DominatorHouseCore.Utility;
using ProtoBuf;

namespace FaceDominatorCore.FDLibrary.FdClassLibrary
{
    public class MangeBlacklist
    {
        public interface IManageBlackWhiteListModel
        {
            //            bool IsSkipWhiteListUsers { get; set; }
            //            bool IsUsePrivateWhiteList { get; set; }
            //            bool IsUseGroupWhiteList { get; set; }
            //            bool IsAddToBlackListOnceUnfriend { get; set; }
            //            bool IsAddToPrivateBlackList { get; set; }
            //            bool IsAddToGroupBlackList { get; set; }
        }

        public interface ISkipBlacklist
        {
            //            bool IsSkipBlackListUsers { get; set; }
            //            bool IsSkipPrivateBlackListUser { get; set; }
            //            bool IsSkipGroupBlackListUsers { get; set; }
        }

        [ProtoContract]
        public class ManageBlackWhiteListModel : BindableBase, IManageBlackWhiteListModel
        {
            #region BlackList and Whitelist
            private bool _isSkipWhiteListUsers;
            [ProtoMember(1)]
            public bool IsSkipWhiteListUsers
            {
                get { return _isSkipWhiteListUsers; }
                set { SetProperty(ref _isSkipWhiteListUsers, value); }
            }

            private bool _isUsePrivateWhiteList;
            [ProtoMember(2)]
            public bool IsUsePrivateWhiteList
            {
                get { return _isUsePrivateWhiteList; }
                set { SetProperty(ref _isUsePrivateWhiteList, value); }
            }

            private bool _isUseGroupWhiteList;
            [ProtoMember(2)]
            public bool IsUseGroupWhiteList
            {
                get { return _isUseGroupWhiteList; }
                set { SetProperty(ref _isUseGroupWhiteList, value); }
            }

            private bool _isAddToBlackListOnceUnfollowed;
            [ProtoMember(4)]
            public bool IsAddToBlackListOnceUnfriend
            {
                get { return _isAddToBlackListOnceUnfollowed; }
                set { SetProperty(ref _isAddToBlackListOnceUnfollowed, value); }
            }

            private bool _isAddToPrivateBlackList;
            [ProtoMember(5)]
            public bool IsAddToPrivateBlackList
            {
                get { return _isAddToPrivateBlackList; }
                set { SetProperty(ref _isAddToPrivateBlackList, value); }
            }

            private bool _isAddToGroupBlackList;
            [ProtoMember(6)]
            public bool IsAddToGroupBlackList
            {
                get { return _isAddToGroupBlackList; }
                set { SetProperty(ref _isAddToGroupBlackList, value); }
            }

            private bool _isAddToBlacklistOnceRequestWithdrawn;
            [ProtoMember(7)]
            public bool IsAddToBlacklistOnceRequestWithdrawn
            {
                get { return _isAddToBlacklistOnceRequestWithdrawn; }
                set { SetProperty(ref _isAddToBlacklistOnceRequestWithdrawn, value); }
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
                get { return _isSkipBlackListUsers; }
                set { SetProperty(ref _isSkipBlackListUsers, value); }
            }

            private bool _isSkipPrivateBlackListUser;
            [ProtoMember(2)]
            public bool IsSkipPrivateBlackListUser
            {
                get { return _isSkipPrivateBlackListUser; }
                set { SetProperty(ref _isSkipPrivateBlackListUser, value); }
            }

            private bool _isSkipGroupBlackListUsers;
            [ProtoMember(3)]
            public bool IsSkipGroupBlackListUsers
            {
                get { return _isSkipGroupBlackListUsers; }
                set { SetProperty(ref _isSkipGroupBlackListUsers, value); }
            }
            #endregion
        }
    }
}
