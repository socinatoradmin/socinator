using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using DominatorHouseCore.Command;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Utility;
using PinDominatorCore.PDModel;

namespace PinDominator.CustomControl
{
    /// <summary>
    ///     Interaction logic for UserFiltersControl.xaml
    /// </summary>
    public partial class UserFiltersControl : UserControl
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
            UserFilter = new UserFilterModel {SaveCloseButtonVisible = true};
            SaveInvalidWordsInputBoxCommand = new BaseCommand<object>(sender => true, SaveInvalidWordsInputBox);
            FilterMinimumFollowRatioCheckedCommand =
                new BaseCommand<object>(sender => true, FilterMinimumFollowRatioChecked);
            FilterMaximumFollowRatioCheckedCommand =
                new BaseCommand<object>(sender => true, FilterMaximumFollowRatioChecked);
            FilterSpecificFollowRatioCheckedCommand =
                new BaseCommand<object>(sender => true, FilterSpecificFollowRatioChecked);
            MainGrid.DataContext = this;
        }

        public ICommand SaveInvalidWordsInputBoxCommand { get; set; }

        public ICommand FilterMinimumFollowRatioCheckedCommand { get; set; }

        public ICommand FilterMaximumFollowRatioCheckedCommand { get; set; }

        public ICommand FilterSpecificFollowRatioCheckedCommand { get; set; }

        public UserFilterModel UserFilter
        {
            get => (UserFilterModel) GetValue(UserFilterProperty);
            set => SetValue(UserFilterProperty, value);
        }

        public void SaveInvalidWordsInputBox(object sender)
        {
            UserFilter.LstInvalidWord = Regex.Split(InvalidWordsInputBox.InputText, "\r\n").ToList();
            GlobusLogHelper.log.Info("Specific word details saved successfully");
        }

        public void FilterMinimumFollowRatioChecked(object sender)
        {
            try
            {
                UserFilter.FilterSpecificFollowRatio = false;
                UserFilter.FilterMaximumFollowRatio = false;
            }
            catch (Exception)
            {
                // ignored
            }
        }

        public void FilterMaximumFollowRatioChecked(object sender)
        {
            try
            {
                UserFilter.FilterSpecificFollowRatio = false;
                UserFilter.FilterMinimumFollowRatio = false;
            }
            catch (Exception)
            {
                // ignored
            }
        }

        public void FilterSpecificFollowRatioChecked(object sender)
        {
            try
            {
                UserFilter.FilterMinimumFollowRatio = false;
                UserFilter.FilterMaximumFollowRatio = false;
            }
            catch (Exception)
            {
                // ignored
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


        private void BtnImportBlacklistsText_OnClick(object sender, RoutedEventArgs e)
        {
            UserFilter.LstInvalidWord = FileUtilities.FileBrowseAndReader();
        }

        private void BtnImportCaptionOnPostText_OnClick(object sender, RoutedEventArgs e)
        {
            UserFilter.LstPostCaption = FileUtilities.FileBrowseAndReader();
        }

        //private void CaptionPostInputBox_OnGetInputClick(object sender, RoutedEventArgs e)
        //{
        //    UserFilter.LstPostCaption = Regex.Split(CaptionPostInputBox.InputText, "\r\n").ToList();

        //}

        private void InvalidWordsInputBox_OnGetInputClick(object sender, RoutedEventArgs e)
        {
            UserFilter.LstInvalidWord = Regex.Split(InvalidWordsInputBox.InputText, "\r\n").ToList();
            GlobusLogHelper.log.Info("Specific word details saved successfully");
        }
    }
}