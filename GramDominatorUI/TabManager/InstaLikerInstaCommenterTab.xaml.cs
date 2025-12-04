using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using DominatorHouseCore.Models;
using GramDominatorUI.GDViews.InstaLikeComment;

namespace GramDominatorUI.TabManager
{
    /// <summary>
    ///     Interaction logic for InstaLikerInstaCommenterTab.xaml
    /// </summary>
    public partial class InstaLikerInstaCommenterTab : UserControl
    {
        private InstaLikerInstaCommenterTab()
        {
            InitializeComponent();
            var tabItems = new List<TabItemTemplates>
            {
                new TabItemTemplates
                {
                    Title = Application.Current.FindResource("LangKeyLike") != null
                        ? Application.Current.FindResource("LangKeyLike").ToString()
                        : "Like",
                    Content = new Lazy<UserControl>(Like.GetSingeltonObjectLike)
                },
                new TabItemTemplates
                {
                    Title = Application.Current.FindResource("LangKeyComment") != null
                        ? Application.Current.FindResource("LangKeyComment").ToString()
                        : "Comment",
                    Content = new Lazy<UserControl>(Comment.GetSingeltonObjectComment)
                },
                new TabItemTemplates
                {
                    Title = Application.Current.FindResource("LangKeyLikeComments") != null
                        ? Application.Current.FindResource("LangKeyLikeComments").ToString()
                        : "Like Comment",
                    Content = new Lazy<UserControl>(LikeComment.GetSingeltonObjectLikeComment)
                },
                new TabItemTemplates
                {
                    Title = Application.Current.FindResource("LangKeyMediaUnliker") != null
                        ? Application.Current.FindResource("LangKeyMediaUnliker").ToString()
                        : "Media Unliker",
                    Content = new Lazy<UserControl>(MediaUnliker.GetSingeltonObjectMediaUnliker)
                },
                new TabItemTemplates
                {
                    Title = Application.Current.FindResource("LangKeyReplyComment") != null
                        ? Application.Current.FindResource("LangKeyReplyComment").ToString()
                        : "Reply Comment",
                    Content = new Lazy<UserControl>(ReplyComment.SingletonReplyComment)
                }
            };

            InstaLikerInstaCommenterTabs.ItemsSource = tabItems;
        }

        private static InstaLikerInstaCommenterTab CurrentInstaLikerInstaCommenterTab { get; set; }

        /// <summary>
        ///     GetSingeltonObjectInstaLikerInstaCommenterTab is used to get the object of the current user control,
        ///     if object is already created then its won't create a new object, simply it returns already created object,
        ///     otherwise create a new object and then its return.
        /// </summary>
        /// <returns>Current UI class object</returns>
        public static InstaLikerInstaCommenterTab GetSingeltonObjectInstaLikerInstaCommenterTab()
        {
            return CurrentInstaLikerInstaCommenterTab ??
                   (CurrentInstaLikerInstaCommenterTab = new InstaLikerInstaCommenterTab());
        }

        public void SetIndex(int index)
        {
            InstaLikerInstaCommenterTabs.SelectedIndex = index;
        }
    }
}