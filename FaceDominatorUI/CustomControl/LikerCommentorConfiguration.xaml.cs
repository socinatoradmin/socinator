using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using DominatorHouseCore.Command;
using DominatorHouseCore.Models;
using DominatorUIUtility.CustomControl;
using FaceDominatorCore.FDEnums;
using FaceDominatorCore.FDModel.FilterModel;
using MahApps.Metro.Controls.Dialogs;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using DominatorHouseCore.Utility;

namespace FaceDominatorUI.CustomControl
{
    /// <summary>
    /// Interaction logic for LikerCommentorConfiguration.xaml
    /// </summary>
    public partial class LikerCommentorConfiguration
    {
        public LikerCommentorConfiguration()
        {
            InitializeComponent();
            MainGrid.DataContext = this;
            AddCommentsBindCommand = new BaseCommand<object>(sender => true, AddCommentsBindExecute);
        }

        private void AddCommentsBindExecute(object sender)
        {
            //var CommentData = sender as CommentControl;           
            //LikerCommentorConfig.LstManageCommentModel.Add(CommentData.Comments);
            //CommentData.Comments = new ManageCommentModel();
            //LikerCommentorConfig.ManageCommentModel = CommentData.Comments;
            //CommentData.ComboBoxQueries.ItemsSource = LikerCommentorConfig.ManageCommentModel.LstQueries;

            var messageData = sender as CommentControl;

            if (messageData == null) return;

            messageData.Comments.SelectedQuery = new ObservableCollection<QueryContent>(messageData.Comments.LstQueries.Where(x => x.IsContentSelected));

            if (messageData.Comments.SelectedQuery.Count == 0)
            {
                DialogCoordinator.Instance.ShowModalMessageExternal(Application.Current.MainWindow, "LangKeyWarning".FromResourceDictionary(),
                     "LangKeyErrorAddAtLeastOneQuery".FromResourceDictionary());
                return;
            }

            if (string.IsNullOrEmpty(messageData.Comments.CommentText))
            {
                DialogCoordinator.Instance.ShowModalMessageExternal(Application.Current.MainWindow, "LangKeyWarning".FromResourceDictionary(),
                     "LangKeyAddAtLeastOneComment".FromResourceDictionary());
                return;
            }

            messageData.Comments.SelectedQuery.Remove(messageData.Comments.SelectedQuery.FirstOrDefault(x => x.Content.QueryValue == "All"));

            if (LikerCommentorConfig.IsSplitOnNextLine)
            {
                string[] messagesSplit = Regex.Split(messageData.Comments.CommentText, "\r\n");
                foreach (var message in messagesSplit)
                {
                    LikerCommentorConfig.LstManageCommentModel.Add(
                        new ManageCommentModel()
                        {
                            CommentText = message,
                            LstQueries = messageData.Comments.LstQueries,
                            FilterText = messageData.Comments.FilterText,
                            SelectedQuery = messageData.Comments.SelectedQuery,
                        });
                }
            }
            else
                LikerCommentorConfig.LstManageCommentModel.Add(messageData.Comments);

            messageData.Comments = new ManageCommentModel
            {
                LstQueries = LikerCommentorConfig.ManageCommentModel.LstQueries
            };

            // ReSharper disable once ReturnValueOfPureMethodIsNotUsed
            messageData.Comments.LstQueries.Select(query => { query.IsContentSelected = false; return query; }).ToList();

            LikerCommentorConfig.ManageCommentModel = messageData.Comments;

            messageData.ComboBoxQueries.ItemsSource = LikerCommentorConfig.ManageCommentModel.LstQueries;
        }

        public ICommand AddCommentsBindCommand { get; set; }
        public LikerCommentorConfigModel LikerCommentorConfig
        {
            get { return (LikerCommentorConfigModel)GetValue(LikerCommentorConfigProperty); }
            set { SetValue(LikerCommentorConfigProperty, value); }
        }

        // Using a DependencyProperty as the backing store for PostFilter.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LikerCommentorConfigProperty =
            DependencyProperty.Register("LikerCommentorConfig", typeof(LikerCommentorConfigModel), typeof(LikerCommentorConfiguration), new FrameworkPropertyMetadata(OnAvailableItemsChanged)
            {
                BindsTwoWayByDefault = true
            });

