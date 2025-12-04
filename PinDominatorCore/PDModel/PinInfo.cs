using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Utility;

namespace PinDominatorCore.PDModel
{
    public class PinInfo : BindableBase, IPost
    {
        private string _account;
        private string _board;
        private string _title;

        private string _pinDescription;

        private string _pinToBeEdit;


        private string _section;

        private int _selectedIndex;

        private string _websiteUrl;

        public string Board
        {
            get => _board;
            set
            {
                if (_board != null && _board == value)
                    return;
                SetProperty(ref _board, value);
            }
        }
        public string Title
        {
            get => _title;
            set
            {
                if (_title != null && _title == value)
                    return;
                SetProperty(ref _title, value);
            }
        }
        public string PinDescription
        {
            get => _pinDescription;
            set
            {
                if (_pinDescription != null && _pinDescription == value)
                    return;
                SetProperty(ref _pinDescription, value);
            }
        }

        public string Section
        {
            get => _section;
            set
            {
                if (_section != null && _section == value)
                    return;
                SetProperty(ref _section, value);
            }
        }

        public string WebsiteUrl
        {
            get => _websiteUrl;
            set
            {
                if (_websiteUrl != null && _websiteUrl == value)
                    return;
                SetProperty(ref _websiteUrl, value);
            }
        }

        public string PinToBeEdit
        {
            get => _pinToBeEdit;
            set
            {
                if (_pinToBeEdit != null && _pinToBeEdit == value)
                    return;
                SetProperty(ref _pinToBeEdit, value);
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

        public string Id { get; set; }
        public string Caption { get; set; }
        public string Code { get; set; }
    }
}