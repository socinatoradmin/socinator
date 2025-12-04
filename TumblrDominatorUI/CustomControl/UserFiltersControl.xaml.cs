using DominatorHouseCore;
using DominatorHouseCore.Command;
using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;
using TumblrDominatorCore.Models;

namespace TumblrDominatorUI.CustomControl
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
            UserFilter = new UserFilterModel { SaveCloseButtonVisible = true };
            UserFilter.InvalidWordsInBio = "xxxx";
            InitializeCommands();
            MainGrid.DataContext = this;
        }

        #region commands

        public ICommand SaveInvalidWordCommand { get; set; }

        #endregion

        private void InitializeCommands()
        {
            try
            {
                SaveInvalidWordCommand = new BaseCommand<object>(sender => true, SaveInvalidWordExecute);
                //SpecificWordCommand = new BaseCommand<Object>((sender) => true, SpecificWordExecute);
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

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
        //private void SpecificWordExecute(object sender)
        //{
        //    try
        //    {
        //        UserFilter.LstSpecificWords = Regex.Split(SpecificWordsInputBox.InputText, ",").ToList();
        //    }
        //    catch (Exception ex)
        //    {
        //        ex.DebugLog();
        //    }
        //}

        #endregion

        #region OnClick Operation

        private void SaveButton_OnClick(object sender, RoutedEventArgs e)
        {
            SaveUserFilterEventArgsHandler();
        }

        #endregion

        #region Setting dependancy property

        public UserFilterModel UserFilter
        {
            get => (UserFilterModel)GetValue(UserFilterProperty);
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
            get => (Visibility)GetValue(SkipUsersWhoWereAlreadySentAMessageFromSoftwareVisibilityProperty);
            set => SetValue(SkipUsersWhoWereAlreadySentAMessageFromSoftwareVisibilityProperty, value);
        }

        // Using a DependencyProperty as the backing store for SkipUsersWhoWereAlreadySentAMessageFromSoftwareVisibility.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SkipUsersWhoWereAlreadySentAMessageFromSoftwareVisibilityProperty =
            DependencyProperty.Register("SkipUsersWhoWereAlreadySentAMessageFromSoftwareVisibility", typeof(Visibility),
                typeof(UserFiltersControl));

        #endregion


        #region Routed Events for save

        private static readonly RoutedEvent SaveUserFilterEvent =
            EventManager.RegisterRoutedEvent("SaveUserFilterEventHandler", RoutingStrategy.Bubble,
                typeof(RoutedEventHandler), typeof(UserFiltersControl));


        private void SaveUserFilterEventArgsHandler()
        {
            var rountedargs = new RoutedEventArgs(SaveUserFilterEvent);
            RaiseEvent(rountedargs);
        }

        #endregion
    }
}