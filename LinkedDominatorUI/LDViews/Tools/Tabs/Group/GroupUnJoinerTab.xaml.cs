using System;
using System.Collections.Generic;
using System.Windows.Controls;
using DominatorHouseCore;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using LinkedDominatorUI.CustomControl;
using LinkedDominatorUI.LDViews.Tools.Group;

namespace LinkedDominatorUI.LDViews.Tools.Tabs
{
    /// <summary>
    ///     Interaction logic for GroupUnJoinerTab.xaml
    /// </summary>
    public partial class GroupUnJoinerTab : UserControl
    {
        private static GroupUnJoinerTab objGroupUnJoinerTab;

        public GroupUnJoinerTab()
        {
            try
            {
                InitializeComponent();
                var tab_items = new List<TabItemTemplates>
                {
                    new TabItemTemplates
                    {
                        Title = FindResource("LangKeyConfiguration").ToString(),
                        Content = new Lazy<UserControl>(() =>
                            GroupUnJoinerConfiguration.GetSingeltonObjectGroupUnJoinerConfiguration())
                    },
                    new TabItemTemplates
                    {
                        Title = FindResource("LangKeyReports").ToString(),
                        Content = new Lazy<UserControl>(() => new AccountReport(ActivityType.GroupUnJoiner))
                    }
                };
                GroupUnJoinerTabs.ItemsSource = tab_items;
            }
            catch (Exception ex)
            {
                ex.ErrorLog();
            }
        }


        public static GroupUnJoinerTab GetSingeltonObjectGroupUnJoinerTab()
        {
            return objGroupUnJoinerTab ?? (objGroupUnJoinerTab = new GroupUnJoinerTab());
        }
    }
}