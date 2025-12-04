using System;

namespace DominatorUIUtility.CustomControl
{
    /// <summary>
    ///     Interaction logic for SchedulerControl.xaml
    /// </summary>
    public partial class SchedulerControl
    {
        public SchedulerControl()
        {
            var startTime = "00:00:00";
            var endTime = "23:59:59";

            InitializeComponent();
            StartTimePicker.SelectedTime = Convert.ToDateTime(startTime).TimeOfDay;
            EndTimePicker.SelectedTime = Convert.ToDateTime(endTime).TimeOfDay;
        }
    }
}