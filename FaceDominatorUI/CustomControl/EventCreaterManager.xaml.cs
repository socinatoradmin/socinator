using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using DominatorHouseCore;
using DominatorHouseCore.Annotations;
using DominatorHouseCore.Command;
using FaceDominatorCore.FDModel.FbEvents;
using MahApps.Metro.Controls.Dialogs;
using Microsoft.Win32;
using System.Collections.ObjectModel;
using System.Linq;
using DominatorHouseCore.Utility;
using System.Collections.Generic;
using FaceDominatorCore.FDModel.CustomControlModel;
using FaceDominatorCore.FDEnums;

namespace FaceDominatorUI.CustomControl
{
    /// <summary>
    /// Interaction logic for EventCreaterManager.xaml
    /// </summary>
    public partial class EventCreaterManager : UserControl, INotifyPropertyChanged
    {
        public EventCreaterManager()
        {
            InitializeComponent();
            MainGrid.DataContext = this;
            AddEventCommand = new BaseCommand<object>((sender) => true, AddEventsExecute);
            ListComboBox.ItemsSource = new List<string>
            {
                EventType.CreatePrivateEvent.ToString(),
                EventType.CreatePublicEvent.ToString()
            };
        }

        public bool Isupdated { get; set; }

        public ICommand AddEventCommand
        {
            get { return (ICommand)GetValue(AddEventCommandProperty); }
            set { SetValue(AddEventCommandProperty, value); }
        }

        // Using a DependencyProperty as the backing store for AddEventCommand.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty AddEventCommandProperty =
            DependencyProperty.Register("AddEventCommand", typeof(ICommand), typeof(EventCreaterManager));

        public ObservableCollection<EventCreaterManagerModel> LstManageEventModel
        {
            get { return (ObservableCollection<EventCreaterManagerModel>)GetValue(LstManageEventModelProperty); }
            set { SetValue(LstManageEventModelProperty, value); }
        }

        // Using a DependencyProperty as the backing store for LstManageCommentModel.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LstManageEventModelProperty =
            DependencyProperty.Register("LstManageEventModel", typeof(ObservableCollection<EventCreaterManagerModel>),
                typeof(EventCreaterManager), new PropertyMetadata(OnAvailableItemsChanged));


        private void AddEventsExecute(object sender)
        {

            if (string.IsNullOrEmpty(EventCreaterManagerModelCommand.EventName))
            {
                DialogCoordinator.Instance.ShowModalMessageExternal(Application.Current.MainWindow, "LangKeyWarning".FromResourceDictionary(),
                    "LangKeyPleaseGiveAProperEventName".FromResourceDictionary());
                return;
            }

            if (EventCreaterManagerModelCommand.EventStartDate < DateTime.Now
                || EventCreaterManagerModelCommand.EventEndDate < DateTime.Now)
            {
                DialogCoordinator.Instance.ShowModalMessageExternal(Application.Current.MainWindow, "LangKeyWarning".FromResourceDictionary(),
                    "LangKeyGiveStartAndEndDate".FromResourceDictionary());
                return;
            }

            if (EventCreaterManagerModelCommand.EventStartDate > EventCreaterManagerModelCommand.EventEndDate)
            {
                DialogCoordinator.Instance.ShowModalMessageExternal(Application.Current.MainWindow, "LangKeyWarning".FromResourceDictionary(),
                    "LangKeyEndDateShouldNotBeLessThanStartDate".FromResourceDictionary());
                return;
            }

            if (EventCreaterManagerModelCommand.EventStartDate.AddDays(13) < EventCreaterManagerModelCommand.EventEndDate)
            {
                DialogCoordinator.Instance.ShowModalMessageExternal(Application.Current.MainWindow, "LangKeyWarning".FromResourceDictionary(),
                    "LangKeyEventWillBeOfDay".FromResourceDictionary());
                return;
            }

            if (BtnAddEventToList.Content.ToString() == "Update Event")
            {
                LstManageEventModel.Select(x =>
                {
                    if (x.Id == EventCreaterManagerModelCommand.Id)
                    {
                        x.EventName = EventCreaterManagerModelCommand.EventName;
                        x.EventDescription = EventCreaterManagerModelCommand.EventDescription;
                        x.EventStartDate = EventCreaterManagerModelCommand.EventStartDate;
                        x.EventEndDate = EventCreaterManagerModelCommand.EventEndDate;
                        x.EventLocation = EventCreaterManagerModelCommand.EventLocation;
                        x.EventType = EventCreaterManagerModelCommand.EventType;
                        x.MediaPath = EventCreaterManagerModelCommand.FbMultiMediaModel.MediaPaths.FirstOrDefault().MediaPath;
                        x.FbMultiMediaModel.MediaPaths = EventCreaterManagerModelCommand.FbMultiMediaModel.MediaPaths;
                        x.IsAnyOneCanPostForAllPost = EventCreaterManagerModelCommand.IsAnyOneCanPostForAllPost;
                        x.IsGuestCanInviteFriends = EventCreaterManagerModelCommand.IsGuestCanInviteFriends;
                        x.FbMultiMediaModel.IsAddImageVisibile = EventCreaterManagerModelCommand.FbMultiMediaModel.IsAddImageVisibile;
                        x.IsPrivatePostingVisibile = EventCreaterManagerModelCommand.IsPrivatePostingVisibile;
                        x.IsShowGuestList = EventCreaterManagerModelCommand.IsShowGuestList;
                        x.TextLength = EventCreaterManagerModelCommand.TextLength;
                        x.IsPublicPostingVisibile = EventCreaterManagerModelCommand.IsPublicPostingVisibile;
                        x.IsSelectLocation = EventCreaterManagerModelCommand.IsSelectLocation;
                        x.IsPostMustApproved = EventCreaterManagerModelCommand.IsPostMustApproved;
                        x.IsQuesOnMessanger = EventCreaterManagerModelCommand.IsQuesOnMessanger;
                    }
                    return x;
                }).ToList();

                Isupdated = true;

                Dialog.CloseDialog(this);
            }

        }


