using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using DominatorHouseCore;
using DominatorHouseCore.Command;
using LinkedDominatorCore.LDModel.Filters;

namespace LinkedDominatorUI.CustomControl
{
    /// <summary>
    ///     Interaction logic for UserFiltersControl.xaml
    /// </summary>
    public partial class UserFiltersControl : UserControl
    {
        // Using a DependencyProperty as the backing store for UserFilter.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty UserFilterProperty =
            DependencyProperty.Register("LDUserFilter", typeof(LDUserFilterModel), typeof(UserFiltersControl),
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
            LDUserFilter = new LDUserFilterModel();
            MainGrid.DataContext = this;
            InitializeCommands();
        }

        public LDUserFilterModel LDUserFilter
        {
            get => (LDUserFilterModel) GetValue(UserFilterProperty);
            set => SetValue(UserFilterProperty, value);
        }

        private void InitializeCommands()
        {
            try
            {
                SaveInvalidWordCommand = new BaseCommand<object>(sender => true, SaveInvalidWordExecute);
                SaveValidWordCommand = new BaseCommand<object>(sender => true, SaveValidWordExecute);
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        public static void OnAvailableItemsChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var newValue = e.NewValue;
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

        #region commands

        public ICommand SaveInvalidWordCommand { get; set; }
        public ICommand SaveValidWordCommand { get; set; }

        #endregion

        #region command execute

        private void SaveInvalidWordExecute(object obj)
        {
            try
            {
                LDUserFilter.LstInvalidWords = Regex.Split(LDUserFilter.InvalidWords, ",").Distinct().ToList();
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        private void SaveValidWordExecute(object sender)
        {
            try
            {
                LDUserFilter.LstValidWords = Regex.Split(ValidWordsInputBox.InputText, ",").Distinct().ToList();
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        #endregion
    }
}