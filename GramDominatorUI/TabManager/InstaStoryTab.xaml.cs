using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using DominatorHouseCore.Models;
using GramDominatorUI.GDViews.StoryViews;

namespace GramDominatorUI.TabManager
{
    /// <summary>
    ///     Interaction logic for InstaStoryTab.xaml
    /// </summary>
    public partial class InstaStoryTab : UserControl
    {
        private static InstaStoryTab objStoryViewerTab;

        public InstaStoryTab()
        {
            InitializeComponent();
            var tabItems = new List<TabItemTemplates>
            {
                new TabItemTemplates
                {
                    Title = Application.Current.FindResource("LangKeyStoryViewer") != null
                        ? Application.Current.FindResource("LangKeyStoryViewer").ToString()
                        : "Story Viewer",
                    Content = new Lazy<UserControl>(StoryViewers.GetSingletonViewers)
                },
                //new TabItemTemplates
                //{
                //    Title = Application.Current.FindResource("LangKeyInstaStory") != null
                //        ? Application.Current.FindResource("LangKeyInstaStory").ToString()
                //        : "Insta Story",
                //    Content = new Lazy<UserControl>(Instagram_Story.GetSingletonViewers)
                //},
                new TabItemTemplates
                {
                    Title = Application.Current.FindResource("LangKeyAddInstaStory") != null ?
                    Application.Current.FindResource("LangKeyAddInstaStory").ToString():
                    "Add Story",
                    Content = new Lazy<UserControl>(()=>AddInstaStory.Instance)
                }
            };
            StoryViewerTabs.ItemsSource = tabItems;
        }

        public static InstaStoryTab objSingaltonStoryViewerTab()
        {
            return objStoryViewerTab ?? (objStoryViewerTab = new InstaStoryTab());
        }
        public void SetIndex(int index)
        {
            StoryViewerTabs.SelectedIndex = index;
        }
    }
}