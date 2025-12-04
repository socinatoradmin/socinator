#region

using DominatorHouseCore.Utility;

#endregion

namespace DominatorHouseCore.Models
{
    public class BlacklistUserModel : BindableBase
    {
        private string _blacklistUser;

        public string BlacklistUser
        {
            get => _blacklistUser;
            set
            {
                if (value == _blacklistUser)
                    return;
                SetProperty(ref _blacklistUser, value);
            }
        }

        private bool _isBlackListUserChecked;

        public bool IsBlackListUserChecked
        {
            get => _isBlackListUserChecked;
            set
            {
                if (value == _isBlackListUserChecked)
                    return;
                SetProperty(ref _isBlackListUserChecked, value);
            }
        }
    }
}