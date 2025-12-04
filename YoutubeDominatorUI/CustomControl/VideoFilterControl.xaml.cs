using DominatorHouseCore.Utility;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using YoutubeDominatorCore.YDEnums.VideoSearchFilterEnums;
using YoutubeDominatorCore.YoutubeModels;
using Duration = YoutubeDominatorCore.YDEnums.VideoSearchFilterEnums.Duration;

namespace YoutubeDominatorUI.CustomControl
{
    public partial class VideoFilterControl : UserControl
    {
        public static readonly DependencyProperty VideoFilterProperty =
            DependencyProperty.Register("VideoFilter", typeof(VideoFilterModel), typeof(VideoFilterControl),
                new FrameworkPropertyMetadata
                {
                    BindsTwoWayByDefault = true
                });

        // Using a DependencyProperty as the backing store for ListQueryType.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ListUploadDateFiltersProperty =
            DependencyProperty.Register("ListUploadDateFilters", typeof(ObservableCollection<string>),
                typeof(VideoFilterControl), new FrameworkPropertyMetadata
                {
                    BindsTwoWayByDefault = true
                });

        // Using a DependencyProperty as the backing store for ListQueryType.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ListDurationFiltersProperty =
            DependencyProperty.Register("ListDurationFilters", typeof(ObservableCollection<string>),
                typeof(VideoFilterControl), new FrameworkPropertyMetadata
                {
                    BindsTwoWayByDefault = true
                });

        // Using a DependencyProperty as the backing store for ListQueryType.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ListSortByFiltersProperty =
            DependencyProperty.Register("ListSortByFilters", typeof(ObservableCollection<string>),
                typeof(VideoFilterControl), new FrameworkPropertyMetadata
                {
                    BindsTwoWayByDefault = true
                });

        public VideoFilterControl()
        {
            InitializeComponent();
            VideoFilter = new VideoFilterModel();
            MainGrid.DataContext = this;
        }

        public VideoFilterModel VideoFilter
        {
            get => (VideoFilterModel)GetValue(VideoFilterProperty);
            set => SetValue(VideoFilterProperty, value);
        }

        public ObservableCollection<string> ListUploadDateFilters
        {
            get => (ObservableCollection<string>)GetValue(ListUploadDateFiltersProperty);
            set => SetValue(ListUploadDateFiltersProperty, value);
        }

        public ObservableCollection<string> ListDurationFilters
        {
            get => (ObservableCollection<string>)GetValue(ListDurationFiltersProperty);
            set => SetValue(ListDurationFiltersProperty, value);
        }

        public ObservableCollection<string> ListSortByFilters
        {
            get => (ObservableCollection<string>)GetValue(ListSortByFiltersProperty);
            set => SetValue(ListSortByFiltersProperty, value);
        }

        private void CaptionTitleShouldContainsWordPhrase_OnGetInputClick(object sender, RoutedEventArgs e)
        {
        }

        private void CaptionDescriptionShouldContainsWordPhrase_OnGetInputClick(object sender, RoutedEventArgs e)
        {
        }

        private void CaptionTitleShouldNotContainsWordPhrase_OnGetInputClick(object sender, RoutedEventArgs e)
        {
        }

        private void CaptionDescriptionShouldNotContainsWordPhrase_OnGetInputClick(object sender, RoutedEventArgs e)
        {
        }

        private void UploadDate_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (UploadDateComboBox.SelectedIndex == -1)
                    return;
                VideoFilter.SearchVideoFilterModel.UploadDate =
                    Enum.GetNames(typeof(UploadDate)).ToList()[UploadDateComboBox.SelectedIndex];
            }
            catch (Exception)
            {
                // ignored
            }
        }

        private void DurationComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (DurationComboBox.SelectedIndex == -1)
                    return;
                VideoFilter.SearchVideoFilterModel.Duration =
                    Enum.GetNames(typeof(Duration)).ToList()[DurationComboBox.SelectedIndex];
            }
            catch (Exception)
            {
                // ignored
            }
        }

        private void SortByComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (SortByComboBox.SelectedIndex == -1)
                    return;
                VideoFilter.SearchVideoFilterModel.SortBy =
                    Enum.GetNames(typeof(SortBy)).ToList()[SortByComboBox.SelectedIndex];
            }
            catch (Exception)
            {
                // ignored
            }
        }

        private void FilterSearchingVideos_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            try
            {
                if ((sender as StackPanel).Visibility ==
                    Visibility.Visible /*(VideoFilter?.IsCheckedSearchVideoFilter ?? false)*/)
                {
                    if (VideoFilter == null)
                        VideoFilter = new VideoFilterModel();
                    if (VideoFilter.SearchVideoFilterModel == null)
                        VideoFilter.SearchVideoFilterModel = new SearchVideoFilterModel();

                    ListUploadDateFilters = new ObservableCollection<string>();
                    Enum.GetValues(typeof(UploadDate)).Cast<UploadDate>().ToList().ForEach(query =>
                    {
                        var queryDesc = Application.Current.FindResource(query.GetDescriptionAttr()).ToString();
                        ListUploadDateFilters.Add(queryDesc);
                        var queryName = query.ToString();
                        if (VideoFilter.SearchVideoFilterModel.UploadDate == queryName)
                            UploadDateComboBox.SelectedItem = queryDesc;
                    });

                    ListDurationFilters = new ObservableCollection<string>();
                    Enum.GetValues(typeof(Duration)).Cast<Duration>().ToList().ForEach(query =>
                    {
                        var queryDesc = Application.Current.FindResource(query.GetDescriptionAttr()).ToString();
                        ListDurationFilters.Add(queryDesc);
                        var queryName = query.ToString();
                        if (VideoFilter.SearchVideoFilterModel.Duration == queryName)
                            DurationComboBox.SelectedItem = queryDesc;
                    });

                    ListSortByFilters = new ObservableCollection<string>();
                    Enum.GetValues(typeof(SortBy)).Cast<SortBy>().ToList().ForEach(query =>
                    {
                        var queryDesc = Application.Current.FindResource(query.GetDescriptionAttr()).ToString();
                        ListSortByFilters.Add(queryDesc);
                        var queryName = query.ToString();
                        if (VideoFilter.SearchVideoFilterModel.SortBy == queryName)
                            SortByComboBox.SelectedItem = queryDesc;
                    });
                }
            }
            catch (Exception)
            {
                // ignored
            }
        }
    }
}