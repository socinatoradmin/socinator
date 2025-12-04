using System.Windows;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;

namespace DominatorUIUtility.CustomControl
{
    /// <summary>
    ///     Interaction logic for SingleAccountControl.xaml
    /// </summary>
    public partial class SingleAccountControl
    {
        public SingleAccountControl()
        {
            InitializeComponent();
        }

        public SingleAccountControl(SingleAccountModel objSingleAccountModelBinding) : this()
        {
            DataContext = objSingleAccountModelBinding;
        }
    }

    public class SingleAccountModel : BindableBase
    {
        private Proxy _accountProxy = new Proxy();

        private Visibility? _advancedOptions = Visibility.Collapsed;


        private string _btnContent;

        private string _groupName;
        private string _pageTitle;

        private string _password;

        private bool? _showAdvancedSettings = false;


        private string _userName;

        public string PageTitle
        {
            get => _pageTitle;
            set
            {
                if (_pageTitle != null && value == _pageTitle)
                    return;
                SetProperty(ref _pageTitle, value);
            }
        }

        public string GroupName
        {
            get => _groupName;
            set
            {
                if (_groupName != null && value == _groupName)
                    return;
                SetProperty(ref _groupName, value);
            }
        }

        public string UserName
        {
            get => _userName;
            set
            {
                if (_userName != null && value == _userName)
                    return;
                SetProperty(ref _userName, value);
            }
        }

        public Proxy AccountProxy
        {
            get => _accountProxy;
            set
            {
                if (_accountProxy != null && value == _accountProxy)
                    return;
                SetProperty(ref _accountProxy, value);
            }
        }

        public string Password
        {
            get => _password;
            set
            {
                if (_password != null && value == _password)
                    return;
                SetProperty(ref _password, value);
            }
        }

        public Visibility? AdvancedOptions
        {
            get => _advancedOptions;
            set
            {
                if (_advancedOptions != null && value == _advancedOptions)
                    return;
                SetProperty(ref _advancedOptions, value);
            }
        }

        public bool? ShowAdvancedSettings
        {
            get => _showAdvancedSettings;
            set
            {
                if (_showAdvancedSettings != null && value == _showAdvancedSettings)
                    return;

                if (value == true)
                    AdvancedOptions = Visibility.Visible;
                else
                    AdvancedOptions = Visibility.Collapsed;

                SetProperty(ref _showAdvancedSettings, value);
            }
        }

        public string BtnContent
        {
            get => _btnContent;
            set
            {
                if (_btnContent != null && value == _btnContent)
                    return;
                SetProperty(ref _btnContent, value);
            }
        }
    }
}