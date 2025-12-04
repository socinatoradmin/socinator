using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using DominatorHouseCore;
using DominatorHouseCore.Command;
using TwtDominatorCore.TDModels;

namespace TwtDominatorUI.CustomControl
{
    /// <summary>
    ///     Interaction logic for TweetFiltersControl.xaml
    /// </summary>
    public partial class TweetFiltersControl : UserControl
    {
        public TweetFiltersControl()
        {
            InitializeComponent();
            MainGrid.DataContext = this;
            InitializeCommands();
        }

        private void InitializeCommands()
        {
            try
            {
                SaveInvalidWordCommand = new BaseCommand<object>(sender => true, SaveInvalidWordExecute);
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
                TweetFilter.SkipTweetsContainingWords = SkipTweetsContainingSpecificWordsInputBox.InputText;
                TweetFilter.LstSkipTweetsContainingWords =
                    Regex.Split(SkipTweetsContainingSpecificWordsInputBox.InputText, ",").ToList();
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        #endregion

        #region OnClick operation

        private void SkipTweetsContainingSpecificWordsInputBox_OnGetInputClick(object sender, RoutedEventArgs e)
        {
            try
            {
                TweetFilter.SkipTweetsContainingWords = SkipTweetsContainingSpecificWordsInputBox.InputText;
                TweetFilter.LstSkipTweetsContainingWords =
                    Regex.Split(SkipTweetsContainingSpecificWordsInputBox.InputText, "\r\n").ToList();
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        #endregion

        #region commands

        public ICommand SpecificWordCommand { get; set; }
        public ICommand SaveInvalidWordCommand { get; set; }

        #endregion


        #region Setting Dependency Property

        public TweetFilterModel TweetFilter
        {
            get => (TweetFilterModel) GetValue(TweetFilterProperty);
            set => SetValue(TweetFilterProperty, value);
        }

        // Using a DependencyProperty as the backing store for TweetFilter.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TweetFilterProperty =
            DependencyProperty.Register("TweetFilter", typeof(TweetFilterModel), typeof(TweetFiltersControl),
                new FrameworkPropertyMetadata(OnAvailableItemsChanged)
                {
                    BindsTwoWayByDefault = true
                });


        public static void OnAvailableItemsChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var newValue = e.NewValue;
        }

        #endregion
    }
}