using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using DominatorHouseCore.Models;
using GramDominatorUI.GDViews.Tools.Instachat;

namespace GramDominatorUI.GDViews.Tools
{
    /// <summary>
    ///     Interaction logic for InstachatTab.xaml
    /// </summary>
    public partial class InstachatTab : UserControl
    {
        public InstachatTab()
        {
            InitializeComponent();
            var tabItems = new List<TabItemTemplates>
            {
                new TabItemTemplates
                {
                    Title = Application.Current.FindResource("LangKeyConfiguration") != null
                        ? Application.Current.FindResource("LangKeyConfiguration").ToString()
                        : "Configuration",
                    Content = new Lazy<UserControl>(() => new InstachatConfiguration())
                },
                new TabItemTemplates
                {
                    Title = Application.Current.FindResource("LangKeyReports") != null
                        ? Application.Current.FindResource("LangKeyReports").ToString()
                        : "Reports",
                    Content = new Lazy<UserControl>(() => new InstachatReports())
                }
            };
            InstachatTabs.ItemsSource = tabItems;
        }
    }
}