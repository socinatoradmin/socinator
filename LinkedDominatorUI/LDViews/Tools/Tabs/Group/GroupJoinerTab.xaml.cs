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
    ///     Interaction logic for GroupJoinerTab.xaml
    /// </summary>
    public partial class GroupJoinerTab : UserControl
    {
        private static GroupJoinerTab objGroupJoinerTab;

        public GroupJoinerTab()
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
                            GroupJoinerConfiguration.GetSingeltonObjectGroupJoinerConfiguration())
                    },
                    new TabItemTemplates
                    {
                        Title = FindResource("LangKeyReports").ToString(),
                        Content = new Lazy<UserControl>(() => new AccountReport(ActivityType.GroupJoiner))
                    }
                };
                GroupJoinerTabs.ItemsSource = tab_items;
            }
            catch (Exception ex)
            {
                ex.ErrorLog();
            }
        }


        public static GroupJoinerTab GetSingeltonObjectGroupJoinerTab()
        {
            return objGroupJoinerTab ?? (objGroupJoinerTab = new GroupJoinerTab());
        }
    }
}