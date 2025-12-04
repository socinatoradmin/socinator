#region

using System.Collections.ObjectModel;
using DominatorHouseCore.Utility;

#endregion

namespace DominatorHouseCore.Models
{
    public class SelectAccountModel : BindableBase
    {
        private bool _isAccountSelected;

        public bool IsAccountSelected
        {
            get => _isAccountSelected;
            set
            {
                if (_isAccountSelected == value)
                    return;
                SetProperty(ref _isAccountSelected, value);
            }
        }

        private string _userName;

        public string UserName
        {
            get => _userName;
            set
            {
                if (_userName == value)
                    return;
                SetProperty(ref _userName, value);
            }
        }

        private string _groupName;

        public string GroupName
        {
            get => _groupName;
            set
            {
                if (_groupName == value)
                    return;
                SetProperty(ref _groupName, value);
            }
        }

        private bool _browserAutomation;

        public bool BrowserAutomation
        {
            get => _browserAutomation;
            set
            {
                if (_browserAutomation == value)
                    return;
                SetProperty(ref _browserAutomation, value);
            }
        }

        private ObservableCollection<ContentSelectGroup> _groups = new ObservableCollection<ContentSelectGroup>();

        public ObservableCollection<ContentSelectGroup> Groups
        {
            get => _groups;
            set
            {
                if (_groups == value)
                    return;
                SetProperty(ref _groups, value);
            }
        }

        private string _groupText = "LangKeySelectGroups".FromResourceDictionary();

        public string GroupText
        {
            get => _groupText;
            set => SetProperty(ref _groupText, value);
        }

        private string _accountNikeName;

        public string AccountNikeName
        {
            get => _accountNikeName;
            set => SetProperty(ref _accountNikeName, value);
        }
    }
}