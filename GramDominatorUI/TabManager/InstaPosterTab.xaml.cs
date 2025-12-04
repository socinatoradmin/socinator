using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using DominatorHouseCore.Models;
using GramDominatorUI.GDViews.InstaPoster;

namespace GramDominatorUI.TabManager
{
    /// <summary>
    ///     Interaction logic for InstaPosterTab.xaml
    /// </summary>
    public partial class InstaPosterTab : UserControl
    {
        private InstaPosterTab()
        {
            InitializeComponent();

            var tabItems = new List<TabItemTemplates>
            {
                new TabItemTemplates
                {
                    Title = Application.Current.FindResource("LangKeyReposter") != null
                        ? Application.Current.FindResource("LangKeyReposter").ToString()
                        : "Reposter",
                    Content = new Lazy<UserControl>(RePoster.GetSingeltonObjectRePoster)
                },
                new TabItemTemplates
                {
                    Title = Application.Current.FindResource("LangKeyDelete") != null
                        ? Application.Current.FindResource("LangKeyDelete").ToString()
                        : "Delete",
                    Content = new Lazy<UserControl>(DeletePosts.GetSingeltonObjectDeletePosts)
                }
            };

            InstaPosterTabs.ItemsSource = tabItems;
        }

        private static InstaPosterTab objInstaPosterTab { get; set; }

        /// <summary>
        ///     GetSingeltonObjectInstaPosterTab is used to get the object of the current user control,
        ///     if object is already created then it won't create a new object, simply it returns already created object,
        ///     otherwise will return a new created object.
        /// </summary>
        /// <returns>Current UI class object</returns>
        public static InstaPosterTab GetSingeltonObjectInstaPosterTab()
        {
            return objInstaPosterTab ?? (objInstaPosterTab = new InstaPosterTab());
        }

        public void SetIndex(int index)
        {
            InstaPosterTabs.SelectedIndex = index;
        }
    }
}