        public bool IsLikeTypeFilterRequired
        {
            get { return (bool)GetValue(IsLikeTypeFilterRequiredProperty); }
            set { SetValue(IsLikeTypeFilterRequiredProperty, value); }
        }

        // Using a DependencyProperty as the backing store for PostFilter.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsLikeTypeFilterRequiredProperty =
            DependencyProperty.Register("IsLikeTypeFilterRequired", typeof(bool), typeof(LikerCommentorConfiguration), new FrameworkPropertyMetadata(OnAvailableItemsChanged)
            {
                BindsTwoWayByDefault = true
            });


        public bool IsCommentOptionVisible
        {
            get { return (bool)GetValue(IsCommentOptionVisibleProperty); }
            set { SetValue(IsCommentOptionVisibleProperty, value); }
        }

        // Using a DependencyProperty as the backing store for PostFilter.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsCommentOptionVisibleProperty =
            DependencyProperty.Register("IsCommentOptionVisible", typeof(bool), typeof(LikerCommentorConfiguration), new FrameworkPropertyMetadata(OnAvailableItemsChanged)
            {
                BindsTwoWayByDefault = true
            });


        public bool IsLikeOptionVisible
        {
            get { return (bool)GetValue(IsLikeOptionVisibleProperty); }
            set { SetValue(IsLikeOptionVisibleProperty, value); }
        }

        // Using a DependencyProperty as the backing store for PostFilter.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsLikeOptionVisibleProperty =
            DependencyProperty.Register("IsLikeOptionVisible", typeof(bool), typeof(LikerCommentorConfiguration), new FrameworkPropertyMetadata(OnAvailableItemsChanged)
            {
                BindsTwoWayByDefault = true
            });


        public static void OnAvailableItemsChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
        }

        private void Comment_AddCommentToListChanged(object sender, RoutedEventArgs e)
        {


        }

        private void Liketype_checked(object sender, RoutedEventArgs e)
        {
            var currentCheckBoxItem = sender as CheckBox;
            if (currentCheckBoxItem != null && currentCheckBoxItem.IsChecked == true)
            {
                string content = currentCheckBoxItem.Name;
                ReactionType objReactionType = (ReactionType)Enum.Parse(typeof(ReactionType), content);

                if (!LikerCommentorConfig.ListReactionType.Contains(objReactionType))
                    LikerCommentorConfig.ListReactionType.Add(objReactionType);
                
            }
        }

        private void Liketype_Unchecked(object sender, RoutedEventArgs e)
        {
            RemoveLikerOption();
        }

        public void RemoveLikerOption()
        {
            try
            {
                List<ReactionType> listReactionRemove = new List<ReactionType>();

                for (int i = 0; i < LikerCommentorConfig.ListReactionType.Count; i++)
                {
                    ReactionType objReactionType = (ReactionType)Enum.Parse(typeof(ReactionType), LikerCommentorConfig.ListReactionType[i].ToString());

                    switch (objReactionType)
                    {
                        case ReactionType.Like:
                            if (!LikerCommentorConfig.IsLikeFilterChkd)
                                listReactionRemove.Add(objReactionType);
                            break;
                        case ReactionType.Haha:
                            if (!LikerCommentorConfig.IsHahaFilterChkd)
                                listReactionRemove.Add(objReactionType);
                            break;
                        case ReactionType.Angry:
                            if (!LikerCommentorConfig.IsAngryFilterChkd)
                                listReactionRemove.Add(objReactionType);
                            break;
                        case ReactionType.Love:
                            if (!LikerCommentorConfig.IsLoveFilterChkd)
                                listReactionRemove.Add(objReactionType);
                            break;
                        case ReactionType.Sad:
                            if (!LikerCommentorConfig.IsSadFilterChkd)
                                listReactionRemove.Add(objReactionType);
                            break;
                        case ReactionType.Wow:
                            if (!LikerCommentorConfig.IsWowFilterChkd)
                                listReactionRemove.Add(objReactionType);
                            break;
                    }
                }

                listReactionRemove.Distinct();


                LikerCommentorConfig.ListReactionType.RemoveAll(x => listReactionRemove.Any(y => y == x));

                LikerCommentorConfig.ListReactionType.Distinct();
            }
            catch (Exception ex)
            {
                
            }
        }

        private void CommentorConfigControl_Loaded(object sender, RoutedEventArgs e)
        {

        }
    }
}
