using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using DominatorHouseCore.Annotations;
using DominatorHouseCore.Command;
using DominatorHouseCore.Enums.FdQuery;
using DominatorHouseCore.Models.FacebookModels;
using DominatorHouseCore.Utility;
using MahApps.Metro.Controls.Dialogs;

namespace DominatorUIUtility.CustomControl.FacebookCustomControl
{
    /// <summary>
    ///     Interaction logic for EventCreaterManager.xaml
    /// </summary>
    public partial class EventCreaterManager : INotifyPropertyChanged
    {
        // Using a DependencyProperty as the backing store for AddEventCommand.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty AddEventCommandProperty =
            DependencyProperty.Register("AddEventCommand", typeof(ICommand), typeof(EventCreaterManager));

        // Using a DependencyProperty as the backing store for LstManageCommentModel.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LstManageEventModelProperty =
            DependencyProperty.Register("LstManageEventModel", typeof(ObservableCollection<EventCreaterManagerModel>),
                typeof(EventCreaterManager), new PropertyMetadata());

        // Using a DependencyProperty as the backing store for CommandParameter.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CommandParameterProperty =
            DependencyProperty.Register("CommandParameter", typeof(object), typeof(EventCreaterManager),
                new PropertyMetadata());

        // Using a DependencyProperty as the backing store for CommandParameter.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TypeSelectionChangedCommandProperty =
            DependencyProperty.Register("TypeSelectionChangedCommand", typeof(object), typeof(EventCreaterManager),
                new PropertyMetadata());

        // Using a DependencyProperty as the backing store for EventCreaterManagerModel.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty EventCreaterManagerModelProperty =
            DependencyProperty.Register("EventCreaterManagerModelCommand", typeof(EventCreaterManagerModel),
                typeof(EventCreaterManager), new FrameworkPropertyMetadata()
                {
                    BindsTwoWayByDefault = true
                });

        private readonly List<KeyValuePair<string, string>> ListKeyValuePair;

        public EventCreaterManager()
        {
            InitializeComponent();
            MainGrid.DataContext = this;
            AddEventCommand = new BaseCommand<object>(sender => true, AddEventsExecute);
            ListComboBox.ItemsSource = new List<string>
            {
                EventType.CreatePrivateEvent.ToString(),
                EventType.CreatePublicEvent.ToString()
            };

            ListKeyValuePair = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("1116111648515721", "Art"),
                new KeyValuePair<string, string>("1284277608291920", "Causes"),
                new KeyValuePair<string, string>("660032617536373", "Comedy"),
                new KeyValuePair<string, string>("258647957895086", "Crafts"),
                new KeyValuePair<string, string>("363764800677393", "Dance"),
                new KeyValuePair<string, string>("412284995786529", "Drinks"),
                new KeyValuePair<string, string>("392955781081975", "Film"),
                new KeyValuePair<string, string>("1138994019544264", "Fitness"),
                new KeyValuePair<string, string>("370585540007142", "Food"),
                new KeyValuePair<string, string>("1219165261515884", "Gardening"),
                new KeyValuePair<string, string>("1254988834549294", "Health"),
                new KeyValuePair<string, string>("220618358412161", "Home"),
                new KeyValuePair<string, string>("432347013823672", "Literature"),
                new KeyValuePair<string, string>("1821948261404481", "Music"),
                new KeyValuePair<string, string>("1915104302042536", "Networking"),
                new KeyValuePair<string, string>("183019258855149", "Party"),
                new KeyValuePair<string, string>("1763934757268181", "Religion"),
                new KeyValuePair<string, string>("1759906074034918", "Shopping"),
                new KeyValuePair<string, string>("607999416057365", "Sports"),
                new KeyValuePair<string, string>("664694117046626", "Theatre"),
                new KeyValuePair<string, string>("1712245629067288", "Wellness"),
                new KeyValuePair<string, string>("359809011100389", "Others")
            };

            ListCategoryBox.ItemsSource = new List<string>
            {
                "Select Category",
                "Art",
                "Causes",
                "Comedy",
                "Crafts",
                "Dance",
                "Drinks",
                "Film",
                "Fitness",
                "Food",
                "Gardening",
                "Health",
                "Home",
                "Literature",
                "Music",
                "Networking",
                "Party",
                "Religion",
                "Shopping",
                "Sports",
                "Theatre",
                "Wellness",
                "Others"
            };
        }

        public bool Isupdated { get; set; }

        public ICommand AddEventCommand
        {
            get => (ICommand) GetValue(AddEventCommandProperty);
            set => SetValue(AddEventCommandProperty, value);
        }

        public ObservableCollection<EventCreaterManagerModel> LstManageEventModel
        {
            get => (ObservableCollection<EventCreaterManagerModel>) GetValue(LstManageEventModelProperty);
            set => SetValue(LstManageEventModelProperty, value);
        }


        public object CommandParameter
        {
            get => GetValue(CommandParameterProperty);
            set => SetValue(CommandParameterProperty, value);
        }

