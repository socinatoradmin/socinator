using DominatorHouseCore.Models;
using FaceDominatorUI.FDViews.FbEvents;
using System;
using System.Collections.Generic;
using System.Windows.Controls;

namespace FaceDominatorUI.FDViews.TabManager
{
    /// <summary>
    ///     Interaction logic for FbCreaterTab.xaml
    /// </summary>
    public partial class FbCreaterTab
    {
        public FbCreaterTab()
        {
            InitializeComponent();

            FbEventsTabs.ItemsSource = new List<TabItemTemplates>
            {
                new TabItemTemplates
                {
                    Title = FindResource("LangKeyEventCreater").ToString(),
                    Content = new Lazy<UserControl>(EventCreator.GetSingeltonObjectEventCreator)
                }
            };
        }

        private static FbCreaterTab CurrentFbCreaterTab { get; set; }

        public static FbCreaterTab GetSingeltonObjectCreaterTab()
        {
            return CurrentFbCreaterTab ?? (CurrentFbCreaterTab = new FbCreaterTab());
        }
    }
}