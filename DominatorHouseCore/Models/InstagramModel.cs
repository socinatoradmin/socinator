#region

using DominatorHouseCore.Utility;
using ProtoBuf;

#endregion

namespace DominatorHouseCore.Models
{
    [ProtoContract]
    public class InstagramModel : BindableBase
    {
        private int _waitMinimumOf;

        [ProtoMember(1)]
        public int WaitMinimumOf
        {
            get => _waitMinimumOf;
            set
            {
                if (value == _waitMinimumOf)
                    return;
                SetProperty(ref _waitMinimumOf, value);
            }
        }

        private bool _isDisableSyncForNewAccountsChecked;

        [ProtoMember(2)]
        public bool IsDisableSyncForNewAccountsChecked
        {
            get => _isDisableSyncForNewAccountsChecked;
            set
            {
                if (value == _isDisableSyncForNewAccountsChecked)
                    return;
                SetProperty(ref _isDisableSyncForNewAccountsChecked, value);
            }
        }

        private int _maxOfFbAccount;

        [ProtoMember(3)]
        public int MaxOfFbAccount
        {
            get => _maxOfFbAccount;
            set
            {
                if (value == _maxOfFbAccount)
                    return;
                SetProperty(ref _maxOfFbAccount, value);
            }
        }
    }
}