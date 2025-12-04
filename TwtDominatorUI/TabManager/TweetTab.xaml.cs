using System.Collections.Generic;
using System.Windows.Controls;
using DominatorHouseCore.Models;

namespace TwtDominatorUI.TabManager
{
    /// <summary>
    ///     Interaction logic for TweetTab.xaml
    /// </summary>
    public partial class TweetTab : UserControl
    {
        public TweetTab()
        {
            InitializeComponent();

            var tabItems = new List<TabItemTemplates>();

            TweetTabs.ItemsSource = tabItems;
        }
    }
}