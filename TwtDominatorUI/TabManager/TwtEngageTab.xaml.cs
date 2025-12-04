using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using DominatorHouseCore.Models;
using TwtDominatorUI.TDViews.TwtEngage;

namespace TwtDominatorUI.TabManager
{
    /// <summary>
    ///     Interaction logic for TwtEngage.xaml
    /// </summary>
    public partial class TwtEngageTab : UserControl
    {
        private static TwtEngageTab objTwtEngageTab;

        public TwtEngageTab()
        {
            InitializeComponent();
            var tabItems = new List<TabItemTemplates>
            {
                new TabItemTemplates
                {
                    Title = Application.Current.FindResource("LangKeyLike").ToString(),
                    Content = new Lazy<UserControl>(TwtLiker.GetSingletonObjectTwtLiker)
                },
                new TabItemTemplates
                {
                    Title = Application.Current.FindResource("LangKeyComment").ToString(),
                    Content = new Lazy<UserControl>(Comment.GetSingletonObjectComment)
                },
                new TabItemTemplates
                {
                    Title = Application.Current.FindResource("LangKeyUnlike").ToString(),
                    Content = new Lazy<UserControl>(TwtUnLiker.GetSingletonObjectTwtUnLiker)
                }
            };

            twtEngageTab.ItemsSource = tabItems;
        }


        public static TwtEngageTab GetSingeltonObjectTwtEngageTab()
        {
            return objTwtEngageTab ?? (objTwtEngageTab = new TwtEngageTab());
        }

        public void SetIndex(int index)
        {
            twtEngageTab.SelectedIndex = index;
        }
    }
}