using System.Collections.ObjectModel;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using DominatorHouseCore.Command;
using GramDominatorCore.GDModel;

namespace GramDominatorUI.CustomControl
{
    /// <summary>
    ///     Interaction logic for PostFilterControl.xaml
    /// </summary>
    public partial class PostFilterControl : UserControl
    {
        // Using a DependencyProperty as the backing store for PostFilter.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PostFilterProperty =
            DependencyProperty.Register("PostFilter", typeof(PostFilterModel), typeof(PostFilterControl),
                new PropertyMetadata(OnAvailableItemsChanged));

        public static readonly DependencyProperty PostTypeVisibilityProperty =
            DependencyProperty.Register("PostTypeVisibility", typeof(Visibility), typeof(PostFilterControl));

        public PostFilterControl()
        {
            InitializeComponent();
            MainGrid.DataContext = this;
            CaptionBlacklistInputBoxs = new BaseCommand<object>(sender => true, CaptionPostInput);
            CaptionWhitelistInputBoxs = new BaseCommand<object>(sender => true, CaptionWhitelist);
        }

        public ICommand CaptionBlacklistInputBoxs { get; set; }
        public ICommand CaptionWhitelistInputBoxs { get; set; }

        public PostFilterModel PostFilter
        {
            get => (PostFilterModel) GetValue(PostFilterProperty);
            set => SetValue(PostFilterProperty, value);
        }


        public Visibility PostTypeVisibility
        {
            get => (Visibility) GetValue(PostTypeVisibilityProperty);
            set => SetValue(PostTypeVisibilityProperty, value);
        }

        public void CaptionPostInput(object sender)
        {
            PostFilter.CaptionBlacklists = CaptionBlacklistInputBox.InputText;
            PostFilter.RestrictedPostCaptionList =
                new ObservableCollection<string>(Regex.Split(CaptionBlacklistInputBox.InputText, "\r\n").ToList());
        }

        public void CaptionWhitelist(object sender)
        {
            PostFilter.CaptionWhitelist = CaptionWhitelistInputBox.InputText;
            PostFilter.AcceptedPostCaptionList =
                new ObservableCollection<string>(Regex.Split(CaptionWhitelistInputBox.InputText, "\r\n").ToList());
        }


        public static void OnAvailableItemsChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var newValue = e.NewValue;
        }


        private void CaptionBlacklistInputBox_OnGetInputClick(object sender, RoutedEventArgs e)
        {
            PostFilter.CaptionBlacklists = CaptionBlacklistInputBox.InputText;
            PostFilter.RestrictedPostCaptionList =
                new ObservableCollection<string>(Regex.Split(CaptionBlacklistInputBox.InputText, "\r\n").ToList());
        }

        private void CaptionWhitelistInputBox_OnGetInputClick(object sender, RoutedEventArgs e)
        {
            PostFilter.CaptionWhitelist = CaptionWhitelistInputBox.InputText;
            PostFilter.AcceptedPostCaptionList =
                new ObservableCollection<string>(Regex.Split(CaptionWhitelistInputBox.InputText, "\r\n").ToList());
        }
    }
}