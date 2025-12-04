using DominatorHouseCore.Utility;
using ProtoBuf;

namespace YoutubeDominatorCore.YoutubeModels
{
    public interface IManageBlackWhiteListModel
    {
        bool IsSkipWhiteListUsers { get; set; }
        bool IsUsePrivateWhiteList { get; set; }
        bool IsUseGroupWhiteList { get; set; }
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

        private bool _IsSkipWhiteListUsers;

        [ProtoMember(1)]
        public bool IsSkipWhiteListUsers
        {
            get => _IsSkipWhiteListUsers;
            set => SetProperty(ref _IsSkipWhiteListUsers, value);
        }

        private bool _IsUsePrivateWhiteList;

        [ProtoMember(2)]
        public bool IsUsePrivateWhiteList
        {
            get => _IsUsePrivateWhiteList;
            set => SetProperty(ref _IsUsePrivateWhiteList, value);
        }

        private bool _IsUseGroupWhiteList;

        [ProtoMember(2)]
        public bool IsUseGroupWhiteList
        {
            get => _IsUseGroupWhiteList;
            set => SetProperty(ref _IsUseGroupWhiteList, value);
        }

        private bool _IsAddToBlackListOnceUnfollowed;

        [ProtoMember(4)]
        public bool IsAddToBlackListOnceUnfollowed
        {
            get => _IsAddToBlackListOnceUnfollowed;
            set => SetProperty(ref _IsAddToBlackListOnceUnfollowed, value);
        }

        private bool _IsAddToPrivateBlackList;

        [ProtoMember(5)]
        public bool IsAddToPrivateBlackList
        {
            get => _IsAddToPrivateBlackList;
            set => SetProperty(ref _IsAddToPrivateBlackList, value);
        }

        private bool _IsAddToGroupBlackList;

        [ProtoMember(6)]
        public bool IsAddToGroupBlackList
        {
            get => _IsAddToGroupBlackList;
            set => SetProperty(ref _IsAddToGroupBlackList, value);
        }

        #endregion
    }

    [ProtoContract]
    public class SkipBlacklist : BindableBase, ISkipBlacklist
    {
        #region Skip Blacklist

        private bool _IsSkipBlackListUsers;

        [ProtoMember(1)]
        public bool IsSkipBlackListUsers
        {
            get => _IsSkipBlackListUsers;
            set => SetProperty(ref _IsSkipBlackListUsers, value);
        }

        private bool _IsSkipPrivateBlackListUser;

        [ProtoMember(2)]
        public bool IsSkipPrivateBlackListUser
        {
            get => _IsSkipPrivateBlackListUser;
            set => SetProperty(ref _IsSkipPrivateBlackListUser, value);
        }

        private bool _IsSkipGroupBlackListUsers;

        [ProtoMember(3)]
        public bool IsSkipGroupBlackListUsers
        {
            get => _IsSkipGroupBlackListUsers;
            set => SetProperty(ref _IsSkipGroupBlackListUsers, value);
        }

        #endregion
    }
}