using System;
using System.Collections.Generic;
using System.Windows.Controls;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using TwtDominatorUI.CustomControl;
using TwtDominatorUI.TDViews.Tools.FollowBack;

namespace TwtDominatorUI.TDViews.Tools
{
    /// <summary>
    ///     Interaction logic for FollowBackTab.xaml
    /// </summary>
    public partial class FollowBackTab : UserControl
    {
        public FollowBackTab()
        {
            InitializeComponent();
            var TabItems = new List<TabItemTemplates>
            {
                new TabItemTemplates
                {
                    Title = FindResource("LangKeyConfiguration").ToString(),
                    Content = new Lazy<UserControl>(() => new FollowBackConfiguration())
                },
                new TabItemTemplates
                {
                    Title = FindResource("LangKeyReports").ToString(),
                    Content = new Lazy<UserControl>(() => new AccountWiseReport(ActivityType.FollowBack))
                }
            };
            FollowBackTabControl.ItemsSource = TabItems;
        }

        //private static FollowBackTab objFollowBackTab;
        //public static FollowBackTab GetSingletonFollowBackTab()
        //{
        //    return objFollowBackTab ?? (objFollowBackTab = new FollowBackTab());
        //}
    }
}