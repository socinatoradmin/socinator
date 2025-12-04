using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using DominatorHouseCore;
using DominatorHouseCore.Command;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;

namespace DominatorUIUtility.CustomControl
{
    /// <summary>
    ///     Interaction logic for CommentControl.xaml
    /// </summary>
    public partial class CommentControl
    {
        private static readonly RoutedEvent AddCommentToListEvent =
            EventManager.RegisterRoutedEvent("AddCommentToListChanged", RoutingStrategy.Bubble,
                typeof(RoutedEventHandler),
                typeof(CommentControl));
        private static readonly DependencyProperty CommentText =
            DependencyProperty.Register("CommentText", typeof(string), typeof(CommentControl), new PropertyMetadata("LangKeyCommentText".FromResourceDictionary()));
        private static readonly DependencyProperty AddCommentText =
            DependencyProperty.Register("AddCommentToListText", typeof(string), typeof(CommentControl), new PropertyMetadata("LangKeyAddCommentToList".FromResourceDictionary()));
        // Using a DependencyProperty as the backing store for ManageComments.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ManageCommentsProperty =
            DependencyProperty.Register("Comments", typeof(ManageCommentModel), typeof(CommentControl),
                new PropertyMetadata());

        // Using a DependencyProperty as the backing store for LstManageCommentModel.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LstManageCommentModelProperty =
            DependencyProperty.Register("LstManageCommentModel", typeof(ObservableCollection<ManageCommentModel>),
                typeof(CommentControl), new PropertyMetadata(new ObservableCollection<ManageCommentModel>()));

        // Using a DependencyProperty as the backing store for AddCommentsCommand.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty AddCommentsCommandProperty =
            DependencyProperty.Register("AddCommentsCommand", typeof(ICommand), typeof(CommentControl));

        // Using a DependencyProperty as the backing store for CommandParameter.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CommandParameterProperty =
            DependencyProperty.Register("CommandParameter", typeof(object), typeof(CommentControl),
                new PropertyMetadata());

        private bool _isUncheckfromList;

        public CommentControl()
        {
            InitializeComponent();
            MainGrid.DataContext = this;
            AddCommentsCommand = new BaseCommand<object>(sender => true, AddCommentsExecute);
        }

        public string CommentTextValue
        {
            get => (string)GetValue(CommentText);
            set => SetValue(CommentText, value);
        }
        public string AddCommentTextValue
        {
            get => (string)GetValue(AddCommentText);
            set => SetValue(AddCommentText, value);
        }
        public ManageCommentModel Comments
        {
            get => (ManageCommentModel) GetValue(ManageCommentsProperty);
            set => SetValue(ManageCommentsProperty, value);
        }

        public ObservableCollection<ManageCommentModel> LstManageCommentModel
        {
            get => (ObservableCollection<ManageCommentModel>) GetValue(LstManageCommentModelProperty);
            set => SetValue(LstManageCommentModelProperty, value);
        }

        public bool Isupdated { get; set; }

        public ICommand AddCommentsCommand
        {
            get => (ICommand) GetValue(AddCommentsCommandProperty);
            set => SetValue(AddCommentsCommandProperty, value);
        }

        public object CommandParameter
        {
            get => GetValue(CommandParameterProperty);
            set => SetValue(CommandParameterProperty, value);
        }

        public event RoutedEventHandler AddCommentToListChanged
        {
            add => AddHandler(AddCommentToListEvent, value);
            remove => RemoveHandler(AddCommentToListEvent, value);
        }

        private void AddCommentToListEventHandler()
        {
            var routedEventArgs = new RoutedEventArgs(AddCommentToListEvent);
            RaiseEvent(routedEventArgs);
        }

        private void chkQuery_Checked(object sender, RoutedEventArgs e)
        {
            CheckUncheckAll(sender, true);
        }

        private void chkQuery_Unchecked(object sender, RoutedEventArgs e)
        {
            CheckUncheckAll(sender, false);
        }

        private void CheckUncheckAll(object sender, bool IsChecked)
        {
            try
            {
                var currentQuery = ((QueryContent) (sender as CheckBox)?.DataContext).Content.QueryValue;
                if (!Comments.LstQueries.Skip(1).All(x => x.IsContentSelected))
                    if (!IsChecked)
                    {
                        _isUncheckfromList = true;
                        Comments.LstQueries[0].IsContentSelected = false;
                    }

                if (Comments.LstQueries.Skip(1).All(x => x.IsContentSelected))
                {
                    _isUncheckfromList = false;
                    Comments.LstQueries[0].IsContentSelected = IsChecked;
                }

                if (_isUncheckfromList)
                {
                    _isUncheckfromList = false;
                    return;
                }

                if (currentQuery == "All" || currentQuery == "Default")
                {
                    _isUncheckfromList = false;
                    Comments.LstQueries.ToList().ForEach(query =>
                    {
                        query.IsContentSelected = IsChecked;
                    });
                }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        private void AddCheckedQueryToList()
        {
            Comments.SelectedQuery.Clear();
            Comments.LstQueries.ToList().ForEach(query =>
            {
                if (query.IsContentSelected)
                    Comments.SelectedQuery.Add(query);
            });
        }

        private void AddCommentsExecute(object sender)
        {
            if (string.IsNullOrEmpty(Comments.CommentText))
            {
                Dialog.ShowDialog("Warning", "Please type some comment !!");
                return;
            }

            if (!Comments.LstQueries.Any(x => x.IsContentSelected))
            {
                Dialog.ShowDialog("Warning", "Please select atleast one query.");
                return;
            }

            AddCheckedQueryToList();
            if (btnAddCommentToList.Content.ToString() == "Update Comment")
            {
                LstManageCommentModel.ForEach(x =>
                {
                    if (x.CommentId == Comments.CommentId)
                    {
                        x.CommentText = Comments.CommentText;
                        x.FilterText = Comments.FilterText;
                        x.LstQueries = Comments.LstQueries;
                        x.SelectedQuery = Comments.SelectedQuery;
                    }
                });
                Comments.SelectedQuery.Remove(
                    Comments.SelectedQuery.FirstOrDefault(x => x.Content.QueryValue == "All"));
                Comments.LstQueries.ForEach(x => { x.IsContentSelected = false; });
                Isupdated = true;
                Dialog.CloseDialog(this);
            }
            else
            {
                AddCommentToListEventHandler();
            }
        }
    }
}