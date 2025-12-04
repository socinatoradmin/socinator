using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;
using DominatorHouseCore.Command;
using QuoraDominatorCore.Models;

namespace QuoraDominatorUI.CustomControl
{
    /// <summary>
    ///     Interaction logic for UserFiltersControl.xaml
    /// </summary>
    public partial class UserFiltersControl
    {
        // Using a DependencyProperty as the backing store for UserFilter.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty UserFilterProperty =
            DependencyProperty.Register("UserFilter", typeof(UserFilterModel), typeof(UserFiltersControl),
                new FrameworkPropertyMetadata(OnAvailableItemsChanged)
                {
                    BindsTwoWayByDefault = true
                });

        private static readonly RoutedEvent SaveUserFilterEvent =
            EventManager.RegisterRoutedEvent("SaveUserFilterEventHandler", RoutingStrategy.Bubble,
                typeof(RoutedEventHandler), typeof(UserFiltersControl));

        public UserFiltersControl()
        {
            InitializeComponent();
            UserFilter = new UserFilterModel();
            MainGrid.DataContext = this;
            SaveBlacklistStudiedCommand = new BaseCommand<object>(sender => true, SaveBlacklistStudiedPlace);
            SaveBlacklistLivedInCommand = new BaseCommand<object>(sender => true, SaveBlacklistLivedInPlace);
            SaveBlacklistWorkedInCommand = new BaseCommand<object>(sender => true, SaveBlacklistWorkedInPlace);
            SaveBioRestrictedWordsCommand = new BaseCommand<object>(sender => true, SaveBioRestrictedWords);
            SaveUsernameRestrictedWordsCommand = new BaseCommand<object>(sender => true, SaveUsernameRestrictedWords);
        }


        public UserFilterModel UserFilter
        {
            get => (UserFilterModel) GetValue(UserFilterProperty);
            set => SetValue(UserFilterProperty, value);
        }

        public static void OnAvailableItemsChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
        }

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

        private void SaveButton_OnClick(object sender, RoutedEventArgs e)
        {
            SaveUserFilterEventArgsHandler();
        }


        #region Command Declaration

        public ICommand SaveBlacklistStudiedCommand { get; set; }
        public ICommand SaveBlacklistLivedInCommand { get; set; }
        public ICommand SaveBlacklistWorkedInCommand { get; set; }
        public ICommand SaveBioRestrictedWordsCommand { get; set; }
        public ICommand SaveUsernameRestrictedWordsCommand { get; set; }

        #endregion

        #region Execute Command Methods

        public void SaveBlacklistStudiedPlace(object sender)
        {
            UserFilter.BlacklistedStudiedPlaces =
                Regex.Split(BlacklistedStudiedPlacesInputBox.InputText, "\r\n").ToList();
        }

        private void SaveBlacklistLivedInPlace(object obj)
        {
            UserFilter.BlacklistedLivesInPlaces =
                Regex.Split(BlacklistedLivesInPlacesInputBox.InputText, "\r\n").ToList();
        }

        private void SaveBlacklistWorkedInPlace(object obj)
        {
            UserFilter.BlacklistedWorkPlaces = Regex.Split(BlacklistedWorkPlacesInputBox.InputText, "\r\n").ToList();
        }

        private void SaveBioRestrictedWords(object obj)
        {
            UserFilter.BioRestrictedWords = Regex.Split(BioRestrictedWordsInputBox.InputText, "\r\n").ToList();
        }

        private void SaveUsernameRestrictedWords(object obj)
        {
            UserFilter.UserNameRestrictedWords =
                Regex.Split(UserNameRestrictedWordsInputBox.InputText, "\r\n").ToList();
        }

        #endregion

        #region Not Used Code

        private void UserRestrictedWordsInputBox_OnGetInputClick(object sender, RoutedEventArgs e)
        {
            UserFilter.UserNameRestrictedWords =
                Regex.Split(UserNameRestrictedWordsInputBox.InputText, "\r\n").ToList();
        }

        private void BioRestrictedWordsInputBox_OnGetInputClick(object sender, RoutedEventArgs e)
        {
            UserFilter.BioRestrictedWords = Regex.Split(BioRestrictedWordsInputBox.InputText, "\r\n").ToList();
        }

        private void BlacklistedLivesInPlacesInputBox_OnGetInputClick(object sender, RoutedEventArgs e)
        {
            UserFilter.BlacklistedLivesInPlaces =
                Regex.Split(BlacklistedLivesInPlacesInputBox.InputText, "\r\n").ToList();
        }

        private void BlacklistedStudiedPlacesInputBox_OnGetInputClick(object sender, RoutedEventArgs e)
        {
            UserFilter.BlacklistedStudiedPlaces =
                Regex.Split(BlacklistedStudiedPlacesInputBox.InputText, "\r\n").ToList();
        }

        private void BlacklistedWorkPlacesInputBox_OnGetInputClick(object sender, RoutedEventArgs e)
        {
            UserFilter.BlacklistedWorkPlaces = Regex.Split(BlacklistedWorkPlacesInputBox.InputText, "\r\n").ToList();
        }

        private void BlacklistedLivesInPlacesInputBox_Loaded(object sender, RoutedEventArgs e)
        {
            BlacklistedWorkPlacesInputBox.InputText =
                string.Join(Environment.NewLine, UserFilter.BlacklistedLivesInPlaces);
        }

        private void BlacklistedStudiedPlacesCheckBox_Loaded(object sender, RoutedEventArgs e)
        {
            BlacklistedWorkPlacesInputBox.InputText =
                string.Join(Environment.NewLine, UserFilter.BlacklistedStudiedPlaces);
        }

        private void BlacklistedWorkPlacesCheckBox_Loaded(object sender, RoutedEventArgs e)
        {
            BlacklistedWorkPlacesInputBox.InputText =
                string.Join(Environment.NewLine, UserFilter.BlacklistedWorkPlaces);
        }

        #endregion
    }
}