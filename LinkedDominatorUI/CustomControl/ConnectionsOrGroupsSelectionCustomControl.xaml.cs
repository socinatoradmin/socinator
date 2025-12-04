using System;
using System.Windows;
using System.Windows.Controls;

namespace LinkedDominatorUI.CustomControl
{
    /// <summary>
    ///     Interaction logic for ConnectionsOrGroupsSelectionCustomControl.xaml
    /// </summary>
    public partial class ConnectionsOrGroupsSelectionCustomControl : UserControl
    {
        public ConnectionsOrGroupsSelectionCustomControl()
        {
            InitializeComponent();
        }


        private void cmbAllAccounts_DropDownClosed(object sender, EventArgs e)
        {
        }

        private void chkAccount_Checked(object sender, RoutedEventArgs e)
        {
        }

        private void chkAccount_Unchecked(object sender, RoutedEventArgs e)
        {
        }

        private void chkSelectAll_Checked(object sender, RoutedEventArgs e)
        {
        }

        private void chkSelectAll_Unchecked(object sender, RoutedEventArgs e)
        {
        }
    }
}