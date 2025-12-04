using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using DominatorHouseCore;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;

namespace DominatorUIUtility.CustomControl
{
    /// <summary>
    ///     Interaction logic for ManageComment.xaml
    /// </summary>
    public partial class ManageComment
    {
        // Using a DependencyProperty as the backing store for LstManageCommentModel.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LstManageCommentModelProperty =
            DependencyProperty.Register("LstManageCommentModel", typeof(ObservableCollection<ManageCommentModel>),
                typeof(ManageComment), new PropertyMetadata());
        public static readonly DependencyProperty CommentListText =
            DependencyProperty.Register("CommentListText", typeof(string),
                typeof(ManageComment), new PropertyMetadata("LangKeyCommentLists".FromResourceDictionary()));
        private static readonly DependencyProperty ListHeaderCommentText =
            DependencyProperty.Register("HeaderText", typeof(string), typeof(ManageComment), new PropertyMetadata("LangKeyComment".FromResourceDictionary()));
        public ManageComment()
        {
            InitializeComponent();
            MainGrid.DataContext = this;
        }
        public string CommentListTextValue
        {
            get => (string)GetValue(CommentListText);
            set => SetValue(CommentListText, value);
        }
        public string ListHeaderCommentTextValue
        {
            get => (string)GetValue(ListHeaderCommentText);
            set => SetValue(ListHeaderCommentText, value);
        }

        public ObservableCollection<ManageCommentModel> LstManageCommentModel
        {
            get => (ObservableCollection<ManageCommentModel>) GetValue(LstManageCommentModelProperty);
            set => SetValue(LstManageCommentModelProperty, value);
        }

        private void BtnAction_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var contextMenu = ((Button) sender).ContextMenu;
                if (contextMenu != null)
                {
                    contextMenu.DataContext = ((Button) sender).DataContext;
                    contextMenu.IsOpen = true;
                }
            }
            catch (Exception exc)
            {
                exc.DebugLog();
            }
        }

        private void EditComment_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!(((FrameworkElement) sender).DataContext is ManageCommentModel currentItem))
                    return;

                var editComment = new CommentControl
                {
                    btnAddCommentToList = {Content = "Update Comment"},
                    Comments = new ManageCommentModel
                    {
                        CommentText = currentItem.CommentText,
                        LstQueries = new ObservableCollection<QueryContent>(currentItem.LstQueries),
                        CommentId = currentItem.CommentId,
                        SelectedQuery = new ObservableCollection<QueryContent>(currentItem.SelectedQuery),
                        FilterText = currentItem.FilterText
                    },
                    LstManageCommentModel = LstManageCommentModel
                };

                editComment.Comments.LstQueries.ToList().ForEach(x =>
                {
                    x.IsContentSelected = false;
                    if (editComment.Comments.SelectedQuery.Any(y =>
                        y.Content.QueryValue == x.Content.QueryValue && y.Content.QueryType == x.Content.QueryType))
                        x.IsContentSelected = true;
                });
                editComment.MainGrid.Margin = new Thickness(20);
                var dialog = new Dialog();
                var window = dialog.GetMetroWindow(editComment, "Edit comment");
                window.Closed += (s, evnt) =>
                {
                    if (editComment.Isupdated)
                    {
                        var indexToUpdate = LstManageCommentModel.IndexOf(currentItem);
                        LstManageCommentModel[indexToUpdate] = editComment.Comments;
                    }

                    currentItem.LstQueries.ForEach(query => query.IsContentSelected = false);
                };
                window.ShowDialog();
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        private void DeleteSingleComment_Click(object sender, RoutedEventArgs e)
        {
            var currentItem = ((FrameworkElement) sender).DataContext as ManageCommentModel;
            LstManageCommentModel.Remove(currentItem);
        }
    }
}