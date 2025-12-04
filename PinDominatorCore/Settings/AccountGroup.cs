using DominatorHouseCore.Utility;
using ProtoBuf;

namespace PinDominatorCore.Settings
{
    [ProtoContract]
    public class AccountGroup : BindableBase
    {
        private string _accountGroupName;
        private bool _isAccountGroupSelected;

        [ProtoMember(1)]
        public string AccountGroupName
        {
            get => _accountGroupName;
            set
            {
                if (_accountGroupName != null && value == _accountGroupName)
                    return;
                SetProperty(ref _accountGroupName, value);
            }
        }

        [ProtoMember(2)]
        public bool IsAccountGroupSelected
        {
            get => _isAccountGroupSelected;
            set
            {
                if (value == _isAccountGroupSelected)
                    return;
                SetProperty(ref _isAccountGroupSelected, value);
            }
        }
    }
}