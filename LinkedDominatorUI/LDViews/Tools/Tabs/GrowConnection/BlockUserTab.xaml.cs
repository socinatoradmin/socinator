using System;
using System.Collections.Generic;
using System.Windows.Controls;
using DominatorHouseCore;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using LinkedDominatorUI.CustomControl;
using LinkedDominatorUI.LDViews.Tools.GrowConnection;

namespace LinkedDominatorUI.LDViews.Tools.Tabs.GrowConnection
{
    /// <summary>
    ///     Interaction logic for BlockUserTab.xaml
    /// </summary>
    public partial class BlockUserTab : UserControl
    {
        private static BlockUserTab _objBlockUserTab;


        public BlockUserTab()
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
                            BlockUserConfiguration.GetSingletonObjectBlockUserConfiguration())
                    },
                    new TabItemTemplates
                    {
                        Title = FindResource("LangKeyReports").ToString(),
                        Content = new Lazy<UserControl>(() => new AccountReport(ActivityType.BlockUser))
                    }
                };
                BlockUserTabs.ItemsSource = tab_items;
            }
            catch (Exception ex)
            {
                ex.ErrorLog();
            }
        }


        public static BlockUserTab GetSingletonObjectBlockUserTab()
        {
            return _objBlockUserTab ?? (_objBlockUserTab = new BlockUserTab());
        }
    }
}