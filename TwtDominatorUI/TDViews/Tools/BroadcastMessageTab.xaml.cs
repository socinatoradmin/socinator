using System;
using System.Collections.Generic;
using System.Windows.Controls;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using TwtDominatorUI.CustomControl;
using TwtDominatorUI.TDViews.Tools.Message;

namespace TwtDominatorUI.TDViews.Tools
{
    /// <summary>
    ///     Interaction logic for BroadcastMessageTab.xaml
    /// </summary>
    public partial class BroadcastMessageTab : UserControl
    {
        public BroadcastMessageTab()
        {
            InitializeComponent();
            var TabItems = new List<TabItemTemplates>
            {
                new TabItemTemplates
                {
                    Title = FindResource("LangKeyConfiguration").ToString(),
                    Content = new Lazy<UserControl>(() => new BroadCastMessageConfig())
                },
                new TabItemTemplates
                {
                    Title = FindResource("LangKeyReports").ToString(),
                    Content = new Lazy<UserControl>(() => new AccountWiseReport(ActivityType.BroadcastMessages))
                }
            };
            BroadcastMessageTabControl.ItemsSource = TabItems;
        }

        //private static BroadcastMessageTab objBroadcastMessageTab;
        //public static BroadcastMessageTab GetSingletonobjBroadcastMessageTab()
        //{
        //    return objBroadcastMessageTab ?? (objBroadcastMessageTab = new BroadcastMessageTab());
        //}
    }
}