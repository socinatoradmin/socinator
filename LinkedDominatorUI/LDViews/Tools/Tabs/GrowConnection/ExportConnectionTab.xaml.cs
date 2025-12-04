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
    ///     Interaction logic for ExportConnectionTab.xaml
    /// </summary>
    public partial class ExportConnectionTab : UserControl
    {
        private static ExportConnectionTab ObjExportConnectionTab;

        public ExportConnectionTab()
        {
            try
            {
                InitializeComponent();
                var tab_items = new List<TabItemTemplates>
                {
                    new TabItemTemplates
                    {
                        Title = FindResource("LangKeyConfiguration").ToString(),
                        Content = new Lazy<UserControl>(ExportConnectionConfiguration
                            .GetSingeltonObjectExportConnectionConfiguration)
                    },
                    new TabItemTemplates
                    {
                        Title = FindResource("LangKeyReports").ToString(),
                        Content = new Lazy<UserControl>(() => new AccountReport(ActivityType.ExportConnection))
                    }
                };
                ExportConnectionTabs.ItemsSource = tab_items;
            }
            catch (Exception ex)
            {
                ex.ErrorLog();
            }
        }


        public static ExportConnectionTab GetSingeltonObjExportConnectionTab()
        {
            return ObjExportConnectionTab ?? (ObjExportConnectionTab = new ExportConnectionTab());
        }
    }
}