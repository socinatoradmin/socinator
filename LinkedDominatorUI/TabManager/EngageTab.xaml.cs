using System;
using System.Collections.Generic;
using System.Windows.Controls;
using DominatorHouseCore;
using DominatorHouseCore.Models;
using LinkedDominatorUI.LDViews.Engage;

namespace LinkedDominatorUI.TabManager
{
    /// <summary>
    ///     Interaction logic for EngageTab.xaml
    /// </summary>
    public partial class EngageTab : UserControl
    {
        private static EngageTab objEngageTab;

        public EngageTab()
        {
            try
            {
                InitializeComponent();
                var tab_items = new List<TabItemTemplates>
                {
                    new TabItemTemplates
                    {
                        Title = FindResource("LangKeyLike").ToString(),
                        Content = new Lazy<UserControl>(() => Like.GetSingeltonObjectLike())
                    },
                    new TabItemTemplates
                    {
                        Title = FindResource("LangKeyComment").ToString(),
                        Content = new Lazy<UserControl>(() => Comment.GetSingeltonObjectComment())
                    },
                    new TabItemTemplates
                    {
                        Title = FindResource("LangKeySharePost").ToString(),
                        Content = new Lazy<UserControl>(() => Share.GetSingeltonObjectShare())
                    }
                };
                EngageTabs.ItemsSource = tab_items;
            }
            catch (Exception ex)
            {
                ex.ErrorLog();
            }
        }


        public static EngageTab GetSingeltonObjectEngageTab()
        {
            return objEngageTab ?? (objEngageTab = new EngageTab());
        }

        public void SetIndex(int index)
        {
            //GrowConnectionsTab is the name of this Tab
            EngageTabs.SelectedIndex = index;
        }
    }
}