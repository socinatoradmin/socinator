using DominatorHouseCore.Enums;
using DominatorHouseCore.ViewModel;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using Dragablz;
using DominatorHouseCore.Models;

namespace DominatorHouse.Support.Logs.Views
{
    /// <summary>
    /// Interaction logic for LogView.xaml
    /// </summary>
    public partial class LogView
    {

        public IEnumerable<SocialNetworks?> AvailableNetworks
        {
            get
            {
                return (IEnumerable<SocialNetworks?>)GetValue(AvailableNetworksProperty);
            }
            set
            {
                SetValue(AvailableNetworksProperty, value);
            }
        }

        public IEnumerable<DominatorAccountModel> AvailableAccounts
        {
            get
            {
                return (IEnumerable<DominatorAccountModel>)GetValue(AvailableAccountsProperty);
            }
            set
            {
                SetValue(AvailableAccountsProperty, value);
            }
        }

        public ILogViewModel ViewModel
        {
            get
            {
                return (ILogViewModel)GetValue(ViewModelProperty);
            }
            set
            {
                SetValue(ViewModelProperty, value);
            }
        }

        public static readonly DependencyProperty ViewModelProperty =
            DependencyProperty.Register("ViewModel", typeof(ILogViewModel), typeof(LogView), new PropertyMetadata(null));


        public static readonly DependencyProperty AvailableNetworksProperty =
            DependencyProperty.Register("AvailableNetworks", typeof(IEnumerable<SocialNetworks?>), typeof(LogView), new PropertyMetadata(null));

        public static readonly DependencyProperty AvailableAccountsProperty =
            DependencyProperty.Register("AvailableAccounts", typeof(IEnumerable<DominatorAccountModel>), typeof(LogView), new PropertyMetadata(null));


        public LogView()
        {
            InitializeComponent();
        }

        private void TabSelectionChange(object sender, SelectionChangedEventArgs e)
        {
            var selectedTab = (sender as TabablzControl)?.SelectedItem as TabItem;
            var header = selectedTab?.Header;
            if (header != null && ViewModel != null && ViewModel.LogType != header.ToString())
                ViewModel.LogType = header.ToString();
        }

    }
}
