#region

using DominatorHouseCore.Utility;
using ProtoBuf;

#endregion

namespace DominatorHouseCore.Models
{
    [ProtoContract]
    internal class PinterestModel : BindableBase
    {
        private bool _isEnableFollowDifferentUsersAcrossPDAccountsChecked;

        [ProtoMember(1)]
        public bool IsEnableFollowDifferentUsersAcrossPDAccountsChecked
        {
            get => _isEnableFollowDifferentUsersAcrossPDAccountsChecked;
            set
            {
                if (value == _isEnableFollowDifferentUsersAcrossPDAccountsChecked)
                    return;
                SetProperty(ref _isEnableFollowDifferentUsersAcrossPDAccountsChecked, value);
            }
        }

        private bool _isAllowDoubleRepinChecked;

        [ProtoMember(2)]
        public bool IsAllowDoubleRepinChecked
        {
            get => _isAllowDoubleRepinChecked;
            set
            {
                if (value == _isAllowDoubleRepinChecked)
                    return;
                SetProperty(ref _isAllowDoubleRepinChecked, value);
            }
        }
    }
}