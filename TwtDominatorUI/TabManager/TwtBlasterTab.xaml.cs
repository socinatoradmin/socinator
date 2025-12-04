using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using DominatorHouseCore;
using DominatorHouseCore.Models;
using TwtDominatorUI.TDViews.TwtBlaster;

namespace TwtDominatorUI.TabManager
{
    /// <summary>
    ///     Interaction logic for TwtBlasterTab.xaml
    /// </summary>
    public partial class TwtBlasterTab : UserControl
    {
        private static TwtBlasterTab objTwtBlasterTab;

        private TwtBlasterTab()
        {
            InitializeComponent();
            string TitleReposter = null;
            string TitleRetweet = null;
            string TitleDelete = null;
            string TitleWelcomeTweet = null;
            string TitleTweetTo = null;

            try
            {
                TitleReposter = Application.Current.FindResource("LangKeyReposter﻿")?.ToString();
                TitleRetweet = Application.Current.FindResource("LangKeyRetweet﻿")?.ToString();
                TitleWelcomeTweet = Application.Current.FindResource("LangKeyWelcomeTweet﻿")?.ToString();
                TitleTweetTo = Application.Current.FindResource("LangKeyTweetTo﻿")?.ToString();
                TitleDelete = Application.Current.FindResource("LangKeyDelete﻿")?.ToString();
            }
            catch (Exception ex)
            {
                ex.ErrorLog();
            }

            var tabItems = new List<TabItemTemplates>
            {
                new TabItemTemplates
                {
                    Title = TitleReposter == null ? "Reposter" : TitleReposter,

                    Content = new Lazy<UserControl>(Reposter.GetSingletonObjectReposter)
                },
                new TabItemTemplates
                {
                    Title = TitleRetweet == null ? "Retweet" : TitleRetweet,

                    Content = new Lazy<UserControl>(Retweet.GetSingletonObjectRetweet)
                },
                new TabItemTemplates
                {
                    Title = TitleReposter == null ? "WelcomeTweet" : TitleWelcomeTweet,

                    Content = new Lazy<UserControl>(WelcomeTweet.GetSingletonObjectobjWelcomeTweet)
                },
                new TabItemTemplates
                {
                    Title = TitleReposter == null ? "TweetTo" : TitleTweetTo,

                    Content = new Lazy<UserControl>(TweetTo.GetSingletonObjectTweetTo)
                }
            };

            try
            {
                tabItems.Add(new TabItemTemplates
                {
                    Title = TitleDelete == null ? "Delete" : TitleDelete,

                    Content = new Lazy<UserControl>(Delete.GetSingletonObjectDelete)
                });
            }
            catch (Exception ex)
            {
                ex.ErrorLog();
            }

            TwtBlastersTab.ItemsSource = tabItems;
        }


        public static TwtBlasterTab GetSingeltonObjectTwtBlasterTab()
        {
            return objTwtBlasterTab ?? (objTwtBlasterTab = new TwtBlasterTab());
        }

        public void SetIndex(int index)
        {
            TwtBlastersTab.SelectedIndex = index;
        }
    }
}