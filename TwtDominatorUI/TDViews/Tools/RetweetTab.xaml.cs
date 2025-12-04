using System;
using System.Collections.Generic;
using System.Windows.Controls;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using TwtDominatorUI.CustomControl;
using TwtDominatorUI.TDViews.Tools.Retweet;

namespace TwtDominatorUI.TDViews.Tools
{
    /// <summary>
    ///     Interaction logic for RetweetTab.xaml
    /// </summary>
    public partial class RetweetTab : UserControl
    {
        public RetweetTab()
        {
            InitializeComponent();
            var TabItems = new List<TabItemTemplates>
            {
                new TabItemTemplates
                {
                    Title = FindResource("LangKeyConfiguration").ToString(),
                    Content = new Lazy<UserControl>(() => new RetweetConfiguration())
                },
                new TabItemTemplates
                {
                    Title = FindResource("LangKeyReports").ToString(),
                    Content = new Lazy<UserControl>(() => new AccountWiseReport(ActivityType.Retweet))
                }
            };
            RetweetTabControl.ItemsSource = TabItems;
        }

        //private static RetweetTab objRetweetTab;
        //public static RetweetTab GetSingletonRetweetTab()
        //{
        //    return objRetweetTab ?? (objRetweetTab = new RetweetTab());
        //}
    }
}