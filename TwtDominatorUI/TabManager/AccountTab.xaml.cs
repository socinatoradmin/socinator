using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using CommonServiceLocator;
using DominatorHouseCore.Annotations;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using DominatorUIUtility.CustomControl;
using DominatorUIUtility.ViewModel;
using DominatorUIUtility.Views;

namespace TwtDominatorUI.TabManager
{
    /// <summary>
    ///     Interaction logic for AccountTab.xaml
    /// </summary>
    public partial class AccountTab : UserControl, INotifyPropertyChanged
    {
        private List<TabItemTemplates> _tabItems;

        public AccountTab(AccessorStrategies strategies)
        {
            InitializeComponent();
            TwitterAccountTab.DataContext = this;
            TabItems = new List<TabItemTemplates>
            {
                new TabItemTemplates
                {
                    Title = Application.Current.FindResource("LangKeyAccountsManager").ToString(),
                    Content = new Lazy<UserControl>(() =>
                        AccountManager.GetSingletonAccountManager("AccountManager", null, SocialNetworks.Twitter))
                },
                new TabItemTemplates
                {
                    Title = "LangKeyAccountGrowth".FromResourceDictionary(),
                    Content = new Lazy<UserControl>(() => InstanceProvider.GetInstance<AccountGrowthControl>())
                }
            };
        }

        public List<TabItemTemplates> TabItems
        {
            get => _tabItems;
            set
            {
                _tabItems = value;
                OnPropertyChanged(nameof(TabItems));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void AccountManager_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var textBlockDetails = (FrameworkElement) sender as TextBlock;
            if (textBlockDetails?.Text == FindResource("LangKeyAccountsManager").ToString() &&
                AccountManagerViewModel.GetSingletonAccountManagerViewModel().LastControlType != "AccountDetail")
                AccountManagerViewModel.GetSingletonAccountManagerViewModel().SelectedUserControl =
                    AccountCustomControl.GetAccountCustomControl(SocialNetworks.Twitter);
        }
    }
}