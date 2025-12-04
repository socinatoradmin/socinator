using System;
using System.Collections.Generic;
using System.Windows.Controls;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using TwtDominatorUI.CustomControl;
using TwtDominatorUI.TDViews.Tools.Unfollow;

namespace TwtDominatorUI.TDViews.Tools
{
    /// <summary>
    ///     Interaction logic for UnfollowTab.xaml
    /// </summary>
    public partial class UnfollowTab : UserControl
    {
        public UnfollowTab()
        {
            InitializeComponent();
            var TabItems = new List<TabItemTemplates>
            {
                new TabItemTemplates
                {
                    Title = FindResource("LangKeyConfiguration").ToString(),
                    Content = new Lazy<UserControl>(() => new UnfollowConfiguration())
                },
                new TabItemTemplates
                {
                    Title = FindResource("LangKeyReports").ToString(),
                    Content = new Lazy<UserControl>(() => new AccountWiseReport(ActivityType.Unfollow))
                }
            };
            UnfollowTabControl.ItemsSource = TabItems;
        }

        //private static UnfollowTab objUnfollowTab;
        //public static UnfollowTab GetSingletonUnfollowTab()
        //{
        //    return objUnfollowTab ?? (objUnfollowTab = new UnfollowTab());
        //}
    }
}