        public object CommandParameter
        {
            get { return GetValue(CommandParameterProperty); }
            set { SetValue(CommandParameterProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CommandParameter.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CommandParameterProperty =
            DependencyProperty.Register("CommandParameter", typeof(object), typeof(EventCreaterManager),
                new PropertyMetadata(OnAvailableItemsChanged));

        public object TypeSelectionChangedCommand
        {
            get { return GetValue(TypeSelectionChangedCommandProperty); }
            set { SetValue(TypeSelectionChangedCommandProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CommandParameter.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TypeSelectionChangedCommandProperty =
            DependencyProperty.Register("TypeSelectionChangedCommand", typeof(object), typeof(EventCreaterManager),
                new PropertyMetadata(OnAvailableItemsChanged));


        public EventCreaterManagerModel EventCreaterManagerModelCommand
        {
            get { return (EventCreaterManagerModel)GetValue(EventCreaterManagerModelProperty); }
            set { SetValue(EventCreaterManagerModelProperty, value); }
        }

        // Using a DependencyProperty as the backing store for EventCreaterManagerModel.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty EventCreaterManagerModelProperty =
            DependencyProperty.Register("EventCreaterManagerModelCommand", typeof(EventCreaterManagerModel),
                typeof(EventCreaterManager), new FrameworkPropertyMetadata(OnAvailableItemsChanged)
                {
                    BindsTwoWayByDefault = true
                });


        public static void OnAvailableItemsChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var newValue = e.NewValue;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        //private void btnPhotos_Click(object sender, RoutedEventArgs e)
        //{
        //    try
        //    {
        //        OpenFileDialog opf = new OpenFileDialog();
        //        opf.Multiselect = true;
        //        opf.Filter = "Image Files(*.BMP;*.JPG;*.GIF)|*.BMP;*.JPG;*.GIF | Video Files(*.dat; *.wmv; *.mp4;)|*.dat; *.wmv; *.mp4";
        //        if (opf.ShowDialog().Value)
        //        {
        //            EventCreaterManagerModelCommand.MediaPath = opf.FileName;
        //            EventCreaterManagerModelCommand.PhotoButtonVisibility = "Collapsed";
        //            EventCreaterManagerModelCommand.PhotoDeleteVisibility = "Visible";
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        ex.DebugLog();
        //    }
        //}

        //private void DeleteMedia_Click(object sender, RoutedEventArgs e)
        //{
        //    EventCreaterManagerModelCommand.MediaPath = "";
        //    EventCreaterManagerModelCommand.PhotoButtonVisibility = "Visible";
        //    EventCreaterManagerModelCommand.PhotoDeleteVisibility = "Collapsed";
        //}

        //private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        //{
        //    EventCreaterManagerModelCommand.MediaPath = "";
        //    EventCreaterManagerModelCommand.FbMultiMediaModel.AddImageVisibility = "Visible";
        //}

        public string TextLength { get; set; }

        private void TextBoxBase_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            EventCreaterManagerModelCommand.TextLength = $"{TextBoxEvent.Text.Length.ToString()}/64";
        }

    }
}