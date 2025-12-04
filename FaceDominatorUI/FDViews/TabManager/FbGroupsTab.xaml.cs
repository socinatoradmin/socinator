using DominatorHouseCore.Models;
using FaceDominatorUI.FDViews.FbGroups;
using System;
using System.Collections.Generic;
using System.Windows.Controls;

namespace FaceDominatorUI.FDViews.TabManager
{
    /// <summary>
    ///     Interaction logic for FbGroupsTab.xaml
    /// </summary>
    public partial class FbGroupsTab
    {
        public FbGroupsTab()
        {
            InitializeComponent();
            var items = new List<TabItemTemplates>
            {
                new TabItemTemplates
                {
                    Title = FindResource("LangKeyGroupJoiner").ToString(),
                    Content = new Lazy<UserControl>(GroupJoiner.GetSingeltonObjectGroupJoiner)
                },
                //new TabItemTemplates
                //{
                //    Title=FindResource("LangKeyGroupUnjoiner").ToString(),
                //    Content = new Lazy<UserControl>(GroupUnjoiner.GetSingeltonObjectGroupUnjoiner)
                //},
                new TabItemTemplates
                {
                    Title = FindResource("LangKeyGroupUnjoiner").ToString(),
                    Content = new Lazy<UserControl>(GroupUnjoinerNew.GetSingeltonObjectGroupUnjoinerNew)
                }
                //new TabItemTemplates
                //{
                //    Title=FindResource("LangKeyMakeGroupAdmin").ToString(),
                //    Content = new Lazy<UserControl>(MakeGroupAdmin.GetSingletonMakeGroupAdmin)
                //}
            };

            FbGoupsTabs.ItemsSource = items;
        }


        private static FbGroupsTab CurrentFbGroupsTab { get; set; }

        public static FbGroupsTab GetSingeltonObjectFbGroupsTab()
        {
            return CurrentFbGroupsTab ?? (CurrentFbGroupsTab = new FbGroupsTab());
        }

        public void SetIndex(int tabIndex)
        {
            FbGoupsTabs.SelectedIndex = tabIndex;
        }
    }
}