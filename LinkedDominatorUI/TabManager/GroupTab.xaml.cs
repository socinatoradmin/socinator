using System;
using System.Collections.Generic;
using System.Windows.Controls;
using DominatorHouseCore;
using DominatorHouseCore.Models;
using LinkedDominatorUI.LDViews.Group;

namespace LinkedDominatorUI.TabManager
{
    /// <summary>
    ///     Interaction logic for GroupTab.xaml
    /// </summary>
    public partial class GroupTab : UserControl
    {
        private static GroupTab objGroupTab;

        public GroupTab()
        {
            try
            {
                InitializeComponent();
                var tab_items = new List<TabItemTemplates>
                {
                    new TabItemTemplates
                    {
                        Title = FindResource("LangKeyGroupJoiner").ToString(),
                        Content = new Lazy<UserControl>(() => GroupJoiner.GetSingeltonObjectGroupJoiner())
                    },
                    new TabItemTemplates
                    {
                        Title = FindResource("LangKeyGroupUnjoiner").ToString(),
                        Content = new Lazy<UserControl>(() => GroupUnJoiner.GetSingeltonObjectGroupUnJoiner())
                    }
                    //new TabItemTemplates
                    //{
                    //    Title=FindResource("LangKeyGroupInviter").ToString(),
                    //    Content=new Lazy<UserControl>(()=> GroupInviter.GetSingeltonObjectGroupInviter())
                    //}
                };
                GroupTabs.ItemsSource = tab_items;
            }
            catch (Exception ex)
            {
                ex.ErrorLog();
            }
        }


        public static GroupTab GetSingeltonObjectGroupTab()
        {
            return objGroupTab ?? (objGroupTab = new GroupTab());
        }

        public void SetIndex(int index)
        {
            //GrowConnectionsTab is the name of this Tab
            GroupTabs.SelectedIndex = index;
        }
    }
}