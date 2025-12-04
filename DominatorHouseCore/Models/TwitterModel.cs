#region

using DominatorHouseCore.Utility;
using ProtoBuf;

#endregion

namespace DominatorHouseCore.Models
{
    [ProtoContract]
    internal class TwitterModel : BindableBase
    {
        private bool _isEnableFollowDifferentUsersAcrossTwitterAccountsChecked;

        [ProtoMember(1)]
        public bool IsEnableFollowDifferentUsersAcrossTwitterAccountsChecked
        {
            get => _isEnableFollowDifferentUsersAcrossTwitterAccountsChecked;
            set
            {
                if (value == _isEnableFollowDifferentUsersAcrossTwitterAccountsChecked)
                    return;
                SetProperty(ref _isEnableFollowDifferentUsersAcrossTwitterAccountsChecked, value);
            }
        }

        private bool _isEnableFollowDifferentUsersAcrossTwitterAccountsWithSameTagnameChecked;

        [ProtoMember(2)]
        public bool IsEnableFollowDifferentUsersAcrossTwitterAccountsWithSameTagnameChecked
        {
            get => _isEnableFollowDifferentUsersAcrossTwitterAccountsWithSameTagnameChecked;
            set
            {
                if (value == _isEnableFollowDifferentUsersAcrossTwitterAccountsWithSameTagnameChecked)
                    return;
                SetProperty(ref _isEnableFollowDifferentUsersAcrossTwitterAccountsWithSameTagnameChecked, value);
            }
        }
    }
}