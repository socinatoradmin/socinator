using System;
using System.Collections.Generic;
using System.Windows.Controls;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using TwtDominatorUI.CustomControl;
using TwtDominatorUI.TDViews.Tools.Reposter;

namespace TwtDominatorUI.TDViews.Tools
{
    /// <summary>
    ///     Interaction logic for ReposterTab.xaml
    /// </summary>
    public partial class ReposterTab : UserControl
    {
        public ReposterTab()
        {
            InitializeComponent();
            var TabItems = new List<TabItemTemplates>
            {
                new TabItemTemplates
                {
                    Title = FindResource("LangKeyConfiguration").ToString(),
                    Content = new Lazy<UserControl>(() => new ReposterConfiguration())
                },
                new TabItemTemplates
                {
                    Title = FindResource("LangKeyReports").ToString(),
                    Content = new Lazy<UserControl>(() => new AccountWiseReport(ActivityType.Reposter))
                }
            };
            ReposterTabControl.ItemsSource = TabItems;
        }

        //private static ReposterTab objReposterTab;
        //public static ReposterTab GetSingletonReposterTab()
        //{
        //    return objReposterTab ?? (objReposterTab = new ReposterTab());
        //}
    }
}