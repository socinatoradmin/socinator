using System.Windows;
using DominatorHouseCore.Models.FacebookModels;

namespace DominatorUIUtility.CustomControl.FacebookCustomControl
{
    /// <summary>
    ///     Interaction logic for InviterOptionsControl.xaml
    /// </summary>
    public partial class InviterOptionsControl
    {
        // Using a DependencyProperty as the backing store for PostFilter.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty InviterOptionsModelProperty =
            DependencyProperty.Register("InviterOptionsModel", typeof(InviterOptions), typeof(InviterOptionsControl),
                new FrameworkPropertyMetadata(OnAvailableItemsChanged)
                {
                    BindsTwoWayByDefault = true
                });

        // Using a DependencyProperty as the backing store for PostFilter.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsReinviteOptionNeededProperty =
            DependencyProperty.Register("IsReinviteOptionNeeded", typeof(bool), typeof(InviterOptionsControl),
                new FrameworkPropertyMetadata(OnAvailableItemsChanged)
                {
                    BindsTwoWayByDefault = true
                });

        // Using a DependencyProperty as the backing store for PostFilter.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsInviteInMessageVisibleProperty =
            DependencyProperty.Register("IsInviteInMessageVisible", typeof(bool), typeof(InviterOptionsControl),
                new FrameworkPropertyMetadata(OnAvailableItemsChanged)
                {
                    BindsTwoWayByDefault = true
                });

        public InviterOptionsControl()
        {
            InitializeComponent();
            MainGrid.DataContext = this;
        }

        public InviterOptions InviterOptionsModel
        {
            get => (InviterOptions) GetValue(InviterOptionsModelProperty);
            set => SetValue(InviterOptionsModelProperty, value);
        }

        public bool IsReinviteOptionNeeded
        {
            get => (bool) GetValue(IsReinviteOptionNeededProperty);
            set => SetValue(IsReinviteOptionNeededProperty, value);
        }

        public bool IsInviteInMessageVisible
        {
            get => (bool) GetValue(IsInviteInMessageVisibleProperty);
            set => SetValue(IsInviteInMessageVisibleProperty, value);
        }


        public static void OnAvailableItemsChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
        }
    }
}