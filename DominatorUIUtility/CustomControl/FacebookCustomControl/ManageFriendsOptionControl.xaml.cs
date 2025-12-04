using System.Windows;
using DominatorHouseCore.Models.FacebookModels;

namespace DominatorUIUtility.CustomControl.FacebookCustomControl
{
    /// <summary>
    ///     Interaction logic for ManageFriendsOptionControl.xaml
    /// </summary>
    public partial class ManageFriendsOptionControl
    {
        // Using a DependencyProperty as the backing store for PostFilter.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ManageFriendsProperty =
            DependencyProperty.Register("ManageFriendsModel", typeof(ManageFriends), typeof(ManageFriendsOptionControl),
                new FrameworkPropertyMetadata(OnAvailableItemsChanged)
                {
                    BindsTwoWayByDefault = true
                });

        public ManageFriendsOptionControl()
        {
            InitializeComponent();
            MainGrid.DataContext = this;
        }

        public ManageFriends ManageFriendsModel
        {
            get => (ManageFriends) GetValue(ManageFriendsProperty);
            set => SetValue(ManageFriendsProperty, value);
        }

        public static void OnAvailableItemsChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
        }

        private void Postoptions_Checked(object sender, RoutedEventArgs e)
        {
            if (ManageFriendsModel.Count < 2)
                ManageFriendsModel.Count++;
        }

        private void Postoptions_Unchecked(object sender, RoutedEventArgs e)
        {
            if (ManageFriendsModel.Count > 0)
                ManageFriendsModel.Count--;
        }
    }
}