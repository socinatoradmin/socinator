using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using DominatorHouseCore;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;

namespace TwtDominatorUI.CustomControl
{
    /// <summary>
    ///     Interaction logic for TwtManageMessages.xaml
    /// </summary>
    public partial class TwtManageMessages : UserControl
    {
        // Using a DependencyProperty as the backing store for LstManageCommentModel.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LstManageMessagesModelProperty =
            DependencyProperty.Register("LstManageMessagesModel", typeof(ObservableCollection<ManageMessagesModel>),
                typeof(TwtManageMessages), new PropertyMetadata(OnAvailableItemsChanged));

        public TwtManageMessages()
        {
            InitializeComponent();
            MainGrid.DataContext = this;
        }

        public ObservableCollection<ManageMessagesModel> LstManageMessagesModel
        {
            get => (ObservableCollection<ManageMessagesModel>) GetValue(LstManageMessagesModelProperty);
            set => SetValue(LstManageMessagesModelProperty, value);
        }

        public static void OnAvailableItemsChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var newValue = e.NewValue;
        }

        private void BtnAction_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ((Button) sender).ContextMenu.DataContext = ((Button) sender).DataContext;
                ((Button) sender).ContextMenu.IsOpen = true;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        private void EditMessage_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                var currentItem = ((FrameworkElement) sender).DataContext as ManageMessagesModel;
                var editMessage = new TwtMessagesControl();
                editMessage.btnAddMessagesToList.Content = "Update Message";
                editMessage.Messages = currentItem;

                editMessage.Messages.LstQueries.ToList().ForEach(x =>
                {
                    if (editMessage.Messages.SelectedQuery.Contains(x))
                        x.IsContentSelected = true;
                });
                editMessage.MainGrid.Margin = new Thickness(20);
                var dialog = new Dialog();
                var window = dialog.GetMetroWindow(editMessage, "Edit Message");
                window.Show();
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        private void DeleteSingleMessage_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                var currentItem = ((FrameworkElement) sender).DataContext as ManageMessagesModel;
                LstManageMessagesModel.Remove(currentItem);
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }
    }
}