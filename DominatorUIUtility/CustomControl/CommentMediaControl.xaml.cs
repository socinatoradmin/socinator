using DominatorHouseCore;
using DominatorHouseCore.Command;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using Microsoft.Win32;
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
    /// Interaction logic for CommentMediaControl.xaml
    /// </summary>
    public partial class CommentMediaControl
    {
        private static readonly RoutedEvent AddCommentToListEvent =
            EventManager.RegisterRoutedEvent("AddCommentToListChanged", RoutingStrategy.Bubble,
                typeof(RoutedEventHandler),
                typeof(CommentMediaControl));

        // Using a DependencyProperty as the backing store for ManageComments.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ManageCommentsProperty =
            DependencyProperty.Register("Comments", typeof(ManageCommentModel), typeof(CommentMediaControl),
                new PropertyMetadata());

        // Using a DependencyProperty as the backing store for LstManageCommentModel.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LstManageCommentModelProperty =
            DependencyProperty.Register("LstManageCommentModel", typeof(ObservableCollection<ManageCommentModel>),
                typeof(CommentMediaControl), new PropertyMetadata(new ObservableCollection<ManageCommentModel>()));

        // Using a DependencyProperty as the backing store for AddCommentsCommand.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty AddCommentsCommandProperty =
            DependencyProperty.Register("AddCommentsCommand", typeof(ICommand), typeof(CommentMediaControl));

        // Using a DependencyProperty as the backing store for CommandParameter.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CommandParameterProperty =
            DependencyProperty.Register("CommandParameter", typeof(object), typeof(CommentMediaControl),
                new PropertyMetadata());

        // Using a DependencyProperty as the backing store for IsEnablePreviousPointer.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsEnablePreviousPointerProperty =
            DependencyProperty.Register("IsEnablePreviousPointer", typeof(bool), typeof(CommentMediaControl),
                new PropertyMetadata(false));

        // Using a DependencyProperty as the backing store for IsEnableNextPointer.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsEnableNextPointerProperty =
            DependencyProperty.Register("IsEnableNextPointer", typeof(bool), typeof(CommentMediaControl),
                new PropertyMetadata(false));

        // Using a DependencyProperty as the backing store for CurrentPointer.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CurrentPointerProperty =
            DependencyProperty.Register("CurrentPointer", typeof(int), typeof(CommentMediaControl),
                new PropertyMetadata(0));

        private bool _isUncheckfromList;
        public CommentMediaControl()
        {
            InitializeComponent();
            MainGrid.DataContext = this;
            AddCommentsCommand = new BaseCommand<object>(sender => true, AddCommentsExecute);
        }

        public int CurrentPointer {
            get => (int)GetValue(CurrentPointerProperty);
            set => SetValue(CurrentPointerProperty, value);
        }

        public bool IsEnablePreviousPointer
        {
            get => (bool)GetValue(IsEnablePreviousPointerProperty);
            set => SetValue(IsEnablePreviousPointerProperty, value);
        }

        public bool IsEnableNextPointer
        {
            get => (bool)GetValue(IsEnableNextPointerProperty);
            set => SetValue(IsEnableNextPointerProperty, value);
        }

        public ManageCommentModel Comments
        {
            get => (ManageCommentModel)GetValue(ManageCommentsProperty);
            set => SetValue(ManageCommentsProperty, value);
        }

        public ObservableCollection<ManageCommentModel> LstManageCommentModel
        {
            get => (ObservableCollection<ManageCommentModel>)GetValue(LstManageCommentModelProperty);
            set => SetValue(LstManageCommentModelProperty, value);
        }

        public bool Isupdated { get; set; }

        public ICommand AddCommentsCommand
        {
            get => (ICommand)GetValue(AddCommentsCommandProperty);
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
                var currentQuery = ((QueryContent)(sender as CheckBox)?.DataContext).Content.QueryValue;
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

        private void btnPhotos_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var opf = new OpenFileDialog();
                opf.Multiselect = true;
                opf.Filter =
                    "Image Files |*.jpg;*.jpeg;*.png;*.gif"; //"Image Files(*.BMP;*.JPG;*.GIF)|*.BMP;*.JPG;*.GIF";
                if (opf.ShowDialog().Value)
                {
                    foreach (string file in opf.FileNames)
                        Comments.MediaList.Add(file);
                }
                if(Comments.MediaList.Count() > 0)
                {
                    var mediaUtilities = new MediaUtilites();
                    Comments.MediaPath = mediaUtilities.GetThumbnail(Comments.MediaList[CurrentPointer]);
                    IsEnablePreviousPointer = CurrentPointer > 0;
                    IsEnableNextPointer = Comments.MediaList.Count() > 0;
                }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        private void DeleteMedia_Click(object sender, RoutedEventArgs e)
        {
            Comments.MediaPath = "";
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
            if (string.IsNullOrEmpty(Comments.CommentText) && string.IsNullOrEmpty(Comments.MediaPath))
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
                        x.MediaPath = Comments.MediaPath;
                        x.MediaList = Comments.MediaList;
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

        private void btnVideos_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var opf = new OpenFileDialog();
                opf.Filter = "Video Files |*.mp4;"; //"Image Files(*.BMP;*.JPG;*.GIF)|*.BMP;*.JPG;*.GIF";
                if (opf.ShowDialog().Value) Comments.MediaPath = opf.FileName;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        private void PART_NavigateNextImage_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                CurrentPointer++;
                if (CurrentPointer < Comments.MediaList.Count())
                {
                    var mediaUtilites = new MediaUtilites();
                    Comments.MediaPath = mediaUtilites.GetThumbnail(Comments.MediaList[CurrentPointer]);
                    IsEnableNextPointer = Comments.MediaList.Count() - (CurrentPointer + 1) > 0;
                    IsEnablePreviousPointer = CurrentPointer >= 0;
                }
                if (CurrentPointer > Comments.MediaList.Count() - 1)
                    CurrentPointer = Comments.MediaList.Count() - 1;
            }
            catch (Exception ex)
            {
                ex.DebugLog(ex.Message);
            }
        }

        private void PART_NavigatePreviousImage_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                CurrentPointer--;
                if(CurrentPointer >= 0)
                {
                    var mediaUtilities = new MediaUtilites();
                    Comments.MediaPath = mediaUtilities.GetThumbnail(Comments.MediaList[CurrentPointer]);
                    IsEnableNextPointer = Comments.MediaList.Count() - (CurrentPointer + 1) > 0;
                    IsEnablePreviousPointer = CurrentPointer > 0;
                }
                if (CurrentPointer < 0)
                    CurrentPointer = 0;
            }
            catch(Exception ex)
            {
                ex.DebugLog();
            }
        }
    }
}
