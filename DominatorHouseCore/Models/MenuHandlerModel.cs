#region

using DominatorHouseCore.Utility;

#endregion

namespace DominatorHouseCore.Models
{
    public class MenuHandlerModel : BindableBase
    {
        private bool _isImportAccountsVisible=true;

        public bool IsImportMultipleAccountsVisible
        {
            get => _isImportAccountsVisible;
            set
            {
                if (_isImportAccountsVisible == value)
                    return;
                SetProperty(ref _isImportAccountsVisible, value);
            }
        }

        private bool _isSelctAccountsVisible=true;

        public bool IsSelectAccountsVisible
        {
            get => _isSelctAccountsVisible;
            set
            {
                if (_isSelctAccountsVisible == value)
                    return;
                SetProperty(ref _isSelctAccountsVisible, value);
            }
        }

        private bool _isUpdateAccountVisible = true;

        public bool IsUpdateAccountVisible
        {
            get => _isUpdateAccountVisible;
            set
            {
                if (_isUpdateAccountVisible == value)
                    return;
                SetProperty(ref _isUpdateAccountVisible, value);
            }
        }

        private bool _isExportAccountVisible = true;

        public bool IsExportAccountVisible
        {
            get => _isExportAccountVisible;
            set
            {
                if (_isExportAccountVisible == value)
                    return;
                SetProperty(ref _isExportAccountVisible, value);
            }
        }

        private bool _isDeleteAccountVisible = true;

        public bool IsDeleteAccountVisible
        {
            get => _isDeleteAccountVisible;
            set
            {
                if (_isDeleteAccountVisible == value)
                    return;
                SetProperty(ref _isDeleteAccountVisible, value);
            }
        }

        private bool _isBrowserAutomationVisible = true;

        public bool IsBrowserAutomationVisible
        {
            get => _isBrowserAutomationVisible;
            set
            {
                if (_isBrowserAutomationVisible == value)
                    return;
                SetProperty(ref _isBrowserAutomationVisible, value);
            }
        }

        private bool _isInfoVisible = true;

        public bool IsInfoVisible
        {
            get => _isInfoVisible;
            set
            {
                if (_isInfoVisible == value)
                    return;
                SetProperty(ref _isInfoVisible, value);
            }
        }

        private bool _isMenuHandlerVisible = true;

        public bool IsMenuHandlerVisible
        {
            get => _isMenuHandlerVisible;
            set
            {
                if (_isMenuHandlerVisible == value)
                    return;
                SetProperty(ref _isMenuHandlerVisible, value);
            }
        }

        private bool _isMenuHandlerChecked = true;

        public bool IsMenuHandlerChecked
        {
            get => _isMenuHandlerChecked;
            set
            {
                if (_isMenuHandlerChecked == value)
                    return;
                SetProperty(ref _isMenuHandlerChecked, value);
            }
        }
    }
}