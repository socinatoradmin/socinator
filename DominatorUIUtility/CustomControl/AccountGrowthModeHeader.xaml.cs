using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using DominatorHouseCore.Command;
using DominatorHouseCore.Utility;
using DominatorUIUtility.Behaviours;
using Prism.Commands;

namespace DominatorUIUtility.CustomControl
{
    /// <summary>
    ///     Interaction logic for AccountGrowthModeHeader.xaml
    /// </summary>
    public partial class AccountGrowthModeHeader
    {
        // Using a DependencyProperty as the backing store for AccountItemSource.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty AccountItemSourceProperty =
            DependencyProperty.Register("AccountItemSource", typeof(ObservableCollectionBase<string>),
                typeof(AccountGrowthModeHeader), new FrameworkPropertyMetadata(OnAvailableItemsChanged)
                {
                    BindsTwoWayByDefault = true
                });

        public static readonly DependencyProperty SelectedItemProperty =
            DependencyProperty.Register("SelectedItem", typeof(string), typeof(AccountGrowthModeHeader),
                new FrameworkPropertyMetadata(OnAvailableItemsChanged)
                {
                    BindsTwoWayByDefault = true
                });

        public static readonly DependencyProperty SelectedIndexProperty =
            DependencyProperty.Register("SelectedIndex", typeof(int), typeof(AccountGrowthModeHeader),
                new FrameworkPropertyMetadata(OnAvailableItemsChanged)
                {
                    BindsTwoWayByDefault = true
                });

        private static readonly RoutedEvent SelectionChangedRoutedEvent =
            EventManager.RegisterRoutedEvent("SelectionChangedEvent", RoutingStrategy.Bubble,
                typeof(RoutedEventHandler), typeof(AccountGrowthModeHeader));

        private static readonly RoutedEvent SaveEvent =
            EventManager.RegisterRoutedEvent("SaveClick", RoutingStrategy.Bubble,
                typeof(RoutedEventHandler), typeof(AccountGrowthModeHeader));

        // Using a DependencyProperty as the backing store for IsExpanded.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsExpandedProperty =
            DependencyProperty.Register("IsExpanded", typeof(bool), typeof(AccountGrowthModeHeader),
                new FrameworkPropertyMetadata(OnAvailableItemsChanged)
                {
                    BindsTwoWayByDefault = true
                });

        private static readonly DependencyProperty SaveCommandProperty
            = DependencyProperty.Register("SaveCommand", typeof(ICommand), typeof(AccountGrowthModeHeader));

        // Using a DependencyProperty as the backing store for SelectionChangedCommand.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SelectionChangedCommandProperty =
            DependencyProperty.Register("SelectionChangedCommand", typeof(ICommand), typeof(AccountGrowthModeHeader));

        // Using a DependencyProperty as the backing store for CommandParameter.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty VideoTutorialLinkProperty =
            DependencyProperty.Register("VideoTutorialLink", typeof(string), typeof(AccountGrowthModeHeader),
                new PropertyMetadata(string.Empty));

        // Using a DependencyProperty as the backing store for TutorialCommand.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TutorialCommandProperty =
            DependencyProperty.Register("TutorialCommand", typeof(ICommand), typeof(AccountGrowthModeHeader));

        public AccountGrowthModeHeader()
        {
            InitializeComponent();
            mainGrid.DataContext = this;
            SaveCommand = new BaseCommand<object>(CanExecute, Execute);
            TutorialCommand = new DelegateCommand<string>(TutorialExecute);
            IsExpanded = true;
            AccountItemSource = new ObservableCollectionBase<string>();
        }

        public ObservableCollectionBase<string> AccountItemSource
        {
            get => (ObservableCollectionBase<string>) GetValue(AccountItemSourceProperty);
            set => SetValue(AccountItemSourceProperty, value);
        }

        public string SelectedItem
        {
            get => (string) GetValue(SelectedItemProperty);
            set => SetValue(SelectedItemProperty, value);
        }

        public int SelectedIndex
        {
            get => (int) GetValue(SelectedIndexProperty);
            set => SetValue(SelectedIndexProperty, value);
        }

        public bool IsExpanded
        {
            get => (bool) GetValue(IsExpandedProperty);
            set => SetValue(IsExpandedProperty, value);
        }

        public ICommand SaveCommand
        {
            get => (ICommand) GetValue(SaveCommandProperty);
            set => SetValue(SaveCommandProperty, value);
        }


        public ICommand SelectionChangedCommand
        {
            get => (ICommand) GetValue(SelectionChangedCommandProperty);
            set => SetValue(SelectionChangedCommandProperty, value);
        }

        public string VideoTutorialLink
        {
            get => (string) GetValue(VideoTutorialLinkProperty);
            set => SetValue(VideoTutorialLinkProperty, value);
        }

        public ICommand TutorialCommand
        {
            get => (ICommand) GetValue(TutorialCommandProperty);
            set => SetValue(TutorialCommandProperty, value);
        }

        public static void OnAvailableItemsChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            // ReSharper disable once UnusedVariable
            var newValue = e.NewValue;
        }

        public event RoutedEventHandler SelectionChangedEvent
        {
            add => AddHandler(SelectionChangedRoutedEvent, value);
            remove => RemoveHandler(SelectionChangedRoutedEvent, value);
        }

        private void SelectionChangedEventHandler()
        {
            var objRoutedEventArgs = new RoutedEventArgs(SelectionChangedRoutedEvent);
            RaiseEvent(objRoutedEventArgs);
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SelectionChangedEventHandler();
        }

        public event RoutedEventHandler SaveClick
        {
            add => AddHandler(SaveEvent, value);
            remove => RemoveHandler(SaveEvent, value);
        }

        private void SaveEventArgsHandler()
        {
            var rountedargs = new RoutedEventArgs(SaveEvent);
            RaiseEvent(rountedargs);
        }

        private void ClpsExpnd_OnClick(object sender, RoutedEventArgs e)
        {
            HeaderHelper.ExpandCollapseAllExpander(sender, IsExpanded);
        }

        public bool CanExecute(object sender)
        {
            return true;
        }

        public void Execute(object sender)
        {
            SaveEventArgsHandler();
        }

        private void TutorialExecute(string tutorialLink)
        {
            Process.Start(tutorialLink);
        }
    }
}