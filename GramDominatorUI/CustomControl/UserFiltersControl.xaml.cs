using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using DominatorHouseCore.Command;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Utility;
using GramDominatorCore.GDModel;

namespace GramDominatorUI.CustomControl
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

        // Using a DependencyProperty as the backing store for MustHaveEmailIdVisibility.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MustHaveEmailIdVisibilityProperty =
            DependencyProperty.Register("MustHaveEmailIdVisibility", typeof(Visibility), typeof(Visibility));

        // Using a DependencyProperty as the backing store for MustHaveBusinessAccountVisibility.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MustHaveBusinessAccountVisibilityProperty =
            DependencyProperty.Register("LangKeyMustHaveBusinessAccount", typeof(Visibility), typeof(Visibility));

        // Using a DependencyProperty as the backing store for MustHaveContactVisibility.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MustHaveContactVisibilityProperty =
            DependencyProperty.Register("MustHaveContactVisibility", typeof(Visibility), typeof(Visibility));

        public static readonly DependencyProperty ShouldHavePostedOnRecentDaysVisibilityProperty =
            DependencyProperty.Register("ShouldHavePostedOnRecentDaysVisibility", typeof(Visibility),
                typeof(UserFiltersControl));

        public static readonly DependencyProperty ShouldNotHavePostedOnRecentDaysVisibilityProperty =
            DependencyProperty.Register("ShouldNotHavePostedOnRecentDaysVisibility", typeof(Visibility),
                typeof(UserFiltersControl), new PropertyMetadata(Visibility.Collapsed));

        private static readonly RoutedEvent SaveUserFilterEvent =
            EventManager.RegisterRoutedEvent("SaveUserFilterEventHandler", RoutingStrategy.Bubble,
                typeof(RoutedEventHandler), typeof(UserFiltersControl));

        public UserFiltersControl()
        {
            InitializeComponent();
            if (UserFilter == null)
                UserFilter = new UserFilterModel();
            MainGrid.DataContext = this;
            MustHaveEmailIdVisibility = Visibility.Collapsed;
            MustHaveContactVisibility = Visibility.Collapsed;
            MustHaveBusinessAccountVisibility = Visibility.Collapsed;
            CaptionPostInputBoxCommand = new BaseCommand<object>(sender => true, CaptionPostInputBoxs);
            InvalidWordsInputBoxCommand = new BaseCommand<object>(sender => true, InvalidWordsInputBoxs);
            validWordsInputBoxCommand = new BaseCommand<object>(sender => true, validWordsInputBoxs);
        }

        public ICommand CaptionPostInputBoxCommand { get; set; }

        public ICommand InvalidWordsInputBoxCommand { get; set; }
        public ICommand validWordsInputBoxCommand { get; set; }

        public UserFilterModel UserFilter
        {
            get => (UserFilterModel) GetValue(UserFilterProperty);
            set => SetValue(UserFilterProperty, value);
        }


        public Visibility MustHaveEmailIdVisibility
        {
            get => (Visibility) GetValue(MustHaveEmailIdVisibilityProperty);
            set => SetValue(MustHaveEmailIdVisibilityProperty, value);
        }

        public Visibility MustHaveBusinessAccountVisibility
        {
            get => (Visibility) GetValue(MustHaveBusinessAccountVisibilityProperty);
            set => SetValue(MustHaveBusinessAccountVisibilityProperty, value);
        }


        public Visibility MustHaveContactVisibility
        {
            get => (Visibility) GetValue(MustHaveContactVisibilityProperty);
            set => SetValue(MustHaveContactVisibilityProperty, value);
        }


        public Visibility ShouldHavePostedOnRecentDaysVisibility
        {
            get => (Visibility) GetValue(ShouldHavePostedOnRecentDaysVisibilityProperty);
            set => SetValue(ShouldHavePostedOnRecentDaysVisibilityProperty, value);
        }

        public Visibility ShouldNotHavePostedOnRecentDaysVisibility
        {
            get => (Visibility) GetValue(ShouldNotHavePostedOnRecentDaysVisibilityProperty);
            set => SetValue(ShouldNotHavePostedOnRecentDaysVisibilityProperty, value);
        }

        public static void OnAvailableItemsChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var newValue = e.NewValue;
        }

        public static void ShouldNotHavePostedOnRecentDaysVisibilityChanged(DependencyObject obj,
            DependencyPropertyChangedEventArgs e)
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
            GlobusLogHelper.log.Info("Must have atleast one post => query has been saved");
        }

        private void CaptionPostInputBoxs(object sender)
        {
            UserFilter.LstPostCaption = Regex.Split(CaptionPostInputBox.InputText, "\r\n").ToList();
            UserFilter.CaptionPosts = CaptionPostInputBox.InputText;
            GlobusLogHelper.log.Info("Must have atleast one post => query has been saved");
        }

        private void InvalidWordsInputBoxs(object sender)
        {
            UserFilter.LstInvalidWord = Regex.Split(InvalidWordsInputBox.InputText, "\r\n").ToList();
            UserFilter.InvalidWords = InvalidWordsInputBox.InputText;
            GlobusLogHelper.log.Info("Must not contain => query has been saved");
        }

        private void validWordsInputBoxs(object sender)
        {
            UserFilter.LstvalidWord = Regex.Split(validWordsInputBox.InputText, "\r\n").ToList();
            UserFilter.ValidWords = validWordsInputBox.InputText;
            GlobusLogHelper.log.Info("Must contain => query has been saved");
        }
    }
}