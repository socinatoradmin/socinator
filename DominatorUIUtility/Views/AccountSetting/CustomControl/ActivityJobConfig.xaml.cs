using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using DominatorHouseCore.Annotations;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;

namespace DominatorUIUtility.Views.AccountSetting.CustomControl
{
    /// <summary>
    ///     Interaction logic for ActivityJobConfig.xaml
    /// </summary>
    public partial class ActivityJobConfig : UserControl
    {
        // Using a DependencyProperty as the backing store for JobConfiguration.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty JobConfigurationProperty =
            DependencyProperty.Register("JobConfiguration", typeof(JobConfiguration), typeof(ActivityJobConfig),
                new FrameworkPropertyMetadata(OnAvailableItemsChanged)
                {
                    BindsTwoWayByDefault = true,
                    DefaultUpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
                });

        // Using a DependencyProperty as the backing store for PerJobActivity.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PerJobActivityProperty =
            DependencyProperty.Register("PerJobActivity", typeof(string), typeof(ActivityJobConfig),
                new FrameworkPropertyMetadata(OnAvailableItemsChanged)
                {
                    BindsTwoWayByDefault = true,
                    DefaultValue = "LangKeyUsers".FromResourceDictionary()
                });

        // Using a DependencyProperty as the backing store for PerHourActivity.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PerHourActivityProperty =
            DependencyProperty.Register("PerHourActivity", typeof(string), typeof(ActivityJobConfig),
                new FrameworkPropertyMetadata(OnAvailableItemsChanged)
                {
                    BindsTwoWayByDefault = true,
                    DefaultValue = "LangKeyUsers".FromResourceDictionary()
                });

        // Using a DependencyProperty as the backing store for PerDayActivity.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PerDayActivityProperty =
            DependencyProperty.Register("PerDayActivity", typeof(string), typeof(ActivityJobConfig),
                new FrameworkPropertyMetadata(OnAvailableItemsChanged)
                {
                    BindsTwoWayByDefault = true,
                    DefaultValue = "LangKeyUsers".FromResourceDictionary()
                });

        // Using a DependencyProperty as the backing store for PerWeekActivity.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PerWeekActivityProperty =
            DependencyProperty.Register("PerWeekActivity", typeof(string), typeof(ActivityJobConfig),
                new FrameworkPropertyMetadata(OnAvailableItemsChanged)
                {
                    BindsTwoWayByDefault = true,
                    DefaultValue = "LangKeyUsers".FromResourceDictionary()
                });

        public ActivityJobConfig()
        {
            InitializeComponent();
            MainGrid.DataContext = this;
        }

        public Speed Model => new Speed();

        public JobConfiguration JobConfiguration
        {
            get => (JobConfiguration) GetValue(JobConfigurationProperty);
            set => SetValue(JobConfigurationProperty, value);
        }


        public string PerJobActivity
        {
            get => (string) GetValue(PerJobActivityProperty);
            set => SetValue(PerJobActivityProperty, value);
        }

        public string PerHourActivity
        {
            get => (string) GetValue(PerHourActivityProperty);
            set => SetValue(PerHourActivityProperty, value);
        }

        public string PerDayActivity
        {
            get => (string) GetValue(PerDayActivityProperty);
            set => SetValue(PerDayActivityProperty, value);
        }

        public string PerWeekActivity
        {
            get => (string) GetValue(PerWeekActivityProperty);
            set => SetValue(PerWeekActivityProperty, value);
        }

        public static void OnAvailableItemsChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var newValue = e.NewValue;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}