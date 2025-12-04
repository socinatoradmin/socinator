using DominatorHouseCore.Models;
using System;
using System.Collections.Generic;
using System.Windows.Controls;
using TumblrDominatorUI.TumblrView.Blogs;

namespace TumblrDominatorUI.TabManager
{
    /// <summary>
    ///     Interaction logic for BlogTab.xaml
    /// </summary>
    public partial class BlogTab
    {
        private BlogTab()
        {
            InitializeComponent();
            var tabItems = new List<TabItemTemplates>
            {
                new TabItemTemplates
                {
                    Title = FindResource("LangKeyReblog").ToString(),
                    Content = new Lazy<UserControl>(Reblog.GetSingeltonObjectReblog)
                }
            };

            BlogTabs.ItemsSource = tabItems;
        }

        private static BlogTab CurrentBlogTab { get; set; }

        /// <summary>
        ///     GetSingeltonObjectInstaLikerInstaCommenterTab is used to get the object of the current user control,
        ///     if object is already created then its won't create a new object, simply it returns already created object,
        ///     otherwise create a new object and then its return.
        /// </summary>
        /// <returns>Current UI class object</returns>
        public static BlogTab GetSingeltonObjectBlogTab()
        {
            return CurrentBlogTab ?? (CurrentBlogTab = new BlogTab());
        }

        public void SetIndex(int index)
        {
            BlogTabs.SelectedIndex = index;
        }
    }
}