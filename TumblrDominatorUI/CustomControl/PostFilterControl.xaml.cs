using DominatorHouseCore.Annotations;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using TumblrDominatorCore.Models;
using TumblrDominatorCore.ViewModels.CustomControls;

namespace TumblrDominatorUI.CustomControl
{
    /// <summary>
    ///     Interaction logic for PostFilterControl.xaml
    /// </summary>
    public partial class PostFilterControl : INotifyPropertyChanged
    {
        // Using a DependencyProperty as the backing store for PostFilter.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PostFilterProperty =
            DependencyProperty.Register("PostFilter", typeof(PostFilterModel), typeof(PostFilterControl),
                new FrameworkPropertyMetadata(OnAvailableItemsChanged)
                {
                    BindsTwoWayByDefault = true
                });
        public static readonly DependencyProperty VisibilityPropertyValue =
            DependencyProperty.Register("Visibility", typeof(Visibility), typeof(PostFilterControl),
                new PropertyMetadata(Visibility.Visible));
        public static readonly DependencyProperty SearchFilterProperty =
            DependencyProperty.Register("SearchFilter", typeof(SearchFilterModel), typeof(PostFilterControl),
                new FrameworkPropertyMetadata(OnAvailableItemsChanged)
                {
                    BindsTwoWayByDefault = true
                });
        private PostFilterViewmodel _postFilterViewmodel = new PostFilterViewmodel();

        public PostFilterControl()
        {
            InitializeComponent();
            MainGrid.DataContext = this;
        }
        public Visibility IsVisibleFilter
        {
            get => (Visibility)GetValue(VisibilityPropertyValue);
            set => SetValue(VisibilityPropertyValue, value);
        }
        public PostFilterModel PostFilter
        {
            get => (PostFilterModel)GetValue(PostFilterProperty);
            set => SetValue(PostFilterProperty, value);
        }
        public SearchFilterModel SearchFilter
        {
            get => (SearchFilterModel)GetValue(SearchFilterProperty);
            set => SetValue(SearchFilterProperty, value);
        }
        public PostFilterViewmodel PostFilterViewmodel
        {
            get => _postFilterViewmodel;

            set
            {
                _postFilterViewmodel = value;
                OnPropertyChanged(nameof(PostFilterViewmodel));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


        public static void OnAvailableItemsChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
        }
    }
}