        public object TypeSelectionChangedCommand
        {
            get => GetValue(TypeSelectionChangedCommandProperty);
            set => SetValue(TypeSelectionChangedCommandProperty, value);
        }

        public EventCreaterManagerModel EventCreaterManagerModelCommand
        {
            get => (EventCreaterManagerModel) GetValue(EventCreaterManagerModelProperty);
            set => SetValue(EventCreaterManagerModelProperty, value);
        }

        public string TextLength { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;


        private void AddEventsExecute(object sender)
        {
            EventCreaterManagerModelCommand.CategoryId = ListKeyValuePair
                .FirstOrDefault(x => x.Key == EventCreaterManagerModelCommand.Category).Value;

            if (string.IsNullOrEmpty(EventCreaterManagerModelCommand.EventName))
            {
                Dialog.ShowDialog(Application.Current.MainWindow, "Warning",
                    $"{"LangKeyGiveAProperEventName".FromResourceDictionary()}");
                return;
            }

            if (EventCreaterManagerModelCommand.EventType == "CreatePublicEvent" &&
                EventCreaterManagerModelCommand.Category == "Select Category")
            {
                Dialog.ShowDialog(Application.Current.MainWindow, "Warning",
                    "LangKeySelectCategory".FromResourceDictionary());
                return;
            }

            if (EventCreaterManagerModelCommand.EventStartDate < DateTime.Now
                || EventCreaterManagerModelCommand.EventEndDate < DateTime.Now)
            {
                Dialog.ShowDialog(Application.Current.MainWindow, "Warning",
                    "LangKeyGiveStartAndEndDate".FromResourceDictionary());
                return;
            }

            if (EventCreaterManagerModelCommand.EventStartDate > EventCreaterManagerModelCommand.EventEndDate)
            {
                Dialog.ShowDialog(Application.Current.MainWindow, "Warning",
                    "LangKeyEndDateShouldNotBeLessThanStartDate".FromResourceDictionary());
                return;
            }

            if (EventCreaterManagerModelCommand.EventStartDate.AddDays(13) <
                EventCreaterManagerModelCommand.EventEndDate)
            {
                Dialog.ShowDialog(Application.Current.MainWindow, "Warning",
                    "LangKeyEventWillBeOfDay".FromResourceDictionary());
                return;
            }

            if (BtnAddEventToList.Content.ToString() == "Update Event")
            {
                LstManageEventModel.ForEach(x =>
                {
                    if (x.Id == EventCreaterManagerModelCommand.Id)
                    {
                        x.EventName = EventCreaterManagerModelCommand.EventName;
                        x.Category = EventCreaterManagerModelCommand.Category;
                        x.CategoryId = EventCreaterManagerModelCommand.CategoryId;
                        x.EventDescription = EventCreaterManagerModelCommand.EventDescription;
                        x.EventStartDate = EventCreaterManagerModelCommand.EventStartDate;
                        x.EventEndDate = EventCreaterManagerModelCommand.EventEndDate;
                        x.EventLocation = EventCreaterManagerModelCommand.EventLocation;
                        x.EventType = EventCreaterManagerModelCommand.EventType;
                        x.MediaPath = EventCreaterManagerModelCommand.FbMultiMediaModel.MediaPaths.Count > 0
                            ? EventCreaterManagerModelCommand.FbMultiMediaModel.MediaPaths.FirstOrDefault()?.MediaPath
                            : null;
                        x.FbMultiMediaModel.MediaPaths = EventCreaterManagerModelCommand.FbMultiMediaModel.MediaPaths;
                        x.IsAnyOneCanPostForAllPost = EventCreaterManagerModelCommand.IsAnyOneCanPostForAllPost;
                        x.IsGuestCanInviteFriends = EventCreaterManagerModelCommand.IsGuestCanInviteFriends;
                        x.FbMultiMediaModel.IsAddImageVisibile =
                            EventCreaterManagerModelCommand.FbMultiMediaModel.IsAddImageVisibile;
                        x.IsPrivatePostingVisibile = EventCreaterManagerModelCommand.IsPrivatePostingVisibile;
                        x.IsShowGuestList = EventCreaterManagerModelCommand.IsShowGuestList;
                        x.TextLength = EventCreaterManagerModelCommand.TextLength;
                        x.IsPublicPostingVisibile = EventCreaterManagerModelCommand.IsPublicPostingVisibile;
                        x.IsSelectLocation = EventCreaterManagerModelCommand.IsSelectLocation;
                        x.IsPostMustApproved = EventCreaterManagerModelCommand.IsPostMustApproved;
                        x.IsQuesOnMessanger = EventCreaterManagerModelCommand.IsQuesOnMessanger;
                    }
                });

                Isupdated = true;

                Dialog.CloseDialog(this);
            }
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void TextBoxBase_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            EventCreaterManagerModelCommand.TextLength = $"{TextBoxEvent.Text.Length.ToString()}/64";
        }
    }
}