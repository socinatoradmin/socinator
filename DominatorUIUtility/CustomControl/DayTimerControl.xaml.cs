using System;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;

namespace DominatorUIUtility.CustomControl
{
    /// <summary>
    ///     Interaction logic for DayTimerControl.xaml
    /// </summary>
    public partial class DayTimerControl
    {
        // Using a DependencyProperty as the backing store for RunningTimes.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty RunningTimesProperty =
            DependencyProperty.Register("RunningTimes", typeof(RunningTimes), typeof(DayTimerControl),
                new FrameworkPropertyMetadata
                {
                    BindsTwoWayByDefault = true
                });

        public DayTimerControl()
        {
            InitializeComponent();
            TimerMainGrid.DataContext = this;
        }

        public RunningTimes RunningTimes
        {
            get => (RunningTimes) GetValue(RunningTimesProperty);
            set => SetValue(RunningTimesProperty, value);
        }

        private void AddTimer_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            RunningTimes.IsEnabled = RunningTimes.Timings.Count > 0;

            var objDialogWindow = new Dialog();

            var objSchedulerControl = new SchedulerControl {TextBlockWarning = {Visibility = Visibility.Collapsed}};


            var dialogWindow =
                objDialogWindow.GetMetroWindow(objSchedulerControl, "Please provide the start and end time");
            dialogWindow.Topmost = true;
            objSchedulerControl.btnAddInterval.Click += (btnSender, args) =>
            {
                if (objSchedulerControl.StartTimePicker.SelectedTime == null ||
                    objSchedulerControl.EndTimePicker.SelectedTime == null)
                {
                    Console.WriteLine(Properties.Resources
                        .DayTimerControl_AddTimer_OnMouseLeftButtonDown_Please_select_date_time__);
                }
                else
                {
                    if (TimeSpan.Compare(objSchedulerControl.StartTimePicker.SelectedTime.Value, objSchedulerControl
                            .EndTimePicker.SelectedTime.Value) >= 0)
                    {
                        ToasterNotification.ShowWarning("End time should be more then Start time");
                        return;
                    }

                    var isAlreadyGivenTimeAdded = true;

                    foreach (var timing in RunningTimes.Timings)
                    {
                        isAlreadyGivenTimeAdded = IsBetweenRange(timing.StartTime, timing.EndTime,
                            (TimeSpan) objSchedulerControl.StartTimePicker.SelectedTime,
                            (TimeSpan) objSchedulerControl.EndTimePicker.SelectedTime);

                        if (!isAlreadyGivenTimeAdded)
                            break;
                    }

                    if (isAlreadyGivenTimeAdded)
                    {
                        RunningTimes.AddTimeRange(new TimingRange(
                            objSchedulerControl.StartTimePicker.SelectedTime.Value,
                            objSchedulerControl.EndTimePicker.SelectedTime.Value));

                        RunningTimes.IsEnabled = RunningTimes.Timings.Count > 0;

                        objSchedulerControl.TextBlockWarning.Visibility = Visibility.Collapsed;
                        dialogWindow.Close();
                    }
                    else
                    {
                        objSchedulerControl.TextBlockWarning.Visibility = Visibility.Visible;
                    }
                }
            };
            dialogWindow.ShowDialog();
        }


        public bool IsBetweenRange(TimeSpan startTime, TimeSpan endTime, TimeSpan givenStartTime, TimeSpan givenEndTime)
        {
            return (givenEndTime.TotalSeconds - startTime.TotalSeconds) *
                   (givenStartTime.TotalSeconds - endTime.TotalSeconds) >= 0;
        }

        private void DeleteTimer_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var currentTimer = ((FrameworkElement) sender).DataContext as TimingRange;

            if (RunningTimes.Timings.Any(range => range.TimeId == currentTimer?.TimeId))
            {
                RunningTimes.Timings.Remove(currentTimer);
                RunningTimes.IsEnabled = RunningTimes.Timings.Count > 0;
            }
        }
    }
}