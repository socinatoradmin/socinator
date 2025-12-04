using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace DominatorUIUtility.CustomControl
{
    /// <summary>
    ///     Interaction logic for FooterControl.xaml
    /// </summary>
    public partial class FooterControl
    {
        public static readonly DependencyProperty list_SelectedAccountsProperty =
            DependencyProperty.Register("list_SelectedAccounts", typeof(List<string>), typeof(FooterControl),
                new FrameworkPropertyMetadata
                {
                    BindsTwoWayByDefault = true
                });

        public static readonly DependencyProperty NoOfAccountsSelectedProperty =
            DependencyProperty.Register("NoOfAccountsSelected", typeof(string), typeof(FooterControl),
                new FrameworkPropertyMetadata
                {
                    BindsTwoWayByDefault = true
                });

        public static readonly DependencyProperty AccountsSelectedColorProperty =
            DependencyProperty.Register("AccountsSelectedColor", typeof(Brush), typeof(FooterControl),
                new FrameworkPropertyMetadata
                {
                    BindsTwoWayByDefault = true
                });

        public static readonly DependencyProperty IsStatCampaignNowVisibleProperty =
            DependencyProperty.Register("IsStatCampaignNowVisible", typeof(Visibility), typeof(FooterControl),
                new FrameworkPropertyMetadata
                {
                    BindsTwoWayByDefault = true
                });

        public static readonly DependencyProperty CampaignManagerProperty =
            DependencyProperty.Register("CampaignManager", typeof(string), typeof(FooterControl),
                new FrameworkPropertyMetadata
                {
                    BindsTwoWayByDefault = true
                });

        /// <summary>
        ///     Select accounts event registeration
        /// </summary>
        private static readonly RoutedEvent SelectAccountChangedRoutedEvent =
            EventManager.RegisterRoutedEvent("SelectAccountChanged", RoutingStrategy.Bubble, typeof(RoutedEventHandler),
                typeof(FooterControl));

        /// <summary>
        ///     Create campaign event registeration
        /// </summary>
        private static readonly RoutedEvent CreateCampaignChangedRoutedEvent =
            EventManager.RegisterRoutedEvent("CreateCampaignChanged", RoutingStrategy.Bubble,
                typeof(RoutedEventHandler), typeof(FooterControl));

        /// <summary>
        ///     Update campaign event registeration
        /// </summary>
        private static readonly RoutedEvent UpdateCampaignChangedRoutedEvent =
            EventManager.RegisterRoutedEvent("UpdateCampaignChanged", RoutingStrategy.Bubble,
                typeof(RoutedEventHandler), typeof(FooterControl));

        // Using a DependencyProperty as the backing store for CreateCampaignCommand.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CreateCampaignCommandProperty =
            DependencyProperty.Register("CreateCampaignCommand", typeof(ICommand), typeof(FooterControl));

        // Using a DependencyProperty as the backing store for SelectAccountCommand.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SelectAccountCommandProperty =
            DependencyProperty.Register("SelectAccountCommand", typeof(ICommand), typeof(FooterControl));

        public static readonly DependencyProperty SelectAccountVisibilityProperty =
            DependencyProperty.Register("SelectAccountVisibility", typeof(bool), typeof(FooterControl),
                new FrameworkPropertyMetadata(true)
                {
                    BindsTwoWayByDefault = true
                });

        public FooterControl()
        {
            InitializeComponent();
            mainGrid.DataContext = this;
            list_SelectedAccounts = new List<string>();
        }

        public List<string> list_SelectedAccounts
        {
            get => (List<string>) GetValue(list_SelectedAccountsProperty);
            set => SetValue(list_SelectedAccountsProperty, value);
        }


        public string NoOfAccountsSelected
        {
            get => (string) GetValue(NoOfAccountsSelectedProperty);
            set => SetValue(NoOfAccountsSelectedProperty, value);
        }


        public Brush AccountsSelectedColor
        {
            get => (Brush) GetValue(AccountsSelectedColorProperty);
            set => SetValue(AccountsSelectedColorProperty, value);
        }


        public Visibility IsStatCampaignNowVisible
        {
            get => (Visibility) GetValue(IsStatCampaignNowVisibleProperty);
            set => SetValue(IsStatCampaignNowVisibleProperty, value);
        }

        public string CampaignManager
        {
            get => (string) GetValue(CampaignManagerProperty);
            set => SetValue(CampaignManagerProperty, value);
        }

        public ICommand CreateCampaignCommand
        {
            get => (ICommand) GetValue(CreateCampaignCommandProperty);
            set => SetValue(CreateCampaignCommandProperty, value);
        }


        public ICommand SelectAccountCommand
        {
            get => (ICommand) GetValue(SelectAccountCommandProperty);
            set => SetValue(SelectAccountCommandProperty, value);
        }

        public bool SelectAccountVisibility
        {
            get => (bool) GetValue(SelectAccountVisibilityProperty);
            set => SetValue(SelectAccountVisibilityProperty, value);
        }

        public event RoutedEventHandler SelectAccountChanged
        {
            add => AddHandler(SelectAccountChangedRoutedEvent, value);
            remove => RemoveHandler(SelectAccountChangedRoutedEvent, value);
        }

        public event RoutedEventHandler CreateCampaignChanged
        {
            add => AddHandler(CreateCampaignChangedRoutedEvent, value);
            remove => RemoveHandler(CreateCampaignChangedRoutedEvent, value);
        }


        public event RoutedEventHandler UpdateCampaignChanged
        {
            add => AddHandler(UpdateCampaignChangedRoutedEvent, value);
            remove => RemoveHandler(UpdateCampaignChangedRoutedEvent, value);
        }
    }
}