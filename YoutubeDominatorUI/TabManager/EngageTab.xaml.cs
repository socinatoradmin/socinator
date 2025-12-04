using DominatorHouseCore.Models;
using System;
using System.Collections.Generic;
using System.Windows.Controls;
using YoutubeDominatorUI.YDViews.Engage;

namespace YoutubeDominatorUI.TabManager
{
    public partial class EngageTab
    {
        private static EngageTab _objEngageTab;

        private EngageTab()
        {
            InitializeComponent();
            _objEngageTab = this;

            var tabItems = new List<TabItemTemplates>
            {
                new TabItemTemplates
                {
                    Title = FindResource("LangKeyLike").ToString(),
                    Content = new Lazy<UserControl>(Like.GetSingeltonObjectLike)
                },
                new TabItemTemplates
                {
                    Title = FindResource("LangKeyDislike").ToString(),
                    Content = new Lazy<UserControl>(Dislike.GetSingeltonObjectDislike)
                },
                new TabItemTemplates
                {
                    Title = FindResource("LangKeyComment").ToString(),
                    Content = new Lazy<UserControl>(Comment.GetSingeltonObjectComment)
                },
                new TabItemTemplates
                {
                    Title = FindResource("LangKeyLikeComments").ToString(),
                    Content = new Lazy<UserControl>(LikeComment.GetSingeltonObjectLikeComment)
                },
                new TabItemTemplates
                {
                    Title = FindResource("LangKeyReportVideo").ToString(),
                    Content = new Lazy<UserControl>(ReportVideo.GetSingeltonObjectReportVideo)
                }
                //,
                //  new TabItemTemplates
                //{
                //    Title=FindResource("LangKeyReplyToComment").ToString(),
                //    Content = new Lazy<UserControl>(ReplyToComment.GetSingeltonObjectReplyToComment)
                //}
                //,
                //  new TabItemTemplates
                //{
                //    Title=FindResource("LangKeyLikeToComment").ToString(),
                //    Content = new Lazy<UserControl>(LikeToComment.GetSingeltonObjectLikeToComment)
                //},
                // new TabItemTemplates
                //{
                //    Title=FindResource("LangKeyShare").ToString(),
                //    Content = new Lazy<UserControl>(Share.GetSingeltonObjectShare)
                //}
            };
            EngageTabs.ItemsSource = tabItems;
        }

        public static EngageTab GetSingeltonObject_EngageTab()
        {
            return _objEngageTab ?? (_objEngageTab = new EngageTab());
        }

        public void SetIndex(int index)
        {
            EngageTabs.SelectedIndex = index;
        }
    }
}