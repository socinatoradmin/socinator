using DominatorHouseCore.Models;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using TumblrDominatorUI.TumblrView.Liker;

namespace TumblrDominatorUI.TabManager
{
    /// <summary>
    ///     Interaction logic for EngageTab.xaml
    /// </summary>
    public partial class EngageTab
    {
        private EngageTab()
        {
            InitializeComponent();

            var tabItems = new List<TabItemTemplates>
            {
                new TabItemTemplates
                {
                    Title = Application.Current.FindResource("LangKeyLike") == null
                        ? "Like"
                        : Application.Current.FindResource("LangKeyLike")?.ToString(),
                    Content = new Lazy<UserControl>(Like.GetSingeltonObjectLike)
                },
                new TabItemTemplates
                {
                    Title = Application.Current.FindResource("LangKeyUnLike") == null
                        ? "UnLike"
                        : Application.Current.FindResource("LangKeyUnLike")?.ToString(),
                    Content = new Lazy<UserControl>(UnLike.GetSingeltonObjectUnLike)
                },
                new TabItemTemplates
                {
                    Title = Application.Current.FindResource("LangKeyComments") == null
                        ? "Comment"
                        : Application.Current.FindResource("LangKeyComments")?.ToString(),

                    Content = new Lazy<UserControl>(Comment.GetSingeltonObjectComment)
                }
            };
            EngageTabs.ItemsSource = tabItems;
        }

        private static EngageTab CurrentEngageTab { get; set; }

        /// <summary>
        ///     GetSingeltonObjectInstaLikerInstaCommenterTab is used to get the object of the current user control,
        ///     if object is already created then its won't create a new object, simply it returns already created object,
        ///     otherwise create a new object and then its return.
        /// </summary>
        /// <returns>Current UI class object</returns>
        public static EngageTab GetSingeltonObjectEngageTab()
        {
            return CurrentEngageTab ?? (CurrentEngageTab = new EngageTab());
        }

        public void SetIndex(int index)
        {
            EngageTabs.SelectedIndex = index;
        }
    }
}