using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using DominatorHouseCore;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using DominatorUIUtility.Behaviours;
using Prism.Commands;

namespace DominatorUIUtility.Views.AccountSetting.CustomControl
{
    /// <summary>
    ///     Interaction logic for ActivitySetting.xaml
    /// </summary>
    public partial class ActivitySetting : UserControl
    {
        // Using a DependencyProperty as the backing store for Heading.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty HeadingProperty =
            DependencyProperty.Register("Heading", typeof(string), typeof(ActivitySetting),
                new PropertyMetadata(string.Empty));

        // Using a DependencyProperty as the backing store for HeaderNextPreViousVisiblity.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty HeaderNextPreViousVisiblityProperty =
            DependencyProperty.Register("HeaderNextPreViousVisiblity", typeof(Visibility), typeof(ActivitySetting),
                new PropertyMetadata(Visibility.Visible));

        // Using a DependencyProperty as the backing store for JobConfiguration.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty JobConfigurationProperty =
            DependencyProperty.Register("JobConfiguration", typeof(JobConfiguration), typeof(ActivitySetting),
                new FrameworkPropertyMetadata
                {
                    BindsTwoWayByDefault = true,
                    DefaultUpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
                });

        // Using a DependencyProperty as the backing store for ListQueryType.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ListQueryTypeProperty =
            DependencyProperty.Register("ListQueryType", typeof(List<string>), typeof(ActivitySetting),
                new PropertyMetadata(new List<string>()));

        // Using a DependencyProperty as the backing store for SavedQueries.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SavedQueriesProperty =
            DependencyProperty.Register("SavedQueries", typeof(ObservableCollection<QueryInfo>),
                typeof(ActivitySetting), new PropertyMetadata(new ObservableCollection<QueryInfo>()));

        // Using a DependencyProperty as the backing store for IsUseGlobalQuery.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsUseGlobalQueryProperty =
            DependencyProperty.Register("IsUseGlobalQuery", typeof(bool), typeof(ActivitySetting),
                new PropertyMetadata(false));

        // Using a DependencyProperty as the backing store for MyProperty.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty NextCommandProperty =
            DependencyProperty.Register("NextCommand", typeof(ICommand), typeof(ActivitySetting));

        // Using a DependencyProperty as the backing store for MyProperty.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PreviousCommandProperty =
            DependencyProperty.Register("PreviousCommand", typeof(ICommand), typeof(ActivitySetting));

        private static readonly DependencyProperty AddQueryCommandProperty
            = DependencyProperty.Register("AddQueryCommand", typeof(ICommand), typeof(ActivitySetting));

        // Using a DependencyProperty as the backing store for CommandParameter.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CommandParameterProperty =
            DependencyProperty.Register("CommandParameter", typeof(object), typeof(ActivitySetting),
                new FrameworkPropertyMetadata(OnAvailableItemsChanged));

        // Using a DependencyProperty as the backing store for NextButtonContent.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty NextButtonContentProperty =
            DependencyProperty.Register("NextButtonContent", typeof(string), typeof(ActivitySetting),
                new FrameworkPropertyMetadata("LangKeyNext".FromResourceDictionary())
                {
                    BindsTwoWayByDefault = true,
                    DefaultUpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
                });

        // Using a DependencyProperty as the backing store for PreviousVisiblity.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PreviousVisiblityProperty =
            DependencyProperty.Register("PreviousVisiblity", typeof(Visibility), typeof(ActivitySetting),
                new PropertyMetadata(Visibility.Visible));

        // Using a DependencyProperty as the backing store for IsExpanded.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsExpandedProperty =
            DependencyProperty.Register("IsExpanded", typeof(bool), typeof(ActivitySetting),
                new FrameworkPropertyMetadata(OnAvailableItemsChanged)
                {
                    BindsTwoWayByDefault = true
                });

        // Using a DependencyProperty as the backing store for View.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ViewProperty =
            DependencyProperty.Register("View", typeof(object), typeof(ActivitySetting));

