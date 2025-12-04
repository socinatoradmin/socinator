using System;
using System.Collections.Generic;
using System.Windows.Controls;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using TwtDominatorUI.CustomControl;
using TwtDominatorUI.TDViews.Tools.Like;

namespace TwtDominatorUI.TDViews.Tools
{
    /// <summary>
    ///     Interaction logic for LikeTab.xaml
    /// </summary>
    public partial class LikeTab : UserControl
    {
        public LikeTab()
        {
            InitializeComponent();
            var TabItems = new List<TabItemTemplates>
            {
                new TabItemTemplates
                {
                    Title = FindResource("LangKeyConfiguration").ToString(),
                    Content = new Lazy<UserControl>(() => new LikeConfiguration())
                },
                new TabItemTemplates
                {
                    Title = FindResource("LangKeyReports").ToString(),
                    Content = new Lazy<UserControl>(() => new AccountWiseReport(ActivityType.Like))
                }
            };
            LikeTabControl.ItemsSource = TabItems;
        }

        //private static LikeTab objLikeTab;
        //public static LikeTab GetSingletonLikeTab()
        //{
        //    return objLikeTab ?? (objLikeTab = new LikeTab());
        //}
    }
}