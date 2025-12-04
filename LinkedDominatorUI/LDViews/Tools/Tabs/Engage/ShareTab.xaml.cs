using System;
using System.Collections.Generic;
using System.Windows.Controls;
using DominatorHouseCore;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using LinkedDominatorUI.CustomControl;
using LinkedDominatorUI.LDViews.Tools.Engage;

namespace LinkedDominatorUI.LDViews.Tools.Tabs.Engage
{
    /// <summary>
    ///     Interaction logic for ShareTab.xaml
    /// </summary>
    public partial class ShareTab : UserControl
    {
        private static ShareTab objShareTab;

        public ShareTab()
        {
            try
            {
                InitializeComponent();
                var tab_items = new List<TabItemTemplates>
                {
                    new TabItemTemplates
                    {
                        Title = FindResource("LangKeyConfiguration").ToString(),
                        Content = new Lazy<UserControl>(ShareConfiguration.GetSingletonObjectShareConfiguration)
                    },
                    new TabItemTemplates
                    {
                        Title = FindResource("LangKeyReports").ToString(),
                        Content = new Lazy<UserControl>(() => new AccountReport(ActivityType.Share))
                    }
                };
                ShareTabs.ItemsSource = tab_items;
            }
            catch (Exception ex)
            {
                ex.ErrorLog();
            }
        }


        public static ShareTab GetSingeltonObjectShareTab()
        {
            return objShareTab ?? (objShareTab = new ShareTab());
        }
    }
}