        // Using a DependencyProperty as the backing store for DeleteQueryCommand.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DeleteQueryCommandProperty =
            DependencyProperty.Register("DeleteQueryCommand", typeof(ICommand), typeof(ActivitySetting));

        // Using a DependencyProperty as the backing store for DeleteMulipleCommand.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DeleteMulipleCommandProperty =
            DependencyProperty.Register("DeleteMulipleCommand", typeof(ICommand), typeof(ActivitySetting));

        private static readonly RoutedEvent DeleteQueryEvent = EventManager.RegisterRoutedEvent("DeleteQuery",
            RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(ActivitySetting));

        public ActivitySetting()
        {
            InitializeComponent();
            Setting.DataContext = this;
            DeleteQueryCommand = new DelegateCommand<object>(DeleteQueryExecute);
            DeleteMulipleCommand = new DelegateCommand<object>(DeleteMulipleExecute);
            HeaderHelper.UpdateToggleForQuery = UpdateToggleButton;
        }

        public Speed Model => new Speed();

        public string Heading
        {
            get => (string) GetValue(HeadingProperty);
            set => SetValue(HeadingProperty, value);
        }


        public Visibility HeaderNextPreViousVisiblity
        {
            get => (Visibility) GetValue(HeaderNextPreViousVisiblityProperty);
            set => SetValue(HeaderNextPreViousVisiblityProperty, value);
        }


        public JobConfiguration JobConfiguration
        {
            get => (JobConfiguration) GetValue(JobConfigurationProperty);
            set => SetValue(JobConfigurationProperty, value);
        }

        public List<string> ListQueryType
        {
            get => (List<string>) GetValue(ListQueryTypeProperty);
            set => SetValue(ListQueryTypeProperty, value);
        }

        public ObservableCollection<QueryInfo> SavedQueries
        {
            get => (ObservableCollection<QueryInfo>) GetValue(SavedQueriesProperty);
            set => SetValue(SavedQueriesProperty, value);
        }


        public bool IsUseGlobalQuery
        {
            get => (bool) GetValue(IsUseGlobalQueryProperty);
            set => SetValue(IsUseGlobalQueryProperty, value);
        }


        public ICommand NextCommand
        {
            get => (ICommand) GetValue(NextCommandProperty);
            set => SetValue(NextCommandProperty, value);
        }

        public ICommand PreviousCommand
        {
            get => (ICommand) GetValue(PreviousCommandProperty);
            set => SetValue(PreviousCommandProperty, value);
        }

        public ICommand AddQueryCommand
        {
            get => (ICommand) GetValue(AddQueryCommandProperty);
            set => SetValue(AddQueryCommandProperty, value);
        }

        public object CommandParameter
        {
            get => GetValue(CommandParameterProperty);
            set => SetValue(CommandParameterProperty, value);
        }

        public string NextButtonContent
        {
            get => (string) GetValue(NextButtonContentProperty);
            set => SetValue(NextButtonContentProperty, value);
        }

        public Visibility PreviousVisiblity
        {
            get => (Visibility) GetValue(PreviousVisiblityProperty);
            set => SetValue(PreviousVisiblityProperty, value);
        }

        public bool IsExpanded
        {
            get => (bool) GetValue(IsExpandedProperty);
            set => SetValue(IsExpandedProperty, value);
        }

        public object View
        {
            get => GetValue(ViewProperty);
            set => SetValue(ViewProperty, value);
        }

        public ICommand DeleteQueryCommand
        {
            get => (ICommand) GetValue(DeleteQueryCommandProperty);
            set => SetValue(DeleteQueryCommandProperty, value);
        }


        public ICommand DeleteMulipleCommand
        {
            get => (ICommand) GetValue(DeleteMulipleCommandProperty);
            set => SetValue(DeleteMulipleCommandProperty, value);
        }

        private bool IsClickedFromToggle { get; set; }

