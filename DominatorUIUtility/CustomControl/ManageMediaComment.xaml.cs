using DominatorHouseCore;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DominatorUIUtility.CustomControl
{
    /// <summary>
    /// Interaction logic for ManageMediaComment.xaml
    /// </summary>
    public partial class ManageMediaComment : UserControl
    {

        // Using a DependencyProperty as the backing store for LstManageCommentModel.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LstManageCommentModelProperty =
            DependencyProperty.Register("LstManageCommentModel", typeof(ObservableCollection<ManageCommentModel>),
                typeof(ManageMediaComment), new PropertyMetadata());

        public ManageMediaComment()
        {
            InitializeComponent();
            MainGrid.DataContext = this;
        }

        public ObservableCollection<ManageCommentModel> LstManageCommentModel
        {
            get => (ObservableCollection<ManageCommentModel>)GetValue(LstManageCommentModelProperty);
            set => SetValue(LstManageCommentModelProperty, value);
        }

        private void BtnAction_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var contextMenu = ((Button)sender).ContextMenu;
                if (contextMenu != null)
                {
                    contextMenu.DataContext = ((Button)sender).DataContext;
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
                if (!(((FrameworkElement)sender).DataContext is ManageCommentModel currentItem))
                    return;

                var editComment = new CommentMediaControl
                {
                    btnAddCommentToList = { Content = "Update Comment" },
                    Comments = new ManageCommentModel
                    {
                        CommentText = currentItem.CommentText,
                        LstQueries = new ObservableCollection<QueryContent>(currentItem.LstQueries),
                        CommentId = currentItem.CommentId,
                        SelectedQuery = new ObservableCollection<QueryContent>(currentItem.SelectedQuery),
                        MediaPath = currentItem.MediaPath,
                        MediaList = currentItem.MediaList,
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
            var currentItem = ((FrameworkElement)sender).DataContext as ManageCommentModel;
            LstManageCommentModel.Remove(currentItem);
        }
    }
}
