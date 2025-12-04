using System;
using System.Collections.Generic;
using System.Windows.Controls;
using DominatorHouseCore;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using LinkedDominatorUI.CustomControl;
using LinkedDominatorUI.LDViews.Tools.Engage;

namespace LinkedDominatorUI.LDViews.Tools.Tabs
{
    /// <summary>
    ///     Interaction logic for LikeTab.xaml
    /// </summary>
    public partial class LikeTab : UserControl
    {
        private static LikeTab objLikeTab;

        public LikeTab()
        {
            try
            {
                InitializeComponent();
                var tab_items = new List<TabItemTemplates>
                {
                    new TabItemTemplates
                    {
                        Title = FindResource("LangKeyConfiguration").ToString(),
                        Content = new Lazy<UserControl>(LikeConfiguration.GetSingletonObjectLikeConfiguration)
                    },
                    new TabItemTemplates
                    {
                        Title = FindResource("LangKeyReports").ToString(),
                        Content = new Lazy<UserControl>(() => new AccountReport(ActivityType.Like))
                    }
                };
                LikeTabs.ItemsSource = tab_items;
            }
            catch (Exception ex)
            {
                ex.ErrorLog();
            }
        }


        public static LikeTab GetSingeltonObjectLikeTab()
        {
            return objLikeTab ?? (objLikeTab = new LikeTab());
        }
    }
}