        public static void OnAvailableItemsChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var newValue = e.NewValue;
        }

        public event RoutedEventHandler DeleteQuery
        {
            add => AddHandler(DeleteQueryEvent, value);
            remove => RemoveHandler(DeleteQueryEvent, value);
        }

        private void DeleteQueryEventHandler()
        {
            var routedEventArgs = new RoutedEventArgs(DeleteQueryEvent);
            RaiseEvent(routedEventArgs);
        }

        private void DeleteQueryExecute(object sender)
        {
            try
            {
                var QueryToDelete = sender as QueryInfo;
                DeleteQueryEventHandler();
                if (SavedQueries.Any(x => QueryToDelete != null && x.Id == QueryToDelete.Id))
                    SavedQueries.Remove(QueryToDelete);
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        private void DeleteMulipleExecute(object obj)
        {
            try
            {
                foreach (var queryInfo in SavedQueries.ToList())
                    if (queryInfo.IsQuerySelected)
                        SavedQueries.Remove(queryInfo);
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        private void ClpsExpnd_OnClick(object sender, RoutedEventArgs e)
        {
            IsClickedFromToggle = true;
            HeaderHelper.ExpandCollapseAllExpander(sender, IsExpanded);
        }

        private void UpdateToggleButton()
        {
            if (IsClickedFromToggle)
            {
                var isAllCollapsed = HeaderHelper.IsAllExpanderCollapseOrNot(View);
                IsExpanded = !isAllCollapsed;
            }
        }
    }

    public class Speed
    {
        public Activity FastSpeed = new Activity
        {
            ActivitiesPerDay = new RangeUtilities(266, 400),
            ActivitiesPerHour = new RangeUtilities(26, 40),
            ActivitiesPerWeek = new RangeUtilities(1600, 2400),
            ActivitiesPerJob = new RangeUtilities(33, 50),
            DelayBetweenJobs = new RangeUtilities(65, 97),
            DelayBetweenActivity = new RangeUtilities(0, 1)
        };

        public Activity MediumSpeed = new Activity
        {
            ActivitiesPerDay = new RangeUtilities(133, 200),
            ActivitiesPerHour = new RangeUtilities(13, 20),
            ActivitiesPerWeek = new RangeUtilities(800, 1200),
            ActivitiesPerJob = new RangeUtilities(16, 25),
            DelayBetweenJobs = new RangeUtilities(73, 110),
            DelayBetweenActivity = new RangeUtilities(0, 1)
        };

        public Activity SlowSpeed = new Activity
        {
            ActivitiesPerDay = new RangeUtilities(66, 100),
            ActivitiesPerHour = new RangeUtilities(6, 10),
            ActivitiesPerWeek = new RangeUtilities(400, 600),
            ActivitiesPerJob = new RangeUtilities(8, 12),
            DelayBetweenJobs = new RangeUtilities(73, 110),
            DelayBetweenActivity = new RangeUtilities(1, 2)
        };


        public Activity SuperfastSpeed = new Activity
        {
            ActivitiesPerDay = new RangeUtilities(400, 600),
            ActivitiesPerHour = new RangeUtilities(40, 60),
            ActivitiesPerWeek = new RangeUtilities(2400, 3600),
            ActivitiesPerJob = new RangeUtilities(50, 75),
            DelayBetweenJobs = new RangeUtilities(77, 116),
            DelayBetweenActivity = new RangeUtilities(0, 1)
        };
    }

    public class Activity
    {
        public RangeUtilities ActivitiesPerDay = new RangeUtilities();
        public RangeUtilities ActivitiesPerHour = new RangeUtilities();
        public RangeUtilities ActivitiesPerJob = new RangeUtilities();
        public RangeUtilities ActivitiesPerWeek = new RangeUtilities();
        public RangeUtilities DelayBetweenActivity = new RangeUtilities();
        public RangeUtilities DelayBetweenJobs = new RangeUtilities();
    }
}