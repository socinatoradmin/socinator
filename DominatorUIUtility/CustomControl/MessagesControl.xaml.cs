using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using DominatorHouseCore.Command;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;

namespace DominatorUIUtility.CustomControl
{
    /// <summary>
    ///     Interaction logic for MessagesControl.xaml
    /// </summary>
    public partial class MessagesControl
    {
        private static readonly RoutedEvent AddMessagesToListEvent =
            EventManager.RegisterRoutedEvent("AddMessagesToListChanged", RoutingStrategy.Bubble,
                typeof(RoutedEventHandler),
                typeof(MessagesControl));

        // Using a DependencyProperty as the backing store for ManageComments.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ManageMessagesProperty =
            DependencyProperty.Register("Messages", typeof(ManageMessagesModel), typeof(MessagesControl),
                new PropertyMetadata());

        // Using a DependencyProperty as the backing store for LstManageCommentModel.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LstManageMessagesModelProperty =
            DependencyProperty.Register("LstManageMessagesModel", typeof(ObservableCollection<ManageMessagesModel>),
                typeof(MessagesControl), new PropertyMetadata(new ObservableCollection<ManageMessagesModel>()));

        // Using a DependencyProperty as the backing store for AddMessagesCommand.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty AddMessagesCommandProperty =
            DependencyProperty.Register("AddMessagesCommand", typeof(ICommand), typeof(MessagesControl));

        // Using a DependencyProperty as the backing store for CommandParameter.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CommandParameterProperty =
            DependencyProperty.Register("CommandParameter", typeof(object), typeof(MessagesControl),
                new PropertyMetadata());

        private bool _isUncheckfromList;

        public MessagesControl()
        {
            InitializeComponent();
            MainGrid.DataContext = this;
            AddMessagesCommand = new BaseCommand<object>(sender => true, AddMessagesExecute);
        }

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

        public bool Isupdated { get; set; }

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

        private void CheckUncheckAll(object sender, bool IsChecked)
        {
            var currentQuery = ((QueryContent) (sender as CheckBox)?.DataContext).Content.QueryValue;
            if (!Messages.LstQueries.Skip(1).All(x => x.IsContentSelected))
                if (!IsChecked)
                {
                    _isUncheckfromList = true;
                    Messages.LstQueries[0].IsContentSelected = false;
                }

            if (Messages.LstQueries.Skip(1).All(x => x.IsContentSelected))
            {
                _isUncheckfromList = false;
                Messages.LstQueries[0].IsContentSelected = IsChecked;
            }

            if (_isUncheckfromList)
            {
                _isUncheckfromList = false;
                return;
            }

            if (currentQuery == "All")
            {
                _isUncheckfromList = false;
                Messages.LstQueries.ToList().ForEach(query =>
                {
                    query.IsContentSelected = IsChecked;
                });
            }
        }

        private void AddCheckedQueryToList()
        {
            Messages.SelectedQuery.Clear();
            Messages.LstQueries.ToList().ForEach(query =>
            {
                query.Content.Index = Messages.LstQueries.IndexOf(query);
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

        private void AddMessagesExecute(object sender)
        {
            if (string.IsNullOrEmpty(Messages.MessagesText))
            {
                Dialog.ShowDialog("Warning", "Please type some message !!");
                return;
            }

            if (!Messages.LstQueries.Any(x => x.IsContentSelected))
            {
                Dialog.ShowDialog("Warning", "Please select atleast one query.");
                return;
            }

            AddCheckedQueryToList();
            if (Messages.SelectedQuery.Count == 0)
            {
                Dialog.ShowDialog("Warning", "Please add atleast one query!!");
                return;
            }

            if (btnAddMessagesToList.Content.ToString() == "Update Message")
            {
                LstManageMessagesModel.ForEach(x =>
                {
                    if (x.MessageId == Messages.MessageId)
                    {
                        x.MessagesText = Messages.MessagesText;
                        x.LstQueries = Messages.LstQueries;
                        x.SelectedQuery = Messages.SelectedQuery;
                    }
                });
                Messages.SelectedQuery.Remove(
                    Messages.SelectedQuery.FirstOrDefault(x => x.Content.QueryValue == "All"));
                Messages.LstQueries.ForEach(x =>
                {
                    x.IsContentSelected = false;
                });
                Isupdated = true;

                Dialog.CloseDialog(this);
            }
        }
    }
}