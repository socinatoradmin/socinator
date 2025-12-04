using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using DominatorHouseCore;
using DominatorHouseCore.Command;
using DominatorHouseCore.Enums.FdQuery;
using DominatorHouseCore.Models;
using DominatorHouseCore.Models.FacebookModels;
using DominatorHouseCore.Utility;
using MahApps.Metro.Controls.Dialogs;

namespace DominatorUIUtility.CustomControl.FacebookCustomControl
{
    /// <summary>
    ///     Interaction logic for LikerCommentorConfiguration.xaml
    /// </summary>
    public partial class LikerCommentorConfiguration
    {
        // Using a DependencyProperty as the backing store for PostFilter.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LikerCommentorConfigProperty =
            DependencyProperty.Register("LikerCommentorConfig", typeof(LikerCommentorConfigModel),
                typeof(LikerCommentorConfiguration), new FrameworkPropertyMetadata(OnAvailableItemsChanged)
                {
                    BindsTwoWayByDefault = true
                });

        // Using a DependencyProperty as the backing store for PostFilter.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsLikeTypeFilterRequiredProperty =
            DependencyProperty.Register("IsLikeTypeFilterRequired", typeof(bool), typeof(LikerCommentorConfiguration),
                new FrameworkPropertyMetadata(OnAvailableItemsChanged)
                {
                    BindsTwoWayByDefault = true
                });

        // Using a DependencyProperty as the backing store for PostFilter.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsCommentOptionVisibleProperty =
            DependencyProperty.Register("IsCommentOptionVisible", typeof(bool), typeof(LikerCommentorConfiguration),
                new FrameworkPropertyMetadata(OnAvailableItemsChanged)
                {
                    BindsTwoWayByDefault = true
                });

        // Using a DependencyProperty as the backing store for PostFilter.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsLikeOptionVisibleProperty =
            DependencyProperty.Register("IsLikeOptionVisible", typeof(bool), typeof(LikerCommentorConfiguration),
                new FrameworkPropertyMetadata(OnAvailableItemsChanged)
                {
                    BindsTwoWayByDefault = true
                });

        public LikerCommentorConfiguration()
        {
            InitializeComponent();
            MainGrid.DataContext = this;
            AddCommentsBindCommand = new BaseCommand<object>(sender => true, AddCommentsBindExecute);
        }

        public ICommand AddCommentsBindCommand { get; set; }

        public LikerCommentorConfigModel LikerCommentorConfig
        {
            get => (LikerCommentorConfigModel)GetValue(LikerCommentorConfigProperty);
            set => SetValue(LikerCommentorConfigProperty, value);
        }

        public bool IsLikeTypeFilterRequired
        {
            get => (bool)GetValue(IsLikeTypeFilterRequiredProperty);
            set => SetValue(IsLikeTypeFilterRequiredProperty, value);
        }


        public bool IsCommentOptionVisible
        {
            get => (bool)GetValue(IsCommentOptionVisibleProperty);
            set => SetValue(IsCommentOptionVisibleProperty, value);
        }


        public bool IsLikeOptionVisible
        {
            get => (bool)GetValue(IsLikeOptionVisibleProperty);
            set => SetValue(IsLikeOptionVisibleProperty, value);
        }


        private void AddCommentsBindExecute(object sender)
        {
            if (!(sender is CommentControl messageData)) return;

            messageData.Comments.SelectedQuery =
                new ObservableCollection<QueryContent>(messageData.Comments.LstQueries.Where(x => x.IsContentSelected));

            if (messageData.Comments.SelectedQuery.Count == 0)
            {
                Dialog.ShowDialog(Application.Current.MainWindow, "Warning",
                    "Please select atleast one query!!");
                return;
            }

            if (string.IsNullOrEmpty(messageData.Comments.CommentText))
            {
                Dialog.ShowDialog(Application.Current.MainWindow, "Warning",
                    "Please enter comment text!!");
                return;
            }

            messageData.Comments.SelectedQuery.Remove(messageData.Comments.SelectedQuery.FirstOrDefault(x => x.Content.QueryValue == "All"));

            if (LikerCommentorConfig.IsNewLineSepratedCommentsChecked)
            {
                var commentTextList = GetListSplittedWithNextLine(messageData.Comments.CommentText);


                foreach (var commentText in commentTextList)
                {
                    var commentData = new ManageCommentModel
                    {
                        LstQueries = LikerCommentorConfig.ManageCommentModel.LstQueries,
                        SelectedQuery = messageData.Comments.SelectedQuery,
                        CommentText = commentText

                    };

                    LikerCommentorConfig.LstManageCommentModel.Add(commentData);
                }

            }
            else
                LikerCommentorConfig.LstManageCommentModel.Add(messageData.Comments);

            messageData.Comments = new ManageCommentModel
            {
                LstQueries = LikerCommentorConfig.ManageCommentModel.LstQueries
            };

            // ReSharper disable once ReturnValueOfPureMethodIsNotUsed
            messageData.Comments.LstQueries.Select(query =>
            {
                query.IsContentSelected = false;
                return query;
            }).ToList();

            LikerCommentorConfig.ManageCommentModel = messageData.Comments;

            messageData.ComboBoxQueries.ItemsSource = LikerCommentorConfig.ManageCommentModel.LstQueries;
        }


        public static void OnAvailableItemsChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
        }

        private void Comment_AddCommentToListChanged(object sender, RoutedEventArgs e)
        {
        }

        private void Liketype_checked(object sender, RoutedEventArgs e)
        {
            var currentCheckBoxItem = sender as RadioButton;
            AddRemoveCheckItems(currentCheckBoxItem);
        }

        private void Liketype_Unchecked(object sender, RoutedEventArgs e)
        {
            var currentCheckBoxItem = sender as RadioButton;
            AddRemoveCheckItems(currentCheckBoxItem);
        }


        public void AddRemoveCheckItems(RadioButton content)
        {
            try
            {
                var objReactionType = (ReactionType)Enum.Parse(typeof(ReactionType), content.Name);

                if (content.IsChecked == true)
                    LikerCommentorConfig.ListReactionType.Add(objReactionType);
                else
                    LikerCommentorConfig.ListReactionType.Remove(objReactionType);

                LikerCommentorConfig.ListReactionType = LikerCommentorConfig.ListReactionType.Distinct().ToList();
            }
            catch (Exception exc)
            {
                exc.DebugLog();
            }
        }

        private void CommentorConfigControl_Loaded(object sender, RoutedEventArgs e)
        {
        }

        public List<string> GetListSplittedWithNextLine(string text)
        {
            try
            {
                if (!string.IsNullOrEmpty(text))
                    return text.Split('\n').Where(x => !string.IsNullOrEmpty(x.Trim())).Select(y => y.Trim()).Distinct()
                        .ToList();
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            return new List<string>();
        }
    }
}