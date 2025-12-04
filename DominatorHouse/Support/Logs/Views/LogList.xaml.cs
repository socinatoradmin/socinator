using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using System.Collections.Specialized;
using System.Windows;

namespace DominatorHouse.Support.Logs.Views
{
    /// <summary>
    /// Interaction logic for LogList.xaml
    /// </summary>
    public partial class LogList
    {
        public const string LogTypeInfo = "INFO";
        public const string LogTypeError = "ERROR";

        public INotifyCollectionChanged Logs
        {
            get { return (INotifyCollectionChanged)GetValue(LogsProperty); }
            set { SetValue(LogsProperty, value); }
        }

        public ActivityType? ActivityTypeFilter
        {
            get { return (ActivityType?)GetValue(ActivityTypeFilterProperty); }
            set { SetValue(ActivityTypeFilterProperty, value); }
        }

        public SocialNetworks? NetworkFilter
        {
            get { return (SocialNetworks?)GetValue(NetworkFilterProperty); }
            set { SetValue(NetworkFilterProperty, value); }
        }

        public string LogTypeFilter
        {
            get { return (string)GetValue(LogTypeFilterProperty); }
            set { SetValue(LogTypeFilterProperty, value); }
        }

        public LoggerModel SelectedLoggerModel
        {
            get { return (LoggerModel)GetValue(SelectedLoggerModelProperty); }
            set { SetValue(SelectedLoggerModelProperty, value); }
        }

        public object SyncObject
        {
            get { return GetValue(SyncObjectProperty); }
            set { SetValue(SyncObjectProperty, value); }
        }

        public static readonly DependencyProperty SyncObjectProperty =
            DependencyProperty.Register("SyncObject", typeof(object), typeof(LogList), new PropertyMetadata(new DefaultObject()));

        public static readonly DependencyProperty SelectedLoggerModelProperty =
            DependencyProperty.Register("SelectedLoggerModel", typeof(LoggerModel), typeof(LogList), new PropertyMetadata(null));

        public static readonly DependencyProperty LogTypeFilterProperty =
            DependencyProperty.Register("LogTypeFilter", typeof(string), typeof(LogList), new PropertyMetadata(LogTypeInfo));

        public static readonly DependencyProperty NetworkFilterProperty =
            DependencyProperty.Register("NetworkFilter", typeof(SocialNetworks?), typeof(LogList), new PropertyMetadata(null));

        public static readonly DependencyProperty ActivityTypeFilterProperty =
            DependencyProperty.Register("ActivityTypeFilter", typeof(ActivityType?), typeof(LogList), new PropertyMetadata(null));

        public static readonly DependencyProperty LogsProperty =
            DependencyProperty.Register("Logs", typeof(INotifyCollectionChanged), typeof(LogList), new PropertyMetadata(null));


        public LogList()
        {
            InitializeComponent();
        }

        private class DefaultObject
        {

        }
    }
}
