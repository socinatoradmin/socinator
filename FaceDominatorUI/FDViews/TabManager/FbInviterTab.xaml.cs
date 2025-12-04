using DominatorHouseCore.Models;
using FaceDominatorUI.FDViews.FbInviter;
using System;
using System.Collections.Generic;
using System.Windows.Controls;

namespace FaceDominatorUI.FDViews.TabManager
{
    /// <summary>
    ///     Interaction logic for FbInviterTab.xaml
    /// </summary>
    public partial class FbInviterTab
    {
        public FbInviterTab()
        {
            InitializeComponent();
            var items = new List<TabItemTemplates>
            {
                new TabItemTemplates
                {
                    Title = FindResource("LangKeyGroupInviter").ToString(),
                    Content = new Lazy<UserControl>(GroupInviter.GetSingeltonObjectGroupInviter)
                },
                new TabItemTemplates
                {
                    Title = FindResource("LangKeyPageInviter").ToString(),
                    Content = new Lazy<UserControl>(PageInviter.GetSingeltonObjectPageInviter)
                },
                new TabItemTemplates
                {
                    Title = FindResource("LangKeyEventInviter").ToString(),
                    Content = new Lazy<UserControl>(EventInviter.GetSingeltonObjectEventInviter)
                },
                new TabItemTemplates
                {
                    Title = FindResource("LangKeyWatchPartyInviter").ToString(),
                    Content = new Lazy<UserControl>(WatchPartyInviter.GetSingeltonObjectWatchPartyInviter)
                }
            };

            FbInviterTabs.ItemsSource = items;
        }

        private static FbInviterTab CurrentFbInviterTab { get; set; }

        public static FbInviterTab GetSingeltonObjectFbInviterTab()
        {
            return CurrentFbInviterTab ?? (CurrentFbInviterTab = new FbInviterTab());
        }

        public void SetIndex(int tabIndex)
        {
            FbInviterTabs.SelectedIndex = tabIndex;
        }
    }
}