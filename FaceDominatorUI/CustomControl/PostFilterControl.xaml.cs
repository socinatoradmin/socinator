using FaceDominatorCore.FDModel.FilterModel;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;

namespace FaceDominatorUI.CustomControl
{
    /// <summary>
    ///     Interaction logic for PostFilterControl.xaml
    /// </summary>
    public partial class PostFilterControl
    {
        // Using a DependencyProperty as the backing store for PostFilter.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PostFilterProperty =
            DependencyProperty.Register("PostFilter", typeof(PostFilterModel), typeof(PostFilterControl),
                new FrameworkPropertyMetadata(OnAvailableItemsChanged)
                {
                    BindsTwoWayByDefault = true
                });

        // Using a DependencyProperty as the backing store for PostFilter.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsIgnoreWithoutMediaEnabledProperty =
            DependencyProperty.Register("IsIgnoreWithoutMediaEnabled", typeof(bool), typeof(PostFilterControl),
                new FrameworkPropertyMetadata(OnAvailableItemsChanged)
                {
                    BindsTwoWayByDefault = true
                });

        public PostFilterControl()
        {
            InitializeComponent();
            MainGrid.DataContext = this;
        }


        public PostFilterModel PostFilter
        {
            get => (PostFilterModel)GetValue(PostFilterProperty);
            set => SetValue(PostFilterProperty, value);
        }

        public bool IsIgnoreWithoutMediaEnabled
        {
            get => (bool)GetValue(IsIgnoreWithoutMediaEnabledProperty);
            set => SetValue(IsIgnoreWithoutMediaEnabledProperty, value);
        }


        public static void OnAvailableItemsChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
        }

        private void CaptionBlacklistInputBox_OnGetInputClick(object sender, RoutedEventArgs e)
        {
            PostFilter.CaptionBlacklists = CaptionBlacklistInputBox.InputText;
            PostFilter.RestrictedPostCaptionList =
                new ObservableCollection<string>(Regex.Split(CaptionBlacklistInputBox.InputText, "\r\n").ToList());
        }

        private void CaptionWhitelistInputBox_OnGetInputClick(object sender, RoutedEventArgs e)
        {
            PostFilter.CaptionBlacklists = CaptionBlacklistInputBox.InputText;
            PostFilter.RestrictedPostCaptionList =
                new ObservableCollection<string>(Regex.Split(CaptionBlacklistInputBox.InputText, "\r\n").ToList());
        }
    }
}