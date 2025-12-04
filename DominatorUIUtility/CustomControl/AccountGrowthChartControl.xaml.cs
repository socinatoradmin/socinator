using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using DominatorHouseCore.Enums.DHEnum;
using DominatorHouseCore.ViewModel;
using LiveCharts;

namespace DominatorUIUtility.CustomControl
{
    /// <summary>
    ///     Interaction logic for AccountGrowthChartControl.xaml
    /// </summary>
    public partial class AccountGrowthChartControl
    {
        public static readonly DependencyProperty SeriesCollectionProperty =
            DependencyProperty.Register("SeriesCollection", typeof(SeriesCollection), typeof(AccountGrowthChartControl),
                new PropertyMetadata(null));


        public static readonly DependencyProperty AxisXLabelsProperty =
            DependencyProperty.Register("AxisXLabels", typeof(string[]), typeof(AccountGrowthChartControl),
                new PropertyMetadata(null));


        public static readonly DependencyProperty GrowthListProperty =
            DependencyProperty.Register("GrowthList", typeof(ObservableCollection<DailyStatisticsViewModel>),
                typeof(AccountGrowthChartControl), new PropertyMetadata(null, GrowthListChanged));


        public static readonly DependencyProperty GrowthChartPeriodProperty =
            DependencyProperty.Register("GrowthChartPeriod", typeof(GrowthChartPeriod),
                typeof(AccountGrowthChartControl),
                new PropertyMetadata(GrowthChartPeriod.Past30Days, GrowthCartPeriodChanged));


        public static readonly DependencyProperty AxisXTitleProperty =
            DependencyProperty.Register("AxisXTitle", typeof(string), typeof(AccountGrowthChartControl),
                new PropertyMetadata("Month"));


        public AccountGrowthChartControl()
        {
            InitializeComponent();
        }

        public static Func<double, string> YFormatter => value => value.ToString();

        public string AxisXTitle
        {
            get => (string) GetValue(AxisXTitleProperty);
            set => SetValue(AxisXTitleProperty, value);
        }

        public GrowthChartPeriod GrowthChartPeriod
        {
            get => (GrowthChartPeriod) GetValue(GrowthChartPeriodProperty);
            set => SetValue(GrowthChartPeriodProperty, value);
        }

        public ObservableCollection<DailyStatisticsViewModel> GrowthList
        {
            get => (ObservableCollection<DailyStatisticsViewModel>) GetValue(GrowthListProperty);
            set => SetValue(GrowthListProperty, value);
        }

        public string[] AxisXLabels
        {
            get => (string[]) GetValue(AxisXLabelsProperty);
            set => SetValue(AxisXLabelsProperty, value);
        }

        public SeriesCollection SeriesCollection
        {
            get => (SeriesCollection) GetValue(SeriesCollectionProperty);
            set => SetValue(SeriesCollectionProperty, value);
        }

        private static void GrowthCartPeriodChanged(DependencyObject dependencyObject,
            DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            SetTitleAndLabels(dependencyObject);
        }

        private static void GrowthListChanged(DependencyObject dependencyObject,
            DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            SetTitleAndLabels(dependencyObject);
        }

        private static void SetTitleAndLabels(DependencyObject dependencyObject)
        {
            try
            {
                var period = (GrowthChartPeriod) dependencyObject.GetValue(GrowthChartPeriodProperty);
                var growthList =
                    (ObservableCollection<DailyStatisticsViewModel>) dependencyObject.GetValue(GrowthListProperty);


                switch (period)
                {
                    case GrowthChartPeriod.PastDay:
                        dependencyObject.SetCurrentValue(AxisXLabelsProperty,
                            growthList.Select(x => x.Date.ToString("MM/dd/yyyy")).ToArray());
                        dependencyObject.SetCurrentValue(AxisXTitleProperty, "Days");
                        break;
                    case GrowthChartPeriod.PastWeek:
                    case GrowthChartPeriod.Past30Days:
                        dependencyObject.SetCurrentValue(AxisXLabelsProperty,
                            growthList.Select(x => x.Date.ToString("MM/dd/yyyy")).ToArray());
                        dependencyObject.SetCurrentValue(AxisXTitleProperty, "Days");
                        break;
                    case GrowthChartPeriod.Past3Months:
                    case GrowthChartPeriod.Past6Months:
                        dependencyObject.SetCurrentValue(AxisXLabelsProperty,
                            growthList.Select(x => x.Date.ToString("MMM")).ToArray());
                        dependencyObject.SetCurrentValue(AxisXTitleProperty, "Months");
                        break;
                    case GrowthChartPeriod.AllTime:

                        if (growthList.LastOrDefault()?.Date <=
                            growthList.FirstOrDefault()?.Date.AddDays(-30))
                        {
                            dependencyObject.SetCurrentValue(AxisXLabelsProperty,
                                growthList.Select(x => x.Date.ToString("MMM")).ToArray());
                            dependencyObject.SetCurrentValue(AxisXTitleProperty, "Months");
                        }
                        else
                        {
                            dependencyObject.SetCurrentValue(AxisXLabelsProperty,
                                growthList.Select(x => x.Date.ToString("MM/dd/yyyy")).ToArray());
                            dependencyObject.SetCurrentValue(AxisXTitleProperty, "Days");
                        }

                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            catch
            {
                Debugger.Break();
                throw;
            }
        }
    }
}