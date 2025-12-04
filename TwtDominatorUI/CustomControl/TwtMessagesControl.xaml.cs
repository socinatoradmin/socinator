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
using MahApps.Metro.Controls.Dialogs;
using Microsoft.Win32;

namespace TwtDominatorUI.CustomControl
{
    /// <summary>
    ///     Interaction logic for TwtMessagesControl.xaml
    /// </summary>
    public partial class TwtMessagesControl : UserControl
    {
        private static readonly RoutedEvent AddMessagesToListEvent =
            EventManager.RegisterRoutedEvent("AddMessagesToListChanged", RoutingStrategy.Bubble,
                typeof(RoutedEventHandler),
                typeof(TwtMessagesControl));

        // Using a DependencyProperty as the backing store for ManageComments.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ManageMessagesProperty =
            DependencyProperty.Register("Messages", typeof(ManageMessagesModel), typeof(TwtMessagesControl),
                new PropertyMetadata(OnAvailableItemsChanged));

        // Using a DependencyProperty as the backing store for LstManageCommentModel.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LstManageMessagesModelProperty =
            DependencyProperty.Register("LstManageMessagesModel", typeof(ObservableCollection<ManageMessagesModel>),
                typeof(TwtMessagesControl), new PropertyMetadata(new ObservableCollection<ManageMessagesModel>()));

        // Using a DependencyProperty as the backing store for AddMessagesCommand.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty AddMessagesCommandProperty =
            DependencyProperty.Register("AddMessagesCommand", typeof(ICommand), typeof(TwtMessagesControl));

        // Using a DependencyProperty as the backing store for CommandParameter.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CommandParameterProperty =
            DependencyProperty.Register("CommandParameter", typeof(object), typeof(TwtMessagesControl),
                new PropertyMetadata(OnAvailableItemsChanged));

        public TwtMessagesControl()
        {
            InitializeComponent();
            MainGrid.DataContext = this;
            AddMessagesCommand = new BaseCommand<object>(sender => true, AddMessagesExecute);
        }

        public bool Isupdated { get; set; }


        public ManageMessagesModel Messages
        {
            get => (ManageMessagesModel) GetValue(ManageMessagesProperty);
            set => SetValue(ManageMessagesProperty, value);
        }

        public ObservableCollection<ManageMessagesModel> LstManageMessagesModel
        {
            get => (ObservableCollection<ManageMessagesModel>) GetValue(LstManageMessagesModelProperty);
            set => SetValue(LstManageMessagesModelProperty, value);
        }

        public ICommand AddMessagesCommand
        {
            get => (ICommand) GetValue(AddMessagesCommandProperty);
            set => SetValue(AddMessagesCommandProperty, value);
        }

        public object CommandParameter
        {
            get => GetValue(CommandParameterProperty);
            set => SetValue(CommandParameterProperty, value);
        }

        public event RoutedEventHandler AddMessagesToListChanged
        {
            add => AddHandler(AddMessagesToListEvent, value);
            remove => RemoveHandler(AddMessagesToListEvent, value);
        }

        private void AddCommentToListEventHandler()
        {
            var routedEventArgs = new RoutedEventArgs(AddMessagesToListEvent);
            RaiseEvent(routedEventArgs);
        }

        public static void OnAvailableItemsChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var newValue = e.NewValue;
        }

        private void BtnAddMessagesToList_OnClick(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(Messages.MessagesText) && string.IsNullOrEmpty(Messages.MediaPath))
            {
                Dialog.ShowDialog(Application.Current.MainWindow, "Warning",
                    "Please type some message !!");
                return;
            }

            AddCheckedQueryToList();
            if (Messages.SelectedQuery.Count == 0)
            {
                Dialog.ShowDialog(Application.Current.MainWindow, "Warning",
                    "Please add atleast one query!!");
                return;
            }

            if (btnAddMessagesToList.Content.ToString() == "Update Message")
            {
                LstManageMessagesModel.Select(x =>
                {
                    if (x.MessageId == Messages.MessageId)
                    {
                        x.MessagesText = Messages.MessagesText;
                        x.LstQueries = Messages.LstQueries;
                    }

                    return x;
                });
                Messages.SelectedQuery.Remove(
                    Messages.SelectedQuery.FirstOrDefault(x => x.Content.QueryValue == "All"));
                Messages.LstQueries.Select(x =>
                {
                    x.IsContentSelected = false;
                    return x;
                }).ToList();
                Isupdated = true;
                Dialog.CloseDialog(this);
            }
            else
            {
                AddCommentToListEventHandler();
            }
        }

        private void CheckUncheckAll(object sender, bool IsChecked)
        {
            if (((QueryContent) (sender as CheckBox).DataContext).Content.QueryValue == "All")
                Messages.LstQueries.ToList().Select(query =>
                {
                    query.IsContentSelected = IsChecked;
                    return query;
                }).ToList();
        }

        private void AddCheckedQueryToList()
        {
            Messages.SelectedQuery.Clear();
            Messages.LstQueries.ToList().ForEach(query =>
            {
                if (query.IsContentSelected)
                    Messages.SelectedQuery.Add(query);
            });
        }

        private void chkQuery_Checked(object sender, RoutedEventArgs e)
        {
            CheckUncheckAll(sender, true);
        }

        private void chkQuery_Unchecked(object sender, RoutedEventArgs e)
        {
            CheckUncheckAll(sender, false);
        }

        private void btnPhotos_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var opf = new OpenFileDialog();
                opf.Filter = "Image Files(*.BMP;*.JPG;*.GIF)|*.BMP;*.JPG;*.GIF";
                if (opf.ShowDialog().Value) Messages.MediaPath = opf.FileName;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        private void DeleteMedia_Click(object sender, RoutedEventArgs e)
        {
            Messages.MediaPath = "";
        }

        private void AddMessagesExecute(object sender)
        {
            if (string.IsNullOrEmpty(Messages.MessagesText) && string.IsNullOrEmpty(Messages.MediaPath))
            {
                Dialog.ShowDialog(Application.Current.MainWindow, "Warning",
                    "Please type some message !!");
                return;
            }

            AddCheckedQueryToList();
            if (Messages.SelectedQuery.Count == 0)
            {
                Dialog.ShowDialog(Application.Current.MainWindow, "Warning",
                    "Please add atleast one query!!");
                return;
            }

            if (btnAddMessagesToList.Content.ToString() == "Update Message")
            {
                LstManageMessagesModel.Select(x =>
                {
                    if (x.MessageId == Messages.MessageId)
                    {
                        x.MessagesText = Messages.MessagesText;
                        x.LstQueries = Messages.LstQueries;
                    }

                    return x;
                });
                Messages.SelectedQuery.Remove(
                    Messages.SelectedQuery.FirstOrDefault(x => x.Content.QueryValue == "All"));
                Messages.LstQueries.Select(x =>
                {
                    x.IsContentSelected = false;
                    return x;
                }).ToList();
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