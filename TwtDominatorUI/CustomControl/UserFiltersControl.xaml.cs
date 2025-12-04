using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;
using DominatorHouseCore;
using DominatorHouseCore.Command;
using TwtDominatorCore.TDModels;

namespace TwtDominatorUI.CustomControl
{
    /// <summary>
    ///     Interaction logic for UserFiltersControl.xaml
    /// </summary>
    public partial class UserFiltersControl
    {
        public UserFiltersControl()
        {
            InitializeComponent();
            SkipUsersWhoWereAlreadySentAMessageFromSoftwareVisibility = Visibility.Collapsed;
            UserFilter = new UserFilterModel {SaveCloseButtonVisible = true};
            InitializeCommands();
            MainGrid.DataContext = this;
        }

        private void InitializeCommands()
        {
            try
            {
                SaveInvalidWordCommand = new BaseCommand<object>(sender => true, SaveInvalidWordExecute);
                SpecificWordCommand = new BaseCommand<object>(sender => true, SpecificWordExecute);
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        #region OnClick Operation

        private void SaveButton_OnClick(object sender, RoutedEventArgs e)
        {
            SaveUserFilterEventArgsHandler();
        }

        #endregion

        public static void OnAvailableItemsChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var newValue = e.NewValue;
        }

        #region commands

        public ICommand SaveInvalidWordCommand { get; set; }
        public ICommand SpecificWordCommand { get; set; }

        #endregion

        #region command execute

        private void SaveInvalidWordExecute(object obj)
        {
            try
            {
                UserFilter.LstInvalidWords = Regex.Split(InvalidWordsInputBox.InputText, ",").ToList();
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        private void SpecificWordExecute(object sender)
        {
            try
            {
                UserFilter.LstSpecificWords = Regex.Split(SpecificWordsInputBox.InputText, ",").ToList();
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        #endregion

        #region Setting dependancy property

        public UserFilterModel UserFilter
        {
            get => (UserFilterModel) GetValue(UserFilterProperty);
            set => SetValue(UserFilterProperty, value);
        }

        // Using a DependencyProperty as the backing store for UserFilter.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty UserFilterProperty =
            DependencyProperty.Register("UserFilter", typeof(UserFilterModel), typeof(UserFiltersControl),
                new FrameworkPropertyMetadata
                {
                    BindsTwoWayByDefault = true
                });


        public Visibility SkipUsersWhoWereAlreadySentAMessageFromSoftwareVisibility
        {
            get => (Visibility) GetValue(SkipUsersWhoWereAlreadySentAMessageFromSoftwareVisibilityProperty);
            set => SetValue(SkipUsersWhoWereAlreadySentAMessageFromSoftwareVisibilityProperty, value);
        }

        // Using a DependencyProperty as the backing store for SkipUsersWhoWereAlreadySentAMessageFromSoftwareVisibility.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SkipUsersWhoWereAlreadySentAMessageFromSoftwareVisibilityProperty =
            DependencyProperty.Register("SkipUsersWhoWereAlreadySentAMessageFromSoftwareVisibility", typeof(Visibility),
                typeof(UserFiltersControl));

        /*
         * new FrameworkPropertyMetadata(OnAvailableItemsChanged)
            {
                BindsTwoWayByDefault = true
            }
         */

        #endregion


        #region Routed Events for save

        private static readonly RoutedEvent SaveUserFilterEvent =
            EventManager.RegisterRoutedEvent("SaveUserFilterEventHandler", RoutingStrategy.Bubble,
                typeof(RoutedEventHandler), typeof(UserFiltersControl));

        public event RoutedEventHandler SaveUserFilterEventHandler
        {
            add => AddHandler(SaveUserFilterEvent, value);
            remove => RemoveHandler(SaveUserFilterEvent, value);
        }


        private void SaveUserFilterEventArgsHandler()
        {
            var rountedargs = new RoutedEventArgs(SaveUserFilterEvent);
            RaiseEvent(rountedargs);
        }

        #endregion
    }
}