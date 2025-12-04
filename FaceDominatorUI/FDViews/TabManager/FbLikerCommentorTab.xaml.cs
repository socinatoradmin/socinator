using DominatorHouseCore.Models;
using FaceDominatorUI.FDViews.FbLikerCommentor;
using System;
using System.Collections.Generic;
using System.Windows.Controls;

namespace FaceDominatorUI.FDViews.TabManager
{
    /// <summary>
    ///     Interaction logic for FbLikerCommentorTab.xaml
    /// </summary>
    public partial class FbLikerCommentorTab
    {
        public FbLikerCommentorTab()
        {
            InitializeComponent();
            var items = new List<TabItemTemplates>
            {
                new TabItemTemplates
                {
                    Title = FindResource("LangKeyFanPageLiker").ToString(),
                    Content = new Lazy<UserControl>(FanapgeLiker.GetSingeltonObjectFanapgeLiker)
                },
                //new TabItemTemplates
                //{
                //    Title =FindResource("LangKeyPostLikeAndComment").ToString(),
                //    Content = new Lazy<UserControl>(PostLikerCommentor.GetSingeltonObjectPostLikerCommentor)
                //},

                new TabItemTemplates
                {
                    Title = FindResource("LangKeyPostLike").ToString(),
                    Content = new Lazy<UserControl>(PostLiker.GetSingeltonObjectPostLiker)
                },
                new TabItemTemplates
                {
                    Title = FindResource("LangKeyPostComment").ToString(),
                    Content = new Lazy<UserControl>(PostCommentor.GetSingeltonObjectPostCommentor)
                },
                new TabItemTemplates
                {
                    Title = FindResource("LangKeyCommentLiker").ToString(),
                    Content = new Lazy<UserControl>(CommentLiker.GetSingeltonObjectCommentLiker)
                },
                new TabItemTemplates
                {
                    Title = FindResource("LangKeyReplyToComments").ToString(),
                    Content = new Lazy<UserControl>(ReplyToComment.GetSingeltonObjectReplyToComment)
                }
                //new TabItemTemplates
                //{
                //Title =FindResource("LangKeyWebPostscommenter").ToString(),
                //Content = new Lazy<UserControl>(WebPostCommentLiker.GetSingeltonObjectWebPostCommentLiker)

                //new TabItemTemplates
                //{
                //    Title =FindResource("LangKeyCommentLiker").ToString(),
                //    Content = new Lazy<UserControl>(ReplyToComments.GetSingeltonObjectCommentLiker)
                //}
            };

            FbLikerCommentorTabs.ItemsSource = items;
        }


        private static FbLikerCommentorTab CurrentFbLikerCommentorTab { get; set; }

        public static FbLikerCommentorTab GetSingeltonObjectFbLikerCommentorTab()
        {
            return CurrentFbLikerCommentorTab ?? (CurrentFbLikerCommentorTab = new FbLikerCommentorTab());
        }

        public void SetIndex(int tabIndex)
        {
            FbLikerCommentorTabs.SelectedIndex = tabIndex;
        }
    }
}