using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using DominatorHouseCore;
using DominatorHouseCore.Models.FacebookModels;
using DominatorHouseCore.Utility;

namespace DominatorUIUtility.CustomControl.FacebookCustomControl
{
    /// <summary>
    ///     Interaction logic for ManageEventDetails.xaml
    /// </summary>
    public partial class ManageEventDetails
    {
        // Using a DependencyProperty as the backing store for LstManageCommentModel.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LstManageEventModelProperty =
            DependencyProperty.Register("LstManageEventModel", typeof(ObservableCollection<EventCreaterManagerModel>),
                typeof(ManageEventDetails), new PropertyMetadata());

        // Using a DependencyProperty as the backing store for EditEventCommand.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty EditValueCommandProperty =
            DependencyProperty.Register("EditValueCommand", typeof(ICommand), typeof(ManageEventDetails));

        // Using a DependencyProperty as the backing store for AddEventCommand.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DeleteValueCommandProperty =
            DependencyProperty.Register("DeleteValueCommand", typeof(ICommand), typeof(ManageEventDetails));

        public ManageEventDetails()
        {
            InitializeComponent();
            MainGrid.DataContext = this;
        }

        public ObservableCollection<EventCreaterManagerModel> LstManageEventModel
        {
            get => (ObservableCollection<EventCreaterManagerModel>) GetValue(LstManageEventModelProperty);
            set => SetValue(LstManageEventModelProperty, value);
        }

        public ICommand EditValueCommand
        {
            get => (ICommand) GetValue(EditValueCommandProperty);
            set => SetValue(EditValueCommandProperty, value);
        }


        public ICommand DeleteValueCommand
        {
            get => (ICommand) GetValue(DeleteValueCommandProperty);
            set => SetValue(DeleteValueCommandProperty, value);
        }


        private void MenuItem_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                if (((FrameworkElement) sender).DataContext is EventCreaterManagerModel currentItem)
                {
                    var editMessage = new EventCreaterManager
                    {
                        BtnAddEventToList = {Content = "Update Event"},
                        EventCreaterManagerModelCommand = new EventCreaterManagerModel
                        {
                            Id = currentItem.Id,
                            EventType = currentItem.EventType,
                            EventName = currentItem.EventName,
                            EventDescription = currentItem.EventDescription,
                            EventLocation = currentItem.EventLocation,
                            EventStartDate = currentItem.EventStartDate,
                            EventEndDate = currentItem.EventEndDate,
                            IsAnyOneCanPostForAllPost = currentItem.IsAnyOneCanPostForAllPost,
                            IsGuestCanInviteFriends = currentItem.IsGuestCanInviteFriends,
                            IsShowGuestList = currentItem.IsShowGuestList,
                            MediaPath = currentItem.MediaPath,
                            FbMultiMediaModel = currentItem.FbMultiMediaModel,
                            IsPostMustApproved = currentItem.IsPostMustApproved,
                            IsQuesOnMessanger = currentItem.IsQuesOnMessanger,
                            IsSelectLocation = currentItem.IsSelectLocation,
                            IsPrivatePostingVisibile = currentItem.IsPrivatePostingVisibile,
                            IsPublicPostingVisibile = currentItem.IsPublicPostingVisibile,
                            TextLength = currentItem.TextLength,
                            EventId = currentItem.EventId
                        },
                        LstManageEventModel = LstManageEventModel,
                        MainGrid = {Margin = new Thickness(20)}
                    };


                    var dialog = new Dialog();
                    var window = dialog.GetMetroWindow(editMessage, "Edit Message");
                    window.ShowDialog();
                    window.Closed += (s, evnt) =>
                    {
                        if (editMessage.Isupdated)
                        {
                            var indexToUpdate = LstManageEventModel.IndexOf(currentItem);
                            LstManageEventModel[indexToUpdate] = editMessage.EventCreaterManagerModelCommand;
                        }
                    };
                }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        private void DeleteSingleMessage_OnClick(object sender, RoutedEventArgs e)
        {
            var currentItem = ((FrameworkElement) sender).DataContext as EventCreaterManagerModel;

            LstManageEventModel.Remove(currentItem);
        }

        private void BtnAction_OnClick(object sender, RoutedEventArgs e)
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
            catch (Exception)
            {
                //ignored
            }
        }
    }
}