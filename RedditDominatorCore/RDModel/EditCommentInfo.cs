using DominatorHouseCore.Utility;

namespace RedditDominatorCore.RDModel
{
    public class EditCommentInfo : BindableBase
    {
        private string _accounts;
        private string _editCommentUrl;

        private string _message;

        private int _selectedIndex;

        public string EditCommentUrl
        {
            get => _editCommentUrl;
            set => SetProperty(ref _editCommentUrl, value);
        }

        public string Message
        {
            get => _message;
            set => SetProperty(ref _message, value);
        }

        public string Accounts
        {
            get => _accounts;
            set => SetProperty(ref _accounts, value);
        }

        public int SelectedIndex
        {
            get => _selectedIndex;
            set => SetProperty(ref _selectedIndex, value);
        }
    }
}