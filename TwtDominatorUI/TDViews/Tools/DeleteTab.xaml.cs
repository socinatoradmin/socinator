using System;
using System.Collections.Generic;
using System.Windows.Controls;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using TwtDominatorUI.CustomControl;
using TwtDominatorUI.TDViews.Tools.Delete;

namespace TwtDominatorUI.TDViews.Tools
{
    /// <summary>
    ///     Interaction logic for DeleteTab.xaml
    /// </summary>
    public partial class DeleteTab : UserControl
    {
        public DeleteTab()
        {
            InitializeComponent();
            var TabItems = new List<TabItemTemplates>
            {
                new TabItemTemplates
                {
                    Title = FindResource("LangKeyConfiguration").ToString(),
                    Content = new Lazy<UserControl>(() => new DeleteConfiguration())
                },
                new TabItemTemplates
                {
                    Title = FindResource("LangKeyReports").ToString(),
                    Content = new Lazy<UserControl>(() => new AccountWiseReport(ActivityType.Delete))
                }
            };
            DeleteTabControl.ItemsSource = TabItems;
        }

        //private static DeleteTab objDeleteTab;
        //public static DeleteTab GetSingletonobjDeleteTab()
        //{
        //    return objDeleteTab ?? (objDeleteTab = new DeleteTab());
        //}
    }
}