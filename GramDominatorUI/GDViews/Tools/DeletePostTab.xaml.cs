using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using GramDominatorUI.CustomControl;
using GramDominatorUI.GDViews.Tools.DeletePost;

namespace GramDominatorUI.GDViews.Tools
{
    /// <summary>
    ///     Interaction logic for DeletePostTab.xaml
    /// </summary>
    public partial class DeletePostTab : UserControl
    {
        public DeletePostTab()
        {
            InitializeComponent();
            var TabItems = new List<TabItemTemplates>
            {
                new TabItemTemplates
                {
                    Title = Application.Current.FindResource("LangKeyConfiguration") != null
                        ? Application.Current.FindResource("LangKeyConfiguration").ToString()
                        : "Configuration",
                    Content = new Lazy<UserControl>(DeletePostConfiguration.GetSingeltonObjectDeletePostConfiguration)
                },
                new TabItemTemplates
                {
                    Title = Application.Current.FindResource("LangKeyReports") != null
                        ? Application.Current.FindResource("LangKeyReports").ToString()
                        : "Reports",
                    Content = new Lazy<UserControl>(() => new AccountReport(ActivityType.DeletePost))
                }
            };
            DeletePost.ItemsSource = TabItems;
        }
    }
}