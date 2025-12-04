using System.Windows;
using System.Windows.Input;
using DominatorUIUtility.Behaviours;

namespace DominatorUIUtility.CustomControl
{
    /// <summary>
    ///     Interaction logic for HeaderControl.xaml
    /// </summary>
    public partial class HeaderControl
    {
        public static readonly DependencyProperty IsCancelEditVisibleProperty =
            DependencyProperty.Register("IsCancelEditVisible", typeof(Visibility), typeof(HeaderControl),
                new FrameworkPropertyMetadata
                {
                    BindsTwoWayByDefault = true
                });

        public static readonly DependencyProperty CampaignNameProperty =
            DependencyProperty.Register("CampaignName", typeof(string), typeof(HeaderControl),
                new FrameworkPropertyMetadata
                {
                    BindsTwoWayByDefault = true
                });

        public static readonly DependencyProperty IsCampaignNameEditableProperty =
            DependencyProperty.Register("IsCampaignNameEditable", typeof(bool), typeof(HeaderControl),
                new FrameworkPropertyMetadata
                {
                    BindsTwoWayByDefault = true
                });


        private static readonly RoutedEvent InfoRoutedEvent = EventManager.RegisterRoutedEvent("InfoChanged",
            RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(HeaderControl));

        private static readonly RoutedEvent CancelEditRoutedEvent = EventManager.RegisterRoutedEvent("CancelEditClick",
            RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(HeaderControl));

        // Using a DependencyProperty as the backing store for IsExpanded.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsExpandedProperty =
            DependencyProperty.Register("IsExpanded", typeof(bool), typeof(HeaderControl), new FrameworkPropertyMetadata
            {
                BindsTwoWayByDefault = true
            });

        // Using a DependencyProperty as the backing store for CancelEditCommand.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CancelEditCommandProperty =
            DependencyProperty.Register("CancelEditCommand", typeof(ICommand), typeof(HeaderControl));

        // Using a DependencyProperty as the backing store for InfoCommand.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty InfoCommandProperty =
            DependencyProperty.Register("InfoCommand", typeof(ICommand), typeof(HeaderControl));

        public HeaderControl()
        {
            InitializeComponent();
            mainGrid.DataContext = this;
            IsExpanded = true;
        }


        public Visibility IsCancelEditVisible
        {
            get => (Visibility) GetValue(IsCancelEditVisibleProperty);
            set => SetValue(IsCancelEditVisibleProperty, value);
        }


        public string CampaignName
        {
            get => (string) GetValue(CampaignNameProperty);
            set => SetValue(CampaignNameProperty, value);
        }


        public bool IsCampaignNameEditable
        {
            get => (bool) GetValue(IsCampaignNameEditableProperty);
            set => SetValue(IsCampaignNameEditableProperty, value);
        }

        public bool IsExpanded
        {
            get => (bool) GetValue(IsExpandedProperty);
            set => SetValue(IsExpandedProperty, value);
        }


        public ICommand CancelEditCommand
        {
            get => (ICommand) GetValue(CancelEditCommandProperty);
            set => SetValue(CancelEditCommandProperty, value);
        }


        public ICommand InfoCommand
        {
            get => (ICommand) GetValue(InfoCommandProperty);
            set => SetValue(InfoCommandProperty, value);
        }

        public event RoutedEventHandler InfoChanged
        {
            add => AddHandler(InfoRoutedEvent, value);
            remove => RemoveHandler(InfoRoutedEvent, value);
        }

        public event RoutedEventHandler CancelEditClick
        {
            add => AddHandler(CancelEditRoutedEvent, value);
            remove => RemoveHandler(CancelEditRoutedEvent, value);
        }

        private void ClpsExpnd_OnClick(object sender, RoutedEventArgs e)
        {
            HeaderHelper.ExpandCollapseAllExpander(sender, IsExpanded);
        }
    }
}