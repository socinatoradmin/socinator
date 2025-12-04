#region

using DominatorHouseCore.Utility;
using ProtoBuf;

#endregion

namespace DominatorHouseCore.Models
{
    [ProtoContract]
    internal class TumblrModel : BindableBase
    {
        private bool _isEnableFollowDifferentUsersAcrossTumblrAccountsChecked;

        [ProtoMember(1)]
        public bool IsEnableFollowDifferentUsersAcrossTumblrAccountsChecked
        {
            get => _isEnableFollowDifferentUsersAcrossTumblrAccountsChecked;
            set
            {
                if (value == _isEnableFollowDifferentUsersAcrossTumblrAccountsChecked)
                    return;
                SetProperty(ref _isEnableFollowDifferentUsersAcrossTumblrAccountsChecked, value);
            }
        }
    }
}