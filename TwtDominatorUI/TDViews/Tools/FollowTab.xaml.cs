using System;
using System.Collections.Generic;
using System.Windows.Controls;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using TwtDominatorUI.CustomControl;
using TwtDominatorUI.TDViews.Tools.Follow;

namespace TwtDominatorUI.TDViews.Tools
{
    /// <summary>
    ///     Interaction logic for FollowTab.xaml
    /// </summary>
    public partial class FollowTab : UserControl
    {
        public FollowTab()
        {
            InitializeComponent();
            var TabItems = new List<TabItemTemplates>
            {
                new TabItemTemplates
                {
                    Title = FindResource("LangKeyConfiguration").ToString(),
                    Content = new Lazy<UserControl>(() => new FollowConfiguration())
                },
                new TabItemTemplates
                {
                    Title = FindResource("LangKeyReports").ToString(),
                    Content = new Lazy<UserControl>(() => new AccountWiseReport(ActivityType.Follow))
                }
            };
            FollowTabControl.ItemsSource = TabItems;
        }

        //private static FollowTab objFollowTab;
        //public static FollowTab GetSingletonFollowTab()
        //{
        //    return objFollowTab ?? (objFollowTab = new FollowTab());
        //}
    }
}