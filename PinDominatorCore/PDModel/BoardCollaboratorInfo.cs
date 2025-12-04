using DominatorHouseCore.Utility;

namespace PinDominatorCore.PDModel
{
    public class BoardCollaboratorInfo : BindableBase
    {
        private string _account;
        private string _boardUrl;

        private string _email;

        private int _selectedIndex;

        public string BoardUrl
        {
            get => _boardUrl;
            set
            {
                if (_boardUrl != null && _boardUrl == value)
                    return;
                SetProperty(ref _boardUrl, value);
            }
        }

        public string Email
        {
            get => _email;
            set
            {
                if (_email != null && _email == value)
                    return;
                SetProperty(ref _email, value);
            }
        }

        public string Account
        {
            get => _account;
            set
            {
                if (_account != null && _account == value)
                    return;
                SetProperty(ref _account, value);
            }
        }

        public int SelectedIndex
        {
            get => _selectedIndex;
            set
            {
                if (_selectedIndex != 0 && _selectedIndex == value)
                    return;
                SetProperty(ref _selectedIndex, value);
            }
        }
    }
}